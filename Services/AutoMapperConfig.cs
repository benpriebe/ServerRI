#region Using directives

using AutoMapper;
using AutoMapper.Mappers;
using Services.Mappings;

#endregion


namespace Services
{
    /// <summary>
    /// Helper class to create the AutoMapper configuration.
    /// </summary>
    internal static class AutoMapperConfig
    {
        public static MappingEngine CreateMappingEngine()
        {
            var mapperConfig = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.AllMappers());
            new ProductMapper().ConfigureMap(mapperConfig);
            new CustomerMapper().ConfigureMap(mapperConfig);

            var mapper = new MappingEngine(mapperConfig);
            Mapper.AssertConfigurationIsValid();

            return mapper;
        }
    }
}