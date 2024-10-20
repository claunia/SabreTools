using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.DatTools;
using SabreTools.FileTypes;
using SabreTools.Hashing;
using SabreTools.Help;

namespace RombaSharp.Features
{
    internal class RescanDepots : BaseFeature
    {
        public const string Value = "Rescan Depots";

        // Unique to RombaSharp
        public RescanDepots()
        {
            Name = Value;
            Flags.AddRange(["depot-rescan"]);
            Description = "Rescan a specific depot to get new information";
            _featureType = ParameterType.Flag;
            LongDescription = "Rescan a specific depot to get new information";

            // Common Features
            AddCommonFeatures();
        }

        public override bool ProcessFeatures(Dictionary<string, SabreTools.Help.Feature?> features)
        {
            // If the base fails, just fail out
            if (!base.ProcessFeatures(features))
                return false;

            logger.Error("This feature is not yet implemented: rescan-depots");

            foreach (string depotname in Inputs)
            {
                // Check that it's a valid depot first
                if (!_depots.ContainsKey(depotname))
                {
                    logger.User($"'{depotname}' is not a recognized depot. Please add it to your configuration file and try again");
                    return false;
                }

                // Then check that the depot is online
                if (!Directory.Exists(depotname))
                {
                    logger.User($"'{depotname}' does not appear to be online. Please check its status and try again");
                    return false;
                }

                // Open the database connection
                SqliteConnection dbc = new SqliteConnection(_connectionString);
                dbc.Open();

                // If we have it, then check for all hashes that are in that depot
                List<string> hashes = [];
                string query = $"SELECT sha1 FROM sha1 WHERE depot=\"{depotname}\"";
                SqliteCommand slc = new SqliteCommand(query, dbc);
                SqliteDataReader sldr = slc.ExecuteReader();
                if (sldr.HasRows)
                {
                    while (sldr.Read())
                    {
                        hashes.Add(sldr.GetString(0));
                    }
                }

                // Now rescan the depot itself
                DatFile depot = DatFile.Create();
                DatFromDir.PopulateFromDir(depot, depotname, asFiles: TreatAsFile.NonArchive, hashes: [HashType.CRC32, HashType.MD5, HashType.SHA1]);
                depot.Items.BucketBy(ItemKey.SHA1, DedupeType.None);

                // Set the base queries to use
                string crcquery = "INSERT OR IGNORE INTO crc (crc) VALUES";
                string md5query = "INSERT OR IGNORE INTO md5 (md5) VALUES";
                string sha1query = "INSERT OR IGNORE INTO sha1 (sha1, depot) VALUES";
                string crcsha1query = "INSERT OR IGNORE INTO crcsha1 (crc, sha1) VALUES";
                string md5sha1query = "INSERT OR IGNORE INTO md5sha1 (md5, sha1) VALUES";

                // Once we have both, check for any new files
                List<string> dupehashes = [];
                IEnumerable<string> keys = depot.Items.Keys;
                foreach (string key in keys)
                {
                    ConcurrentList<DatItem>? roms = depot.Items[key];
                    if (roms == null)
                        continue;

                    foreach (Rom rom in roms)
                    {
                        if (hashes.Contains(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)!))
                        {
                            dupehashes.Add(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)!);
                            hashes.Remove(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)!);
                        }
                        else if (!dupehashes.Contains(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)!))
                        {
                            if (!string.IsNullOrWhiteSpace(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.CRCKey)))
                                crcquery += $" (\"{rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.CRCKey)}\"),";

                            if (!string.IsNullOrWhiteSpace(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.MD5Key)))
                                md5query += $" (\"{rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.MD5Key)}\"),";

                            if (!string.IsNullOrWhiteSpace(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)))
                            {
                                sha1query += $" (\"{rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)}\", \"{depotname}\"),";

                                if (!string.IsNullOrWhiteSpace(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.CRCKey)))
                                    crcsha1query += $" (\"{rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.CRCKey)}\", \"{rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)}\"),";

                                if (!string.IsNullOrWhiteSpace(rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.MD5Key)))
                                    md5sha1query += $" (\"{rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.MD5Key)}\", \"{rom.GetStringFieldValue(SabreTools.Models.Metadata.Rom.SHA1Key)}\"),";
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

                if (sha1query != "INSERT OR IGNORE INTO sha1 (sha1, depot) VALUES")
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

                // Now that we've added the information, we get to remove all of the hashes that we want to
                query = @"DELETE FROM sha1
JOIN crcsha1
    ON sha1.sha1=crcsha1.sha1
JOIN md5sha1
    ON sha1.sha1=md5sha1.sha1
JOIN crc
    ON crcsha1.crc=crc.crc
JOIN md5
    ON md5sha1.md5=md5.md5
WHERE sha1.sha1 IN ";
                query += $"({string.Join("\",\"", hashes)}\")";
                slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();

                // Dispose of the database connection
                slc.Dispose();
                dbc.Dispose();
            }

            return true;
        }
    }
}
