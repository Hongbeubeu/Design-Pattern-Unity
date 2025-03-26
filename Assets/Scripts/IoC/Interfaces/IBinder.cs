using System;

namespace hcore.IoC
{
    public interface IBinder
    {
        IBinding Bind(Type type);
        IBinding Bind<TContract>();
        bool Unbind(Type type);
        bool Unbind<TContract>();
        bool Unbind(Type type, object id);
        bool Unbind<TContract>(object id);
    }
}