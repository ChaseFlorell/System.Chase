using System.Text;

namespace System.Chase
{
    public static class SequentialGuid
    {
        private const int _numberOfDateBytes = 8;
        private const int _embedAtIndex = 0; // index 0 ensures we're sequential by date
        private static readonly System.DateTime _epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static readonly object _lock = new object();
        private static long _previousTicks;
        private static Guid _previousSeed;

        public static Guid NewGuid()
            => NewGuid(Guid.NewGuid());

        public static Guid NewGuid(string seed)
        {
            if (seed.Length < 8)
                throw new ArgumentException("Seed must be a minimum of 8 characters", nameof(seed));

            var seedBytes = Encoding.ASCII.GetBytes(seed);
            var truncated = new byte[16];

            Array.Copy(seedBytes, 0, truncated, 8, 8);

            return NewGuid(new Guid(truncated));
        }

        public static Guid NewGuid(Guid guid)
        {
            lock (_lock)
            {
                var outputBytes = guid.ToByteArray();
                var stamp = System.DateTime.UtcNow;

                // don't allow another timestamp within the same CPU Tick and Seed
                while (stamp.Ticks == _previousTicks && guid == _previousSeed) stamp = System.DateTime.UtcNow;

                _previousTicks = stamp.Ticks;
                _previousSeed = guid;

                var dateBytes = DateTimeToBytes(stamp);
                Array.Copy(dateBytes, 0, outputBytes, _embedAtIndex, _numberOfDateBytes);
                return new Guid(outputBytes);
            }
        }

        public static System.DateTime GetTimestamp(Guid guid)
        {
            lock (_lock)
            {
                var combBytes = guid.ToByteArray();
                var dateBytes = new byte[_numberOfDateBytes];
                Array.Copy(combBytes, _embedAtIndex, dateBytes, 0, _numberOfDateBytes);
                try
                {
                    var result = BytesToDateTime(dateBytes);
                    if (result <= _epoch
                        || result > System.DateTime.UtcNow.AddMinutes(1))
                        throw new InvalidOperationException($"Cannot extract a valid DateTime from {guid}");
                    return result;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new InvalidOperationException($"Cannot extract a valid DateTime from {guid}", ex);
                }
            }
        }

        private static byte[] DateTimeToBytes(System.DateTime timestamp)
        {
            var ms = ToUnixTicks(timestamp);
            var msBytes = BitConverter.GetBytes(ms);
            if (BitConverter.IsLittleEndian) Array.Reverse(msBytes);
            var result = new byte[_numberOfDateBytes];
            var index = msBytes.GetUpperBound(0) + 1 - _numberOfDateBytes;
            Array.Copy(msBytes, index, result, 0, _numberOfDateBytes);
            return result;
        }

        private static System.DateTime BytesToDateTime(byte[] value)
        {
            var tickBytes = new byte[8];
            const int index = 8 - _numberOfDateBytes;
            Array.Copy(value, 0, tickBytes, index, _numberOfDateBytes);
            if (BitConverter.IsLittleEndian) Array.Reverse(tickBytes);
            var ms = BitConverter.ToInt64(tickBytes, 0);
            return FromUnixTicks(ms);
        }

        private static long ToUnixTicks(System.DateTime timestamp)
            => timestamp.Ticks - _epoch.Ticks;

        private static System.DateTime FromUnixTicks(long ms)
            => _epoch.AddTicks(ms);
    }
}
