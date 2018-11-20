using System.Threading.Tasks;

// ReSharper disable InvalidXmlDocComment
// ReSharper disable MemberCanBePrivate.Global

namespace System.Chase
{
    public abstract class SafeMethodRunner
    {
         /// <summary>
        ///     Implement this if you wish to do something with your exceptions.
        /// </summary>
        /// <param name="ex">The thrown exception</param>
        protected virtual void OnError(Exception ex) { }
        
        /// <summary>
        ///     Wrap your potentially volatile calls with <see cref="RunSafe(System.Action)"/> and have any exceptions automatically handled for you in <see cref="OnError"/>
        /// </summary>
        /// <param name="action">Action to run</param>
        protected void RunSafe(Action action) 
            => RunSafe(action, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with <see cref="RunSafe(System.Action, System.Action{System.Exception})"/> and have any exceptions automatically handled for you in <see cref="OnError"/>
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected void RunSafe(Action action, Action<Exception> handleErrorAction) 
            => RunSafeImpl(action, handleErrorAction);

        /// <summary>
        ///     Wrap your potentially volatile calls with <see cref="RunSafeAsync(System.Func{System.Threading.Tasks.Task})"/> and have any exceptions automatically handled for you in <see cref="OnError"/>
        /// </summary>
        /// <param name="task">Task to run</param>
        protected Task RunSafeAsync(Func<Task> task) 
            => RunSafeAsync(task, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with <see cref="RunSafeAsync(System.Func{System.Threading.Tasks.Task}, System.Action{System.Exception})"/> and have any exceptions automatically handled for you in <see cref="OnError"/>
        /// </summary>
        /// <param name="task">Task to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected Task RunSafeAsync(Func<Task> task, Action<Exception> handleErrorAction) 
            => RunSafeImplAsync(task, handleErrorAction);

        /// <summary>
        ///     Wrap your potentially volatile calls with <see cref="RunSafeAsync(System.Func{System.Threading.Tasks.Task{T}})"/> and have any exceptions automatically handled for you in <see cref="OnError"/>
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="task">Task to run</param>
        /// <returns><see cref="T"/></returns>
        protected Task<T> RunSafeAsync<T>(Func<Task<T>> task) 
            => RunSafeAsync(task, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with <see cref="RunSafeAsync(System.Func{System.Threading.Tasks.Task{T}}, System.Action{System.Exception})"/> and have any exceptions automatically handled for you in <see cref="OnError"/>
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="task">Task to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        /// <returns><see cref="T"/></returns>
        protected Task<T> RunSafeAsync<T>(Func<Task<T>> task, Action<Exception> handleErrorAction) 
            => RunSafeImplAsync(task, handleErrorAction);

        private static async Task<T> RunSafeImplAsync<T>(Func<Task<T>> task, Action<Exception> handleErrorAction)
        {
            try
            {
                // if you don't await this, the call will never 
                // have a chance to fail into the catch.
                return await task().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                handleErrorAction?.Invoke(ex);
            }

            return default;
        }

        private static async Task RunSafeImplAsync(Func<Task> task, Action<Exception> handleErrorAction)
        {
            try
            {
                // if you don't await this, the call will never 
                // have a chance to fail into the catch.
                await task().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                handleErrorAction?.Invoke(ex);
            }
        }

        private static void RunSafeImpl(Action action, Action<Exception> handleErrorAction)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                handleErrorAction?.Invoke(ex);
            }
        }
    }
}
