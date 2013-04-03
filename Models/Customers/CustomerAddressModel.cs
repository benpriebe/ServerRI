#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Addresses;

#endregion


namespace Models.Customers
{
    public class CustomerAddressModel
    {
        [Display]
        public int CustomerID { get; set; }

        [Display, Required]
        public int AddressID { get; set; }

        [Display, Required]
        public string AddressType { get; set; }

        [Display, Required]
        public virtual AddressModel Address { get; set; }
    }
}