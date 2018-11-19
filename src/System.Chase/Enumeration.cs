using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable PossibleNullReferenceException

namespace System.Chase
{   
    public class Enumeration : IComparable
    {
        protected Enumeration() : this(0, null) { }
    
        protected Enumeration(int value, string displayName)
        {
            // Protected to prevent direct use (similar to abstract) 
            Value = value;
            DisplayName = displayName;
        }
        
        public int Value { get; }

        public string DisplayName { get; }

        public override string ToString() => DisplayName;
    
        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is Enumeration enumeration)) return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Value.Equals(enumeration.Value);
    
            return typeMatches && valueMatches;
        }

        public static bool operator == (Enumeration left, Enumeration right) => left.Equals(right);

        public static bool operator != (Enumeration left, Enumeration right) => !left.Equals(right);

        public static bool operator == (int left, Enumeration right) => left == right.Value;

        public static bool operator != (int left, Enumeration right) => left != right.Value;

        public static bool operator == (Enumeration left, int right) => left.Value == right;

        public static bool operator != (Enumeration left, int right) => left.Value != right;

        public static bool operator == (string left, Enumeration right) => left == right.DisplayName;

        public static bool operator != (string left, Enumeration right) => left != right.DisplayName;

        public static bool operator == (Enumeration left, string right) => left.DisplayName == right;

        public static bool operator != (Enumeration left, string right) => left.DisplayName != right;
        
        public static IEnumerable<TEnumeration> GetAll<TEnumeration>() where TEnumeration : Enumeration, new()
        {
            var type = typeof(TEnumeration);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new TEnumeration();

                if (info.GetValue(instance) is TEnumeration locatedValue)
                {
                    yield return locatedValue;
                }
            }
        }
    
        public static TEnumeration FromValue<TEnumeration>(int value) where TEnumeration : Enumeration, new() 
            => ParseInternal<TEnumeration, int>(value, "value", item => item.Value == value);

        public static TEnumeration FromDisplayName<TEnumeration>(string displayName) where TEnumeration : Enumeration, new() 
            => ParseInternal<TEnumeration, string>(displayName, "display name", item => item.DisplayName == displayName);

        public int CompareTo(object other) => Value.CompareTo(((Enumeration)other).Value);

        public static TEnumeration FromValueOrDefault<TEnumeration>(int value) where TEnumeration : Enumeration, new()
        {
            TryParse(item => item.Value == value, out TEnumeration enumeration);
            return enumeration;
        }

        public static TEnumeration FromDisplayNameOrDefault<TEnumeration>(string displayName) where TEnumeration : Enumeration, new()
        {
            TryParse(item => item.DisplayName == displayName, out TEnumeration enumeration);
            return enumeration;
        }

        public static bool TryParse<TEnumeration>(Func<TEnumeration, bool> predicate, out TEnumeration enumeration) where TEnumeration : Enumeration, new()
        {
            enumeration = GetAll<TEnumeration>().FirstOrDefault(predicate);

            if (!(enumeration is null)) return true;
            
            enumeration = new TEnumeration();
            return false;
        }

        public static TEnumeration Parse<TEnumeration>(Func<TEnumeration, bool> predicate) where TEnumeration : Enumeration, new()
        {
            if (TryParse(predicate, out var enumeration)) 
                return enumeration;

            throw new ArgumentException($"Failed to parse {typeof(TEnumeration)}");
        }
        
        private static TEnumeration ParseInternal<TEnumeration, TType>(TType value, string description, Func<TEnumeration, bool> predicate) where TEnumeration : Enumeration, new()
        {
            if (TryParse(predicate, out var enumeration))
                return enumeration;

            throw new ArgumentException($"Failed to parse {typeof(TEnumeration)} because '{value}' is not a valid {description}");
        }
    }
}
