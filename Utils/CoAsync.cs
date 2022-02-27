using System;
using System.Collections;
using System.Threading.Tasks;

namespace KiraiMod.Core.Utils
{
    public static class CoAsync
    {
        public static IEnumerator Create(Task task, Action callback)
        {
            while (!task.IsCompleted)
                yield return null;
            callback();
        }

        public static IEnumerator Create<T>(Task<T> task, Action<T> callback)
        {
            while (!task.IsCompleted)
                yield return null;
            callback(task.Result);
        }
    }
}
