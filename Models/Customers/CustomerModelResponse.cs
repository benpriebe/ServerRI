#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#endregion


namespace Models.Customers
{
    public class CustomerModelResponse
    {
        public CustomerModelResponse()
        {
            CustomerAddresses = new List<CustomerAddressModel>();
        }

        [Display, Required, Key]
        public int CustomerID { get; set; }

        [Display, Required]
        public string FirstName { get; set; }
        
        [Display, Required]
        public string Surname { get; set; }


        [Display]
        public string Title { get; set; }

        [Display]
        public string MiddleName { get; set; }

        [Display]
        public string CompanyName { get; set; }

        [Display]
        public string EmailAddress { get; set; }

        [Display]
        public string Phone { get; set; }

        [Display]
        public virtual IEnumerable<CustomerAddressModel> CustomerAddresses { get; set; }
    }
}