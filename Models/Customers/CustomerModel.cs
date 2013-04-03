#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#endregion


namespace Models.Customers
{
    public class CustomerModel
    {
        public CustomerModel()
        {
            CustomerAddresses = new List<CustomerAddressModel>();
        }

        [Display]
        public int CustomerID { get; set; }

        [Display, Required]
        public bool NameStyle { get; set; }

        [Display, Required]
        public string FirstName { get; set; }
        
        [Display, Required]
        public string LastName { get; set; }

        [Display, Required]
        public string PasswordHash { get; set; }

        [Display, Required]
        public string PasswordSalt { get; set; }


        [Display]
        public string Title { get; set; }

        [Display]
        public string MiddleName { get; set; }

        [Display]
        public string Suffix { get; set; }

        [Display]
        public string CompanyName { get; set; }

        [Display]
        public string SalesPerson { get; set; }

        [Display]
        public string EmailAddress { get; set; }

        [Display]
        public string Phone { get; set; }

        [Display]
        public virtual ICollection<CustomerAddressModel> CustomerAddresses { get; set; }
    }
}