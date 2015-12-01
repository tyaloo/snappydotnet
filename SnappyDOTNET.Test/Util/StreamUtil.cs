using System.IO;

namespace SnappyDOTNET.Test.Util
{
    public static class StreamUtil
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[4096];
                int length;
                while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, length);
                }
                return ms.ToArray();
            }
        }
    }
}