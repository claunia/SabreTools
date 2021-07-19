﻿using System;
using System.IO;
using System.Reflection;

namespace SabreTools.Core
{
    /// <summary>
    /// Generic console preparation for program output
    /// </summary>
    public static class Prepare
    {
        /// <summary>
        /// The current toolset version to be used by all child applications
        /// </summary>
        //public readonly static string Version = $"v1.1.2";
        public readonly static string Version = $"v1.1.2-{File.GetCreationTime(Assembly.GetExecutingAssembly().Location):yyyy-MM-dd HH:mm:ss}";
 
        /// <summary>
        /// Readies the console and outputs the header
        /// </summary>
        /// <param name="program">The name to be displayed as the program</param>
        public static void SetConsoleHeader(string program)
        {
            // Dynamically create the header string, adapted from http://stackoverflow.com/questions/8200661/how-to-align-string-in-fixed-length-string
            int width = Console.WindowWidth - 3;
            string border = $"+{new string('-', width)}+";
            string mid = $"{program} {Version}";
            mid = $"|{mid.PadLeft(((width - mid.Length) / 2) + mid.Length).PadRight(width)}|";

            // If we're outputting to console, do fancy things
            if (!Console.IsOutputRedirected)
            {
                // Set the console to ready state
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    Console.SetBufferSize(Console.BufferWidth, 999);

                ConsoleColor formertext = Console.ForegroundColor;
                ConsoleColor formerback = Console.BackgroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Blue;

                Console.Title = $"{program} {Version}";

                // Output the header
                Console.WriteLine(border);
                Console.WriteLine(mid);
                Console.WriteLine(border);
                Console.WriteLine();

                // Return the console to the original text and background colors
                Console.ForegroundColor = formertext;
                    Console.BackgroundColor = formerback;
            }
        }
    }
}
