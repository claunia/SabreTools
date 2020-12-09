using System.Collections.Generic;

using SabreTools.Help;
using Microsoft.Data.Sqlite;

namespace RombaSharp.Features
{
    internal class DbStats : BaseFeature
    {
        public const string Value = "DbStats";

        public DbStats()
        {
            Name = Value;
            Flags = new List<string>() { "dbstats" };
            Description = "Prints db stats.";
            _featureType = ParameterType.Flag;
            LongDescription = "Print db stats.";
            Features = new Dictionary<string, Feature>();
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            SqliteConnection dbc = new SqliteConnection(_connectionString);
            dbc.Open();

            // Total number of CRCs
            string query = "SELECT COUNT(*) FROM crc";
            SqliteCommand slc = new SqliteCommand(query, dbc);
            logger.User($"Total CRCs: {(long)slc.ExecuteScalar()}");

            // Total number of MD5s
            query = "SELECT COUNT(*) FROM md5";
            slc = new SqliteCommand(query, dbc);
            logger.User($"Total MD5s: {(long)slc.ExecuteScalar()}");

            // Total number of SHA1s
            query = "SELECT COUNT(*) FROM sha1";
            slc = new SqliteCommand(query, dbc);
            logger.User($"Total SHA1s: {(long)slc.ExecuteScalar()}");

            // Total number of DATs
            query = "SELECT COUNT(*) FROM dat";
            slc = new SqliteCommand(query, dbc);
            logger.User($"Total DATs: {(long)slc.ExecuteScalar()}");

            slc.Dispose();
            dbc.Dispose();
        }
    }
}
