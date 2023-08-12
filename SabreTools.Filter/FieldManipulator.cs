using System;
using System.Linq;
using System.Text.RegularExpressions;
using SabreTools.Models.Internal;

namespace SabreTools.Filter
{
    public static class FieldManipulator
    {
        /// <summary>
        /// Regex pattern to match scene dates
        /// </summary>
        private const string _sceneDateRegex = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

        /// <summary>
        /// Replace the machine name with the description
        /// </summary>
        public static (bool Success, string? OriginalName) DescriptionToName(Machine? machine)
        {
            // If the machine is missing, we can't do anything
            if (machine == null)
                return (false, null);

            // Get both the current name and description
            string? name = machine.ReadString(Header.NameKey);
            string? description = machine.ReadString(Header.DescriptionKey);

            // Replace the name with the description
            machine[Header.NameKey] = description;
            return (true, name);
        }

        /// <summary>
        /// Normalize a string to the WoD standard
        /// </summary>
        public static string? NormalizeCharacters(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            ///Run the name through the filters to make sure that it's correct
            input = NormalizeChars(input);
            input = RussianToLatin(input);
            input = SearchPattern(input);

            input = new Regex(@"(([[(].*[\)\]] )?([^([]+))").Match(input).Groups[1].Value;
            input = input.TrimStart().TrimEnd();
            return input;
        }

        /// <summary>
        /// Remove a field from a given DictionaryBase
        /// </summary>
        public static bool RemoveField(DictionaryBase? dictionaryBase, string? fieldName)
        {
            // If the item or field name are missing, we can't do anything
            if (dictionaryBase == null || string.IsNullOrWhiteSpace(fieldName))
                return false;

            // If the key doesn't exist, then it's already removed
            if (!dictionaryBase.ContainsKey(fieldName))
                return true;

            // Remove the key
            dictionaryBase.Remove(fieldName);
            return true;
        }

        /// <summary>
        /// Remove all unicode-specific chars from a string
        /// </summary>
        public static string? RemoveUnicodeCharacters(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return new string(input.Where(c => c <= 255).ToArray());
        }  

        /// <summary>
        /// Set a field in a given DictionaryBase
        /// </summary>
        public static bool SetField(DictionaryBase? dictionaryBase, string? fieldName, object value)
        {
            // If the item or field name are missing, we can't do anything
            if (dictionaryBase == null || string.IsNullOrWhiteSpace(fieldName))
                return false;

            // Retrieve the list of valid fields for the item and validate
            var constants = TypeHelper.GetConstants(dictionaryBase.GetType());
            if (constants == null || !constants.Any(c => string.Equals(c, fieldName, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            // Set the field with the new value
            dictionaryBase[fieldName] = value;
            return true;
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style machine name and description
        /// </summary>
        public static bool StripSceneDates(Machine? machine)
        {
            // If the machine is missing, we can't do anything
            if (machine == null)
                return false;

            // Strip dates from the name field
            string? name = machine.ReadString(Header.NameKey);
            if (name != null && Regex.IsMatch(name, _sceneDateRegex))
                machine[Header.NameKey] = Regex.Replace(name, _sceneDateRegex, @"$2");

            // Strip dates from the description field
            string? description = machine.ReadString(Header.DescriptionKey);
            if (description != null && Regex.IsMatch(description, _sceneDateRegex))
                machine[Header.DescriptionKey] = Regex.Replace(description, _sceneDateRegex, @"$2");

            return true;
        }
    
        #region Helpers

        /// <summary>
        /// Replace accented characters
        /// </summary>
        private static string NormalizeChars(string input)
        {
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
        /// Convert Cyrillic lettering to Latin lettering
        /// </summary>
        private static string RussianToLatin(string input)
        {
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
        private static string SearchPattern(string input)
        {
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
    }
}