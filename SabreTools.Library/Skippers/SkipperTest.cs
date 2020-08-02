using System;
using System.IO;

namespace SabreTools.Library.Skippers
{
    /// <summary>
    /// Individual test that applies to a SkipperRule
    /// </summary>
    public class SkipperTest
    {
        #region Fields

        /// <summary>
        /// Type of test to be run
        /// </summary>
        public HeaderSkipTest Type { get; set; }

        /// <summary>
        /// File offset to run the test
        /// </summary>
        /// <remarks>null is EOF</remarks>
        public long? Offset { get; set; }

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

        #endregion

        /// <summary>
        /// Check if a stream passes the test
        /// </summary>
        /// <param name="input">Stream to check rule against</param>
        /// <remarks>The Stream is assumed to be in the proper position for a given test</remarks>
        public bool Passes(Stream input)
        {
#if NET_FRAMEWORK
            switch (Type)
            {
                case HeaderSkipTest.And:
                    return CheckAnd(input);
                case HeaderSkipTest.Data:
                    return CheckData(input);
                case HeaderSkipTest.File:
                    return CheckFile(input);
                case HeaderSkipTest.Or:
                    return CheckOr(input);
                case HeaderSkipTest.Xor:
                    return CheckXor(input);
                default:
                    return true;
            }
#else
            return Type switch
            {
                HeaderSkipTest.And => CheckAnd(input),
                HeaderSkipTest.Data => CheckData(input),
                HeaderSkipTest.File => CheckFile(input),
                HeaderSkipTest.Or => CheckOr(input),
                HeaderSkipTest.Xor => CheckXor(input),
                _ => true,
            };
#endif
        }

        #region Checking Helpers

        /// <summary>
        /// Run an And test against an input stream
        /// </summary>
        /// <param name="input">Stream to check rule against</param>
        /// <returns>True if the stream passed, false otherwise</returns>
        private bool CheckAnd(Stream input)
        {
            // First seek to the correct position
            Seek(input);

            bool result = true;
            try
            {
                // Then apply the mask if it exists
                byte[] read = new byte[Mask.Length];
                input.Read(read, 0, Mask.Length);

                byte[] masked = new byte[Mask.Length];
                for (int i = 0; i < read.Length; i++)
                {
                    masked[i] = (byte)(read[i] & Mask[i]);
                }

                // Finally, compare it against the value
                for (int i = 0; i < Value.Length; i++)
                {
                    if (masked[i] != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            // Return if the expected and actual results match
            return result == Result;
        }

        /// <summary>
        /// Run a Data test against an input stream
        /// </summary>
        /// <param name="input">Stream to check rule against</param>
        /// <returns>True if the stream passed, false otherwise</returns>
        private bool CheckData(Stream input)
        {
            // First seek to the correct position
            if (!Seek(input))
                return false;

            // Then read and compare bytewise
            bool result = true;
            for (int i = 0; i < Value.Length; i++)
            {
                try
                {
                    if (input.ReadByte() != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
                catch
                {
                    result = false;
                    break;
                }
            }

            // Return if the expected and actual results match
            return result == Result;
        }

        /// <summary>
        /// Run a File test against an input stream
        /// </summary>
        /// <param name="input">Stream to check rule against</param>
        /// <returns>True if the stream passed, false otherwise</returns>
        private bool CheckFile(Stream input)
        {
            // First get the file size from stream
            long size = input.Length;

            // If we have a null size, check that the size is a power of 2
            bool result = true;
            if (Size == null)
            {
                // http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
                result = (((ulong)size & ((ulong)size - 1)) == 0);
            }
            else if (Operator == HeaderSkipTestFileOperator.Less)
            {
                result = (size < Size);
            }
            else if (Operator == HeaderSkipTestFileOperator.Greater)
            {
                result = (size > Size);
            }
            else if (Operator == HeaderSkipTestFileOperator.Equal)
            {
                result = (size == Size);
            }

            // Return if the expected and actual results match
            return result == Result;
        }

        /// <summary>
        /// Run an Or test against an input stream
        /// </summary>
        /// <param name="input">Stream to check rule against</param>
        /// <returns>True if the stream passed, false otherwise</returns>
        private bool CheckOr(Stream input)
        {
            // First seek to the correct position
            Seek(input);

            bool result = true;
            try
            {
                // Then apply the mask if it exists
                byte[] read = new byte[Mask.Length];
                input.Read(read, 0, Mask.Length);

                byte[] masked = new byte[Mask.Length];
                for (int i = 0; i < read.Length; i++)
                {
                    masked[i] = (byte)(read[i] | Mask[i]);
                }

                // Finally, compare it against the value
                for (int i = 0; i < Value.Length; i++)
                {
                    if (masked[i] != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            // Return if the expected and actual results match
            return result == Result;
        }

        /// <summary>
        /// Run an Xor test against an input stream
        /// </summary>
        /// <param name="input">Stream to check rule against</param>
        /// <returns>True if the stream passed, false otherwise</returns>
        private bool CheckXor(Stream input)
        {
            // First seek to the correct position
            Seek(input);

            bool result = true;
            try
            {
                // Then apply the mask if it exists
                byte[] read = new byte[Mask.Length];
                input.Read(read, 0, Mask.Length);

                byte[] masked = new byte[Mask.Length];
                for (int i = 0; i < read.Length; i++)
                {
                    masked[i] = (byte)(read[i] ^ Mask[i]);
                }

                // Finally, compare it against the value
                for (int i = 0; i < Value.Length; i++)
                {
                    if (masked[i] != Value[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            // Return if the expected and actual results match
            return result == Result;
        }

        /// <summary>
        /// Seek an input stream based on the test value
        /// </summary>
        /// <param name="input">Stream to seek</param>
        /// <returns>True if the stream could seek, false on error</returns>
        private bool Seek(Stream input)
        {
            try
            {
                // Null offset means EOF
                if (Offset == null)
                    input.Seek(0, SeekOrigin.End);

                // Positive offset means from beginning
                else if (Offset >= 0 && Offset <= input.Length)
                    input.Seek(Offset.Value, SeekOrigin.Begin);

                // Negative offset means from end
                else if (Offset < 0 && Math.Abs(Offset.Value) <= input.Length)
                    input.Seek(Offset.Value, SeekOrigin.End);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}