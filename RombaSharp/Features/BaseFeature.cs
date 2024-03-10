using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Data.Sqlite;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.DatTools;
using SabreTools.FileTypes;
using SabreTools.Hashing;
using SabreTools.Help;
using SabreTools.Logging;

namespace RombaSharp.Features
{
    internal class BaseFeature : TopLevel
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        protected Logger logger = new Logger();

        #endregion

        #region Features

        #region Flag features

        internal const string CopyValue = "copy";
        internal static SabreTools.Help.Feature CopyFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                    CopyValue,
                    "-copy",
                    "Copy files to output instead of rebuilding",
                    ParameterType.Flag);
            }
        } // Unique to RombaSharp

        internal const string FixdatOnlyValue = "fixdat-only";
        internal static SabreTools.Help.Feature FixdatOnlyFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                    FixdatOnlyValue,
                    "-fixdatOnly",
                    "only fix dats and don't generate torrentzips",
                    ParameterType.Flag);
            }
        }

        internal const string LogOnlyValue = "log-only";
        internal static SabreTools.Help.Feature LogOnlyFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                LogOnlyValue,
                "-log-only",
                "Only write out actions to log",
                ParameterType.Flag);
            }
        }

        internal const string NoDbValue = "no-db";
        internal static SabreTools.Help.Feature NoDbFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                    NoDbValue,
                    "-no-db",
                    "archive into depot but do not touch DB index and ignore only-needed flag",
                    ParameterType.Flag);
            }
        }

        internal const string OnlyNeededValue = "only-needed";
        internal static SabreTools.Help.Feature OnlyNeededFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                    OnlyNeededValue,
                    "-only-needed",
                    "only archive ROM files actually referenced by DAT files from the DAT index",
                    ParameterType.Flag);
            }
        }

        internal const string ScriptValue = "script";
        internal static SabreTools.Help.Feature ScriptFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                    ScriptValue,
                    new List<string>() { "-sc", "--script" },
                    "Enable script mode (no clear screen)",
                    ParameterType.Flag,
                    "For times when RombaSharp is being used in a scripted environment, the user may not want the screen to be cleared every time that it is called. This flag allows the user to skip clearing the screen on run just like if the console was being redirected.");
            }
        }

        internal const string SkipInitialScanValue = "skip-initial-scan";
        internal static SabreTools.Help.Feature SkipInitialScanFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                    SkipInitialScanValue,
                    "-skip-initial-scan",
                    "skip the initial scan of the files to determine amount of work",
                    ParameterType.Flag);
            }
        }

        internal const string UseGolangZipValue = "use-golang-zip";
        internal static SabreTools.Help.Feature UseGolangZipFlag
        {
            get
            {
                return new SabreTools.Help.Feature(
                    UseGolangZipValue,
                    "-use-golang-zip",
                    "use go zip implementation instead of zlib",
                    ParameterType.Flag);
            }
        }

        #endregion

        #region Int32 features

        internal const string Include7ZipsInt32Value = "include-7zips";
        internal static SabreTools.Help.Feature Include7ZipsInt32Input
        {
            get
            {
                return new SabreTools.Help.Feature(
                    Include7ZipsInt32Value,
                    "-include-7zips",
                    "flag value == 0 means: add 7zip files themselves into the depot in addition to their contents, flag value == 2 means add 7zip files themselves but don't add content",
                    ParameterType.Int32);
            }
        }

        internal const string IncludeGZipsInt32Value = "include-gzips";
        internal static SabreTools.Help.Feature IncludeGZipsInt32Input
        {
            get
            {
                return new SabreTools.Help.Feature(
                    IncludeGZipsInt32Value,
                    "-include-gzips",
                    "flag value == 0 means: add gzip files themselves into the depot in addition to their contents, flag value == 2 means add gzip files themselves but don't add content",
                    ParameterType.Int32);
            }
        }

        internal const string IncludeZipsInt32Value = "include-zips";
        internal static SabreTools.Help.Feature IncludeZipsInt32Input
        {
            get
            {
                return new SabreTools.Help.Feature(
                    IncludeZipsInt32Value,
                    "-include-zips",
                    "flag value == 0 means: add zip files themselves into the depot in addition to their contents, flag value == 2 means add zip files themselves but don't add content",
                    ParameterType.Int32);
            }
        }

        internal const string SubworkersInt32Value = "subworkers";
        internal static SabreTools.Help.Feature SubworkersInt32Input
        {
            get
            {
                return new SabreTools.Help.Feature(
                    SubworkersInt32Value,
                    "-subworkers",
                    "how many subworkers to launch for each worker",
                    ParameterType.Int32);
            }
        } // Defaults to Workers count in config

        internal const string WorkersInt32Value = "workers";
        internal static SabreTools.Help.Feature WorkersInt32Input
        {
            get
            {
                return new SabreTools.Help.Feature(
                    WorkersInt32Value,
                    "-workers",
                    "how many workers to launch for the job",
                    ParameterType.Int32);
            }
        } // Defaults to Workers count in config

        #endregion

        #region Int64 features

        internal const string SizeInt64Value = "size";
        internal static SabreTools.Help.Feature SizeInt64Input
        {
            get
            {
                return new SabreTools.Help.Feature(
                    SizeInt64Value,
                    "-size",
                    "size of the rom to lookup",
                    ParameterType.Int64);
            }
        }

        #endregion

        #region List<String> features

        internal const string DatsListStringValue = "dats";
        internal static SabreTools.Help.Feature DatsListStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    DatsListStringValue,
                    "-dats",
                    "purge only roms declared in these dats",
                    ParameterType.List);
            }
        }

        internal const string DepotListStringValue = "depot";
        internal static SabreTools.Help.Feature DepotListStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    DepotListStringValue,
                    "-depot",
                    "work only on specified depot path",
                    ParameterType.List);
            }
        }

        #endregion

        #region String features

        internal const string BackupStringValue = "backup";
        internal static SabreTools.Help.Feature BackupStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    BackupStringValue,
                    "-backup",
                    "backup directory where backup files are moved to",
                    ParameterType.String);
            }
        }

        internal const string DescriptionStringValue = "description";
        internal static SabreTools.Help.Feature DescriptionStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    DescriptionStringValue,
                    "-description",
                    "description value in DAT header",
                    ParameterType.String);
            }
        }

        internal const string LogLevelStringValue = "log-level";
        internal static SabreTools.Help.Feature LogLevelStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    LogLevelStringValue,
                    new List<string>() { "-ll", "--log-level" },
                    "Set the lowest log level for output",
                    ParameterType.String,
                    longDescription: @"Set the lowest log level for output.
Possible values are: Verbose, User, Warning, Error");
            }
        }

        internal const string MissingSha1sStringValue = "missing-sha1s";
        internal static SabreTools.Help.Feature MissingSha1sStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    MissingSha1sStringValue,
                    "-missingSha1s",
                    "write paths of dats with missing sha1s into this file",
                    ParameterType.String);
            }
        }

        internal const string NameStringValue = "name";
        internal static SabreTools.Help.Feature NameStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    NameStringValue,
                    "-name",
                    "name value in DAT header",
                    ParameterType.String);
            }
        }

        internal const string NewStringValue = "new";
        internal static SabreTools.Help.Feature NewStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    NewStringValue,
                    "-new",
                    "new DAT file",
                    ParameterType.String);
            }
        }

        internal const string OldStringValue = "old";
        internal static SabreTools.Help.Feature OldStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    OldStringValue,
                    "-old",
                    "old DAT file",
                    ParameterType.String);
            }
        }

        internal const string OutStringValue = "out";
        internal static SabreTools.Help.Feature OutStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    OutStringValue,
                    "-out",
                    "output file",
                    ParameterType.String);
            }
        }

        internal const string ResumeStringValue = "resume";
        internal static SabreTools.Help.Feature ResumeStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    ResumeStringValue,
                    "-resume",
                    "resume a previously interrupted operation from the specified path",
                    ParameterType.String);
            }
        }

        internal const string SourceStringValue = "source";
        internal static SabreTools.Help.Feature SourceStringInput
        {
            get
            {
                return new SabreTools.Help.Feature(
                    SourceStringValue,
                    "-source",
                    "source directory",
                    ParameterType.String);
            }
        }

        #endregion

        #endregion // Features

        #region Fields

        /// <summary>
        /// Lowest log level for output
        /// </summary>
        public LogLevel LogLevel { get; protected set; }

        /// <summary>
        /// Determines if scripting mode is enabled
        /// </summary>
        public bool ScriptMode { get; protected set; }

        #endregion

        #region Settings

        // General settings
        internal static string? _logdir;		// Log folder location
        internal static string? _tmpdir;     // Temp folder location
        internal static string? _webdir;		// Web frontend location
        internal static string? _baddir;		// Fail-to-unpack file folder location
        internal static int _verbosity;		// Verbosity of the output
        internal static int _cores;			// Forced CPU cores

        // DatRoot settings
        internal static string? _dats;		// DatRoot folder location
        internal static string? _db;			// Database name

        // Depot settings
        internal static Dictionary<string, Tuple<long, bool>>? _depots; // Folder location, Max size

        // Server settings
        internal static int _port;			// Web server port

        // Other internal variables
        internal const string _config = "config.xml";
        internal static string? _connectionString;

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

        public override bool ProcessFeatures(Dictionary<string, SabreTools.Help.Feature?> features)
        {
            LogLevel = GetString(features, LogLevelStringValue).AsEnumValue<LogLevel>();
            ScriptMode = GetBoolean(features, ScriptValue);

            InitializeConfiguration();
            EnsureDatabase(_db, _connectionString);
            return true;
        }

        /// <summary>
        /// Ensure that the databse exists and has the proper schema
        /// </summary>
        /// <param name="db">Name of the databse</param>
        /// <param name="connectionString">Connection string for SQLite</param>
        public void EnsureDatabase(string? db, string? connectionString)
        {
            // Missing database or connection string can't work
            if (db == null || connectionString == null)
                return;

            // Make sure the file exists
            if (!System.IO.File.Exists(db))
                System.IO.File.Create(db);

            // Open the database connection
            SqliteConnection dbc = new SqliteConnection(connectionString);
            dbc.Open();

            // Make sure the database has the correct schema
            try
            {
                string query = @"
CREATE TABLE IF NOT EXISTS crc (
    'crc'	TEXT		NOT NULL,
    PRIMARY KEY (crc)
)";
                SqliteCommand slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();

                query = @"
CREATE TABLE IF NOT EXISTS md5 (
    'md5'	TEXT		NOT NULL,
    PRIMARY KEY (md5)
)";
                slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();

                query = @"
CREATE TABLE IF NOT EXISTS sha1 (
    'sha1'	TEXT		NOT NULL,
    'depot'	TEXT,
    PRIMARY KEY (sha1)
)";
                slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();

                query = @"
CREATE TABLE IF NOT EXISTS crcsha1 (
    'crc'	TEXT		NOT NULL,
    'sha1'	TEXT		NOT NULL,
    PRIMARY KEY (crc, sha1)
)";
                slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();

                query = @"
CREATE TABLE IF NOT EXISTS md5sha1 (
    'md5'	TEXT		NOT NULL,
    'sha1'	TEXT		NOT NULL,
    PRIMARY KEY (md5, sha1)
)";
                slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();

                query = @"
CREATE TABLE IF NOT EXISTS dat (
    'hash'	TEXT		NOT NULL,
    PRIMARY KEY (hash)
)";
                slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();
                slc.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                dbc.Dispose();
            }
        }

        #region Helper methods

        /// <summary>
        /// Gets all valid DATs that match in the DAT root
        /// </summary>
        /// <param name="inputs">List of input strings to check for, presumably file names</param>
        /// <returns>Dictionary of hash/full path for each of the valid DATs</returns>
        internal Dictionary<string, string> GetValidDats(List<string> inputs)
        {
            // Get a dictionary of filenames that actually exist in the DATRoot, logging which ones are not
#if NET20 || NET35
            List<string> datRootDats = Directory.GetFiles(_dats!, "*").ToList();
#else
            List<string> datRootDats = Directory.EnumerateFiles(_dats!, "*", SearchOption.AllDirectories).ToList();
#endif
            List<string> lowerCaseDats = datRootDats.ConvertAll(i => Path.GetFileName(i).ToLowerInvariant());
            Dictionary<string, string> foundDats = [];
            foreach (string input in inputs)
            {
                if (lowerCaseDats.Contains(input.ToLowerInvariant()))
                {
                    string fullpath = Path.GetFullPath(datRootDats[lowerCaseDats.IndexOf(input.ToLowerInvariant())]);
                    string? sha1 = TextHelper.ByteArrayToString(BaseFile.GetInfo(fullpath, hashes: [HashType.SHA1])?.SHA1);
                    if (sha1 != null)
                        foundDats.Add(sha1, fullpath);
                }
                else
                {
                    logger.Warning($"The file '{input}' could not be found in the DAT root");
                }
            }

            return foundDats;
        }

        /// <summary>
        /// Initialize the Romba application from XML config
        /// </summary>
        private void InitializeConfiguration()
        {
            // Get default values if they're not written
            int workers = 4,
                verbosity = 1,
                cores = 4,
                port = 4003;
            string logdir = "logs",
                tmpdir = "tmp",
                webdir = "web",
                baddir = "bad",
                dats = "dats",
                db = "db";
            Dictionary<string, Tuple<long, bool>> depots = new Dictionary<string, Tuple<long, bool>>();

            // Get the XML text reader for the configuration file, if possible
            XmlReader xtr = XmlReader.Create(_config, new XmlReaderSettings
            {
                CheckCharacters = false,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                ValidationFlags = XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
            });

            // Now parse the XML file for settings
            if (xtr != null)
            {
                xtr.MoveToContent();
                while (!xtr.EOF)
                {
                    // We only want elements
                    if (xtr.NodeType != XmlNodeType.Element)
                    {
                        xtr.Read();
                        continue;
                    }

                    switch (xtr.Name)
                    {
                        case "workers":
                            workers = xtr.ReadElementContentAsInt();
                            break;
                        case "logdir":
                            logdir = xtr.ReadElementContentAsString();
                            break;
                        case "tmpdir":
                            tmpdir = xtr.ReadElementContentAsString();
                            break;
                        case "webdir":
                            webdir = xtr.ReadElementContentAsString();
                            break;
                        case "baddir":
                            baddir = xtr.ReadElementContentAsString();
                            break;
                        case "verbosity":
                            verbosity = xtr.ReadElementContentAsInt();
                            break;
                        case "cores":
                            cores = xtr.ReadElementContentAsInt();
                            break;
                        case "dats":
                            dats = xtr.ReadElementContentAsString();
                            break;
                        case "db":
                            db = xtr.ReadElementContentAsString();
                            break;
                        case "depot":
                            XmlReader subreader = xtr.ReadSubtree();
                            if (subreader != null)
                            {
                                string root = string.Empty;
                                long maxsize = -1;
                                bool online = true;

                                while (!subreader.EOF)
                                {
                                    // We only want elements
                                    if (subreader.NodeType != XmlNodeType.Element)
                                    {
                                        subreader.Read();
                                        continue;
                                    }

                                    switch (subreader.Name)
                                    {
                                        case "root":
                                            root = subreader.ReadElementContentAsString();
                                            break;
                                        case "maxsize":
                                            maxsize = subreader.ReadElementContentAsLong();
                                            break;
                                        case "online":
                                            online = subreader.ReadElementContentAsBoolean();
                                            break;
                                        default:
                                            subreader.Read();
                                            break;
                                    }
                                }

                                try
                                {
                                    depots.Add(root, new Tuple<long, bool>(maxsize, online));
                                }
                                catch
                                {
                                    // Ignore add errors
                                }
                            }

                            xtr.Skip();
                            break;
                        case "port":
                            port = xtr.ReadElementContentAsInt();
                            break;
                        default:
                            xtr.Read();
                            break;
                    }
                }
            }

            // Now validate the values given
            if (workers < 1)
                workers = 1;
            if (workers > 8)
                workers = 8;

            if (!Directory.Exists(logdir))
                Directory.CreateDirectory(logdir);

            if (!Directory.Exists(tmpdir))
                Directory.CreateDirectory(tmpdir);

            if (!Directory.Exists(webdir))
                Directory.CreateDirectory(webdir);

            if (!Directory.Exists(baddir))
                Directory.CreateDirectory(baddir);

            if (verbosity < 0)
                verbosity = 0;

            if (verbosity > 3)
                verbosity = 3;

            if (cores < 1)
                cores = 1;

            if (cores > 16)
                cores = 16;

            if (!Directory.Exists(dats))
                Directory.CreateDirectory(dats);

            db = $"{Path.GetFileNameWithoutExtension(db)}.sqlite";
            string connectionString = $"Data Source={db};Version = 3;";
            foreach (string key in depots.Keys)
            {
                if (!Directory.Exists(key))
                {
                    Directory.CreateDirectory(key);
                    System.IO.File.CreateText(Path.Combine(key, ".romba_size"));
                    System.IO.File.CreateText(Path.Combine(key, ".romba_size.backup"));
                }
                else
                {
                    if (!System.IO.File.Exists(Path.Combine(key, ".romba_size")))
                        System.IO.File.CreateText(Path.Combine(key, ".romba_size"));

                    if (!System.IO.File.Exists(Path.Combine(key, ".romba_size.backup")))
                        System.IO.File.CreateText(Path.Combine(key, ".romba_size.backup"));
                }
            }

            if (port < 0)
                port = 0;

            if (port > 65535)
                port = 65535;

            // Finally set all of the fields
            Globals.MaxThreads = workers;
            _logdir = logdir;
            _tmpdir = tmpdir;
            _webdir = webdir;
            _baddir = baddir;
            _verbosity = verbosity;
            _cores = cores;
            _dats = dats;
            _db = db;
            _connectionString = connectionString;
            _depots = depots;
            _port = port;
        }

        /// <summary>
        /// Add a new DAT to the database
        /// </summary>
        /// <param name="dat">DatFile hash information to add</param>
        /// <param name="dbc">Database connection to use</param>
        internal void AddDatToDatabase(Rom dat, SqliteConnection dbc)
        {
            // Get the dat full path
            string fullpath = Path.Combine(_dats!,
                (dat.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(SabreTools.Models.Metadata.Machine.NameKey) == "dats"
                    ? string.Empty
                    : dat.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(SabreTools.Models.Metadata.Machine.NameKey))!
                , dat.GetName()!);

            // Parse the Dat if possible
            logger.User($"Adding from '{dat.GetName()}'");
            DatFile tempdat = Parser.CreateAndParse(fullpath);

            // If the Dat wasn't empty, add the information
            SqliteCommand? slc = null;
            string crcquery = "INSERT OR IGNORE INTO crc (crc) VALUES";
            string md5query = "INSERT OR IGNORE INTO md5 (md5) VALUES";
            string sha1query = "INSERT OR IGNORE INTO sha1 (sha1) VALUES";
            string crcsha1query = "INSERT OR IGNORE INTO crcsha1 (crc, sha1) VALUES";
            string md5sha1query = "INSERT OR IGNORE INTO md5sha1 (md5, sha1) VALUES";

            // Loop through the parsed entries
            bool hasItems = false;
            foreach (string romkey in tempdat.Items.Keys)
            {
                foreach (DatItem datItem in tempdat.Items[romkey]!)
                {
                    logger.Verbose($"Checking and adding file '{datItem.GetName() ?? string.Empty}'");

                    if (datItem is Disk disk)
                    {
                        hasItems = true;

                        if (!string.IsNullOrWhiteSpace(disk.GetFieldValue<string?>(SabreTools.Models.Metadata.Disk.MD5Key)))
                            md5query += $" (\"{disk.GetFieldValue<string?>(SabreTools.Models.Metadata.Disk.MD5Key)}\"),";

                        if (!string.IsNullOrWhiteSpace(disk.GetFieldValue<string?>(SabreTools.Models.Metadata.Disk.SHA1Key)))
                        {
                            sha1query += $" (\"{disk.GetFieldValue<string?>(SabreTools.Models.Metadata.Disk.SHA1Key)}\"),";

                            if (!string.IsNullOrWhiteSpace(disk.GetFieldValue<string?>(SabreTools.Models.Metadata.Disk.MD5Key)))
                                md5sha1query += $" (\"{disk.GetFieldValue<string?>(SabreTools.Models.Metadata.Disk.MD5Key)}\", \"{disk.GetFieldValue<string?>(SabreTools.Models.Metadata.Disk.SHA1Key)}\"),";
                        }
                    }
                    else if (datItem is Media media)
                    {
                        hasItems = true;

                        if (!string.IsNullOrWhiteSpace(media.GetFieldValue<string?>(SabreTools.Models.Metadata.Media.MD5Key)))
                            md5query += $" (\"{media.GetFieldValue<string?>(SabreTools.Models.Metadata.Media.MD5Key)}\"),";

                        if (!string.IsNullOrWhiteSpace(media.GetFieldValue<string?>(SabreTools.Models.Metadata.Media.SHA1Key)))
                        {
                            sha1query += $" (\"{media.GetFieldValue<string?>(SabreTools.Models.Metadata.Media.SHA1Key)}\"),";

                            if (!string.IsNullOrWhiteSpace(media.GetFieldValue<string?>(SabreTools.Models.Metadata.Media.MD5Key)))
                                md5sha1query += $" (\"{media.GetFieldValue<string?>(SabreTools.Models.Metadata.Media.MD5Key)}\", \"{media.GetFieldValue<string?>(SabreTools.Models.Metadata.Media.SHA1Key)}\"),";
                        }
                    }
                    else if (datItem is Rom rom)
                    {
                        hasItems = true;

                        if (!string.IsNullOrWhiteSpace(rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.CRCKey)))
                            crcquery += $" (\"{rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.CRCKey)}\"),";

                        if (!string.IsNullOrWhiteSpace(rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.MD5Key)))
                            md5query += $" (\"{rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.MD5Key)}\"),";

                        if (!string.IsNullOrWhiteSpace(rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.SHA1Key)))
                        {
                            sha1query += $" (\"{rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.SHA1Key)}\"),";

                            if (!string.IsNullOrWhiteSpace(rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.CRCKey)))
                                crcsha1query += $" (\"{rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.CRCKey)}\", \"{rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.SHA1Key)}\"),";

                            if (!string.IsNullOrWhiteSpace(rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.MD5Key)))
                                md5sha1query += $" (\"{rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.MD5Key)}\", \"{rom.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.SHA1Key)}\"),";
                        }
                    }
                }
            }

            // Now run the queries after fixing them
            if (crcquery != "INSERT OR IGNORE INTO crc (crc) VALUES")
            {
                slc = new SqliteCommand(crcquery.TrimEnd(','), dbc);
                slc.ExecuteNonQuery();
            }

            if (md5query != "INSERT OR IGNORE INTO md5 (md5) VALUES")
            {
                slc = new SqliteCommand(md5query.TrimEnd(','), dbc);
                slc.ExecuteNonQuery();
            }

            if (sha1query != "INSERT OR IGNORE INTO sha1 (sha1) VALUES")
            {
                slc = new SqliteCommand(sha1query.TrimEnd(','), dbc);
                slc.ExecuteNonQuery();
            }

            if (crcsha1query != "INSERT OR IGNORE INTO crcsha1 (crc, sha1) VALUES")
            {
                slc = new SqliteCommand(crcsha1query.TrimEnd(','), dbc);
                slc.ExecuteNonQuery();
            }

            if (md5sha1query != "INSERT OR IGNORE INTO md5sha1 (md5, sha1) VALUES")
            {
                slc = new SqliteCommand(md5sha1query.TrimEnd(','), dbc);
                slc.ExecuteNonQuery();
            }

            // Only add the DAT if it's non-empty
            if (hasItems)
            {
                string datquery = $"INSERT OR IGNORE INTO dat (hash) VALUES (\"{dat.GetFieldValue<string?>(SabreTools.Models.Metadata.Rom.SHA1Key)}\")";
                slc = new SqliteCommand(datquery, dbc);
                slc.ExecuteNonQuery();
            }

            slc?.Dispose();
        }

        #endregion
    }
}
