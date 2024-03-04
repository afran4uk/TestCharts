using System.Windows;
using System.Windows.Threading;

namespace Charts.Utils
{
    /// <summary>
    /// Set of methods for working with the main thread.
    /// </summary>
    public static class DispatcherUtils
    {
        private static Dispatcher? MainThreadDispatcher => Application.Current?.Dispatcher;

        /// <summary>
        /// Asynchronously execute an action on the main thread.
        /// </summary>
        /// <remarks>
        /// If called from the main thread, the action will be executed synchronously.
        /// </remarks>
        public static Task ExecuteMainThreadAsync(Action action)
        {
            return ExecuteMainThreadAsync(() =>
            {
                action.Invoke();
                return true;
            });
        }

        /// <summary>
        /// Asynchronously execute an action on the main thread.
        /// </summary>
        public static async Task ExecuteMainThreadAsync(Func<Task> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (MainThreadDispatcher?.CheckAccess() != false)
            {
                await action.Invoke().ConfigureAwait(false);
                return;
            }

            // First, we await a task that will hitch onto the main thread and return an internal (main) task.
            // Then we await the internal task.
            var mainTask = await MainThreadDispatcher
                .InvokeAsync(action)
                .Task
                .ConfigureAwait(false);
            await mainTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously execute an action on the main thread.
        /// </summary>
        /// <remarks>
        /// If called from the main thread, the action will be executed synchronously.
        /// </remarks>
        public static Task<T> ExecuteMainThreadAsync<T>(Func<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (MainThreadDispatcher?.CheckAccess() != false)
            {
                return Task.FromResult(action.Invoke());
            }

            return MainThreadDispatcher.InvokeAsync(action).Task;
        }

        /// <summary>
        /// Execute an action on the main thread.
        /// </summary>
        public static void ExecuteMainThread(Action action)
        {
            ExecuteMainThread(() =>
            {
                action.Invoke();
                return true;
            });
        }

        /// <summary>
        /// Execute an action on the main thread.
        /// </summary>
        private static T ExecuteMainThread<T>(Func<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (MainThreadDispatcher?.CheckAccess() != false)
            {
                return action.Invoke();
            }

            return MainThreadDispatcher.Invoke(action);
        }
    }

}
