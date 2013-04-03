#region Using directives

using System;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using Api.Common;
using Autofac;
using Contracts.Data;
using Data.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Addresses;
using Models.Customers;
using Models.Products;
using Models.Products.Resx;

#endregion


namespace Services.Tests.E2ETests
{
    [TestClass]
    public class CustomerssServiceE2ETest
    {
        private readonly CustomersService _service = IoC.Container.Resolve<CustomersService>();
        private readonly Func<IUnitOfWork> _uow = IoC.Container.Resolve<Func<IUnitOfWork>>();

        private CustomerModel GetBaseCustomer()
        {
            return new CustomerModel
            {
                FirstName = "Mario",
                LastName = "Plumber",
                PasswordHash = "XXX",
                PasswordSalt = "Salty"
            };
        }

        [TestMethod]
        [TestCategory("Database")]
        public void AddCustomer_NoAddresses()
        {
            var customerModel = GetBaseCustomer();

            using (new TransactionScope())
            {
                //Act
                var result = _service.AddCustomer(customerModel);

                //Assert
                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Value);
                Assert.IsTrue(customerModel.CustomerID > 0);

                using (var uow = _uow())
                {
                    var dbCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).SingleOrDefault(c => c.CustomerID == customerModel.CustomerID);
                    Assert.AreEqual(customerModel.FirstName, dbCustomer.FirstName);
                    Assert.AreEqual(customerModel.LastName, dbCustomer.LastName);

                    Assert.IsFalse(dbCustomer.CustomerAddresses.Any());
                }
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void AddCustomer_WithExistingCustomerAddress()
        {
            var customerModel = GetBaseCustomer();

            Address address;
            using (var uow = _uow())
            {
                address = uow.Addresses.GetAll().FirstOrDefault();
            }

            var customerAddressModel = new CustomerAddressModel()
            {
                AddressID = address.AddressID,
                AddressType = "Type"
            };

            customerModel.CustomerAddresses.Add(customerAddressModel);

            using (new TransactionScope())
            {
                //Act
                var result = _service.AddCustomer(customerModel);

                //Assert
                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Value);
                Assert.IsTrue(customerModel.CustomerID > 0);

                using (var uow = _uow())
                {
                    var dbCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).SingleOrDefault(c => c.CustomerID == customerModel.CustomerID);
                    Assert.AreEqual(customerModel.FirstName, dbCustomer.FirstName);
                    Assert.AreEqual(customerModel.LastName, dbCustomer.LastName);

                    Assert.IsTrue(dbCustomer.CustomerAddresses.Count() == 1);
                    Assert.AreEqual(customerAddressModel.AddressType, dbCustomer.CustomerAddresses.First().AddressType);
                    Assert.IsNotNull(dbCustomer.CustomerAddresses.First().Address);
                    Assert.AreEqual(address.AddressLine1, dbCustomer.CustomerAddresses.First().Address.AddressLine1);
                    Assert.AreEqual(address.CountryRegion, dbCustomer.CustomerAddresses.First().Address.CountryRegion);

                }
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void AddCustomer_WithNewAddress()
        {
            var customerModel = GetBaseCustomer();

            AddressModel addressModel = new AddressModel()
            {
                AddressLine1 = "Line 1",
                City = "Florence",
                CountryRegion = "Italy",
                PostalCode = "123",
                StateProvince = "Plumberton"
            };

            var customerAddressModel = new CustomerAddressModel()
            {
                Address = addressModel,
                AddressType = "Type"
            };

            customerModel.CustomerAddresses.Add(customerAddressModel);

            using (new TransactionScope())
            {
                //Act
                var result = _service.AddCustomer(customerModel);

                //Assert
                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Value);
                Assert.IsTrue(customerModel.CustomerID > 0);

                using (var uow = _uow())
                {
                    var dbCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).SingleOrDefault(c => c.CustomerID == customerModel.CustomerID);
                    Assert.AreEqual(customerModel.FirstName, dbCustomer.FirstName);
                    Assert.AreEqual(customerModel.LastName, dbCustomer.LastName);
                    Assert.IsTrue(dbCustomer.CustomerAddresses.Count() == 1);
                    Assert.AreEqual(customerAddressModel.AddressType, dbCustomer.CustomerAddresses.First().AddressType);
                    Assert.IsNotNull(dbCustomer.CustomerAddresses.First().Address);
                    Assert.AreEqual(customerAddressModel.Address.AddressLine1, dbCustomer.CustomerAddresses.First().Address.AddressLine1);
                    Assert.AreEqual(customerAddressModel.Address.CountryRegion, dbCustomer.CustomerAddresses.First().Address.CountryRegion);
                    // etc...
                }
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void UpdateCustomer_NoChangesToAddresses() {
            
            //Arrange
            Customer existingCustomer;
            using (var uow = _uow())
            {
                existingCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).FirstOrDefault(c => c.CustomerAddresses.Any());
            }

            var customerModel = BaseService.Mapper.Map<CustomerModel>(existingCustomer);
            customerModel.FirstName = "Donkey Brothers";

            using (new TransactionScope())
            {
                //Act
                var result = _service.UpdateCustomer(customerModel);

                //Assert
                Assert.IsTrue(result.Success);

                using (var uow = _uow())
                {
                    var updatedCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).SingleOrDefault(c => c.CustomerID == customerModel.CustomerID);
                    Assert.AreEqual(customerModel.FirstName, updatedCustomer.FirstName);
                    Assert.AreEqual(existingCustomer.CustomerAddresses.Count(), updatedCustomer.CustomerAddresses.Count());
                }
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void UpdateCustomer_LinkToExistingAddress()
        {
            //Arrange
            Customer existingCustomer, mappingCustomer;
            using (var uow = _uow())
            {
                existingCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).FirstOrDefault(c => c.CustomerAddresses.Any());
                mappingCustomer = uow.Customers.GetAll().FirstOrDefault(c => c.CustomerID == existingCustomer.CustomerID);
            }

            Address address;
            using (var uow = _uow())
            {
                address = uow.Addresses.GetAll().FirstOrDefault();
            }

            var customerAddressModel = new CustomerAddressModel()
            {
                AddressID = address.AddressID,
                AddressType = "Type"
            };

            var customerModel = BaseService.Mapper.Map<CustomerModel>(mappingCustomer);
            customerModel.FirstName = "Donkey Brothers";
            customerModel.CustomerAddresses.Add(customerAddressModel);
            
            using (new TransactionScope())
            {
                //Act
                var result = _service.UpdateCustomer(customerModel);

                //Assert
                Assert.IsTrue(result.Success);

                using (var uow = _uow())
                {
                    var updatedCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).SingleOrDefault(c => c.CustomerID == customerModel.CustomerID);
                    Assert.IsTrue(updatedCustomer.CustomerAddresses.Count() > existingCustomer.CustomerAddresses.Count());
                    Assert.IsTrue(updatedCustomer.CustomerAddresses.Any(ca => ca.AddressID == address.AddressID));
                }
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void UpdateCustomer_AddNewAddress()
        {
            //Arrange
            Customer existingCustomer;
            Customer mappingCustomer;
            using (var uow = _uow())
            {
                existingCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).FirstOrDefault(c => c.CustomerAddresses.Any());
                mappingCustomer = uow.Customers.GetAll().FirstOrDefault(c => c.CustomerID == existingCustomer.CustomerID);
            }

            var customerModel = BaseService.Mapper.Map<CustomerModel>(mappingCustomer); // no addresses on it.
            customerModel.FirstName = "Donkey Brothers";
            
            AddressModel addressModel = new AddressModel()
            {
                AddressLine1 = "Line 1",
                City = "Florence",
                CountryRegion = "Italy",
                PostalCode = "123",
                StateProvince = "Plumberton"
            };

            var customerAddressModel = new CustomerAddressModel()
            {
                Address = addressModel,
                AddressType = "Type"
            };

            customerModel.CustomerAddresses.Add(customerAddressModel);

            using (new TransactionScope())
            {
                //Act
                var result = _service.UpdateCustomer(customerModel);
                
                //Assert
                Assert.IsTrue(result.Success);

                using (var uow = _uow())
                {
                    var updatedCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).SingleOrDefault(c => c.CustomerID == customerModel.CustomerID);
                    Assert.IsTrue(updatedCustomer.CustomerAddresses.Count() > existingCustomer.CustomerAddresses.Count());
                }
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void UpdateCustomer_ModifyPropertiesOnExistingAddress()
        {
            //Arrange
            Customer existingCustomer;
            using (var uow = _uow())
            {
                existingCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).FirstOrDefault(c => c.CustomerAddresses.Any());
            }

            var customerModel = BaseService.Mapper.Map<CustomerModel>(existingCustomer);
            customerModel.FirstName = "Donkey Brothers";
            var customerAddress = customerModel.CustomerAddresses.First();
            customerAddress.AddressType = "BusterBrown";
            customerAddress.Address.AddressLine1 = "Jackie Brown St."; 

            using (new TransactionScope())
            {
                //Act
                var result = _service.UpdateCustomer(customerModel);

                //Assert
                Assert.IsTrue(result.Success);

                using (var uow = _uow())
                {
                    var updatedCustomer = uow.Customers.GetAll().Include(c => c.CustomerAddresses.Select(ca => ca.Address)).SingleOrDefault(c => c.CustomerID == customerModel.CustomerID);
                    Assert.AreEqual(customerModel.FirstName, updatedCustomer.FirstName);
                    Assert.AreEqual(existingCustomer.CustomerAddresses.Count(), updatedCustomer.CustomerAddresses.Count());
                    Assert.AreEqual(customerAddress.AddressType, updatedCustomer.CustomerAddresses.First().AddressType);
                    Assert.AreEqual(customerAddress.Address.AddressLine1, updatedCustomer.CustomerAddresses.First().Address.AddressLine1);
                }
            }

        }

    }

}