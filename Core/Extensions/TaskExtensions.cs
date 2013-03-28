#region Using directives

using System;
using System.Reflection;
using System.Threading.Tasks;
using Common.Logging;

#endregion


namespace Core.Extensions
{
    public static class TaskExtensions
    {
        public static void PropagateParentTaskStatus(this Task task, string message)
        {
            if (task.IsCanceled)
            {
                throw new OperationCanceledException(message);
            }
            
            if (task.IsFaulted)
            {
                throw new Exception(String.Format("Task faulted: {0}", message), task.Exception);
            }

            if (!task.IsCompleted)
            {
                throw new InvalidOperationException(String.Format("Task did not complete: {0}", message));
            }
        }
    }
}