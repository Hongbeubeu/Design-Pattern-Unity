using System;
using System.Collections.Generic;
using UnityEngine;

namespace hcore.IoC.Services
{
    public class ActionService : BaseService, IActionService
    {
        protected class NullObject
        {
            //Empty
        }

        protected class ListenerPriorityComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return y.CompareTo(x);
            }
        }

        protected static readonly NullObject GLOBAL_OBJECT = new();
        protected readonly ListenerPriorityComparer comparer = new();
        protected readonly Dictionary<Type, Dictionary<object, SortedList<int, Delegate>>> delegates = new();

        private IActionResolver _resolver;

        public override void Inject(IResolver initResolver)
        {
            base.Inject(initResolver);
            _resolver = initResolver.Resolve<IActionResolver>();
        }

        public virtual T Get<T>() where T : class, IAction
        {
            var action = _resolver.Resolve<T>();
            action.ResetAction();
            return action;
        }

        public virtual void Subscribe<T>(Action<T> listener, int priority = ListenerPriority.MEDIUM) where T : class, IAction
        {
            RegisterHandler(listener, GLOBAL_OBJECT, priority);
        }

        public virtual void Subscribe<T>(Action<T> listener, object instance, int priority = ListenerPriority.MEDIUM) where T : class, IAction
        {
            RegisterHandler(listener, instance, priority);
        }

        public virtual void Unsubscribe<T>(Action<T> listener) where T : class, IAction
        {
            UnregisterHandler(listener, GLOBAL_OBJECT);
        }

        public void Unsubscribe<T>(Action<T> listener, object instance) where T : class, IAction
        {
            UnregisterHandler(listener, instance);
        }

        public void Dispatch<T>() where T : class, IAction
        {
            DispatchAction(Get<T>(), GLOBAL_OBJECT);
        }

        public void Dispatch<T>(object instance) where T : class, IAction
        {
            DispatchAction(Get<T>(), instance);
        }

        public void Dispatch<T>(T actionTrigger) where T : class, IAction
        {
            DispatchAction(actionTrigger, GLOBAL_OBJECT);
        }

        public void Dispatch<T>(T actionTrigger, object instance) where T : class, IAction
        {
            DispatchAction(actionTrigger, instance);
        }

        #region Handler Registration

        protected virtual void RegisterHandler<T>(Action<T> listener, object instance, int priority) where T : class, IAction
        {
            if (instance == null)
            {
                Debug.LogWarning($"Tried to {nameof(Subscribe)} to a null instance. No subscription was registered. Either pass a non null instance or use the globally {nameof(Subscribe)}");
                return;
            }

            var actionType = typeof(T);

            // If new action, create and add type actionDelegate.
            if (!delegates.ContainsKey(actionType))
            {
                delegates.Add(actionType, new Dictionary<object, SortedList<int, Delegate>>());
            }

            // If new instance, create and add instanceDelegate.
            if (!delegates[actionType].ContainsKey(instance))
            {
                delegates[actionType].Add(instance, new SortedList<int, Delegate>(comparer));
            }

            // If new priority, add the listener.
            if (!delegates[actionType][instance].ContainsKey(priority))
            {
                delegates[actionType][instance].Add(priority, listener);
            }
            else
            {
                // If priority existed, add onto the listener delegate.
                delegates[actionType][instance][priority] = (Action<T>)delegates[actionType][instance][priority] + listener;
            }
        }

        protected virtual void UnregisterHandler<T>(Action<T> listener, object instance) where T : class, IAction
        {
            if (instance == null)
            {
                Debug.LogWarning($"Tried to {nameof(Unsubscribe)} to a null instance. Either pass a non null instance or use the globally {nameof(Unsubscribe)}");
                return;
            }

            var type = typeof(T);

            if (!delegates.TryGetValue(type, out var typeDelegates) || !typeDelegates.TryGetValue(instance, out var instanceDelegates)) return;

            var priorityLen = instanceDelegates.Count;
            var priorityCopies = new int[priorityLen];
            instanceDelegates.Keys.CopyTo(priorityCopies, 0);

            for (var i = 0; i < priorityLen; i++)
            {
                var priority = priorityCopies[i];
                instanceDelegates[priority] = (Action<T>)instanceDelegates[priority] - listener;

                Debug.Log($"Unsubscribed {listener} from {instance} at priority {ListenerPriority.GetPriorityName(priority)}: {priority}");

                if (instanceDelegates[priority] == null)
                {
                    instanceDelegates.Remove(priority);
                }
            }

            if (instanceDelegates.Count == 0)
            {
                typeDelegates.Remove(instance);
            }

            if (typeDelegates.Count == 0)
            {
                delegates.Remove(type);
            }
        }


        protected virtual void DispatchAction<T>(T actionTrigger, object instance) where T : class, IAction
        {
            if (instance == null)
            {
                Debug.LogWarning($"Tried to {nameof(Dispatch)} to a null instance. Either pass a non null instance or use the globally {nameof(Dispatch)}");
                return;
            }

            var stateAction = actionTrigger as IStateAction;
            stateAction?.Apply();


            DispatchListener(actionTrigger, GLOBAL_OBJECT);
            if (instance != GLOBAL_OBJECT)
            {
                DispatchListener(actionTrigger, instance);
            }
        }

        protected virtual void DispatchListener<T>(T actionTrigger, object instance) where T : class, IAction
        {
            var dispatchAs = actionTrigger.TypesToDispatchAs;
            var len = actionTrigger.TypesToDispatchAs.Length;
            for (var i = 0; i < len; i++)
            {
                var key = dispatchAs[i];
                if (!key.IsInstanceOfType(actionTrigger))
                {
                    Debug.LogWarning($"Failed to dispatch {key} as {actionTrigger.GetType()}. The action does not match the type.");
                }

                if (!delegates.TryGetValue(key, out var typeDelegates) || !typeDelegates.TryGetValue(instance, out var instanceDelegates)) continue;

                var priorityLen = instanceDelegates.Count;
                var priorityCopies = new int[priorityLen];
                instanceDelegates.Keys.CopyTo(priorityCopies, 0);

                for (var j = 0; j < priorityLen; j++)
                {
                    InvokeListener(actionTrigger, instanceDelegates[priorityCopies[j]] as Action<T>);
                }
            }
        }

        protected virtual void InvokeListener<T>(T actionTrigger, Action<T> listener)
        {
            if (listener == null)
            {
                Debug.LogError($"Null {nameof(listener)} cannot be notified.");
                return;
            }

            listener.Invoke(actionTrigger);
        }

        #endregion

        protected override void InitializeService()
        {
            // Empty
        }

        protected override void CleanupService()
        {
            delegates.Clear();
        }
    }
}