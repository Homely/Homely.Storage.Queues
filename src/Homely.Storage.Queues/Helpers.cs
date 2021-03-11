using System;
using System.Text;

namespace Homely.Storage.Queues
{
    public static class Helpers
    {
        // REF: https://stackoverflow.com/a/38998370/30674
        /// <summary>
        /// This checks to see if the Type is a Simple Type (Primitive | string | decimal).
        /// </summary>
        /// <param name="type">The Type to check.</param>
        /// <remarks>To see the full list of .NET Primitive types: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/built-in-types-table</remarks>
        /// <returns>True if this is a Simple Type or False if it is not.</returns>
        public static bool IsASimpleType(this Type type) => type.IsPrimitive ||
                                                            type == typeof(string) ||
                                                            type == typeof(decimal);

        /// <summary>
        /// Returns a string representation of the BinaryData, decoding as base-64.
        /// </summary>
        /// <param name="binaryData">The data to decode</param>
        /// <returns>Decoded string</returns>
        /// <remarks>This method assumes messages are base-64 encoded.</remarks>
        public static string AsString(this BinaryData binaryData)
        {
            if (binaryData is null)
            {
                throw new ArgumentNullException(nameof(binaryData));
            }

            var utf8Body = binaryData.ToString();
            return Encoding.UTF8.GetString(Convert.FromBase64String(utf8Body));
        }
    }
}
