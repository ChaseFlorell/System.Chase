using System;
using System.Reflection;

namespace System.Chase.Events
{
    public sealed class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;

        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            _method = callback.GetMethodInfo();
            _targetReference = new WeakReference(callback.Target, true);
        }

        public void Handler(object sender, TEventArgs e)
        {
            var target = _targetReference.Target;
            if (target == null) return;

            var callback = (Action<object, TEventArgs>)_method.CreateDelegate(typeof(Action<object, TEventArgs>),target);
            callback?.Invoke(sender, e);
        }
    }
}
