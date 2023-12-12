using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    public abstract class GenericEnum<T> : IComparable<GenericEnum<T>> where T : IEquatable<T>, IComparable<T>
    {
        protected GenericEnum(T id, string name)
        {
            (Id, Name) = (id, name);
        }

        public T Id { get; }
        public string Name { get; }

        public int CompareTo(GenericEnum<T> other)
        {
            if (other == null) return 1;
            return Id.CompareTo(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<GenericEnum<T>> GetAll<TEnum>() where TEnum : GenericEnum<T>
        {
            return typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<GenericEnum<T>>();
        }

        public static E FromId<E>(T id) where E : GenericEnum<T>
        {
            var matchingItem = GetAll<E>().FirstOrDefault(item => Equals(item.Id, id));

            if (matchingItem == null)
                throw new InvalidOperationException($"No StringEnumeration found with Name: {id}");

            return (E)matchingItem;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GenericEnum<T> otherValue))
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public static bool operator ==(GenericEnum<T> left, GenericEnum<T> right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(GenericEnum<T> left, GenericEnum<T> right)
        {
            return !(left == right);
        }

        public static bool operator <(GenericEnum<T> left, GenericEnum<T> right)
        {
            if (left is null) return right != null;
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(GenericEnum<T> left, GenericEnum<T> right)
        {
            if (left is null) return true;
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(GenericEnum<T> left, GenericEnum<T> right)
        {
            return !(left <= right);
        }

        public static bool operator >=(GenericEnum<T> left, GenericEnum<T> right)
        {
            return !(left < right);
        }
    }
}