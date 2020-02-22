using System;
using System.IO;
using System.Threading.Tasks;

namespace CSF.Model
{
    public static class StreamExtension
    {
        public static byte[] Read(this Stream stream, int count)
        {
            var bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return bytes;
        }
        public static async Task<byte[]> ReadAsync(this Stream stream, int count)
        {
            var bytes = new byte[count];
            await stream.ReadAsync(bytes, 0, count);
            return bytes;
        }
    }
}
