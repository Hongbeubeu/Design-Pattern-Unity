using System;

namespace IoC.Services
{
    public interface IActionService
    {
        void Subscribe<T>(Action<T> listener, int priority = ListenerPriority.MEDIUM) where T : class, IAction;
        void Subscribe<T>(Action<T> listener, object instance, int priority = ListenerPriority.MEDIUM) where T : class, IAction;

        void Unsubscribe<T>(Action<T> listener) where T : class, IAction;
        void Unsubscribe<T>(Action<T> listener, object instance) where T : class, IAction;
        void Dispatch<T>() where T : class, IAction;
        void Dispatch<T>(object instance) where T : class, IAction;
        void Dispatch<T>(T actionTrigger) where T : class, IAction;

        void Dispatch<T>(T actionTrigger, object instance) where T : class, IAction;

        T Get<T>() where T : class, IAction;
    }
}