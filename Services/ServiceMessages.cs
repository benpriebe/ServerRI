using Api.Common;

namespace Services
{
    public static class ServiceMessages
    {
        public enum Codes
        {
            // general errors prefixed with 000;
            ProviderError = 0666,

            // product related errors prefixed with 100;
            ProductNotFound = 0001,

            // customer related errors prefixed with 200;
        };

        public static Message ProductNotFound(int productId)
        {
            return new Message(MessageLevel.Error, (int) Codes.ProductNotFound, "Product - {0} - not found.", productId.ToString());
        }
    }
}
