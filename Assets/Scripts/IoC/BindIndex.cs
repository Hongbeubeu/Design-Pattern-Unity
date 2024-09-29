using System;

namespace IoC
{
    public struct BindIndex : IEquatable<BindIndex>
    {
        public Type Type { get; }
        public object Id { get; }

        public BindIndex(IBinding binding) : this(binding.ContractType, binding.Id)
        {
        }

        public BindIndex(Type contractType, object id)
        {
            Type = contractType;
            Id = id;
        }

        public bool Equals(BindIndex other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is BindIndex index)
            {
                return this == index;
            }

            return false;
        }

        public static bool operator ==(BindIndex left, BindIndex right)
        {
            return left.Type == right.Type &&
                   (left.Id == null && right.Id == null || left.Id != null && right.Id != null && left.Id.ToString() == right.Id.ToString());
        }

        public static bool operator !=(BindIndex left, BindIndex right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"Type: <b>{Type.Name}</b> Id: <b>{Id}</b> HashCode: <b>{GetHashCode()}</b>";
        }
    }
}