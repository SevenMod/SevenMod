// <copyright file="BloodMoon.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BloodMoon
{
    using SevenMod.Console;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that displays the number of days until the next blood moon.
    /// </summary>
    public sealed class BloodMoon : PluginAbstract
    {
        /// <summary>
        /// The value of the BloodMoonShowOnSpawn <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue showOnSpawn;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "BloodMoon Notice",
            Author = "SevenMod",
            Description = "Displays the number of days until the next blood moon.",
            Version = "0.2.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.LoadTranslations("BloodMoon.Plugin");

            this.showOnSpawn = this.CreateConVar("BloodMoonShowOnSpawn", "True", "Whether to show the number of days until the next blood moon to newly spawned players.").Value;

            this.AutoExecConfig(true, "BloodMoon");

            this.RegAdminCmd("bloodmoon", 0, "Bloodmoon Description").Executed += this.OnBloodmoonCommandExecuted;
        }

        /// <inheritdoc/>
        public override void OnPlayerSpawnedInWorld(SMClient client, SMRespawnType respawnReason, Pos pos)
        {
            if (this.showOnSpawn.AsBool)
            {
                var days = this.GetDays();
                this.PrintToChat(client, this.GetMessage(days), days);
            }
        }

        /// <summary>
        /// Called when the bloodmoon admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnBloodmoonCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            var days = this.GetDays();
            var message = this.GetMessage(days);
            if (!this.ShouldReplyToChat(e.Client))
            {
                this.ReplyToCommand(e.Client, message, days);
            }

            this.PrintToChatAll(message, days);
        }

        /// <summary>
        /// Gets a formatted string describing the number of days until the next blood moon.
        /// </summary>
        /// <param name="days">The number of days until the next blood moon.</param>
        /// <returns>The formatted string.</returns>
        private string GetMessage(int days)
        {
            if (days == 0)
            {
                return "Bloodmoon Tonight";
            }
            else if (days == 1)
            {
                return "Bloodmoon One";
            }
            else
            {
                return "Bloodmoon";
            }
        }

        /// <summary>
        /// Gets the number of days until the next blood moon.
        /// </summary>
        /// <returns>The number of days between today and the next blood moon.</returns>
        private int GetDays()
        {
            var bloodMoonDay = GameStats.GetInt(EnumGameStats.BloodMoonDay);
            var currentDay = GameUtils.WorldTimeToDays(GameManager.Instance.World.GetWorldTime());
            return bloodMoonDay - currentDay;
        }
    }
}
