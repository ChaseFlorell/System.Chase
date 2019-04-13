namespace System.Chase
{
    // todo: <chaseflorell: April 13, 2019> convert this to an extension type when C# 8 becomes a thing.
    public static class DateTimeHelper
    {
        public static DateTime MinSqlValue { get;  } = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime Epoch { get;  } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
