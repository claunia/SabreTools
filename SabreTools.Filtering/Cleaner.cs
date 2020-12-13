using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using SabreTools.Core;
using SabreTools.DatItems;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the cleaning operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Cleaner
    {
        #region Exclusion Fields

        /// <summary>
        /// Dictionary of DatHeader fields to exclude from writing
        /// </summary>
        public List<DatHeaderField> ExcludeDatHeaderFields { get; set; } = new List<DatHeaderField>();

        /// <summary>
        /// Dictionary of DatItem fields to exclude from writing
        /// </summary>
        public List<DatItemField> ExcludeDatItemFields { get; set; } = new List<DatItemField>();

        /// <summary>
        /// Dictionary of Machine fields to exclude from writing
        /// </summary>
        public List<MachineField> ExcludeMachineFields { get; set; } = new List<MachineField>();

        #endregion

        #region Filter Fields

        /// <summary>
        /// Filter for DatHeader fields
        /// </summary>
        public DatHeaderFilter DatHeaderFilter { get; set; }

        /// <summary>
        /// Filter for DatItem fields
        /// </summary>
        public DatItemFilter DatItemFilter { get; set; }

        /// <summary>
        /// Filter for Machine fields
        /// </summary>
        public MachineFilter MachineFilter { get; set; }

        #endregion

        #region Flag Fields

        /// <summary>
        /// Clean all names to WoD standards
        /// </summary>
        public bool Clean { get; set; }

        /// <summary>
        /// Deduplicate items using the given method
        /// </summary>
        public DedupeType DedupeRoms { get; set; }

        /// <summary>
        /// Set Machine Description from Machine Name
        /// </summary>
        public bool DescriptionAsName { get; set; }

        /// <summary>
        /// Keep machines that don't contain any items
        /// </summary>
        public bool KeepEmptyGames { get; set; }

        /// <summary>
        /// Enable "One Rom, One Region (1G1R)" mode
        /// </summary>
        public bool OneGamePerRegion { get; set; }

        /// <summary>
        /// Ordered list of regions for "One Rom, One Region (1G1R)" mode
        /// </summary>
        public List<string> RegionList { get; set; }

        /// <summary>
        /// Ensure each rom is in their own game
        /// </summary>
        public bool OneRomPerGame { get; set; }

        /// <summary>
        /// Remove all unicode characters
        /// </summary>
        public bool RemoveUnicode { get; set; }

        /// <summary>
        /// Include root directory when determing trim sizes
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Remove scene dates from the beginning of machine names
        /// </summary>
        public bool SceneDateStrip { get; set; }

        /// <summary>
        /// Change all machine names to "!"
        /// </summary>
        public bool Single { get; set; }

        /// <summary>
        /// Trim total machine and item name to not exceed NTFS limits
        /// </summary>
        public bool Trim { get; set; }
    
        #endregion
    
        #region Cleaning

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="datItem">DatItem to clean</param>
        public void CleanDatItem(DatItem datItem)
        {
            // If we're stripping unicode characters, strip machine name and description
            if (RemoveUnicode == true)
            {
                datItem.Machine.Name = RemoveUnicodeCharacters(datItem.Machine.Name);
                datItem.Machine.Description = RemoveUnicodeCharacters(datItem.Machine.Description);
                datItem.SetName(RemoveUnicodeCharacters(datItem.GetName()));
            }

            // If we're in cleaning mode, sanitize machine name and description
            if (Clean == true)
            {
                datItem.Machine.Name = CleanGameName(datItem.Machine.Name);
                datItem.Machine.Description = CleanGameName(datItem.Machine.Description);
            }

            // If we are in single game mode, rename the machine
            if (Single == true)
                datItem.Machine.Name = "!";

            // If we are in NTFS trim mode, trim the item name
            if (Trim == true && datItem.GetName() != null)
            {
                // Windows max name length is 260
                int usableLength = 260 - datItem.Machine.Name.Length - (Root?.Length ?? 0);
                if (datItem.GetName().Length > usableLength)
                {
                    string ext = Path.GetExtension(datItem.GetName());
                    datItem.SetName(datItem.GetName().Substring(0, usableLength - ext.Length) + ext);
                }
            }
        }

        /// <summary>
        /// Clean a game (or rom) name to the WoD standard
        /// </summary>
        /// <param name="game">Name of the game to be cleaned</param>
        /// <returns>The cleaned name</returns>
        private string CleanGameName(string game)
        {
            if (game == null)
                return null;

            ///Run the name through the filters to make sure that it's correct
            game = NormalizeChars(game);
            game = RussianToLatin(game);
            game = SearchPattern(game);

            game = new Regex(@"(([[(].*[\)\]] )?([^([]+))").Match(game).Groups[1].Value;
            game = game.TrimStart().TrimEnd();
            return game;
        }

        /// <summary>
        /// Replace accented characters
        /// </summary>
        /// <param name="input">String to be parsed</param>
        /// <returns>String with characters replaced</returns>
        private string NormalizeChars(string input)
        {
            if (input == null)
                return null;

            string[,] charmap = {
                { "Á", "A" },   { "á", "a" },
                { "À", "A" },   { "à", "a" },
                { "Â", "A" },   { "â", "a" },
                { "Ä", "Ae" },  { "ä", "ae" },
                { "Ã", "A" },   { "ã", "a" },
                { "Å", "A" },   { "å", "a" },
                { "Æ", "Ae" },  { "æ", "ae" },
                { "Ç", "C" },   { "ç", "c" },
                { "Ð", "D" },   { "ð", "d" },
                { "É", "E" },   { "é", "e" },
                { "È", "E" },   { "è", "e" },
                { "Ê", "E" },   { "ê", "e" },
                { "Ë", "E" },   { "ë", "e" },
                { "ƒ", "f" },
                { "Í", "I" },   { "í", "i" },
                { "Ì", "I" },   { "ì", "i" },
                { "Î", "I" },   { "î", "i" },
                { "Ï", "I" },   { "ï", "i" },
                { "Ñ", "N" },   { "ñ", "n" },
                { "Ó", "O" },   { "ó", "o" },
                { "Ò", "O" },   { "ò", "o" },
                { "Ô", "O" },   { "ô", "o" },
                { "Ö", "Oe" },  { "ö", "oe" },
                { "Õ", "O" },   { "õ", "o" },
                { "Ø", "O" },   { "ø", "o" },
                { "Š", "S" },   { "š", "s" },
                { "ß", "ss" },
                { "Þ", "B" },   { "þ", "b" },
                { "Ú", "U" },   { "ú", "u" },
                { "Ù", "U" },   { "ù", "u" },
                { "Û", "U" },   { "û", "u" },
                { "Ü", "Ue" },  { "ü", "ue" },
                { "ÿ", "y" },
                { "Ý", "Y" },   { "ý", "y" },
                { "Ž", "Z" },   { "ž", "z" },
            };

            for (int i = 0; i < charmap.GetLength(0); i++)
            {
                input = input.Replace(charmap[i, 0], charmap[i, 1]);
            }

            return input;
        }

        /// <summary>
        /// Remove all unicode-specific chars from a string
        /// </summary>
        /// <param name="s">Input string to clean</param>
        /// <returns>Cleaned string</returns>
        private string RemoveUnicodeCharacters(string s)
        {
            if (s == null)
                return null;

            return new string(s.Where(c => c <= 255).ToArray());
        }  

        /// <summary>
        /// Convert Cyrillic lettering to Latin lettering
        /// </summary>
        /// <param name="input">String to be parsed</param>
        /// <returns>String with characters replaced</returns>
        private string RussianToLatin(string input)
        {
            if (input == null)
                return null;

            string[,] charmap = {
                    { "А", "A" }, { "Б", "B" }, { "В", "V" }, { "Г", "G" }, { "Д", "D" },
                    { "Е", "E" }, { "Ё", "Yo" }, { "Ж", "Zh" }, { "З", "Z" }, { "И", "I" },
                    { "Й", "J" }, { "К", "K" }, { "Л", "L" }, { "М", "M" }, { "Н", "N" },
                    { "О", "O" }, { "П", "P" }, { "Р", "R" }, { "С", "S" }, { "Т", "T" },
                    { "У", "U" }, { "Ф", "f" }, { "Х", "Kh" }, { "Ц", "Ts" }, { "Ч", "Ch" },
                    { "Ш", "Sh" }, { "Щ", "Sch" }, { "Ъ", string.Empty }, { "Ы", "y" }, { "Ь", string.Empty },
                    { "Э", "e" }, { "Ю", "yu" }, { "Я", "ya" }, { "а", "a" }, { "б", "b" },
                    { "в", "v" }, { "г", "g" }, { "д", "d" }, { "е", "e" }, { "ё", "yo" },
                    { "ж", "zh" }, { "з", "z" }, { "и", "i" }, { "й", "j" }, { "к", "k" },
                    { "л", "l" }, { "м", "m" }, { "н", "n" }, { "о", "o" }, { "п", "p" },
                    { "р", "r" }, { "с", "s" }, { "т", "t" }, { "у", "u" }, { "ф", "f" },
                    { "х", "kh" }, { "ц", "ts" }, { "ч", "ch" }, { "ш", "sh" }, { "щ", "sch" },
                    { "ъ", string.Empty }, { "ы", "y" }, { "ь", string.Empty }, { "э", "e" }, { "ю", "yu" },
                    { "я", "ya" },
            };

            for (int i = 0; i < charmap.GetLength(0); i++)
            {
                input = input.Replace(charmap[i, 0], charmap[i, 1]);
            }

            return input;
        }

        /// <summary>
        /// Replace special characters and patterns
        /// </summary>
        /// <param name="input">String to be parsed</param>
        /// <returns>String with characters replaced</returns>
        private string SearchPattern(string input)
        {
            if (input == null)
                return null;

            string[,] charmap = {
                { @"~", " - " },
                { @"_", " " },
                { @":", " " },
                { @">", ")" },
                { @"<", "(" },
                { @"\|", "-" },
                { "\"", "'" },
                { @"\*", "." },
                { @"\\", "-" },
                { @"/", "-" },
                { @"\?", " " },
                { @"\(([^)(]*)\(([^)]*)\)([^)(]*)\)", " " },
                { @"\(([^)]+)\)", " " },
                { @"\[([^]]+)\]", " " },
                { @"\{([^}]+)\}", " " },
                { @"(ZZZJUNK|ZZZ-UNK-|ZZZ-UNK |zzz unknow |zzz unk |Copy of |[.][a-z]{3}[.][a-z]{3}[.]|[.][a-z]{3}[.])", " " },
                { @" (r|rev|v|ver)\s*[\d\.]+[^\s]*", " " },
                { @"(( )|(\A))(\d{6}|\d{8})(( )|(\Z))", " " },
                { @"(( )|(\A))(\d{1,2})-(\d{1,2})-(\d{4}|\d{2})", " " },
                { @"(( )|(\A))(\d{4}|\d{2})-(\d{1,2})-(\d{1,2})", " " },
                { @"[-]+", "-" },
                { @"\A\s*\)", " " },
                { @"\A\s*(,|-)", " " },
                { @"\s+", " " },
                { @"\s+,", "," },
                { @"\s*(,|-)\s*\Z", " " },
            };

            for (int i = 0; i < charmap.GetLength(0); i++)
            {
                input = Regex.Replace(input, charmap[i, 0], charmap[i, 1]);
            }

            return input;
        }

        #endregion
    
        #region Filtering

        

        #endregion
    }
}
