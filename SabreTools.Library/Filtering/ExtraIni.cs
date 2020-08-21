using System.Collections.Generic;

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
                item.PopulateFromInput(input);
                Items.Add(item);
            }
        }

        #endregion
    }
}
