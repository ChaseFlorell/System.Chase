using System.Threading.Tasks;

namespace System.Chase.Internal
{
    internal static class RunSafeHelper
    {
        internal static async Task<T> RunSafeImplAsync<T>(Func<Task<T>> task, Action<Exception> handleErrorAction)
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
        
        internal static async Task RunSafeImplAsync(Func<Task> task, Action<Exception> handleErrorAction)
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
        
        internal static void RunSafeImpl(Action action, Action<Exception> handleErrorAction)
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
