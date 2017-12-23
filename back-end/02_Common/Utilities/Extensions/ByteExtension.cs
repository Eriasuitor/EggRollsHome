using System.Text;

namespace Newegg.MIS.API.Utilities.Extensions
{
    public static class ByteExtension
    {
        public static string ToHexString(this byte[] bytes, bool upperCase)
        {
            var result = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
            {
                result.Append(t.ToString(upperCase ? "X2" : "x2"));
            }

            return result.ToString();
        }
    }
}
