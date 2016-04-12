﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using DamienG.Security.Cryptography;

namespace SabreTools
{
	/*
		Auto set Name and Description from current folder
	*/

	class DATFromDir
	{
		// Path-related variables
		private static string _7zPath;
		private static string _basePath;
		private static string _tempDir;

		// Extraction and listing related variables
		private static char _delim;
		private static string _baseExtract;
		private static ProcessStartInfo _psi;
		private static List<Tuple<string, string, long, string, string, string>> _roms;

		// User specified variables
		private static bool _noMD5;
		private static bool _noSHA1;
		private static bool _forceunzip;
		private static bool _allfiles;
		private static bool _old;
		private static string _name;
		private static string _desc;
		private static string _cat;

		// Other required variables
		private static string _version = DateTime.Now.ToString("yyyyMMddHHmmss");

		static void Main(string[] args)
		{
			Console.Clear();

			// First things first, take care of all of the arguments that this could have
			_noMD5 = false; _noSHA1 = false; _forceunzip = false; _allfiles = false; _old = false;
			_name = ""; _desc = ""; _cat = "SabreTools Dir2DAT";
			List<string> inputs = new List<string>();
			foreach (string arg in args)
			{
				switch (arg)
				{
					case "-h":
					case "-?":
					case "--help":
						Help();
						return;
					case "-m":
					case "--noMD5":
						_noMD5 = true;
						break;
					case "-s":
					case "--noSHA1":
						_noSHA1 = true;
						break;
					case "-u":
					case "--unzip":
						_forceunzip = true;
						break;
					case "-f":
					case "--files":
						_allfiles = true;
						break;
					case "-o":
					case "--old":
						_old = true;
						break;
					default:
						if (arg.StartsWith("-n=") || arg.StartsWith("--name="))
						{
							_name = arg.Split('=')[1];
						}
						else if (arg.StartsWith("-d=") || arg.StartsWith("--desc="))
						{
							_desc = arg.Split('=')[1];
						}
						else if (arg.StartsWith("-c=") || arg.StartsWith("--cat="))
						{
							_cat = arg.Split('=')[1];
						}
						else
						{
							inputs.Add(arg);
						}
						break;
				}
			}

			// If there's no inputs, show the help
			if (inputs.Count == 0)
			{
				Help();
				return;
			}
			
			// Determine the deliminator that is to be used
			if (Environment.CurrentDirectory.Contains("\\"))
			{
				_delim = '\\';
			}
			else
			{
				_delim = '/';
			}

			// Set 7za required variables
			_7zPath = Environment.CurrentDirectory + _delim + "7z" + (Environment.Is64BitOperatingSystem ? _delim + "x64" : "") + _delim;
			_psi = new ProcessStartInfo
			{
				Arguments = "",
				FileName = _7zPath + "7za.exe",
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
			};

			// Create an output array for all found items (parent, name, size, crc, md5, sha1)
			_roms = new List<Tuple<string, string, long, string, string, string>>();

			// Loop over each of the found paths, if any
			foreach (string path in inputs)
			{
				// Set local paths and vars
				_tempDir = Environment.CurrentDirectory + _delim + "temp" + DateTime.Now.ToString("yyyyMMddHHmmss") + _delim;
				_basePath = (args.Length == 0 ? Environment.CurrentDirectory + _delim : (File.Exists(args[0]) ? args[0] : args[0] + _delim));
				_baseExtract = "x -o\"" + _tempDir + "\"";
				_name = (_name == "" ? _basePath.Split(_delim).Last() : _name);
				_desc = (_desc == "" ? _name + " (" + _version + ")" : _desc);

				// This is where the main loop would go
				if (File.Exists(_basePath))
				{
					ProcessFile(_basePath);
				}
				else
				{
					foreach (string item in Directory.GetFiles(_basePath, "*", SearchOption.AllDirectories))
					{
						ProcessFile(item);
					}
				}
			}

			// Order the roms by name of parent, then name of rom
			_roms.Sort(delegate (Tuple<string, string, long, string, string, string> A, Tuple<string, string, long, string, string, string> B)
			{
				if (A.Item1 == B.Item1)
				{
					if (A.Item2 == B.Item2)
					{
						return (int)(A.Item3 - B.Item3);
					}
					return String.Compare(A.Item2, B.Item2);
				}
				return String.Compare(A.Item1, B.Item1);
			});

			//TODO: So, this below section is a pretty much one for one copy of code that is written in generate
			//		this means that in the future, "writing to DAT" will be abstracted out to the DLL so that any
			//		properly formatted data can be passed in and it will get written as necessary. That would open
			//		the possibiliites for different ways to generate a DAT from multiple things

			// Now write it all out as a DAT
			try
			{
				FileStream fs = File.Create(_desc + ".xml");
				StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

				string header_old = "clrmamepro (\n" +
					"\tname \"" + _name + "\"\n" +
					"\tdescription \"" + _desc + "\"\n" +
					"\tversion \"" + _version + "\"\n" +
					"\tcomment \"\"\n" +
					"\tauthor \"Darksabre76\"\n" +
					(_forceunzip ? "\tforcezipping no\n" : "") + 
					")\n";

				string header = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
					"<!DOCTYPE datafile PUBLIC \"-//Logiqx//DTD ROM Management Datafile//EN\" \"http://www.logiqx.com/Dats/datafile.dtd\">\n\n" +
					"\t<datafile>\n" +
					"\t\t<header>\n" +
					"\t\t\t<name>" + HttpUtility.HtmlEncode(_name) + "</name>\n" +
					"\t\t\t<description>" + HttpUtility.HtmlEncode(_desc) + "</description>\n" +
					"\t\t\t<category>SabreTools Dir2DAT</category>\n" +
					"\t\t\t<version>" + _version + "</version>\n" +
					"\t\t\t<date>" + _version + "</date>\n" +
					"\t\t\t<author>Darksabre76</author>\n" +
					"\t\t\t<clrmamepro " + (_forceunzip ? "forcepacking=\"unzip\" " : "") + " />\n" +
					"\t\t</header>\n";

				// Write the header out
				sw.Write((_old ? header_old : header));

				// Write out each of the machines and roms
				string lastgame = "";
				foreach (Tuple<string, string, long, string, string, string> rom in _roms)
				{
					string state = "";
					if (lastgame != "" && lastgame != rom.Item1)
					{
						state += (_old ? ")\n" : "\t</machine>\n");
					}

					if (lastgame != rom.Item1)
					{
						state += (_old ? "game (\n\tname \"" + rom.Item1 + "\"\n" +
							"\tdescription \"" + rom.Item1 + "\"\n" :
							"\t<machine name=\"" + HttpUtility.HtmlEncode(rom.Item1) + "\">\n" +
							"\t\t<description>" + HttpUtility.HtmlEncode(rom.Item1) + "</description>\n");
					}

					if (_old)
					{
						state += "\trom ( name \"" + rom.Item2 + "\"" +
							(rom.Item3 != 0 ? " size " + rom.Item3 : "") +
							(rom.Item4 != "" ? " crc " + rom.Item4.ToLowerInvariant() : "") +
							(rom.Item5 != "" ? " md5 " + rom.Item5.ToLowerInvariant() : "") +
							(rom.Item6 != "" ? " sha1 " + rom.Item6.ToLowerInvariant() : "") +
							" )\n";
					}
					else
					{
						state += "\t\t<rom name=\"" + HttpUtility.HtmlEncode(rom.Item2) + "\"" +
							(rom.Item3 != -1 ? " size=\"" + rom.Item3 + "\"" : "") +
							(rom.Item4 != "" ? " crc=\"" + rom.Item4.ToLowerInvariant() + "\"" : "") +
							(rom.Item5 != "" ? " md5=\"" + rom.Item5.ToLowerInvariant() + "\"" : "") +
							(rom.Item6 != "" ? " sha1=\"" + rom.Item6.ToLowerInvariant() + "\"" : "") +
							" />\n";
					}

					lastgame = rom.Item1;

					sw.Write(state);
				}

				sw.Write((_old ? ")" : "\t</machine>\n</datafile>"));
				Console.Write("File written!");
				sw.Close();
				fs.Close();
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}
		}

		private static void Help()
		{
			Console.WriteLine(@"DATFromDir - Create a DAT file from a directory
-----------------------------------------
Usage: DATFromDir [options] [filename|dirname] <filename|dirname> ...

Options:
  -h, -?, --help	Show this help dialog
  -m, --noMD5		Don't include MD5 in output
  -s, --noSHA1		Don't include SHA1 in output
  -u, --unzip		Force unzipping in created DAT
  -f, --files		Treat archives as files
  -o, --old		Output DAT in RV format instead of XML
  -n=, --name=		Set the name of the DAT
  -d=, --desc=		Set the description of the DAT
  -c=, --cat=		Set the category of the DAT");
		}

		private static void ProcessFile (string item)
		{
			// Create the temporary output directory
			Directory.CreateDirectory(_tempDir);

			bool encounteredErrors = true;
			if (!_allfiles)
			{
				_psi.Arguments = _baseExtract + " " + item;
				Process zip = Process.Start(_psi);
				zip.WaitForExit();

				encounteredErrors = zip.StandardError.ReadToEnd().Contains("ERROR");
			}

			// Get a list of files including size and hashes
			Crc32 crc = new Crc32();
			MD5 md5 = MD5.Create();
			SHA1 sha1 = SHA1.Create();

			// If the file was an archive and was extracted successfully, check it
			if (!encounteredErrors)
			{
				foreach (string entry in Directory.GetFiles(_tempDir, "*", SearchOption.AllDirectories))
				{
					string fileCRC = String.Empty;
					string fileMD5 = String.Empty;
					string fileSHA1 = String.Empty;

					try
					{
						using (FileStream fs = File.Open(entry, FileMode.Open))
						{
							foreach (byte b in crc.ComputeHash(fs))
							{
								fileCRC += b.ToString("x2").ToLower();
							}
						}
						if (!_noMD5)
						{
							using (FileStream fs = File.Open(entry, FileMode.Open))
							{
								fileMD5 = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
							}
						}
						if (!_noSHA1)
						{
							using (FileStream fs = File.Open(entry, FileMode.Open))
							{
								fileSHA1 = BitConverter.ToString(sha1.ComputeHash(fs)).Replace("-", "");
							}
						}
					}
					catch (IOException)
					{
						continue;
					}

					_roms.Add(new Tuple<string, string, long, string, string, string>(
						Path.GetFileNameWithoutExtension(item),
						entry.Remove(0, _tempDir.Length),
						(new FileInfo(entry)).Length,
						fileCRC,
						fileMD5,
						fileSHA1));

					Console.WriteLine("File parsed: " + entry.Remove(0, _tempDir.Length));
				}
			}
			// Otherwise, just get the info on the file itself
			else if (!Directory.Exists(item) && File.Exists(item))
			{
				string fileCRC = String.Empty;
				string fileMD5 = String.Empty;
				string fileSHA1 = String.Empty;

				try
				{
					using (FileStream fs = File.Open(item, FileMode.Open))
					{
						foreach (byte b in crc.ComputeHash(fs))
						{
							fileCRC += b.ToString("x2").ToLower();
						}
					}
					if (!_noMD5)
					{
						using (FileStream fs = File.Open(item, FileMode.Open))
						{
							fileMD5 = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
						}
					}
					if (!_noSHA1)
					{
						using (FileStream fs = File.Open(item, FileMode.Open))
						{
							fileSHA1 = BitConverter.ToString(sha1.ComputeHash(fs)).Replace("-", "");
						}
					}

					string actualroot = (item == _basePath ? "Default" : Path.GetDirectoryName(item.Remove(0, _basePath.Length)).Split(_delim)[0]);
					actualroot = (actualroot == "" ? "Default" : actualroot);
					string actualitem = (item == _basePath ? item : item.Remove(0, _basePath.Length).Remove(0, (actualroot != "Default" ? actualroot.Length + 1 : 0)));

					_roms.Add(new Tuple<string, string, long, string, string, string>(
						actualroot,
						actualitem,
						(new FileInfo(item)).Length,
						fileCRC,
						fileMD5,
						fileSHA1));

					Console.WriteLine("File parsed: " + item.Remove(0, _basePath.Length));
				}
				catch (IOException) { }
			}

			// Delete the temp directory
			if (Directory.Exists(_tempDir))
			{
				Directory.Delete(_tempDir, true);
			}
		}
	}
}
