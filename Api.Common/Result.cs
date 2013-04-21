#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

        public bool NotFound
        {
            get { return Messages != null && Messages.Any(m => m.Code == (int)MessageCodes.NotFound); }
        }

        public bool Unauthorized
        {
            get { return Messages != null && Messages.Any(m => m.Code == (int)MessageCodes.Unauthorized); }
        }

        public List<Message> Messages { get; set; }

        public string ToMessageText(string delimiter = "\n")
        {
            return string.Join(delimiter, Messages.Select(m => string.Format("{0}({1}): {2}", m.Level, m.Code, String.Format(m.Phrase, m.Tokens)).ToArray()));
        }

        public static Result CreateEmpty()
        {
            return new Result
            {
                Success = true
            };
        }

        public static Result CreateNotFound<TEntity>(object identity) where TEntity : class
        {
            return Create(new[]
            {
                new Message(MessageLevel.Error, (int) MessageCodes.NotFound, "{0} - {1}", typeof(TEntity).Name, identity.ToString())
            });
        }

        public static Result CreateValidationErrors(params ValidationResult[] errors)
        {
            return Create(errors.ToList().Select((vr) => new Message(MessageLevel.Error, (int)MessageCodes.ValidationError, vr.ErrorMessage, vr.MemberNames.ToArray())));
        }

        public static Result Create(Message message)
        {
            return Create(new[]
            {
                message
            });
        }

        public static Result Create(params Message[] messages)
        {
            return Create(messages.ToList());
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

        public bool NotFound
        {
            get { return Messages != null && Messages.Any(m => m.Code == (int)MessageCodes.NotFound); }
        }

        public bool Unauthorized
        {
            get { return Messages != null && Messages.Any(m => m.Code == (int)MessageCodes.Unauthorized); }
        }

        public T Value { get; set; }
        public List<Message> Messages { get; set; }

        public string ToMessageText(string delimiter = "\n")
        {
            return string.Join(delimiter, Messages.Select(m => string.Format("{0}({1}): {2}", m.Level, m.Code, String.Format(m.Phrase, m.Tokens)).ToArray()));
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

        public static Result<T> CreateNotFound<TEntity>(object identity) where TEntity : class
        {
            return Create(new[]
            {
                new Message(MessageLevel.Error, (int) MessageCodes.NotFound, "{0} - {1}", typeof(TEntity).Name, identity.ToString())
            });
        }

        public static Result<T> CreateValidationErrors(params ValidationResult[] errors)
        {
            return Create(errors.ToList().Select((vr) => new Message(MessageLevel.Error, (int)MessageCodes.ValidationError, vr.ErrorMessage, vr.MemberNames.ToArray())));
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

        public static Result<T> Create(params Message[] messagees)
        {
            return Create(messagees.ToList());
        }
    }
}