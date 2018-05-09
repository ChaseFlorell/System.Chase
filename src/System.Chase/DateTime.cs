using SystemDateTime = System.DateTime;
namespace System.Chase
{
    public static class DateTime
    {
        public static SystemDateTime MinSqlValue => SystemDateTime.Parse("1/1/1753 12:00:00 AM"); 
    }
}
