using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for OpenMSX models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.SoftwareDb"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        public static Models.Internal.Header ConvertHeaderFromOpenMSX(Models.OpenMSX.SoftwareDb item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.TimestampKey] = item.Timestamp,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Software"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromOpenMSX(Models.OpenMSX.Software item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Title,
                [Models.Internal.Machine.GenMSXIDKey] = item.GenMSXID,
                [Models.Internal.Machine.SystemKey] = item.System,
                [Models.Internal.Machine.CompanyKey] = item.Company,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.CountryKey] = item.Country,
            };

            if (item.Dump != null && item.Dump.Any())
            {
                var dumps = new List<Models.Internal.Dump>();
                foreach (var dump in item.Dump)
                {
                    dumps.Add(ConvertFromOpenMSX(dump));
                }
                machine[Models.Internal.Machine.DumpKey] = dumps.ToArray();
            }

            return machine;
        }

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
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.OpenMSX.Software"/>
        /// </summary>
        public static Models.OpenMSX.Software? ConvertMachineToOpenMSX(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;
            
            var game = new Models.OpenMSX.Software
            {
                Title = item.ReadString(Models.Internal.Machine.NameKey),
                GenMSXID = item.ReadString(Models.Internal.Machine.GenMSXIDKey),
                System = item.ReadString(Models.Internal.Machine.SystemKey),
                Company = item.ReadString(Models.Internal.Machine.CompanyKey),
                Year = item.ReadString(Models.Internal.Machine.YearKey),
                Country = item.ReadString(Models.Internal.Machine.CountryKey),
            };

            var dumps = item.Read<Models.Internal.Dump[]>(Models.Internal.Machine.DumpKey);
            game.Dump = dumps?.Select(ConvertToOpenMSX)?.ToArray();

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Dump"/> to <cref="Models.OpenMSX.Dump"/>
        /// </summary>
        private static Models.OpenMSX.Dump? ConvertToOpenMSX(Models.Internal.Dump? item)
        {
            if (item == null)
                return null;
            
            var dump = new Models.OpenMSX.Dump();

            var original = item.Read<Models.Internal.Original>(Models.Internal.Dump.OriginalKey);
            dump.Original = ConvertToOpenMSX(original);

            var rom = item.Read<Models.Internal.Rom>(Models.Internal.Dump.RomKey);
            dump.Rom = ConvertToOpenMSXRom(rom);

            var megaRom = item.Read<Models.Internal.Rom>(Models.Internal.Dump.MegaRomKey);
            dump.Rom = ConvertToOpenMSXMegaRom(megaRom);

            var sccPlusCart = item.Read<Models.Internal.Rom>(Models.Internal.Dump.SCCPlusCartKey);
            dump.Rom = ConvertToOpenMSXSCCPlusCart(sccPlusCart);

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.MegaRom"/>
        /// </summary>
        private static Models.OpenMSX.MegaRom? ConvertToOpenMSXMegaRom(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
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
        private static Models.OpenMSX.Original? ConvertToOpenMSX(Models.Internal.Original? item)
        {
            if (item == null)
                return null;
            
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
        private static Models.OpenMSX.Rom? ConvertToOpenMSXRom(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
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
        private static Models.OpenMSX.SCCPlusCart? ConvertToOpenMSXSCCPlusCart(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
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