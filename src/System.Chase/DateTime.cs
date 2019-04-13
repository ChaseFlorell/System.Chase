namespace System.Chase
{
    // todo: <chaseflorell: April 13, 2019> convert this to an extension type when C# 8 becomes a thing.
    public static class DateTime
    {
        public static System.DateTime MinSqlValue { get;  } = new System.DateTime(1753, 1, 1, 0, 0, 0);
        public static System.DateTime Epoch { get;  } = new System.DateTime(1970, 1, 1, 0, 0, 0);
    }
}
