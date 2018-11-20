using System.Chase.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Chase
{
    public class ObservableObject : INotifyPropertyChanged
    {
        private static readonly Guid _defaultTracker = new Guid("A9614032-C10A-4D6A-9D82-4987F638F718");
        internal readonly HashSet<string> SuppressedChangedProperties = new HashSet<string>();
        internal readonly IList<Guid> BusyLocks = new List<Guid>();
        internal bool ChangeNotificationsSuppressed;

        /// <summary>
        /// A helper property that is automatically toggled when <see cref="IsBusy"/> is changed. Simply indicates the inverse of <see cref="IsBusy"/>.
        /// </summary>
        public bool IsNotBusy => !IsBusy;
        
        /// <summary>
        /// Set this property to <c>true</c> when you want to indicate that the system is Busy and to <c>false</c> when the system is no longer busy.
        /// </summary>
        /// <remarks>It is wise to use <see cref="Busy"/> instead of toggling this property directly.</remarks>
        public bool IsBusy
        {
            get => BusyLocks.Any();
            set
            {
                if (value && !BusyLocks.Contains(_defaultTracker))
                {
                    BusyLocks.Add(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                    RaisePropertyChanged(nameof(IsNotBusy));
                }

                if (!value && BusyLocks.Contains(_defaultTracker))
                {
                    BusyLocks.Remove(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                    RaisePropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Toggle <see cref="IsBusy"/> and ensure it's set for the duration of the using.
        /// </summary>
        /// <param name="delayInMs">how long should the system delay before toggling <see cref="IsBusy"/></param>
        public IDisposable Busy(int delayInMs = 0) 
            => new BusyHelper(this, delayInMs);
        
        /// <summary>
        /// Caches all <see cref="INotifyPropertyChanged"/> notifications for the duration of the using, and raises them all at the end.
        /// </summary>
        /// <remarks>The cache uses a <see cref="HashSet{T}"/> in order to keep all changed properties unique.</remarks>
        public IDisposable SuppressChangeNotifications() 
            => new SuppressChangeHelper(this);

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event for a given property
        /// </summary>
        /// <param name="propertyName">Name of the changed property</param>
        protected internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (ChangeNotificationsSuppressed) 
            {
                SuppressedChangedProperties.Add(propertyName);
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
        protected void RunSafe(Action action) 
            => RunSafe(action, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafe to have any exceptions automagically handled for you
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected void RunSafe(Action action, Action<Exception> handleErrorAction) 
            => RunSafeHelper.RunSafeImpl(action, handleErrorAction);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <param name="task">Task to run</param>
        protected Task RunSafeAsync(Func<Task> task) 
            => RunSafeAsync(task, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <param name="task">Task to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected Task RunSafeAsync(Func<Task> task, Action<Exception> handleErrorAction) 
            => RunSafeHelper.RunSafeImplAsync(task, handleErrorAction);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="task">Task to run</param>
        protected Task<T> RunSafeAsync<T>(Func<Task<T>> task) 
            => RunSafeAsync(task, OnError);

        /// <summary>
        ///     Wrap your potentially volatile calls with RunSafeAsync to have any exceptions automagically handled for you
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="task">Task to run</param>
        /// <param name="handleErrorAction">(optional) Custom Action to invoke with the thrown Exception</param>
        protected Task<T> RunSafeAsync<T>(Func<Task<T>> task, Action<Exception> handleErrorAction) 
            => RunSafeHelper.RunSafeImplAsync(task, handleErrorAction);
    }
}
