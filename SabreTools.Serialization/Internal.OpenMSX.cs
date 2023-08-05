namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for OpenMSX models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Dump"/> to <cref="Models.Internal.Dump"/>
        /// </summary>
        public static Models.Internal.Dump ConvertFromOpenMSX(Models.OpenMSX.Dump item)
        {
            var dump = new Models.Internal.Dump();

            if (item.Original != null)
                dump[Models.Internal.Dump.OriginalKey] = ConvertFromOpenMSX(item.Original);

            switch (item.Rom)
            {
                case Models.OpenMSX.Rom rom:
                    dump[Models.Internal.Dump.RomKey] = ConvertFromOpenMSX(rom);
                    break;

                case Models.OpenMSX.MegaRom megaRom:
                    dump[Models.Internal.Dump.MegaRomKey] = ConvertFromOpenMSX(megaRom);
                    break;

                case Models.OpenMSX.SCCPlusCart sccPlusCart:
                    dump[Models.Internal.Dump.SCCPlusCartKey] = ConvertFromOpenMSX(sccPlusCart);
                    break;
            }

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Original"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Original ConvertFromOpenMSX(Models.OpenMSX.Original item)
        {
            var original = new Models.Internal.Original
            {
                [Models.Internal.Original.ValueKey] = item.Value,
                [Models.Internal.Original.ContentKey] = item.Content,
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.RomBase"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromOpenMSX(Models.OpenMSX.RomBase item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.StartKey] = item.Start,
                [Models.Internal.Rom.TypeKey] = item.Type,
                [Models.Internal.Rom.SHA1Key] = item.Hash,
                [Models.Internal.Rom.RemarkKey] = item.Remark,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Dump"/> to <cref="Models.OpenMSX.Dump"/>
        /// </summary>
        public static Models.OpenMSX.Dump ConvertToOpenMSX(Models.Internal.Dump item)
        {
            var dump = new Models.OpenMSX.Dump();

            if (item.ContainsKey(Models.Internal.Dump.OriginalKey) && item[Models.Internal.Dump.OriginalKey] is Models.Internal.Original original)
                dump.Original = ConvertToOpenMSX(original);

            if (item.ContainsKey(Models.Internal.Dump.RomKey) && item[Models.Internal.Dump.RomKey] is Models.Internal.Rom rom)
            {
                dump.Rom = ConvertToOpenMSXRom(rom);
            }
            else if (item.ContainsKey(Models.Internal.Dump.MegaRomKey) && item[Models.Internal.Dump.MegaRomKey] is Models.Internal.Rom megaRom)
            {
                dump.Rom = ConvertToOpenMSXMegaRom(megaRom);
            }
            else if (item.ContainsKey(Models.Internal.Dump.SCCPlusCartKey) && item[Models.Internal.Dump.SCCPlusCartKey] is Models.Internal.Rom sccPlusCart)
            {
                dump.Rom = ConvertToOpenMSXSCCPlusCart(sccPlusCart);
            }

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.MegaRom"/>
        /// </summary>
        public static Models.OpenMSX.MegaRom ConvertToOpenMSXMegaRom(Models.Internal.Rom item)
        {
            var megaRom = new Models.OpenMSX.MegaRom
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return megaRom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Original"/> to <cref="Models.OpenMSX.Original"/>
        /// </summary>
        public static Models.OpenMSX.Original ConvertToOpenMSX(Models.Internal.Original item)
        {
            var original = new Models.OpenMSX.Original
            {
                Value = item.ReadString(Models.Internal.Original.ValueKey),
                Content = item.ReadString(Models.Internal.Original.ContentKey),
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.Rom"/>
        /// </summary>
        public static Models.OpenMSX.Rom ConvertToOpenMSXRom(Models.Internal.Rom item)
        {
            var rom = new Models.OpenMSX.Rom
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.SCCPlusCart"/>
        /// </summary>
        public static Models.OpenMSX.SCCPlusCart ConvertToOpenMSXSCCPlusCart(Models.Internal.Rom item)
        {
            var sccPlusCart = new Models.OpenMSX.SCCPlusCart
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return sccPlusCart;
        }

        #endregion
    }
}