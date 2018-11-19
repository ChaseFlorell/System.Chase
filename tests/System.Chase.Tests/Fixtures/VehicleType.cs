namespace System.Chase.Tests.Fixtures
{
    public class VehicleType : Enumeration
    {        
        private VehicleType(int value, string displayName):base(value, displayName) { }
        public VehicleType() { /*Required for the generics and implicit operators to function correctly*/ }
                
        public static readonly VehicleType Unknown = new VehicleType(0, "Unknown Vehicle Type");
        public static readonly VehicleType Car = new VehicleType(1, "Car");
        public static readonly VehicleType Truck = new VehicleType(2, "Truck");
        public static readonly VehicleType Motorcycle = new VehicleType(3, "Motorcycle");
        
        public static implicit operator VehicleType(int value) => FromValueOrDefault<VehicleType>(value);
        public static implicit operator VehicleType(string displayName) => FromDisplayNameOrDefault<VehicleType>(displayName);
    }
}
