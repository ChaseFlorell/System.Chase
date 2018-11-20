namespace System.Chase.Tests.Fixtures
{
    public class EmployeeType : Enumeration
    {
        private EmployeeType(int value, string displayName):base(value, displayName) { }
        private EmployeeType() { /*Required for the generics and implicit operators to function correctly*/ }
        
        public static readonly EmployeeType RegionalManager =               new EmployeeType(1<<0, "Regional Manager");
        public static readonly EmployeeType Sales =                         new EmployeeType(1<<1, "Sales");
        public static readonly EmployeeType AssistantToTheRegionalManager = new EmployeeType(1<<2, "Assistant to the Regional Manager");
        
        public static implicit operator EmployeeType(int value) => FromValueOrDefault<EmployeeType>(value, value);
        public static implicit operator EmployeeType(string displayName) => FromDisplayName<EmployeeType>(displayName);
    }
}
