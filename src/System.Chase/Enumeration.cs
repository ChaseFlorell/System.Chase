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
        
        public static IEnumerable<TEnumeration> GetAll<TEnumeration>() where TEnumeration : Enumeration
        {
            var type = typeof(TEnumeration);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = Construct<TEnumeration>(null, null);

                if (info.GetValue(instance) is TEnumeration locatedValue)
                {
                    yield return locatedValue;
                }
            }
        }
    
        public static TEnumeration FromValue<TEnumeration>(int value) where TEnumeration : Enumeration
            => ParseInternal<TEnumeration, int>(value, "value", item => item.Value == value);

        public static TEnumeration FromDisplayName<TEnumeration>(string displayName) where TEnumeration : Enumeration
            => ParseInternal<TEnumeration, string>(displayName, "display name", item => item.DisplayName == displayName);

        public int CompareTo(object other) => Value.CompareTo(((Enumeration)other).Value);

        public static TEnumeration FromValueOrDefault<TEnumeration>(int value, int defaultValue) where TEnumeration : Enumeration
        {
            TryParse(item => item.Value == value, defaultValue, out TEnumeration enumeration);
            return enumeration;
        }

        public static bool TryParse<TEnumeration>(Func<TEnumeration, bool> predicate, int defaultValue, out TEnumeration enumeration) where TEnumeration : Enumeration
        {
            enumeration = GetAll<TEnumeration>().FirstOrDefault(predicate);

            if (!(enumeration is null)) return true;

            var types = new[] {typeof(int), typeof(string)};
            var values = new object[] {defaultValue, null};
            enumeration = Construct<TEnumeration>(types, values);
            return false;
        }        
        
        public static bool TryParse<TEnumeration>(Func<TEnumeration, bool> predicate, out TEnumeration enumeration) where TEnumeration : Enumeration
        {
            enumeration = GetAll<TEnumeration>().FirstOrDefault(predicate);

            if (!(enumeration is null)) return true;
            
            enumeration = Construct<TEnumeration>(null,null);
            return false;
        }

        public static TEnumeration Parse<TEnumeration>(Func<TEnumeration, bool> predicate) where TEnumeration : Enumeration
        {
            if (TryParse(predicate, out var enumeration)) 
                return enumeration;

            throw new ArgumentException($"Failed to parse {typeof(TEnumeration)}");
        }
        
        private static TEnumeration ParseInternal<TEnumeration, TType>(TType value, string description, Func<TEnumeration, bool> predicate) where TEnumeration : Enumeration
        {
            if (TryParse(predicate, out var enumeration))
                return enumeration;

            throw new ArgumentException($"Failed to parse {typeof(TEnumeration)} because '{value}' is not a valid {description}");
        }
        
        private static T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            paramTypes = paramTypes ?? new Type[0];
            var constructor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, 
                paramTypes, 
                null);

            if (!(constructor is null)) return (T) constructor.Invoke(paramValues);
            
            var errorMessage = paramTypes.Any() 
                ? $"{typeof(T).FullName} must have a private constructor as `{typeof(T).Name}({string.Join(", ", paramTypes.Select(p => p.Name))})`" 
                : $"{typeof(T).FullName} must have an empty private constructor";
                
            throw new Exception(errorMessage);
        }
    }
}
