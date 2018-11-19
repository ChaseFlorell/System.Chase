using System.Chase.Collections;

namespace System.Chase.Tests.Fixtures
{
    public class EmployeeType : Enumeration
    {
        private EmployeeType(int value, string displayName):base(value, displayName) { }
        public EmployeeType() { /*Required for the generics and implicit operators to function correctly*/ }
        
        public static readonly EmployeeType RegionalManager = new EmployeeType(1, "Regional Manager");
        public static readonly EmployeeType Sales = new EmployeeType(2, "Sales");
        public static readonly EmployeeType AssistantToTheRegionalManager = new EmployeeType(3, "Assistant to the Regional Manager");
        
        public static implicit operator EmployeeType(int value) => FromValueOrDefault<EmployeeType>(value);
        public static implicit operator EmployeeType(string displayName) => FromDisplayNameOrDefault<EmployeeType>(displayName);
    }
}
