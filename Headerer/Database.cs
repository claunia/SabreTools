using System.IO;
using Microsoft.Data.Sqlite;
using SabreTools.IO;

namespace Headerer
{
    internal static class Database
    {
        #region Constants

        public static string HeadererFileName = Path.Combine(PathTool.GetRuntimeDirectory(), "Headerer.sqlite");
        public static string HeadererConnectionString = $"Data Source={HeadererFileName};";

        #endregion

        /// <summary>
        /// Ensure that the database exists and has the proper schema
        /// </summary>
        public static void EnsureDatabase()
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
    }
}