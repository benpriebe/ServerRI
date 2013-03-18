#region Using directives

using System;
using System.Reflection;
using Common.Logging;

#endregion


namespace Core.Extensions
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILog log, Type type, MethodBase method, OperationContext context, string message)
        {
            log.Debug(m => m.Invoke("{2} : {0}:{1} - {3} ", type.Name, method.Name, context.UserDetails.LogName, message));
        }

        public static void Info(this ILog log, Type type, MethodBase method, OperationContext context, string message)
        {
            log.Info(m => m.Invoke("{2} : {0}:{1} - {3} ", type.Name, method.Name, context.UserDetails.LogName, message));
        }

        public static void Warn(this ILog log, Type type, MethodBase method, OperationContext context, string message)
        {
            log.Warn(m => m.Invoke("{2} : {0}:{1} - {3} ", type.Name, method.Name, context.UserDetails.LogName, message));
        }

        public static void Enter(this ILog log, Type type, MethodBase method, OperationContext context)
        {
            Debug(log, type, method, context, String.Format("Entered {0} ", String.Empty));
        }

        public static void Enter(this ILog log, Type type, MethodBase method, OperationContext context, string message)
        {
            Debug(log, type, method, context, String.Format("Entered {0} ", message));
        }

        public static void Exit(this ILog log, Type type, MethodBase method, OperationContext context)
        {
            Debug(log, type, method, context, String.Format(" Exited {0} ", String.Empty));
        }

        public static void Exit(this ILog log, Type type, MethodBase method, OperationContext context, string message)
        {
            Debug(log, type, method, context, String.Format(" Exited {0} ", message));
        }

        public static void Exception(this ILog log, Type type, MethodBase method, OperationContext context, Exception e)
        {
            log.Error(m => m.Invoke("{2} : {0}:{1} - Exception {3} ", type.Name, method.Name, context.UserDetails.LogName, String.Empty), e);
        }

        public static void Exception(this ILog log, Type type, MethodBase method, OperationContext context, Exception e, string message)
        {
            log.Error(m => m.Invoke("{2} : {0}:{1} - Exception {3} ", type.Name, method.Name, context.UserDetails.LogName, message), e);
        }
    }
}