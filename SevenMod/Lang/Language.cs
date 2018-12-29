// <copyright file="Language.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Lang
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Handles translating strings into the appropriate language.
    /// </summary>
    internal class Language
    {
        /// <summary>
        /// The default language to use when the requested phrase does not exist in the requested language.
        /// </summary>
        public static readonly string DefaultLang = "en";

        /// <summary>
        /// The custom <see cref="IFormatProvider"/> for formatting strings.
        /// </summary>
        private static readonly SMFormatProvider FormatProvider = new SMFormatProvider();

        /// <summary>
        /// The list of core translation phrases.
        /// </summary>
        private static string[] coreFiles = new string[]
        {
            "Core.Phrases",
        };

        /// <summary>
        /// The value of the ServerLang <see cref="ConVar"/>.
        /// </summary>
        private static ConVarValue serverLang;

        /// <summary>
        /// The translation phrases.
        /// </summary>
        private static Dictionary<string, PhraseDictionary> phrases = new Dictionary<string, PhraseDictionary>();

        /// <summary>
        /// Lists of plugins referencing each translation file.
        /// </summary>
        private static Dictionary<string, HashSet<IPlugin>> pluginReferences = new Dictionary<string, HashSet<IPlugin>>();

        /// <summary>
        /// The map of users to their languages.
        /// </summary>
        private static Dictionary<string, string> userLang = new Dictionary<string, string>();

        /// <summary>
        /// Initializes the language system.
        /// </summary>
        public static void Init()
        {
            serverLang = ConVarManager.CreateConVar(null, "ServerLang", DefaultLang, "The language key for the server").Value;
            serverLang.ConVar.ValueChanged += OnServerLanguageChanged;

            foreach (var file in coreFiles)
            {
                LoadPhrases(file);
            }
        }

        /// <summary>
        /// Loads the files for a set of phrases.
        /// </summary>
        /// <param name="plugin">The plugin loading the translation file.</param>
        /// <param name="file">The name of the translation file.</param>
        public static void LoadPhrases(IPlugin plugin, string file)
        {
            if (coreFiles.Contains(file) || pluginReferences.ContainsKey(file))
            {
                pluginReferences[file].Add(plugin);
                return;
            }

            LoadPhrases(file);

            if (!pluginReferences.ContainsKey(file))
            {
                pluginReferences[file] = new HashSet<IPlugin>();
            }

            pluginReferences[file].Add(plugin);
        }

        /// <summary>
        /// Checks if a translation phrase exists.
        /// </summary>
        /// <param name="phrase">The phrase to check.</param>
        /// <returns><c>true</c> if the phrase specified by <paramref name="phrase"/> exists; otherwise <c>false</c>.</returns>
        public static bool PhraseExists(string phrase)
        {
            return phrases.ContainsKey(phrase);
        }

        /// <summary>
        /// Gets a string translated into the appropriate language.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the user for which to translate the phrase.</param>
        /// <param name="args">The argument values for the phrase.</param>
        /// <returns>The translated phrase.</returns>
        public static string GetString(string phrase, ClientInfo client, params object[] args)
        {
            if (!phrases.ContainsKey(phrase))
            {
                return string.Format(phrase, args);
            }

            var lang = GetLanguageKey(client);
            return string.Format(FormatProvider, phrases[phrase][lang], args);
        }

        /// <summary>
        /// Removes all references to a plugin and cleans up unreferenced resources.
        /// </summary>
        /// <param name="plugin">The plugin to unload.</param>
        public static void UnloadPlugin(IPlugin plugin)
        {
            foreach (var plugins in pluginReferences)
            {
                if (plugins.Value.Remove(plugin) || plugins.Value.Count == 0)
                {
                    phrases.RemoveAll((PhraseDictionary pd) => pd.File == plugins.Key);
                }
            }

            pluginReferences.RemoveAll((HashSet<IPlugin> hs) => hs.Count == 0);
        }

        /// <summary>
        /// Loads the files for a set of phrases.
        /// </summary>
        /// <param name="file">The name of the translation file.</param>
        private static void LoadPhrases(string file)
        {
            var xml = new XmlDocument();
            foreach (var path in Directory.GetFiles(SMPath.Translations, $"{file}.xml", SearchOption.AllDirectories))
            {
                try
                {
                    xml.Load(path);
                }
                catch (XmlException e)
                {
                    SMLog.Error($"Failed reading translation file from {path}: {e.Message}");
                    continue;
                }

                foreach (XmlElement phraseElement in xml.GetElementsByTagName("Phrase"))
                {
                    if (!phraseElement.HasAttribute("Name"))
                    {
                        continue;
                    }

                    var key = phraseElement.GetAttribute("Name");
                    if (phrases.ContainsKey(key))
                    {
                        SMLog.Error($"Phrase {key} in file {file}.xml conflicts with file {phrases[key].File}.xml");
                        continue;
                    }

                    var pd = new PhraseDictionary(file, key);
                    foreach (XmlElement formatElement in phraseElement.GetElementsByTagName("Format"))
                    {
                        var mc = Regex.Matches(formatElement.InnerText, @"\{([1-9][0-9]*)\:([0-9a-zA-Z]+)\}");
                        if (mc.Count == 0)
                        {
                            throw new Exception($"Invalid format string for translation file {file}: {formatElement.InnerText}");
                        }

                        foreach (Match m in mc)
                        {
                            pd.FormatArgs[int.Parse(m.Groups[1].Value)] = m.Groups[2].Value;
                        }

                        break;
                    }

                    foreach (var langElement in phraseElement.ChildNodes)
                    {
                        if (langElement is XmlElement)
                        {
                            var lang = ((XmlElement)langElement).Name;
                            var phraseString = ((XmlElement)langElement).InnerText;
                            if (lang.Equals("Format"))
                            {
                                continue;
                            }

                            pd[lang] = phraseString;
                        }
                    }

                    phrases[key] = pd;
                }
            }
        }

        /// <summary>
        /// Called when the value of the ServerLang <see cref="ConVar"/> is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ConVarChangedEventArgs"/> object containing the event data.</param>
        private static void OnServerLanguageChanged(object sender, ConVarChangedEventArgs e)
        {
            if (!phrases.ContainsKey(e.NewValue))
            {
                serverLang.Value = e.OldValue;
            }
        }

        /// <summary>
        /// Gets the language key for a user.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the user.</param>
        /// <returns>The two character language code for the user.</returns>
        private static string GetLanguageKey(ClientInfo client)
        {
            if (client == null)
            {
                return serverLang.AsString;
            }

            if (!userLang.ContainsKey(client.playerId))
            {
                LoadUser(client);
            }

            return userLang[client.playerId];
        }

        /// <summary>
        /// Loads the language settings for a user.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the user.</param>
        private static void LoadUser(ClientInfo client)
        {
            userLang[client.playerId] = serverLang.AsString;
        }
    }
}
