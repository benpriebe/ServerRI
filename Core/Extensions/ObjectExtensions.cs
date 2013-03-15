#region Using directives

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion


namespace Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonConverter[]
            {
                new StringEnumConverter()
            });
        }
    }
}