#region Using directives

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

#endregion


namespace Providers
{
    public class EFProviderException : ProviderException
    {
        public EFProviderException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = BuildErrors(innerException, new List<string>());
        }

        private IEnumerable<string> BuildErrors(Exception innerException, List<string> errors)
        {
            if (innerException == null)
                return errors;

            var sqlException = innerException as SqlException;
            if (sqlException != null)
            {
                errors.AddRange(sqlException.Errors.Cast<SqlError>().Select(error => error.ToString()));
                return errors;
            }
            return BuildErrors(innerException.InnerException, errors);
        }
    }
}