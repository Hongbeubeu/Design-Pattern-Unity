using System;

namespace hcore.IoC
{
    public class BindingException : Exception
    {
        public Type ContractType { get; }
        public object Id { get; }

        public BindingException(string message) : base(message)
        {
        }

        public BindingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BindingException(Type contractType, string message) : this(contractType, null, message)
        {
        }

        public BindingException(Type contractType, object id, string message) : base(message)
        {
            ContractType = contractType;
            Id = id;
        }
    }
}