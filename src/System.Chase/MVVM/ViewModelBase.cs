using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Chase.Mvvm
{
    public class ObservableObject : INotifyPropertyChanged
    {
        private static readonly Guid _defaultTracker = new Guid("A9614032-C10A-4D6A-9D82-4987F638F718");
        private readonly IList<Guid> _busyLocks = new List<Guid>();
        private bool _suppressChangeNotifications;
        private readonly HashSet<string> _suppressedChangedProperties = new HashSet<string>();

        public bool IsNotBusy => !IsBusy;
        public bool IsBusy
        {
            get => _busyLocks.Any();
            set
            {
                if (value && !_busyLocks.Contains(_defaultTracker))
                {
                    _busyLocks.Add(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                    RaisePropertyChanged(nameof(IsNotBusy));
                }

                if (!value && _busyLocks.Contains(_defaultTracker))
                {
                    _busyLocks.Remove(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                    RaisePropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable Busy(int delayInMs = 0) => new BusyHelper(this, delayInMs);
        
        public IDisposable SuppressChangeNotifications() => new SuppressChangeHelper(this);

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (_suppressChangeNotifications) 
            {
                _suppressedChangedProperties.Add(propertyName);
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Checks if a property already matches a desired value. Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners. This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        /// <remarks>
        ///     Taken from Prism <see href="https://github.com/PrismLibrary/Prism" />
        /// </remarks>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        ///     Checks if a property already matches a desired value. Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners. This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        /// <remarks>
        ///     Taken from Prism <see href="https://github.com/PrismLibrary/Prism" />
        /// </remarks>
        protected bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            var result = SetProperty(ref storage, value, propertyName);
            onChanged?.Invoke();
            return result;
        }
        
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
            => Internal.RunSafe.RunSafeImpl(action, handleErrorAction);

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
        protected Task RunSafeAsync(Func<Task> task, Action<Exception> handleErrorAction) 
            => Internal.RunSafe.RunSafeImplAsync(task, handleErrorAction);

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
        protected Task<T> RunSafeAsync<T>(Func<Task<T>> task, Action<Exception> handleErrorAction) 
            => Internal.RunSafe.RunSafeImplAsync(task, handleErrorAction);
        
        private sealed class SuppressChangeHelper : IDisposable
        {
            private readonly ObservableObject _viewModel;
            private static readonly object _lock = new object();
            
            internal SuppressChangeHelper(ObservableObject viewModel)
            {
                _viewModel = viewModel;
                _viewModel._suppressChangeNotifications = true;
            }

            public void Dispose()
            {
                lock (_lock)
                {
                    _viewModel._suppressChangeNotifications = false;
                
                    foreach (var property in _viewModel._suppressedChangedProperties)
                        _viewModel.RaisePropertyChanged(property);
                
                    _viewModel._suppressedChangedProperties.Clear();
                }
            }
        }

        private sealed class BusyHelper : IDisposable
        {
            private readonly int _delayInMs;
            private readonly ObservableObject _viewModel;
            private bool _delayed;
            private Guid _tracker;

            internal BusyHelper(ObservableObject viewModel, int delayInMs)
            {
                _viewModel = viewModel;
                if (delayInMs <= 0)
                {
                    StartBusy();
                }
                else
                {
                    _delayed = true;
                    _delayInMs = delayInMs;
                    Task.Delay(delayInMs).ContinueWith(_ =>
                    {
                        StartBusy();
                        _delayed = false;
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }

                void StartBusy()
                {
                    _tracker = new Guid();
                    _viewModel._busyLocks.Add(_tracker);
                    _viewModel.RaisePropertyChanged(nameof(IsBusy));
                    _viewModel.RaisePropertyChanged(nameof(IsNotBusy));
                }
            }

            public async void Dispose()
            {
                while (_delayed) await Task.Delay(_delayInMs).ConfigureAwait(false);
                _viewModel._busyLocks.Remove(_tracker);
                _viewModel.RaisePropertyChanged(nameof(IsBusy));
                _viewModel.RaisePropertyChanged(nameof(IsNotBusy));
            }
        }
    }
}
