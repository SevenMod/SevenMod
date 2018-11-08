// <copyright file="BaseCommands.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: BaseCommands</para>
    /// <para>Adds the sm_kick and sm_who admin commands.</para>
    /// </summary>
    public class BaseCommands : PluginAbstract
    {
        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            RegisterAdminCommand(typeof(AdminCmdKick), AdminFlags.Kick);
            RegisterAdminCommand(typeof(AdminCmdWho), AdminFlags.Generic);
        }
    }
}
