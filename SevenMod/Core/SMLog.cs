// <copyright file="SMLog.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Logs messages to the SevenMod log files.
    /// </summary>
    internal class SMLog
    {
        /// <summary>
        /// The <see cref="StreamWriter"/> object for writing to the main log file.
        /// </summary>
        private static StreamWriter mainLog;

        /// <summary>
        /// The <see cref="StreamWriter"/> object for writing to the error log file.
        /// </summary>
        private static StreamWriter errorLog;

        /// <summary>
        /// Writes a message to the main log file.
        /// </summary>
        /// <param name="line">The message to write.</param>
        /// <param name="tag">The tag identifying the source of the message.</param>
        public static void Out(string line, string tag = "SM")
        {
            StartMain();
            WriteLine(mainLog, $"[{tag}] {line}");
        }

        /// <summary>
        /// Writes a message to the error log file.
        /// </summary>
        /// <param name="line">The message to write.</param>
        /// <param name="tag">The tag identifying the source of the message.</param>
        public static void Error(string line, string tag = "SM")
        {
            StartError();
            WriteLine(errorLog, $"[{tag}] {line}");
        }

        /// <summary>
        /// Closes any open streams.
        /// </summary>
        public static void Close()
        {
            if (mainLog != null)
            {
                WriteLine(mainLog, "Log file session closed");
                mainLog.Dispose();
                mainLog = null;
            }

            if (errorLog != null)
            {
                WriteLine(errorLog, "Error log file session closed");
                errorLog.Dispose();
                errorLog = null;
            }
        }

        /// <summary>
        /// Starts a logging session if one is not already started.
        /// </summary>
        private static void StartMain()
        {
            if (mainLog == null)
            {
                var fileName = $"L{DateTime.Now.ToString("yyyyMMdd")}.log";
                mainLog = new StreamWriter($"{SMPath.Logs}{fileName}", true);
                WriteLine(mainLog, $"SevenMod log file session started (file \"{fileName}\") (Version \"{Assembly.GetExecutingAssembly().GetName().Version}\")");
            }
        }

        /// <summary>
        /// Starts an error logging session if one is not already started.
        /// </summary>
        private static void StartError()
        {
            if (errorLog == null)
            {
                var fileName = $"error_{DateTime.Now.ToString("yyyyMMdd")}.log";
                errorLog = new StreamWriter($"{SMPath.Logs}{fileName}", true);
                WriteLine(errorLog, $"SevenMod error log file session started (file \"{fileName}\") (Version \"{Assembly.GetExecutingAssembly().GetName().Version}\")");
            }
        }

        /// <summary>
        /// Writes a log line to a stream.
        /// </summary>
        /// <param name="writer">The <see cref="StreamWriter"/> wrapping the stream to which to write.</param>
        /// <param name="line">The line to write.</param>
        private static void WriteLine(StreamWriter writer, string line)
        {
            var time = DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss");
            writer.WriteLine($"L {time}: {line}");
            writer.Flush();
        }
    }
}
