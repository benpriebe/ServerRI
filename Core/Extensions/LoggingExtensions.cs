#region Using directives

using System;
using System.Reflection;
using Common.Logging;

#endregion


namespace Core.Extensions
{
    public delegate string MessageHandler(Type type, MethodBase method, OperationContext context, string message = null);
    
    public class OperationLogger : IDisposable
    {
        private readonly ILog _log;
        private readonly Func<MessageHandler, string> _messageInvoker;

        public OperationLogger(ILog log, Func<MessageHandler, string> messageInvoker)
        {
            _log = log;
            _messageInvoker = messageInvoker;

            if (_log.IsDebugEnabled)
            {
                var handler = new MessageHandler(WriteEnteredMessage);
                var message = messageInvoker(handler);
                _log.Debug(message);
            }
        }

        public void Dispose()
        {
            if (_log.IsDebugEnabled)
            {
                var handler = new MessageHandler(WriteExitedMessage);
                var message = _messageInvoker(handler);
                _log.Debug(message);
            }
        }

        private static string WriteExitedMessage(Type type, MethodBase method, OperationContext context, string message = null)
        {
            return String.Format("{0} : {1}:{2} - {3} ", context.UserDetails.LogName, type.Name, method.Name, String.IsNullOrWhiteSpace(message) ? "Exited" : String.Format("Exited with {0}", message));
        }

        private static string WriteEnteredMessage(Type type, MethodBase method, OperationContext context, string message = null)
        {
            return String.Format("{0} : {1}:{2} - {3} ", context.UserDetails.LogName, type.Name, method.Name, String.IsNullOrWhiteSpace(message) ? "Entered" : String.Format("Entered with {0}", message));
        }
    }

    
    public static class LoggingExtensions
    {
        public static void Debug(this ILog log, Func<MessageHandler, string> messageInvoker)
        {
            if (log.IsDebugEnabled)
            {
                var handler = new MessageHandler(WriteMessage);
                var message = messageInvoker(handler);
                log.Debug(message);
            }
        }

        public static void Info(this ILog log, Func<MessageHandler, string> messageInvoker)
        {
            if (log.IsInfoEnabled)
            {
                var handler = new MessageHandler(WriteMessage);
                var message = messageInvoker(handler);
                log.Info(message);
            }
        }

        public static void Warn(this ILog log, Func<MessageHandler, string> messageInvoker)
        {
            if (log.IsWarnEnabled)
            {
                var handler = new MessageHandler(WriteMessage);
                var message = messageInvoker(handler);
                log.Warn(message);
            }
        }

        public static void Error(this ILog log, Func<MessageHandler, string> messageInvoker)
        {
            if (log.IsErrorEnabled)
            {
                var handler = new MessageHandler(WriteMessage);
                var message = messageInvoker(handler);
                log.Error(message);
            }
        }

        public static void Exception(this ILog log, Func<MessageHandler, string> messageInvoker, Exception e)
        {
            if (log.IsErrorEnabled)
            {
                var handler = new MessageHandler(WriteExceptionMessage);
                var message = messageInvoker(handler);
                log.Error(message, e);
            }
        }

        private static string WriteMessage(Type type, MethodBase method, OperationContext context, string message)
        {
            return String.Format("{0} : {1}:{2} - {3} ", context.UserDetails.LogName, type.Name, method.Name, message);
        }

        private static string WriteExceptionMessage(Type type, MethodBase method, OperationContext context, string message)
        {
            return WriteMessage(type, method, context, String.Format("Exception - {0}", message));
        }
    }
}