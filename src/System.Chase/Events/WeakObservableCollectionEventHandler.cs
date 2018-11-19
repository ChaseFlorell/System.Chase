using System.Reflection;

namespace System.Chase.Events
{
    public sealed class WeakObservableCollectionEventHandler
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;
        public WeakObservableCollectionEventHandler(EventHandler<Collections.Specialized.NotifyCollectionChangedEventArgs> callback)
        {
            _method = callback.GetMethodInfo();
            _targetReference = new WeakReference(callback.Target, true);
        }
        public void Handler(object sender, Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var target = _targetReference.Target;
            if (target == null) return;
            var callback = (Action<object, Collections.Specialized.NotifyCollectionChangedEventArgs>)_method.CreateDelegate(typeof(Action<object, Collections.Specialized.NotifyCollectionChangedEventArgs>), target);
            callback?.Invoke(sender, e);
        }
    }
}
