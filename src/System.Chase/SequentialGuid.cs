using System.Text;

namespace System.Chase
{
    public static class SequentialGuid
    {
        private const int _guidByteSize = 16;
        private const int _numberOfDateBytes = 8;
        private const int _numberOfSeedBytes = _guidByteSize - _numberOfDateBytes;
        private const int _embedAtIndex = 0; // index 0 ensures we're sequential by date
        private static long _previousTicks;
        private static Guid _previousSeed;
        private static readonly object _getLock = new object();
        private static readonly object _writeLock = new object();

        public static System.DateTime GetTimestamp(Guid guid)
        {
            lock (_getLock)
            {
                var combBytes = guid.ToByteArray();
                var dateBytes = new byte[_numberOfDateBytes];
                Array.Copy(combBytes, _embedAtIndex, dateBytes, 0, _numberOfDateBytes);
                try
                {
                    var result = BytesToDateTime(dateBytes);
                    if (result <= DateTime.Epoch
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
        
        public static Guid NewSequentialGuid() => NewSequentialGuid(Guid.NewGuid());

        public static Guid NewSequentialGuid(string seed)
        {
            if (seed.Length < _numberOfSeedBytes)
                throw new ArgumentException("Seed must be a minimum of 8 characters", nameof(seed));

            var seedBytes = Encoding.ASCII.GetBytes(seed.Substring(0,_numberOfSeedBytes));
            var truncated = new byte[_guidByteSize];

            Array.Copy(seedBytes, 0, truncated, _numberOfDateBytes, _numberOfSeedBytes);

            return NewSequentialGuid(new Guid(truncated));
        }

        private static Guid NewSequentialGuid(Guid seed)
        {
            lock (_writeLock)
            {
                var outputBytes = seed.ToByteArray();
                var stamp = System.DateTime.UtcNow;

                // don't allow another timestamp within the same CPU Tick and Seed
                while (stamp.Ticks == _previousTicks && seed == _previousSeed) stamp = System.DateTime.UtcNow;

                _previousTicks = stamp.Ticks;
                _previousSeed = seed;

                var dateBytes = DateTimeToBytes(stamp);
                Array.Copy(dateBytes, 0, outputBytes, _embedAtIndex, _numberOfDateBytes);
                return new Guid(outputBytes);
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
            var tickBytes = new byte[_numberOfDateBytes];
            const int index = _numberOfSeedBytes - _numberOfDateBytes;
            Array.Copy(value, 0, tickBytes, index, _numberOfDateBytes);
            if (BitConverter.IsLittleEndian) Array.Reverse(tickBytes);
            var ms = BitConverter.ToInt64(tickBytes, 0);
            return FromUnixTicks(ms);
        }

        private static long ToUnixTicks(System.DateTime timestamp)
            => timestamp.Ticks - DateTime.Epoch.Ticks;

        private static System.DateTime FromUnixTicks(long ms)
            => DateTime.Epoch.AddTicks(ms);
    }
}
