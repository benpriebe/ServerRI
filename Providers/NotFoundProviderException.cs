using System;
using System.Collections.Generic;

namespace Providers
{
    public class NotFoundProviderException : ProviderException
    {
        public NotFoundProviderException()
        {
        }

        public NotFoundProviderException(string message) : base(message)
        {
        }

        public NotFoundProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotFoundProviderException(string message, IEnumerable<string> errors) : base(message, errors)
        {
        }
    }
}