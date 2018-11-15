// <copyright file="PlayerCommands.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.PlayerCommands
{
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: PlayerCommands</para>
    /// <para>Adds miscellaneous player admin commands.</para>
    /// </summary>
    public class PlayerCommands : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Player Commands",
            Author = "SevenMod",
            Description = "Adds miscellaneous player admin commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            this.RegAdminCmd("slay", new AdminCmdSlay(), AdminFlags.Slay);
        }
    }
}
