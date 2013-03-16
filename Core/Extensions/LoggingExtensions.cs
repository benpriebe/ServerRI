using System;
using System.Reflection;
using Common.Logging;

namespace Core.Extensions
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILog log, Type type, MethodBase method, string message)
        {
            log.Debug(m => m.Invoke("{0}:{1} - {2} ", type.Name, method.Name, message));
        }

        public static void Info(this ILog log, Type type, MethodBase method, string message)
        {
            log.Info(m => m.Invoke("{0}:{1} - {2} ", type.Name, method.Name, message));
        }

        public static void Warn(this ILog log, Type type, MethodBase method, string message)
        {
            log.Warn(m => m.Invoke("{0}:{1} - {2} ", type.Name, method.Name, message));
        }

        public static void Enter(this ILog log, Type type, MethodBase method)
        {
            Debug(log, type, method, String.Format("Entered {0} ", String.Empty));
        }
        
        public static void Enter(this ILog log, Type type, MethodBase method, string message)
        {
            Debug(log, type, method, String.Format("Entered {0} ", message));
        }

        public static void Exit(this ILog log, Type type, MethodBase method)
        {
            Debug(log, type, method, String.Format(" Exited {0} ", String.Empty));
        }

        public static void Exit(this ILog log, Type type, MethodBase method, string message)
        {
            Debug(log, type, method, String.Format(" Exited {0} ", message));
        }

        public static void Exception(this ILog log, Type type, MethodBase method, Exception e)
        {
            log.Error(m => m.Invoke("{0}:{1} - Exception {2} ", type.Name, method.Name, String.Empty), e);
        }

        public static void Exception(this ILog log, Type type, MethodBase method, Exception e, string message)
        {
            log.Error(m => m.Invoke("{0}:{1} - Exception {2} ", type.Name, method.Name, message), e);
        }

    }
}
