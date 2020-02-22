using System;
using System.IO;
using System.Text;
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
        public static void Write(this Stream stream, byte[] bytes) => stream.Write(bytes, 0, bytes.Length);

        public static Task WriteAsync(this Stream stream, byte[] bytes) => stream.WriteAsync(bytes, 0, bytes.Length);

        public static void WriteEx(this Stream stream, byte[] bytes) => stream.Write(bytes, 0, 4);
        public static void WriteEx(this Stream stream, int i) => stream.Write(BitConverter.GetBytes(i));
        public static void WriteEx(this Stream stream, string s) => stream.Write(Encoding.ASCII.GetBytes(s));


    }
}
