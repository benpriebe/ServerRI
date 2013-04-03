#region Using directives

using System.ComponentModel.DataAnnotations;

#endregion


namespace Models.Addresses
{
    public class AddressModel
    {
        [Display]
        public int AddressID { get; set; }

        [Display, Required]
        public string AddressLine1 { get; set; }

        [Display]
        public string AddressLine2 { get; set; }

        [Display, Required]
        public string City { get; set; }

        [Display, Required]
        public string StateProvince { get; set; }

        [Display, Required]
        public string CountryRegion { get; set; }

        [Display, Required]
        public string PostalCode { get; set; }
      
    }
}