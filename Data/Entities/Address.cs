#region Using directives

using System;
using System.Collections.Generic;

#endregion


namespace Data.Entities
{
    public class Address
    {
        public Address()
        {
            CustomerAddresses = new List<CustomerAddress>();
            SalesOrderHeaders = new List<SalesOrderHeader>();
            SalesOrderHeaders1 = new List<SalesOrderHeader>();
        }

        public int AddressID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string CountryRegion { get; set; }
        public string PostalCode { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders1 { get; set; }
    }
}