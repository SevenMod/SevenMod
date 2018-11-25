// <copyright file="ConVarChangedEventArgs.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.ConVar
{
    using System;

    /// <summary>
    /// Contains the arguments for a <see cref="ConVar.ConVarChanged"/> event.
    /// </summary>
    public class ConVarChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConVarChangedEventArgs"/> class.
        /// </summary>
        /// <param name="conVar">The <see cref="SevenMod.ConVar.ConVar"/> object that raised the event.</param>
        /// <param name="oldValue">The old value of the variable as a string.</param>
        /// <param name="newValue">The new value of the variable as a string.</param>
        internal ConVarChangedEventArgs(ConVar conVar, string oldValue, string newValue)
        {
            this.ConVar = conVar;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        /// <summary>
        /// Gets the <see cref="SevenMod.ConVar.ConVar"/> object representing the console variable whose value changed.
        /// </summary>
        public ConVar ConVar { get; }

        /// <summary>
        /// Gets the old value of the variable as a string.
        /// </summary>
        public string OldValue { get; }

        /// <summary>
        /// Gets the new value of the variable as a string.
        /// </summary>
        public string NewValue { get; }
    }
}
