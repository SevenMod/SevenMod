// <copyright file="PhraseDictionary.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Lang
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a dictionary of translations for a phrase.
    /// </summary>
    internal class PhraseDictionary
    {
        /// <summary>
        /// The translations for this phrase.
        /// </summary>
        private Dictionary<string, string> phrases = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseDictionary"/> class.
        /// </summary>
        /// <param name="file">The name of the file containing this phrase.</param>
        /// <param name="key">The key for this phrase.</param>
        public PhraseDictionary(string file, string key)
        {
            this.File = file;
            this.Key = key;
        }

        /// <summary>
        /// Gets the name of the file containing this phrase.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets the key for this phrase.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the formatting arguments for this phrase.
        /// </summary>
        public Dictionary<int, string> FormatArgs { get; } = new Dictionary<int, string>();

        /// <summary>
        /// Gets or sets the phrase format string for a specified language.
        /// </summary>
        /// <param name="lang">The two character language code.</param>
        /// <returns>The phrase format string for the specified language.</returns>
        public string this[string lang]
        {
            get => this.GetString(lang);
            set => this.SetString(lang, value);
        }

        /// <summary>
        /// Checks if this phrase contains a translation in a specific language.
        /// </summary>
        /// <param name="lang">The two character language code.</param>
        /// <returns><c>true</c> if this phrase contains a translation in the <paramref name="lang"/> language; otherwise <c>false</c>.</returns>
        public bool ContainsLanguage(string lang)
        {
            return this.phrases.ContainsKey(lang);
        }

        /// <summary>
        /// Gets the phrase format string for a specified language.
        /// </summary>
        /// <param name="lang">The two character language code.</param>
        /// <returns>The phrase format string for the specified language.</returns>
        public string GetString(string lang)
        {
            if (!this.phrases.ContainsKey(lang))
            {
                lang = Language.DefaultLang;
                if (!this.phrases.ContainsKey(lang))
                {
                    throw new Exception($"Phrase \"{this.Key}\" does not contain a translation for language \"{lang}\"");
                }
            }

            return this.phrases[lang];
        }

        /// <summary>
        /// Sets the phrase format string for a specified language.
        /// </summary>
        /// <param name="lang">The two character language code.</param>
        /// <param name="str">The phrase format string for the specified language.</param>
        public void SetString(string lang, string str)
        {
            foreach (var f in this.FormatArgs)
            {
                str = str.Replace($"{{{f.Key}}}", $"{{{f.Key - 1}:{f.Value}}}");
            }

            this.phrases[lang] = str;
        }
    }
}
