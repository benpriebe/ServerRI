#region Using directives

using AutoMapper;
using Core.Extensions;
using Data.Entities;
using Models.Addresses;
using Models.Customers;
using Models.Products;

#endregion


namespace Services.Mappings
{
    internal class CustomerMapper : BaseMapper
    {
        internal override void ConfigureMap(ConfigurationStore mapperConfig)
        {
            mapperConfig.CreateMap<CustomerModel, Customer>()
                .ReverseMap();

            mapperConfig.CreateMap<CustomerAddressModel, CustomerAddress>()
                .ReverseMap();

            mapperConfig.CreateMap<AddressModel, Address>()
                .ReverseMap();
        }

    }
}