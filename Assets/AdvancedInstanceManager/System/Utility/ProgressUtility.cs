using System;
using System.Threading.Tasks;
using UnityEditor;

namespace InstanceManager.Utility
{

    /// <summary>
    /// <para>Provides functions for running tasks that display progress.</para>
    /// <para>Note that progress is only displayed in Unity 2020 and higher, task will still run in earlier versions of unity, but no progress will be shown.</para>
    /// </summary>
    public static class ProgressUtility
    {

        /// <summary>Shows progress in unity for a task. Only inderminate progress bar is supported.</summary>
        /// <param name="displayName">The display name for the progress item.</param>
        /// <param name="task">The task that display progress for. The task cannot be running already.</param>
        /// <param name="onComplete">The callback to be invoked when task is done (<paramref name="minDisplayTime"/> has no effect).</param>
        /// <param name="description">The description for the progress item.</param>
        /// <param name="minDisplayTime">The minimum display time of the progress bar, makes sure that the progress is displayed and readable, instead of just flickering.</param>
        /// <param name="canRun">Prevents the task from running and does not create a progress item if false.</param>
        /// <param name="hideError">Prevents the error from getting logged.</param>
        /// <param name="hideProgress">Prevents progress from being shown.</param>
        public static async Task RunTask(string displayName, Task task, Action<Task> onComplete = null, string description = null, int minDisplayTime = 250, bool canRun = true, bool hideProgress = false, bool hideError = false)
        {

            if (!canRun)
                return;

#if !UNITY_2018 && !UNITY_2019

            System.Diagnostics.Stopwatch watch = null;
            int? progress = null;

            if (!hideProgress)
            {
                progress = Progress.Start(displayName, description, options: Progress.Options.Indefinite);
                watch = new System.Diagnostics.Stopwatch();
                watch.Start();
            }
#endif

            try
            {
                task.Start();
                await task;
            }
            catch (Exception e)
            {
                if (!hideError)
                    UnityEngine.Debug.LogError(e);
            }

            EditorApplication.delayCall += () => onComplete?.Invoke(task);
            EditorApplication.QueuePlayerLoopUpdate();

#if !UNITY_2018 && !UNITY_2019
            if (!hideProgress)
            {
                watch?.Stop();
                //Make sure that progress is actually readable, rather than just a flicker
                if (watch.ElapsedMilliseconds < minDisplayTime)
                    await Task.Delay(TimeSpan.FromMilliseconds(minDisplayTime - watch?.ElapsedMilliseconds ?? 0));
                if (progress.HasValue) Progress.Remove(progress.Value);
            }
#endif

        }

    }

}
