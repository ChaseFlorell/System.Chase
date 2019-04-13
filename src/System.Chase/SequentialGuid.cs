using System.Collections.Generic;
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
        private static readonly Func<DateTime> _defaultDateProvider = () => DateTime.UtcNow;
        private static readonly Dictionary<string,Guid> _stringSeedCache = new Dictionary<string, Guid>();

        public static DateTime GetTimestamp(Guid guid)
        {
            lock (_getLock)
            {
                var guidBytes = guid.ToByteArray();
                var dateBytes = new byte[_numberOfDateBytes];
                Array.Copy(guidBytes, _embedAtIndex, dateBytes, 0, _numberOfDateBytes);
                try
                {
                    var result = BytesToDateTime(dateBytes);
                    if (result < DateTimeHelper.Epoch || result > DateTime.MaxValue)
                        throw new InvalidOperationException($"Cannot extract a valid DateTime from {guid}");
                    return result;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new InvalidOperationException($"Cannot extract a valid DateTime from {guid}", ex);
                }
            }
        }

        public static Guid NewSequentialGuid() 
            => NewSequentialGuid(_defaultDateProvider);

        public static Guid NewSequentialGuid(Func<DateTime> dateProvider) 
            => NewSequentialGuid(Guid.NewGuid(), dateProvider);

        public static Guid NewSequentialGuid(string seed)
            => NewSequentialGuid(seed, _defaultDateProvider);

        public static Guid NewSequentialGuid(string seed, Func<DateTime> dateProvider) 
            => NewSequentialGuid(GenerateTruncatedGuid(seed), dateProvider);

        public static Guid NewSequentialGuid(Guid seed)
            => NewSequentialGuid(seed, _defaultDateProvider);

        public static Guid NewSequentialGuid(Guid seed, Func<DateTime> dateProvider)
        {
            lock (_writeLock)
            {
                var outputBytes = seed.ToByteArray();
                var stamp = dateProvider();

                // don't allow another timestamp within the same CPU Tick and Seed
                while (stamp.Ticks == _previousTicks && seed == _previousSeed) stamp = dateProvider();

                _previousTicks = stamp.Ticks;
                _previousSeed = seed;

                var dateBytes = DateTimeToBytes(stamp);
                Array.Copy(dateBytes, 0, outputBytes, _embedAtIndex, _numberOfDateBytes);
                return new Guid(outputBytes);
            }
        }

        private static byte[] DateTimeToBytes(DateTime timestamp)
        {
            var ms = ToUnixTicks(timestamp);
            var msBytes = BitConverter.GetBytes(ms);
            if (BitConverter.IsLittleEndian) Array.Reverse(msBytes);
            var result = new byte[_numberOfDateBytes];
            var index = msBytes.GetUpperBound(0) + 1 - _numberOfDateBytes;
            Array.Copy(msBytes, index, result, 0, _numberOfDateBytes);
            return result;
        }

        private static DateTime BytesToDateTime(byte[] value)
        {
            var tickBytes = new byte[_numberOfDateBytes];
            const int index = _numberOfSeedBytes - _numberOfDateBytes;
            Array.Copy(value, 0, tickBytes, index, _numberOfDateBytes);
            if (BitConverter.IsLittleEndian) Array.Reverse(tickBytes);
            var ms = BitConverter.ToInt64(tickBytes, 0);
            return FromUnixTicks(ms);
        }

        private static long ToUnixTicks(DateTime timestamp)
            => timestamp.Ticks - DateTimeHelper.Epoch.Ticks;

        private static DateTime FromUnixTicks(long ms)
            => DateTimeHelper.Epoch.AddTicks(ms);

        private static Guid GenerateTruncatedGuid(string seed)
        {
            if (seed is null || seed.Length < _numberOfSeedBytes)
                throw new ArgumentException("Seed must be a minimum of 8 characters", nameof(seed));
            
            if (_stringSeedCache.TryGetValue(seed, out var cacheResult)) 
                return cacheResult;
            
            if (Guid.TryParse(seed, out var guidOutput))
            {
                _stringSeedCache[seed] = guidOutput;
                return guidOutput;
            }

            var originalSeedLength = seed.Length;
            var truncatedSeed = seed.Substring(0, _numberOfSeedBytes);
            
            if(originalSeedLength > _numberOfDateBytes)
                Diagnostics.Debug.WriteLine($"Sequential guid seed '{seed}' is longer than {_numberOfSeedBytes} characters and will be truncated to '{truncatedSeed}'");
                
            var seedBytes = Encoding.ASCII.GetBytes(truncatedSeed);
            var truncated = new byte[_guidByteSize];

            Array.Copy(seedBytes, 0, truncated, _numberOfDateBytes, _numberOfSeedBytes);
            var result = new Guid(truncated);
            _stringSeedCache[seed] = result;
            return result;
        }
    }
}
