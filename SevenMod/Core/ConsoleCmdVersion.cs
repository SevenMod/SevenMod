// <copyright file="ConsoleCmdVersion.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Console command to display SevenMod version information.
    /// </summary>
    public class ConsoleCmdVersion : ConsoleCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_version" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "displays SevenMod version information";
        }

        /// <inheritdoc/>
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            SdtdConsole.Instance.Output($"    SevenMod Version: {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
        }
    }
}
