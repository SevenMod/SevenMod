// <copyright file="SMPath.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Contains paths used by the mod.
    /// </summary>
    public class SMPath
    {
        /// <summary>
        /// The path to the directory containing the main mod dll.
        /// </summary>
        public static readonly string ModPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The path to the directory containing data and configuration files.
        /// </summary>
        public static readonly string SavePath = $"{Path.GetFullPath($"{GameUtils.GetSaveGameDir()}{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..")}{Path.DirectorySeparatorChar}SevenMod{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The path to the directory containing the configuration files.
        /// </summary>
        public static readonly string Config = $"{SavePath}Config{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The path to the directory containing the data files.
        /// </summary>
        public static readonly string Data = $"{SavePath}Data{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The path to the directory containing the database files.
        /// </summary>
        public static readonly string Databases = $"{Data}Databases{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The path to the directory containing the log files.
        /// </summary>
        public static readonly string Logs = $"{SavePath}Logs{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The path to the directory containing the plugin files.
        /// </summary>
        public static readonly string Plugins = $"{ModPath}Plugins{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The path to the directory containing the disabled plugin files.
        /// </summary>
        public static readonly string DisabledPlugins = $"{Plugins}Disabled{Path.DirectorySeparatorChar}";

        /// <summary>
        /// Initializes static members of the <see cref="SMPath"/> class.
        /// </summary>
        static SMPath()
        {
            Directory.CreateDirectory(Config);
            Directory.CreateDirectory(Data);
            Directory.CreateDirectory(Databases);
            Directory.CreateDirectory(Logs);
            Directory.CreateDirectory(Plugins);
            Directory.CreateDirectory(DisabledPlugins);
        }
    }
}
