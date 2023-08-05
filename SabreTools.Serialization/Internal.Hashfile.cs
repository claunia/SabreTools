namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Hashfile models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.Hashfile.MD5"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromMD5(Models.Hashfile.MD5 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.MD5Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SFV"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSFV(Models.Hashfile.SFV item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.File,
                [Models.Internal.Rom.CRCKey] = item.Hash,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA1"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA1(Models.Hashfile.SHA1 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA1Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA256"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA256(Models.Hashfile.SHA256 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA256Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA384"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA384(Models.Hashfile.SHA384 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA384Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA512"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA512(Models.Hashfile.SHA512 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA512Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SpamSum"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSpamSum(Models.Hashfile.SpamSum item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SpamSumKey] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.MD5"/>
        /// </summary>
        public static Models.Hashfile.MD5 ConvertToMD5(Models.Internal.Rom item)
        {
            var md5 = new Models.Hashfile.MD5
            {
                Hash = item.ReadString(Models.Internal.Rom.MD5Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return md5;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SFV"/>
        /// </summary>
        public static Models.Hashfile.SFV ConvertToSFV(Models.Internal.Rom item)
        {
            var sfv = new Models.Hashfile.SFV
            {
                File = item.ReadString(Models.Internal.Rom.NameKey),
                Hash = item.ReadString(Models.Internal.Rom.CRCKey),
            };
            return sfv;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA1"/>
        /// </summary>
        public static Models.Hashfile.SHA1 ConvertToSHA1(Models.Internal.Rom item)
        {
            var sha1 = new Models.Hashfile.SHA1
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha1;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA256"/>
        /// </summary>
        public static Models.Hashfile.SHA256 ConvertToSHA256(Models.Internal.Rom item)
        {
            var sha256 = new Models.Hashfile.SHA256
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA256Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha256;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA384"/>
        /// </summary>
        public static Models.Hashfile.SHA384 ConvertToSHA384(Models.Internal.Rom item)
        {
            var sha384 = new Models.Hashfile.SHA384
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA384Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha384;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA512"/>
        /// </summary>
        public static Models.Hashfile.SHA512 ConvertToSHA512(Models.Internal.Rom item)
        {
            var sha512 = new Models.Hashfile.SHA512
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA512Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha512;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SpamSum"/>
        /// </summary>
        public static Models.Hashfile.SpamSum ConvertToSpamSum(Models.Internal.Rom item)
        {
            var spamsum = new Models.Hashfile.SpamSum
            {
                Hash = item.ReadString(Models.Internal.Rom.SpamSumKey),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return spamsum;
        }

        #endregion
    }
}