using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.Help;
using SabreTools.Library.Tools;
using Microsoft.Data.Sqlite;

namespace RombaSharp.Features
{
    internal class Lookup : BaseFeature
    {
        public const string Value = "Lookup";

        public Lookup()
        {
            Name = Value;
            Flags = new List<string>() { "lookup" };
            Description = "For each specified hash it looks up any available information.";
            _featureType = FeatureType.Flag;
            LongDescription = "For each specified hash it looks up any available information (dat or rom).";
            Features = new Dictionary<string, Feature>();

            AddFeature(SizeInt64Input); // Defaults to -1
            AddFeature(OutStringInput);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            long size = GetInt64(features, SizeInt64Value);
            string outdat = GetString(features, OutStringValue);

            // First, try to figure out what type of hash each is by length and clean it
            List<string> crc = new List<string>();
            List<string> md5 = new List<string>();
            List<string> sha1 = new List<string>();
            foreach (string input in Inputs)
            {
                string temp = string.Empty;
                if (input.Length == Constants.CRCLength)
                {
                    temp = Sanitizer.CleanCRC32(input);
                    if (!string.IsNullOrWhiteSpace(temp))
                    {
                        crc.Add(temp);
                    }
                }
                else if (input.Length == Constants.MD5Length)
                {
                    temp = Sanitizer.CleanMD5(input);
                    if (!string.IsNullOrWhiteSpace(temp))
                    {
                        md5.Add(temp);
                    }
                }
                else if (input.Length == Constants.SHA1Length)
                {
                    temp = Sanitizer.CleanSHA1(input);
                    if (!string.IsNullOrWhiteSpace(temp))
                    {
                        sha1.Add(temp);
                    }
                }
            }

            SqliteConnection dbc = new SqliteConnection(_connectionString);
            dbc.Open();

            // Now, search for each of them and return true or false for each
            foreach (string input in crc)
            {
                string query = $"SELECT * FROM crc WHERE crc=\"{input}\"";
                SqliteCommand slc = new SqliteCommand(query, dbc);
                SqliteDataReader sldr = slc.ExecuteReader();
                if (sldr.HasRows)
                {
                    int count = 0;
                    while (sldr.Read())
                    {
                        count++;
                    }

                    logger.User($"For hash '{input}' there were {count} matches in the database");
                }
                else
                {
                    logger.User($"Hash '{input}' had no matches in the database");
                }

                sldr.Dispose();
                slc.Dispose();
            }
            foreach (string input in md5)
            {
                string query = $"SELECT * FROM md5 WHERE md5=\"{input}\"";
                SqliteCommand slc = new SqliteCommand(query, dbc);
                SqliteDataReader sldr = slc.ExecuteReader();
                if (sldr.HasRows)
                {
                    int count = 0;
                    while (sldr.Read())
                    {
                        count++;
                    }

                    logger.User($"For hash '{input}' there were {count} matches in the database");
                }
                else
                {
                    logger.User($"Hash '{input}' had no matches in the database");
                }

                sldr.Dispose();
                slc.Dispose();
            }
            foreach (string input in sha1)
            {
                string query = $"SELECT * FROM sha1 WHERE sha1=\"{input}\"";
                SqliteCommand slc = new SqliteCommand(query, dbc);
                SqliteDataReader sldr = slc.ExecuteReader();
                if (sldr.HasRows)
                {
                    int count = 0;
                    while (sldr.Read())
                    {
                        count++;
                    }

                    logger.User($"For hash '{input}' there were {count} matches in the database");
                }
                else
                {
                    logger.User($"Hash '{input}' had no matches in the database");
                }

                sldr.Dispose();
                slc.Dispose();
            }

            dbc.Dispose();
        }
    }
}
