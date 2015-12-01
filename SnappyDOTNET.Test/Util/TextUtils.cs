namespace SnappyDOTNET.Test.Util
{
    public static class TextUtils
    {
        private const int KB = 1024;
        private const int MB = 1024 * KB;

        public static string GetSizeText(int size)
        {
            if (size < KB)
            {
                return size + "bytes";
            }
            if (size < MB)
            {
                return size / KB + "K";
            }
            return size / MB + "M";
        }
    }
}