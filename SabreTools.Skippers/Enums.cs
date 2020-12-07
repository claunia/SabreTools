namespace SabreTools.Library.Skippers
{
    /// <summary>
    /// Determines the header skip operation
    /// </summary>
    public enum HeaderSkipOperation
    {
        None = 0,
        Bitswap,
        Byteswap,
        Wordswap,
        WordByteswap,
    }

    /// <summary>
    /// Determines the type of test to be done
    /// </summary>
    public enum HeaderSkipTest
    {
        Data = 0,
        Or,
        Xor,
        And,
        File,
    }

    /// <summary>
    /// Determines the operator to be used in a file test
    /// </summary>
    public enum HeaderSkipTestFileOperator
    {
        Equal = 0,
        Less,
        Greater,
    }
}
