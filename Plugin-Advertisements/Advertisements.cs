// <copyright file="Advertisements.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.Advertisements
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Timers;
    using SevenMod.Chat;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that periodically shows messages in chat.
    /// </summary>
    public sealed class Advertisements : PluginAbstract, IDisposable
    {
        /// <summary>
        /// The path to the messages list.
        /// </summary>
        private static readonly string ListPath = $"{SMPath.Config}AdvertisementsList.txt";

        /// <summary>
        /// The value of the AdvertInterval <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue interval;

        /// <summary>
        /// The value of the AdvertRandomOrder <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue randomOrder;

        /// <summary>
        /// The list of messages.
        /// </summary>
        private List<string> messages = new List<string>();

        /// <summary>
        /// The watcher for changes to the message list file.
        /// </summary>
        private FileSystemWatcher watcher;

        /// <summary>
        /// The timer for periodically sending messages.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The current index in the message cycle.
        /// </summary>
        private int index = 0;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Advertisements",
            Author = "SevenMod",
            Description = "Periodically shows messages in chat.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            this.interval = this.CreateConVar("AdvertInterval", "120", "The time in seconds between advertisements.", true, 1).Value;
            this.randomOrder = this.CreateConVar("AdvertRandomOrder", "false", "Whether to show advertisements in random order.").Value;

            this.AutoExecConfig(true, "Advertisements");
        }

        /// <inheritdoc/>
        public override void ConfigsExecuted()
        {
            base.ConfigsExecuted();

            this.LoadMessages();

            this.timer = new Timer(this.interval.AsInt * 60000);
            this.timer.Elapsed += this.OnTimerElapsed;
            this.timer.Start();

            this.interval.ConVar.ConVarChanged += this.OnIntervalConVarChanged;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ((IDisposable)this.timer).Dispose();
            ((IDisposable)this.watcher).Dispose();
        }

        /// <summary>
        /// Called when the interval console variable changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ConVarChangedEventArgs"/> object containing the event data.</param>
        private void OnIntervalConVarChanged(object sender, ConVarChangedEventArgs e)
        {
            this.timer.Interval = this.interval.AsInt * 60000;
        }

        /// <summary>
        /// Loads the message list file.
        /// </summary>
        private void LoadMessages()
        {
            this.messages.Clear();
            this.index = 0;

            if (!File.Exists(ListPath))
            {
                this.CreateList();
            }

            using (var file = File.OpenText(ListPath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0 || line.StartsWith("//"))
                    {
                        continue;
                    }

                    this.messages.Add(line);
                }
            }

            if (this.watcher == null)
            {
                this.watcher = new FileSystemWatcher(SMPath.Config, Path.GetFileName(ListPath));
                this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName;
                this.watcher.Changed += this.OnListFileChanged;
                this.watcher.Deleted += this.OnListFileChanged;
                this.watcher.Renamed += this.OnListFileChanged;
                this.watcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Called by the <see cref="watcher"/> when the message list file changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="FileSystemEventArgs"/> object containing the event data.</param>
        private void OnListFileChanged(object sender, FileSystemEventArgs e)
        {
            this.LoadMessages();
        }

        /// <summary>
        /// Creates the message list file.
        /// </summary>
        private void CreateList()
        {
            var file = File.CreateText(ListPath);
            file.WriteLine("// List your advertisement messages in this file.");
            file.WriteLine();
            file.WriteLine("This server is running [b]SevenMod[/b]");
            file.Close();
        }

        /// <summary>
        /// Called by the <see cref="timer"/> to display the next advertisement message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ElapsedEventArgs"/> object containing the event data.</param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (this.messages.Count == 0)
            {
                return;
            }

            string message;
            if (this.randomOrder.AsBool)
            {
                message = this.messages.RandomObject();
            }
            else
            {
                message = this.messages[this.index];
                this.index = (this.index + 1) % this.messages.Count;
            }

            ChatHelper.SendToAll(message, "AD");
        }
    }
}
