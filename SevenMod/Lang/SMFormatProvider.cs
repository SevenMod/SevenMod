// <copyright file="SMFormatProvider.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Lang
{
    using System;

    /// <summary>
    /// Provides custom formats for SevenMod.
    /// </summary>
    internal class SMFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// Gets or sets the current language code.
        /// </summary>
        public string Lang { get; set; } = Language.DefaultLang;

        /// <inheritdoc/>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == "t")
            {
                return Language.GetRawPhrase(arg.ToString(), this.Lang);
            }
            else if (format == "L")
            {
                var client = arg as ClientInfo;
                if (client == null)
                {
                    return "Console<0><Console><Console>";
                }
                else
                {
                    return $"{client.playerName}<{client.entityId}><{client.playerId}><>";
                }
            }
            else if (format == "N")
            {
                return arg is ClientInfo ? ((ClientInfo)arg).playerName : "Console";
            }
            else if (arg is IFormattable)
            {
                return ((IFormattable)arg).ToString(format, formatProvider);
            }

            return arg.ToString();
        }

        /// <inheritdoc/>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }
    }
}
