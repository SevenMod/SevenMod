﻿// <copyright file="ReservedSlots.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.ReservedSlots
{
    using System.Text;
    using SevenMod.Admin;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that adds reserved slots.
    /// </summary>
    public class ReservedSlots : PluginAbstract
    {
        /// <summary>
        /// The value of the ReservedSlots <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue reservedSlots;

        /// <summary>
        /// The player limit for the server.
        /// </summary>
        private int maxPlayers;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Reserved Slots",
            Author = "SevenMod",
            Description = "Adds reserved slots.",
            Version = "0.1.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.LoadTranslations("ReservedSlots.Plugin");

            this.reservedSlots = this.CreateConVar("ReservedSlots", "0", "The number of reserved slots.", true, 0).Value;

            this.AutoExecConfig(true, "ReservedSlots");

            this.maxPlayers = GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount);
        }

        /// <inheritdoc/>
        public override bool OnPlayerLogin(SMClient client, StringBuilder rejectReason)
        {
            if ((this.maxPlayers - (ClientHelper.ClientCount - 1)) <= this.reservedSlots.AsInt)
            {
                if (!AdminManager.CheckAccess(client, AdminFlags.Reservation))
                {
                    rejectReason.Append(this.GetString("Slot reserved", client));
                    return false;
                }
            }

            return base.OnPlayerLogin(client, rejectReason);
        }
    }
}
