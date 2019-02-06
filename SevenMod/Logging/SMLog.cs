// <copyright file="SMLog.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Logging
{
    using System;
    using System.IO;
    using System.Reflection;
    using SevenMod.Core;
    using SevenMod.Lang;

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
        /// Writes exception details to the error log file.
        /// </summary>
        /// <param name="e">The exception to record.</param>
        /// <param name="tag">The tag identifying the source of the message.</param>
        public static void Error(Exception e, string tag = "SM")
        {
            Error(e.Message, tag);
            foreach (var l in e.StackTrace.Split('\n'))
            {
                WriteLine(errorLog, l.TrimEnd());
            }
        }

        /// <summary>
        /// Logs an action performed by an admin.
        /// </summary>
        /// <param name="plugin">The plugin logging the action.</param>
        /// <param name="client">The <see cref="ClientInfo"/> representing the client performing the action, if applicable.</param>
        /// <param name="target">The <see cref="ClientInfo"/> representing the target of the action, if applicable.</param>
        /// <param name="message">The message describing the action.</param>
        /// <param name="args">The arguments for <paramref name="message"/>.</param>
        public static void LogAction(IPlugin plugin, ClientInfo client, ClientInfo target, string message, params object[] args)
        {
            Out(Language.GetString(message, null, args), plugin.Container.File);
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
