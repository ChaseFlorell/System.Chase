namespace System.Chase
{
    public static class DateTime
    {
        public static System.DateTime MinSqlValue { get;  } = new System.DateTime(1753, 1, 1, 0, 0, 0);
        public static System.DateTime Epoch { get;  } = new System.DateTime(1970, 1, 1, 0, 0, 0);
    }
}
