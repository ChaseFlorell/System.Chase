using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Chase.Events
{
    public class WeakEventManager
    {
        private static readonly object _lock = new object();
        private static readonly Dictionary<WeakReference<object>, WeakEventManager> _weakEventManagers = new Dictionary<WeakReference<object>, WeakEventManager>();
        private readonly Dictionary<string, List<Tuple<WeakReference, MethodInfo>>> _eventHandlers = new Dictionary<string, List<Tuple<WeakReference, MethodInfo>>>();
        private readonly object _syncObj = new object();

        private WeakEventManager() { }

        public void AddEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
            where TEventArgs : EventArgs => BuildEventHandler(eventName, value.Target, value.GetMethodInfo());

        public void AddEventHandler(string eventName, EventHandler value) => BuildEventHandler(eventName, value.Target, value.GetMethodInfo());

        public static WeakEventManager GetWeakEventManager(object source)
        {
            lock (_lock)
            {
                foreach (var kvp in _weakEventManagers.ToList())
                    if (kvp.Key.TryGetTarget(out var target))
                    {
                        if (ReferenceEquals(target, source))
                            return kvp.Value;
                    }
                    else
                    {
                        _weakEventManagers.Remove(kvp.Key);
                    }

                var manager = new WeakEventManager();
                _weakEventManagers.Add(new WeakReference<object>(source), manager);

                return manager;
            }
        }

        public void RaiseEvent(object sender, object args, string eventName)
        {
            var toRaise = new List<Tuple<object, MethodInfo>>();

            lock (_syncObj)
            {
                if (_eventHandlers.TryGetValue(eventName, out var target))
                {
                    var targetList = target.ToList();
                    for (var index = 0; index < targetList.Count; index++)
                    {
                        var tuple = targetList[index];
                        var o = tuple.Item1.Target;

                        if (o == null)
                            target.Remove(tuple);
                        else
                            toRaise.Add(Tuple.Create(o, tuple.Item2));
                    }
                }
            }

            for (var index = 0; index < toRaise.Count; index++)
            {
                var item = toRaise[index];
                item.Item2.Invoke(item.Item1, new[] {sender, args});
            }
        }

        public void RemoveEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
            where TEventArgs : EventArgs => RemoveEventHandlerImpl(eventName, value.Target, value.GetMethodInfo());

        public void RemoveEventHandler(string eventName, EventHandler value) => RemoveEventHandlerImpl(eventName, value.Target, value.GetMethodInfo());

        private void BuildEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
        {
            lock (_syncObj)
            {
                if (!_eventHandlers.TryGetValue(eventName, out var target))
                {
                    target = new List<Tuple<WeakReference, MethodInfo>>();
                    _eventHandlers.Add(eventName, target);
                }

                target.Add(Tuple.Create(new WeakReference(handlerTarget), methodInfo));
            }
        }

        private void RemoveEventHandlerImpl(string eventName, object handlerTarget, MemberInfo methodInfo)
        {
            lock (_syncObj)
            {
                if (!_eventHandlers.TryGetValue(eventName, out var target)) return;

                for (var index = 0; index < target.Where(t => t.Item1.Target == handlerTarget && t.Item2.Name == methodInfo.Name).ToList().Count; index++)
                {
                    var tuple = target.Where(t => t.Item1.Target == handlerTarget && t.Item2.Name == methodInfo.Name).ToList()[index];
                    target.Remove(tuple);
                }
            }
        }
    }

}
