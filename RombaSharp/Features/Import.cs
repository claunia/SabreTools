using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using SabreTools.Help;
using SabreTools.IO;

namespace RombaSharp.Features
{
    internal class Import : BaseFeature
    {
        public const string Value = "Import";

        // Unique to RombaSharp
        public Import()
        {
            Name = Value;
            Flags = ["import"];
            Description = "Import a database from a formatted CSV file";
            _featureType = ParameterType.Flag;
            LongDescription = "Import a database from a formatted CSV file";
            Features = [];

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            logger.Error("This feature is not yet implemented: import");

            // First ensure the inputs and database connection
            Inputs = PathTool.GetFilesOnly(Inputs).Select(p => p.CurrentPath).ToList();
            SqliteConnection dbc = new SqliteConnection(_connectionString);
            SqliteCommand slc = new SqliteCommand();
            dbc.Open();

            // Now, for each of these files, attempt to add the data found inside
            foreach (string input in Inputs)
            {
                StreamReader sr = new StreamReader(File.OpenRead(input));

                // The first line should be the hash header
                string? line = sr.ReadLine();
                if (line != "CRC,MD5,SHA-1") // ,Depot
                {
                    logger.Error($"{input} is not a valid export file");
                    continue;
                }

                // Define the insert queries
                string crcquery = "INSERT OR IGNORE INTO crc (crc) VALUES";
                string md5query = "INSERT OR IGNORE INTO md5 (md5) VALUES";
                string sha1query = "INSERT OR IGNORE INTO sha1 (sha1) VALUES";
                string crcsha1query = "INSERT OR IGNORE INTO crcsha1 (crc, sha1) VALUES";
                string md5sha1query = "INSERT OR IGNORE INTO md5sha1 (md5, sha1) VALUES";

                // For each line until we hit a blank line...
                while (!sr.EndOfStream && line != string.Empty)
                {
                    line = sr.ReadLine() ?? string.Empty;
                    string[] hashes = line.Split(',');

                    // Loop through the parsed entries
                    if (!string.IsNullOrWhiteSpace(hashes[0]))
                        crcquery += $" (\"{hashes[0]}\"),";

                    if (!string.IsNullOrWhiteSpace(hashes[1]))
                        md5query += $" (\"{hashes[1]}\"),";

                    if (!string.IsNullOrWhiteSpace(hashes[2]))
                    {
                        sha1query += $" (\"{hashes[2]}\"),";

                        if (!string.IsNullOrWhiteSpace(hashes[0]))
                            crcsha1query += $" (\"{hashes[0]}\", \"{hashes[2]}\"),";

                        if (!string.IsNullOrWhiteSpace(hashes[1]))
                            md5sha1query += $" (\"{hashes[1]}\", \"{hashes[2]}\"),";
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

                // Now add all of the DAT hashes
                // TODO: Do we really need to save the DAT hashes?

                sr.Dispose();
            }

            slc.Dispose();
            dbc.Dispose();
            return true;
        }
    }
}
