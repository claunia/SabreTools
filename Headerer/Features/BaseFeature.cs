using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using SabreTools.Core.Tools;
using SabreTools.Help;
using SabreTools.IO;
using SabreTools.Logging;

namespace Headerer.Features
{
    internal class BaseFeature : TopLevel
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        protected Logger logger = new();

        #endregion

        #region Constants

        public static string HeadererFileName = Path.Combine(PathTool.GetRuntimeDirectory(), "Headerer.sqlite");
        public static string HeadererConnectionString = $"Data Source={HeadererFileName};";

        #endregion

        #region Features

        #region Flag features

        internal const string NoStoreHeaderValue = "no-store-header";
        internal static Feature NoStoreHeaderFlag
        {
            get
            {
                return new Feature(
                    NoStoreHeaderValue,
                    new List<string>() { "-nsh", "--no-store-header" },
                    "Don't store the extracted header",
                    ParameterType.Flag,
                    longDescription: "By default, all headers that are removed from files are backed up in the database. This flag allows users to skip that step entirely, avoiding caching the headers at all.");
            }
        }

        internal const string ScriptValue = "script";
        internal static Feature ScriptFlag
        {
            get
            {
                return new Feature(
                    ScriptValue,
                    new List<string>() { "-sc", "--script" },
                    "Enable script mode (no clear screen)",
                    ParameterType.Flag,
                    "For times when SabreTools is being used in a scripted environment, the user may not want the screen to be cleared every time that it is called. This flag allows the user to skip clearing the screen on run just like if the console was being redirected.");
            }
        }

        #endregion

        #region String features

        internal const string LogLevelStringValue = "log-level";
        internal static Feature LogLevelStringInput
        {
            get
            {
                return new Feature(
                    LogLevelStringValue,
                    new List<string>() { "-ll", "--log-level" },
                    "Set the lowest log level for output",
                    ParameterType.String,
                    longDescription: @"Set the lowest log level for output.
Possible values are: Verbose, User, Warning, Error");
            }
        }

        internal const string OutputDirStringValue = "output-dir";
        internal static Feature OutputDirStringInput
        {
            get
            {
                return new Feature(
                    OutputDirStringValue,
                    new List<string>() { "-out", "--output-dir" },
                    "Set output directory",
                    ParameterType.String,
                    longDescription: "This sets an output folder to be used when the files are created. If a path is not defined, the runtime directory is used instead.");
            }
        }

        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// Lowest log level for output
        /// </summary>
        public LogLevel LogLevel { get; protected set; }

        /// <summary>
        /// Output directory
        /// </summary>
        protected string? OutputDir { get; set; }

        /// <summary>
        /// Determines if scripting mode is enabled
        /// </summary>
        public bool ScriptMode { get; protected set; }

        #endregion

        #region Add Feature Groups

        /// <summary>
        /// Add common features
        /// </summary>
        protected void AddCommonFeatures()
        {
            AddFeature(ScriptFlag);
            AddFeature(LogLevelStringInput);
        }

        #endregion

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            LogLevel = GetString(features, LogLevelStringValue).AsLogLevel();
            OutputDir = GetString(features, OutputDirStringValue)?.Trim('"');
            ScriptMode = GetBoolean(features, ScriptValue);
            return true;
        }

        #region Protected Helpers

        /// <summary>
        /// Ensure that the database exists and has the proper schema
        /// </summary>
        protected static void EnsureDatabase()
        {
            // Make sure the file exists
            if (!File.Exists(HeadererFileName))
                File.Create(HeadererFileName);

            // Open the database connection
            SqliteConnection dbc = new(HeadererConnectionString);
            dbc.Open();

            // Make sure the database has the correct schema
            string query = @"
CREATE TABLE IF NOT EXISTS data (
    'sha1'		TEXT		NOT NULL,
    'header'	TEXT		NOT NULL,
    'type'		TEXT		NOT NULL,
    PRIMARY KEY (sha1, header, type)
)";
            SqliteCommand slc = new(query, dbc);
            slc.ExecuteNonQuery();
            slc.Dispose();
            dbc.Dispose();
        }

        #endregion
    }
}
