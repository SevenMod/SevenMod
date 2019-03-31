// <copyright file="ServerShutdown.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.ServerShutdown
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Timers;
    using SevenMod.Admin;
    using SevenMod.Console;
    using SevenMod.ConVar;
    using SevenMod.Core;
    using SevenMod.Voting;

    /// <summary>
    /// Plugin that schedules automatic server shutdowns and enables shutdown votes.
    /// </summary>
    public sealed class ServerShutdown : PluginAbstract, IDisposable
    {
        /// <summary>
        /// The list of automatic shutdown times as the number of minutes since midnight.
        /// </summary>
        private readonly List<int> shutdownSchedule = new List<int>();

        /// <summary>
        /// The value of the ServerShutdownAutoRestart <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue autoRestart;

        /// <summary>
        /// The value of the ServerShutdownSchedule <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue schedule;

        /// <summary>
        /// The value of the ServerShutdownCountdownTime <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue countdownTime;

        /// <summary>
        /// The value of the ServerShutdownEnableRestartCommand <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue enableRestartCommand;

        /// <summary>
        /// The value of the ServerShutdownEnableVote <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue enableVote;

        /// <summary>
        /// The value of the ServerShutdownVotePercent <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue votePercent;

        /// <summary>
        /// The <see cref="AdminCommand"/> for cancelling a shutdown/restart.
        /// </summary>
        private AdminCommand cancelCommand;

        /// <summary>
        /// The timer for the next shutdown event.
        /// </summary>
        private Timer shutdownTimer;

        /// <summary>
        /// The current minute of the 5 minute countdown.
        /// </summary>
        private int countdown;

        /// <summary>
        /// A value indicating whether a shutdown is in progress.
        /// </summary>
        private bool shutdownInProgress;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Server Shutdown Scheduler",
            Author = "SevenMod",
            Description = "Schedules automatic server shutdowns and enables shutdown votes.",
            Version = "0.1.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.LoadTranslations("ServerShutdown.Plugin");

            this.autoRestart = this.CreateConVar("ServerShutdownAutoRestart", "True", "Enable if the server is set up to automatically restart after crashing.").Value;
            this.schedule = this.CreateConVar("ServerShutdownSchedule", string.Empty, "The automatic shutdown schedule in the format HH:MM. Separate multiple times with commas.").Value;
            this.countdownTime = this.CreateConVar("ServerShutdownCountdownTime", "5", "The countdown time in minutes for scheduled shutdowns.", true, 1, true, 20).Value;
            this.enableRestartCommand = this.CreateConVar("ServerShutdownEnableRestartCommand", "True", "Enable the restart admin command.").Value;
            this.enableVote = this.CreateConVar("ServerShutdownEnableVote", "True", "Enable the voteshutdown admin command.").Value;
            this.votePercent = this.CreateConVar("ServerShutdownVotePercent", "0.60", "The percentage of players that must vote yes for a successful shutdown vote.", true, 0, true, 1).Value;

            this.AutoExecConfig(true, "ServerShutdown");

            this.RegisterCommands();

            this.autoRestart.ConVar.ValueChanged += this.OnAutoRestartChanged;
            this.schedule.ConVar.ValueChanged += this.OnScheduleChanged;
            this.countdownTime.ConVar.ValueChanged += this.OnCountdownTimeChanged;
        }

        /// <inheritdoc/>
        public override void OnUnloadPlugin()
        {
            if (this.shutdownTimer != null)
            {
                this.shutdownTimer.Stop();
                this.shutdownTimer.Dispose();
                this.shutdownTimer = null;
            }
        }

        /// <inheritdoc/>
        public override bool OnPlayerLogin(SMClient client, StringBuilder rejectReason)
        {
            if (this.shutdownInProgress && ((this.countdown < 1 && !this.cancelCommand.HasAccess(client)) || this.countdown < 0))
            {
                rejectReason.Append(this.GetString($"{(this.autoRestart.AsBool ? "Restart" : "Shutdown")} Kick Reason"));
                return false;
            }

            return base.OnPlayerLogin(client, rejectReason);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ((IDisposable)this.shutdownTimer).Dispose();
        }

        /// <summary>
        /// Registers the admin commands.
        /// </summary>
        private void RegisterCommands()
        {
            if (this.autoRestart.AsBool)
            {
                this.RegAdminCmd("restart", AdminFlags.RCON, "Restart Description").Executed += this.OnRestartCommandExecuted;
                this.RegAdminCmd("voterestart", AdminFlags.Vote, "Voterestart Description").Executed += this.OnVoteshutdownCommandExecuted;
                this.cancelCommand = this.RegAdminCmd("cancelrestart", AdminFlags.Changemap, "Cancelrestart Description");
            }
            else
            {
                this.RegAdminCmd("voteshutdown", AdminFlags.Vote, "Voteshutdown Description").Executed += this.OnVoteshutdownCommandExecuted;
                this.cancelCommand = this.RegAdminCmd("cancelshutdown", AdminFlags.Changemap, "Cancelshutdown Description");
            }

            this.cancelCommand.Executed += this.OnCancelshutdownCommandExecuted;
        }

        /// <summary>
        /// Called when the value of the ServerShutdownAutoRestart <see cref="ConVar"/> is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ConVarChangedEventArgs"/> object containing the event data.</param>
        private void OnAutoRestartChanged(object sender, ConVarChangedEventArgs e)
        {
            this.UnregAdminCommand("restart");
            this.UnregAdminCommand("voteshutdown");
            this.UnregAdminCommand("voterestart");
            this.UnregAdminCommand("cancelshutdown");
            this.UnregAdminCommand("cancelrestart");

            this.RegisterCommands();
        }

        /// <summary>
        /// Called when the value of the ServerShutdownSchedule <see cref="ConVar"/> is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ConVarChangedEventArgs"/> object containing the event data.</param>
        private void OnScheduleChanged(object sender, ConVarChangedEventArgs e)
        {
            if (this.shutdownTimer != null)
            {
                this.shutdownTimer.Dispose();
                this.shutdownTimer = null;
            }

            this.shutdownSchedule.Clear();
            foreach (var time in this.schedule.AsString.Split(','))
            {
                if (string.IsNullOrEmpty(time))
                {
                    continue;
                }

                var index = time.IndexOf(':');
                if (index > 0 && int.TryParse(time.Substring(0, index), out var hour) && int.TryParse(time.Substring(index + 1), out var minute))
                {
                    this.shutdownSchedule.Add((hour * 60) + minute);
                    continue;
                }

                this.LogError($"Invalid schedule time: {time}");
            }

            if (this.shutdownSchedule.Count > 0)
            {
                if (this.shutdownSchedule.Count > 1)
                {
                    this.shutdownSchedule.Sort();
                    for (var i = 0; i < this.shutdownSchedule.Count; i++)
                    {
                        int diff;
                        if (i == 0)
                        {
                            diff = (1440 - this.shutdownSchedule[this.shutdownSchedule.Count - 1]) + this.shutdownSchedule[0];
                        }
                        else
                        {
                            diff = this.shutdownSchedule[i] - this.shutdownSchedule[i - 1];
                        }

                        if (diff <= 5)
                        {
                            this.LogError($"Removing scheduled time {this.shutdownSchedule[i] / 60:D2}:{this.shutdownSchedule[i] % 60:D2} because it is not more than 5 minutes after the previous time");
                            this.shutdownSchedule.RemoveAt(i);
                            i--;
                        }
                    }
                }

                this.ScheduleNext();
            }
        }

        /// <summary>
        /// Called when the value of the ServerShutdownCountdownTime <see cref="ConVar"/> is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ConVarChangedEventArgs"/> object containing the event data.</param>
        private void OnCountdownTimeChanged(object sender, ConVarChangedEventArgs e)
        {
            this.ScheduleNext();
        }

        /// <summary>
        /// Called when the restart admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnRestartCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (!this.enableRestartCommand.AsBool)
            {
                return;
            }

            if (this.shutdownInProgress)
            {
                this.ReplyToCommand(e.Client, "Restart In Progress");
                return;
            }

            this.countdown = this.countdownTime.AsInt;
            if (e.Arguments.Count > 0 && int.TryParse(e.Arguments[0], out var countdown))
            {
                this.countdown = Math.Max(1, Math.Min(20, countdown));
            }

            this.ReplyToCommand(e.Client, "Restart Countdown Started");
            this.ShowActivity(e.Client, "Initiated Restart");
            this.LogAction(e.Client, null, "\"{0:L}\" initiated a server shutdown (countdown \"{1:d}\")", e.Client, this.countdown);
            this.CountDown();
        }

        /// <summary>
        /// Called when the voteshutdown admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnVoteshutdownCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (!this.enableVote.AsBool)
            {
                return;
            }

            if (this.shutdownInProgress)
            {
                this.ReplyToCommand(e.Client, $"{(this.autoRestart.AsBool ? "Restart" : "Shutdown")} In Progress");
                return;
            }

            if (VoteManager.CreateVote($"{(this.autoRestart.AsBool ? "Restart" : "Shutdown")} Vote").Start())
            {
                this.ShowActivity(e.Client, $"Initiated Vote {(this.autoRestart.AsBool ? "Restart" : "Shutdown")}");
                this.LogAction(e.Client, null, "\"{0:L}\" initiated a server shutdown vote", e.Client);
                VoteManager.CurrentVote.Ended += this.OnShutdownVoteEnded;
            }
        }

        /// <summary>
        /// Called when the cancelshutdown admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnCancelshutdownCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (this.shutdownInProgress)
            {
                this.ScheduleNext();
                this.ShowActivity(e.Client, $"Cancelled {(this.autoRestart.AsBool ? "Restart" : "Shutdown")}");

                if (!this.ShouldReplyToChat(e.Client))
                {
                    this.ReplyToCommand(e.Client, $"Cancelled {(this.autoRestart.AsBool ? "Restart" : "Shutdown")}");
                }

                this.LogAction(e.Client, null, "\"{0:L}\" cancelled the server shutdown", e.Client);
            }
            else
            {
                this.ReplyToCommand(e.Client, $"No {(this.autoRestart.AsBool ? "restart" : "shutdown")} in progress");
            }
        }

        /// <summary>
        /// Called when a shutdown vote ends.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="VoteEndedEventArgs"/> object containing the event data.</param>
        private void OnShutdownVoteEnded(object sender, VoteEndedEventArgs e)
        {
            if (this.shutdownInProgress)
            {
                this.PrintToChatAll($"{(this.autoRestart.AsBool ? "Restart" : "Shutdown")} In Progress");
                return;
            }

            if (e.Percents[0] >= this.votePercent.AsFloat)
            {
                this.LogAction(null, null, "Server shutdown vote succeeded");
                this.PrintToChatAll("Vote Succeeded", e.Percents[0]);
                this.PrintToChatAll(this.autoRestart.AsBool ? "Restarting" : "Shutting Down");
                if (this.shutdownTimer != null)
                {
                    this.shutdownTimer.Dispose();
                }

                this.shutdownInProgress = true;
                this.countdown = 0;
                this.shutdownTimer = new Timer(30000);
                this.shutdownTimer.Elapsed += this.OnShutdownTimerElapsed;
                this.shutdownTimer.Enabled = true;
            }
            else
            {
                this.LogAction(null, null, "Server shutdown vote failed");
                this.PrintToChatAll("Vote Failed", e.Percents[0]);
            }
        }

        /// <summary>
        /// Schedules the next automatic shutdown.
        /// </summary>
        private void ScheduleNext()
        {
            if (this.shutdownTimer != null)
            {
                this.shutdownTimer.Dispose();
                this.shutdownTimer = null;
            }

            this.shutdownInProgress = false;

            if (this.shutdownSchedule.Count == 0)
            {
                return;
            }

            this.countdown = this.countdownTime.AsInt;

            var time = this.shutdownSchedule.Find((int t) => (t - this.countdown) >= DateTime.Now.TimeOfDay.TotalMinutes);
            time = time == 0 ? this.shutdownSchedule[0] : time;

            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time / 60, time % 60, 0).AddMinutes(-this.countdown);
            if (dt < DateTime.Now)
            {
                dt = dt.AddDays(1);
            }

            this.shutdownTimer = new Timer(dt.Subtract(DateTime.Now).TotalMilliseconds);
            this.shutdownTimer.Elapsed += this.OnShutdownTimerElapsed;
            this.shutdownTimer.Enabled = true;
        }

        /// <summary>
        /// Called by the <see cref="shutdownTimer"/> to handle the next shutdown event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ElapsedEventArgs"/> object containing the event data.</param>
        private void OnShutdownTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.CountDown();
        }

        /// <summary>
        /// Updates the countdown.
        /// </summary>
        private void CountDown()
        {
            this.shutdownTimer?.Dispose();
            if (this.countdown > 0)
            {
                this.shutdownInProgress = true;
                this.shutdownTimer = new Timer(60000);
                this.shutdownTimer.Elapsed += this.OnShutdownTimerElapsed;
                this.shutdownTimer.Enabled = true;

                if (this.countdown > 1)
                {
                    this.PrintToChatAll(this.autoRestart.AsBool ? "Restart Warning" : "Shutdown Warning", this.countdown);
                }
                else
                {
                    this.PrintToChatAll(this.autoRestart.AsBool ? "Restart Warning Final" : "Shutdown Warning Final");
                }

                this.countdown--;
            }
            else
            {
                this.LogAction(null, null, "Shutting down the server");
                this.ServerCommand("shutdown");
                this.ScheduleNext();
            }
        }
    }
}
