using System.Threading.Tasks;
using System.Windows.Input;

namespace System.Chase.Commands
{
    public abstract class CommandBase : ICommand
    {
        public abstract bool CanExecute(object parameter);
        public event EventHandler CanExecuteChanged;
        public abstract void Execute(object parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        ///     Implement this if you wish to do something with your exceptions.
        /// </summary>
        /// <param name="ex">The thrown exception</param>
        protected virtual void OnError(Exception ex) { }

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafe to have any exceptions automagically handled for you
        /// </summary>
        /// <param name="action">Action to run</param>
        protected void RunSafe(Action action) => RunSafe(action, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafe to have any exceptions automagically handled for you
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected void RunSafe(Action action, Action<Exception> handleErrorAction)
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

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <param name="task">Task to run</param>
        protected Task RunSafeAsync(Func<Task> task) => RunSafeAsync(task, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <param name="task">Task to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected async Task RunSafeAsync(Func<Task> task, Action<Exception> handleErrorAction)
        {
            try
            {
                await task().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                handleErrorAction?.Invoke(ex);
            }
        }

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="task">Task to run</param>
        protected Task<T> RunSafeAsync<T>(Func<Task<T>> task) => RunSafeAsync(task, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="task">Task to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected async Task<T> RunSafeAsync<T>(Func<Task<T>> task, Action<Exception> handleErrorAction)
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

            return default(T);
        }
    }
}