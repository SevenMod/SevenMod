// <copyright file="SteamUtils.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Contains methods for interacting with Steam.
    /// </summary>
    public class SteamUtils
    {
        /// <summary>
        /// Convert any SteamID format into the SteamID64 format.
        /// </summary>
        /// <param name="input">The input SteamID.</param>
        /// <param name="output">This variable will be set to the SteamID64 string if the
        /// conversion is successful; otherwise it will be set to <c>null</c>.</param>
        /// <returns><c>true</c> if the conversion was successful; <c>false</c> if
        /// <paramref name="input"/> is not a valid SteamID.</returns>
        public static bool NormalizeSteamId(string input, out string output)
        {
            if (string.IsNullOrEmpty(input))
            {
                output = null;

                return false;
            }

            input = input.ToUpper();
            Match match;

            if ("BOT".Equals(input))
            {
                output = "BOT";

                return true;
            }
            else if ((match = Regex.Match(input, @"^STEAM_0:([0-1]):([0-9]+)$")).Success)
            {
                var p1 = long.Parse(match.Groups[1].Value);
                var p2 = long.Parse(match.Groups[2].Value);
                output = (((p2 * 2) + p1) + 76561197960265728L).ToString();

                return true;
            }
            else if ((match = Regex.Match(input, @"^\[?U:1:([0-9]+)\]?$")).Success)
            {
                var p1 = long.Parse(match.Groups[1].Value);
                output = (p1 + 76561197960265728L).ToString();

                return true;
            }
            else if (Regex.IsMatch(input, "^[0-9]{17}$"))
            {
                output = string.Copy(input);

                return true;
            }

            output = null;

            return false;
        }
    }
}
