// <copyright file="Advertisements.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.Advertisements
{
    using System;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: Advertisements</para>
    /// <para>Periodically shows messages in chat.</para>
    /// </summary>
    public sealed class Advertisements : PluginAbstract, IDisposable
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Advertisements",
            Author = "SevenMod",
            Description = "Periodically shows messages in chat.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <summary>
        /// The timer for periodically sending messages.
        /// </summary>
        private System.Timers.Timer timer;

        /// <summary>
        /// The current index in the message cycle.
        /// </summary>
        private int index = 0;

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            ConfigManager.ParseConfig(AdvertisementsConfig.Instance, "Advertisements");

            this.timer = new System.Timers.Timer(AdvertisementsConfig.Instance.Interval * 60000);
            this.timer.Elapsed += this.TimerElapsed;
            this.timer.Start();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ((IDisposable)this.timer).Dispose();
        }

        /// <summary>
        /// Called by the <see cref="timer"/> to display the next advertisement message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.Timers.ElapsedEventArgs"/> object that contains
        /// the event data.</param>
        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string message;
            if (AdvertisementsConfig.Instance.RandomOrder)
            {
                message = AdvertisementsConfig.Instance.Messages.RandomObject();
            }
            else
            {
                message = AdvertisementsConfig.Instance.Messages[this.index];
                this.index = (this.index + 1) % AdvertisementsConfig.Instance.Messages.Count;
            }

            foreach (var client in ConnectionManager.Instance.GetClients())
            {
                client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, message, "[AD]", false, "SevenMod", false));
            }
        }
    }
}
