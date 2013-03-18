#region Using directives

using System.Collections.Generic;
using System.Linq;
using Api.Common;
using Providers;

#endregion


namespace Services
{
    public static class ResultExtensions
    {
        public static Result Create(ProviderException e, int errorCode)
        {
            List<Message> messages = new List<Message>();
            messages.Add(new Message
            {
                Code = errorCode, Level = MessageLevel.Error, Phrase = e.Message
            });
            e.Errors.ToList().ForEach(error => messages.Add(new Message
            {
                Code = errorCode, Level = MessageLevel.Error, Phrase = error
            }));
            return Result.Create(messages);
        }

        public static Result<TValue> Create<TValue>(ProviderException e, int errorCode)
        {
            List<Message> messages = new List<Message>();
            messages.Add(new Message
            {
                Code = errorCode, Level = MessageLevel.Error, Phrase = e.Message
            });
            e.Errors.ToList().ForEach(error => messages.Add(new Message
            {
                Code = errorCode, Level = MessageLevel.Error, Phrase = error
            }));
            return Result<TValue>.Create(messages);
        }
    }
}