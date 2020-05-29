using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.Kernel.Extension
{
    public static class StreamExtension
    {
        public static async Task<byte[]> ReadAsync(this Stream stream, int length)
        {
            var buffer = new byte[length];
            await stream.ReadAsync(buffer, 0, length);
            return buffer;
        }
    }
}
