// <copyright file="BaseCommands.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: BaseCommands</para>
    /// <para>Adds the kick and who admin commands.</para>
    /// </summary>
    public class BaseCommands : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Commands",
            Author = "SevenMod",
            Description = "Adds basic admin commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            this.RegAdminCmd("kick", new AdminCmdKick(), AdminFlags.Kick);
            this.RegAdminCmd("reloadadmins", new AdminCmdReloadAdmins(), AdminFlags.Ban);
            this.RegAdminCmd("who", new AdminCmdWho(), AdminFlags.Generic);
        }
    }
}
