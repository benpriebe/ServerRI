#region Using directives

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion


namespace Core.Extensions
{
    public static class ObjectExtensions
    {
        internal static JsonSerializerSettings JsonSerializerSettings; 

        static ObjectExtensions()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            };
            JsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

        public static string ToJson(this object obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, formatting, JsonSerializerSettings);
        }
    }
}