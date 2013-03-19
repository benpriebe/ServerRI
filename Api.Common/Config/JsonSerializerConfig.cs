using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Api.Common.Config
{
    public class JsonSerializerConfig
    {
        public static void Configure(JsonMediaTypeFormatter jsonFormatter)
        {
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
        }
    }
}