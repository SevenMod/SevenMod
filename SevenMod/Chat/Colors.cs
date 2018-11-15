// <copyright file="Colors.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper methods for using colors in chat.
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Map of common color names to their hexadecimal string value.
        /// </summary>
        private static readonly Dictionary<string, string> ColorNames = new Dictionary<string, string>()
        {
            { "white", "FFFFFF" },
            { "red", "FF0000" },
            { "green", "00FF00" },
            { "blue", "0000FF" },
            { "yellow", "FFFF00" },
            { "purple", "FF00FF" },
            { "cyan", "00FFFF" },
            { "orange", "FF8000" },
            { "pink", "FF0080" },
            { "olive", "80FF00" },
            { "lime", "00FF80" },
            { "violet", "8000FF" },
            { "lightblue", "0080FF" },
        };

        /// <summary>
        /// Gets the hexadecimal string value for white.
        /// </summary>
        public static string White { get; } = ColorNames["white"];

        /// <summary>
        /// Gets the hexadecimal string value for red.
        /// </summary>
        public static string Red { get; } = ColorNames["red"];

        /// <summary>
        /// Gets the hexadecimal string value for green.
        /// </summary>
        public static string Green { get; } = ColorNames["green"];

        /// <summary>
        /// Gets the hexadecimal string value for blue.
        /// </summary>
        public static string Blue { get; } = ColorNames["blue"];

        /// <summary>
        /// Gets the hexadecimal string value for yellow.
        /// </summary>
        public static string Yellow { get; } = ColorNames["yellow"];

        /// <summary>
        /// Gets the hexadecimal string value for purple.
        /// </summary>
        public static string Purple { get; } = ColorNames["purple"];

        /// <summary>
        /// Gets the hexadecimal string value for cyan.
        /// </summary>
        public static string Cyan { get; } = ColorNames["cyan"];

        /// <summary>
        /// Gets the hexadecimal string value for orange.
        /// </summary>
        public static string Orange { get; } = ColorNames["orange"];

        /// <summary>
        /// Gets the hexadecimal string value for pink.
        /// </summary>
        public static string Pink { get; } = ColorNames["pink"];

        /// <summary>
        /// Gets the hexadecimal string value for olive.
        /// </summary>
        public static string Olive { get; } = ColorNames["olive"];

        /// <summary>
        /// Gets the hexadecimal string value for lime.
        /// </summary>
        public static string Lime { get; } = ColorNames["lime"];

        /// <summary>
        /// Gets the hexadecimal string value for violet.
        /// </summary>
        public static string Violet { get; } = ColorNames["violet"];

        /// <summary>
        /// Gets the hexadecimal string value for light blue.
        /// </summary>
        public static string Lightblue { get; } = ColorNames["lightblue"];

        /// <summary>
        /// Checks whether a given string is valid named color.
        /// </summary>
        /// <param name="colorStr">The string to check.</param>
        /// <returns><c>true</c> if <paramref name="colorStr"/> is a valid color name; <c>false</c>
        /// otherwise.</returns>
        public static bool IsValidColorName(string colorStr)
        {
            return ColorNames.ContainsKey(colorStr.ToLower());
        }

        /// <summary>
        /// Checks whether a given string is a valid hexadecimal color.
        /// </summary>
        /// <param name="colorStr">The string to check.</param>
        /// <returns><c>true</c> if <paramref name="colorStr"/> is a valid hexadecimal color;
        /// <c>false</c> otherwise.</returns>
        public static bool IsValidColorHex(string colorStr)
        {
            return Regex.IsMatch(colorStr, @"^[0-9A-Za-z]{6}$");
        }

        /// <summary>
        /// Gets the hexadecimal string for a color name.
        /// </summary>
        /// <param name="colorName">The name of the color.</param>
        /// <param name="defaultHex">The default value to return if <paramref name="colorName"/> is
        /// not a valid color name.</param>
        /// <returns>The hexadecimal color string; <paramref name="defaultHex"/> if
        /// <paramref name="colorName"/> is not a valid color name.</returns>
        public static string GetHexFromColorName(string colorName, string defaultHex = "000000")
        {
            if (IsValidColorName(colorName))
            {
                return ColorNames[colorName];
            }

            return defaultHex;
        }
    }
}
