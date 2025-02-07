using System;
using System.Collections;
using System.Threading.Tasks;
namespace Mimizh.UnityUtilities
{
    public static class TaskExtensions
    {
        public static Task<T> AsCompletedTask<T>(this T result) => Task.FromResult(result);
        
        /// <summary>
        /// Converts the Task into an IEnumerator for Unity coroutine usage.
        /// </summary>
        /// <param name="task">The Task to convert.</param>
        /// <returns>An IEnumerator representation of the Task.</returns>
        public static IEnumerator AsCoroutine(this Task task) {
            while (!task.IsCompleted) yield return null;
            // When used on a faulted Task, GetResult() will propagate the original exception. 
            // see: https://devblogs.microsoft.com/pfxteam/task-exception-handling-in-net-4-5/
            task.GetAwaiter().GetResult();
        }
        
        
    }
    
    
}