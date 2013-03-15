#region Using directives

using System.Collections.Generic;
using System.Linq;

#endregion


namespace Api.Common
{
    public class Result
    {
        public bool Success { get; set; }

        public bool Failure
        {
            get { return !Success; }
        }

        public List<Message> Messages { get; set; }

        public string ToMessageText(string delimiter = "\n")
        {
            return string.Join(delimiter, Messages.Select(m => string.Format("{0}({1}): {2}", m.Level, m.Code, m.Phrase)).ToArray());
        }

        public static Result CreateEmpty()
        {
            return new Result
            {
                Success = true
            };
        }

        public static Result Create(Message message)
        {
            return Create(new[]
            {
                message
            });
        }

        public static Result Create(params Message[] message)
        {
            return Create(message.ToList());
        }

        public static Result Create(IEnumerable<Message> messages)
        {
            var m = messages != null ? messages.ToList() : null;
            return new Result
            {
                Success = m == null || m.All(x => x.Level != MessageLevel.Error),
                Messages = m
            };
        }
    }

    public class Result<T>
    {
        public bool Success { get; set; }

        public bool Failure
        {
            get { return !Success; }
        }

        public T Value { get; set; }
        public List<Message> Messages { get; set; }

        public string ToMessageText(string delimiter = "\n")
        {
            return string.Join(delimiter, Messages.Select(m => string.Format("{0}({1}): {2}", m.Level, m.Code, m.Phrase)).ToArray());
        }

        public static Result<T> CreateEmpty()
        {
            return new Result<T>
            {
                Success = true
            };
        }

        public static Result<T> Create(Message message)
        {
            return Create(new[]
            {
                message
            });
        }

        public static Result<T> Create(T value)
        {
            return new Result<T>
            {
                Success = value != null,
                Value = value
            };
        }

        public static Result<T> Create(IEnumerable<Message> messages)
        {
            var m = messages != null ? messages.ToList() : null;
            return new Result<T>
            {
                Success = m == null || m.All(x => x.Level != MessageLevel.Error),
                Messages = m,
                Value = default(T)
            };
        }

        public static Result<T> Create(params Message[] messages)
        {
            return Create(messages.ToList());
        }
    }
}