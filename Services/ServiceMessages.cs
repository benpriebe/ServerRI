#region Using directives

using Api.Common;

#endregion


namespace Services
{
    public static class ServiceMessages
    {
        public static readonly Message ProductNotFound = new Message(MessageLevel.Error, 101, "Product Not Found");
    }
}