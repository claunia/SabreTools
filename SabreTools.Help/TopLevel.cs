using System;
using System.Collections.Generic;
using System.IO;

using SabreTools.Logging;

namespace SabreTools.Help
{
    /// <summary>
    /// Represents an actionable top-level feature
    /// </summary>
    public abstract class TopLevel : Feature
    {
        #region Fields

        /// <summary>
        /// List of files, directories, and potential wildcard paths
        /// </summary>
        public List<string> Inputs = new();

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
        public TopLevel()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Processing

        /// <summary>
        /// Process args list based on current feature
        /// </summary>
        public virtual bool ProcessArgs(string[] args, FeatureSet help)
        {
            for (int i = 1; i < args.Length; i++)
            {
                // Verify that the current flag is proper for the feature
                if (!ValidateInput(args[i]))
                {
                    // Special precautions for files and directories
                    if (File.Exists(args[i]) || Directory.Exists(args[i]))
                    {
                        Inputs.Add(args[i]);
                    }

                    // Special precautions for wildcarded inputs (potential paths)
                    else if (args[i].Contains('*') || args[i].Contains('?'))
                    {
                        Inputs.Add(args[i]);
                    }

                    // Everything else isn't a file
                    else
                    {
                        logger.Error($"Invalid input detected: {args[i]}");
                        help.OutputIndividualFeature(this.Name);
                        LoggerImpl.Close();
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Process and extract variables based on current feature
        /// </summary>
        /// <returns>True if execution was successful, false otherwise</returns>
        public virtual bool ProcessFeatures(Dictionary<string, Feature> features) => true;

        #endregion

        #region Generic Extraction

        /// <summary>
        /// Get boolean value from nullable feature
        /// </summary>
        protected static bool GetBoolean(Dictionary<string, Feature> features, string key)
        {
            if (!features.ContainsKey(key))
                return false;

            return true;
        }

        /// <summary>
        /// Get int value from nullable feature
        /// </summary>
        protected static int GetInt32(Dictionary<string, Feature> features, string key)
        {
            if (!features.ContainsKey(key))
                return Int32.MinValue;

            return features[key].GetInt32Value();
        }

        /// <summary>
        /// Get long value from nullable feature
        /// </summary>
        protected static long GetInt64(Dictionary<string, Feature> features, string key)
        {
            if (!features.ContainsKey(key))
                return Int64.MinValue;

            return features[key].GetInt64Value();
        }

        /// <summary>
        /// Get list value from nullable feature
        /// </summary>
        protected static List<string> GetList(Dictionary<string, Feature> features, string key)
        {
            if (!features.ContainsKey(key))
                return new List<string>();

            return features[key].GetListValue() ?? new List<string>();
        }

        /// <summary>
        /// Get string value from nullable feature
        /// </summary>
        protected static string GetString(Dictionary<string, Feature> features, string key)
        {
            if (!features.ContainsKey(key))
                return null;

            return features[key].GetStringValue();
        }

        #endregion
    }
}
