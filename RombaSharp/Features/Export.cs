using System.Collections.Generic;
using System.IO;

using SabreTools.Help;
using Microsoft.Data.Sqlite;

namespace RombaSharp.Features
{
    internal class Export : BaseFeature
    {
        public const string Value = "Export";

        // Unique to RombaSharp
        public Export()
        {
            Name = Value;
            Flags = new List<string>() { "export" };
            Description = "Exports db to export.csv";
            _featureType = ParameterType.Flag;
            LongDescription = "Exports db to standardized export.csv";
            Features = new Dictionary<string, Feature>();
        }

        // TODO: Add ability to say which depot the files are found in
        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            SqliteConnection dbc = new SqliteConnection(_connectionString);
            dbc.Open();
            StreamWriter sw = new StreamWriter(File.Create("export.csv"));

            // First take care of all file hashes
            sw.WriteLine("CRC,MD5,SHA-1"); // ,Depot

            string query = "SELECT crcsha1.crc, md5sha1.md5, md5sha1.sha1 FROM crcsha1 JOIN md5sha1 ON crcsha1.sha1=md5sha1.sha1"; // md5sha1.sha1=sha1depot.sha1
            SqliteCommand slc = new SqliteCommand(query, dbc);
            SqliteDataReader sldr = slc.ExecuteReader();

            if (sldr.HasRows)
            {
                while (sldr.Read())
                {
                    string line = $"{sldr.GetString(0)},{sldr.GetString(1)},{sldr.GetString(2)}"; // + ",{sldr.GetString(3)}";
                    sw.WriteLine(line);
                }
            }

            // Then take care of all DAT hashes
            sw.WriteLine();
            sw.WriteLine("DAT Hash");

            query = "SELECT hash FROM dat";
            slc = new SqliteCommand(query, dbc);
            sldr = slc.ExecuteReader();

            if (sldr.HasRows)
            {
                while (sldr.Read())
                {
                    sw.WriteLine(sldr.GetString(0));
                }
            }

            sldr.Dispose();
            slc.Dispose();
            sw.Dispose();
            dbc.Dispose();
        }
    }
}
