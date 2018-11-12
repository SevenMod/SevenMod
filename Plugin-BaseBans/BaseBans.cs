// <copyright file="BaseBans.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseBans
{
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: BaseBans</para>
    /// <para>Adds the ban, addban, and unban admin commands.</para>
    /// </summary>
    public class BaseBans : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Ban Commands",
            Author = "SevenMod",
            Description = "Adds basic banning commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            this.RegAdminCmd("addban", new AdminCmdAddban(), AdminFlags.Ban);
            this.RegAdminCmd("ban", new AdminCmdBan(), AdminFlags.Ban);
            this.RegAdminCmd("unban", new AdminCmdUnban(), AdminFlags.Unban);
        }
    }
}
