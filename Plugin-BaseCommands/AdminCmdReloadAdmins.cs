// <copyright file="AdminCmdReloadAdmins.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using System.Collections.Generic;
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// Admin command to reload the admin list.
    /// </summary>
    public class AdminCmdReloadAdmins : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "reloads the admin list";

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            AdminManager.RemoveAllAdmins();
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.ReloadAdmins();
            }
        }
    }
}
