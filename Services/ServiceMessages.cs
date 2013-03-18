#region Using directives

using Api.Common;

#endregion


namespace Services
{
    public static class ServiceMessages
    {
        public enum Codes
        {
            // general errors prefixed with 000;
            ProviderError = 0666,

            // product related errors prefixed with 100;


            // customer related errors prefixed with 200;
        };
    }
}