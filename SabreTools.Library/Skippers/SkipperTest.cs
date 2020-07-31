using SabreTools.Library.Data;

namespace SabreTools.Library.Skippers
{
    /// <summary>
    /// Intermediate class for storing Skipper Test information
    /// </summary>
    public struct SkipperTest
    {
        /// <summary>
        /// Type of test to be run
        /// </summary>
        public HeaderSkipTest Type { get; set; }

        /// <summary>
        /// File offset to run the test
        /// </summary>
        public long? Offset { get; set; } // null is EOF

        /// <summary>
        /// Static value to be checked at the offset
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// Determines whether a pass or failure is expected
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Byte mask to be applied to the tested bytes
        /// </summary>
        public byte[] Mask { get; set; }

        /// <summary>
        /// Expected size of the input byte array, used with the Operator
        /// </summary>
        public long? Size { get; set; } // null is PO2, "power of 2" filesize

        /// <summary>
        /// Expected range value for the input byte array size, used with Size
        /// </summary>
        public HeaderSkipTestFileOperator Operator { get; set; }
    }
}