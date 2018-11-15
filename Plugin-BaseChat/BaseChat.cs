// <copyright file="BaseChat.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseChat
{
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: BaseChat</para>
    /// <para>Adds basic admin chat commands.</para>
    /// </summary>
    public class BaseChat : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Chat Commands",
            Author = "SevenMod",
            Description = "Adds basic admin chat commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            this.RegAdminCmd("say", new AdminCmdSay(), AdminFlags.Chat);
            this.RegAdminCmd("psay", new AdminCmdPsay(), AdminFlags.Chat);
            this.RegAdminCmd("chat", new AdminCmdChat(), AdminFlags.Chat);
        }
    }
}
