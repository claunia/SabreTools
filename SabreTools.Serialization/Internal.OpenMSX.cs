using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for OpenMSX models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.SoftwareDb"/> to <cref="Header"/>
        /// </summary>
        public static Header ConvertHeaderFromOpenMSX(Models.OpenMSX.SoftwareDb item)
        {
            var header = new Header
            {
                [Header.TimestampKey] = item.Timestamp,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Software"/> to <cref="Machine"/>
        /// </summary>
        public static Machine ConvertMachineFromOpenMSX(Models.OpenMSX.Software item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.Title,
                [Machine.GenMSXIDKey] = item.GenMSXID,
                [Machine.SystemKey] = item.System,
                [Machine.CompanyKey] = item.Company,
                [Machine.YearKey] = item.Year,
                [Machine.CountryKey] = item.Country,
            };

            if (item.Dump != null && item.Dump.Any())
            {
                var dumps = new List<Dump>();
                foreach (var dump in item.Dump)
                {
                    dumps.Add(ConvertFromOpenMSX(dump));
                }
                machine[Machine.DumpKey] = dumps.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Dump"/> to <cref="Dump"/>
        /// </summary>
        public static Dump ConvertFromOpenMSX(Models.OpenMSX.Dump item)
        {
            var dump = new Dump();

            if (item.Original != null)
                dump[Dump.OriginalKey] = ConvertFromOpenMSX(item.Original);

            switch (item.Rom)
            {
                case Models.OpenMSX.Rom rom:
                    dump[Dump.RomKey] = ConvertFromOpenMSX(rom);
                    break;

                case Models.OpenMSX.MegaRom megaRom:
                    dump[Dump.MegaRomKey] = ConvertFromOpenMSX(megaRom);
                    break;

                case Models.OpenMSX.SCCPlusCart sccPlusCart:
                    dump[Dump.SCCPlusCartKey] = ConvertFromOpenMSX(sccPlusCart);
                    break;
            }

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Original"/> to <cref="Rom"/>
        /// </summary>
        public static Original ConvertFromOpenMSX(Models.OpenMSX.Original item)
        {
            var original = new Original
            {
                [Original.ValueKey] = item.Value,
                [Original.ContentKey] = item.Content,
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.RomBase"/> to <cref="Rom"/>
        /// </summary>
        public static Rom ConvertFromOpenMSX(Models.OpenMSX.RomBase item)
        {
            var rom = new Rom
            {
                [Rom.StartKey] = item.Start,
                [Rom.TypeKey] = item.Type,
                [Rom.SHA1Key] = item.Hash,
                [Rom.RemarkKey] = item.Remark,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.OpenMSX.SoftwareDb"/>
        /// </summary>
        public static Models.OpenMSX.SoftwareDb? ConvertHeaderToOpenMSX(Header? item)
        {
            if (item == null)
                return null;

            var softwareDb = new Models.OpenMSX.SoftwareDb
            {
                Timestamp = item.ReadString(Header.TimestampKey),
            };
            return softwareDb;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to <cref="Models.OpenMSX.Software"/>
        /// </summary>
        public static Models.OpenMSX.Software? ConvertMachineToOpenMSX(Machine? item)
        {
            if (item == null)
                return null;
            
            var game = new Models.OpenMSX.Software
            {
                Title = item.ReadString(Machine.NameKey),
                GenMSXID = item.ReadString(Machine.GenMSXIDKey),
                System = item.ReadString(Machine.SystemKey),
                Company = item.ReadString(Machine.CompanyKey),
                Year = item.ReadString(Machine.YearKey),
                Country = item.ReadString(Machine.CountryKey),
            };

            var dumps = item.Read<Dump[]>(Machine.DumpKey);
            game.Dump = dumps?.Select(ConvertToOpenMSX)?.ToArray();

            return game;
        }

        /// <summary>
        /// Convert from <cref="Dump"/> to <cref="Models.OpenMSX.Dump"/>
        /// </summary>
        private static Models.OpenMSX.Dump? ConvertToOpenMSX(Dump? item)
        {
            if (item == null)
                return null;
            
            var dump = new Models.OpenMSX.Dump();

            var original = item.Read<Original>(Dump.OriginalKey);
            dump.Original = ConvertToOpenMSX(original);

            var rom = item.Read<Rom>(Dump.RomKey);
            dump.Rom = ConvertToOpenMSXRom(rom);

            var megaRom = item.Read<Rom>(Dump.MegaRomKey);
            dump.Rom = ConvertToOpenMSXMegaRom(megaRom);

            var sccPlusCart = item.Read<Rom>(Dump.SCCPlusCartKey);
            dump.Rom = ConvertToOpenMSXSCCPlusCart(sccPlusCart);

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.OpenMSX.MegaRom"/>
        /// </summary>
        private static Models.OpenMSX.MegaRom? ConvertToOpenMSXMegaRom(Rom? item)
        {
            if (item == null)
                return null;
            
            var megaRom = new Models.OpenMSX.MegaRom
            {
                Start = item.ReadString(Rom.StartKey),
                Type = item.ReadString(Rom.TypeKey),
                Hash = item.ReadString(Rom.SHA1Key),
                Remark = item.ReadString(Rom.RemarkKey),
            };
            return megaRom;
        }

        /// <summary>
        /// Convert from <cref="Original"/> to <cref="Models.OpenMSX.Original"/>
        /// </summary>
        private static Models.OpenMSX.Original? ConvertToOpenMSX(Original? item)
        {
            if (item == null)
                return null;
            
            var original = new Models.OpenMSX.Original
            {
                Value = item.ReadString(Original.ValueKey),
                Content = item.ReadString(Original.ContentKey),
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.OpenMSX.Rom"/>
        /// </summary>
        private static Models.OpenMSX.Rom? ConvertToOpenMSXRom(Rom? item)
        {
            if (item == null)
                return null;
            
            var rom = new Models.OpenMSX.Rom
            {
                Start = item.ReadString(Rom.StartKey),
                Type = item.ReadString(Rom.TypeKey),
                Hash = item.ReadString(Rom.SHA1Key),
                Remark = item.ReadString(Rom.RemarkKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.OpenMSX.SCCPlusCart"/>
        /// </summary>
        private static Models.OpenMSX.SCCPlusCart? ConvertToOpenMSXSCCPlusCart(Rom? item)
        {
            if (item == null)
                return null;
            
            var sccPlusCart = new Models.OpenMSX.SCCPlusCart
            {
                Start = item.ReadString(Rom.StartKey),
                Type = item.ReadString(Rom.TypeKey),
                Hash = item.ReadString(Rom.SHA1Key),
                Remark = item.ReadString(Rom.RemarkKey),
            };
            return sccPlusCart;
        }

        #endregion
    }
}