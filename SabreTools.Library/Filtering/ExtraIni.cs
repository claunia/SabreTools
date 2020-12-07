using System.Collections.Generic;

using SabreTools.Logging;
using SabreTools.Library.Tools;

namespace SabreTools.Library.Filtering
{
    public class ExtraIni
    {
        #region Fields

        /// <summary>
        /// List of extras to apply
        /// </summary>
        public List<ExtraIniItem> Items { get; set; } = new List<ExtraIniItem>();

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ExtraIni()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Extras Population

        /// <summary>
        /// Populate item using field:file inputs
        /// </summary>
        /// <param name="inputs">Field and file combinations</param>
        public void PopulateFromList(List<string> inputs)
        {
            foreach (string input in inputs)
            {
                ExtraIniItem item = new ExtraIniItem();

                // If we don't even have a possible field and file combination
                if (!input.Contains(":"))
                {
                    logger.Warning($"'{input}` is not a valid INI extras string. Valid INI extras strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                    return;
                }

                string inputTrimmed = input.Trim('"', ' ', '\t');
                string fieldString = inputTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
                string fileString = inputTrimmed.Substring(fieldString.Length + 1).Trim('"', ' ', '\t');

                item.Field = fieldString.AsField();
                if (item.PopulateFromFile(fileString))
                    Items.Add(item);
            }
        }

        #endregion
    }
}
