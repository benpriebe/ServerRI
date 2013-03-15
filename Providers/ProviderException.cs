#region Using directives

using System;
using System.Collections.Generic;

#endregion


namespace Providers
{
    public class ProviderException : Exception
    {
        public ProviderException(string message, IEnumerable<String> errors) : base(message)
        {
            Errors = errors;
        }

        public ProviderException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new List<string>();
        }

        public ProviderException(string message) : base(message)
        {
            Errors = new List<string>();
        }

        public ProviderException()
        {
            Errors = new List<string>();
        }

        public IEnumerable<string> Errors { get; set; }
    }
}