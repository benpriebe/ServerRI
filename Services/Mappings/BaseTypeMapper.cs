#region Using directives

using AutoMapper;

#endregion


namespace Services.Mappings
{
    internal abstract class BaseMapper
    {
        internal abstract void ConfigureMap(ConfigurationStore mapperConfig);
    }
}