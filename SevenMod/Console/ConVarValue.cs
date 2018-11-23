// <copyright file="ConVarValue.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents the value of a console variable.
    /// </summary>
    public class ConVarValue
    {
        /// <summary>
        /// Strings that are considered to be falsy.
        /// </summary>
        private static readonly string[] NegativeValues = { "0", "0.0", "false", "no", string.Empty };

        /// <summary>
        /// Initializes a new instance of the <see cref="ConVarValue"/> class.
        /// </summary>
        /// <param name="conVar">The <see cref="SevenMod.Console.ConVar"/> containing this value.</param>
        /// <param name="value">The initial value as a string.</param>
        internal ConVarValue(ConVar conVar, string value)
        {
            this.ConVar = conVar;
            this.AsString = value;
        }

        /// <summary>
        /// Gets the <see cref="SevenMod.Console.ConVar"/> containing this value.
        /// </summary>
        public ConVar ConVar { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value
        {
            get => this.AsString;
            set
            {
                if (this.ConVar.HasMin)
                {
                    var floatValue = this.ConVar.MinValue;
                    float.TryParse(value.ToString(), out floatValue);
                    value = Math.Max(this.ConVar.MinValue, floatValue);
                }

                if (this.ConVar.HasMax)
                {
                    var floatValue = this.ConVar.MaxValue;
                    float.TryParse(value.ToString(), out floatValue);
                    value = Math.Min(this.ConVar.MaxValue, floatValue);
                }

                if (!this.AsString.Equals(value.ToString()))
                {
                    this.ConVar.NotifyConVarChanged(this.AsString, value.ToString());
                    this.AsString = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the value as a string.
        /// </summary>
        public string AsString { get; private set; }

        /// <summary>
        /// Gets the value as a float.
        /// </summary>
        public float AsFloat { get => float.Parse(this.AsString); }

        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        public int AsInt { get => int.Parse(this.AsString); }

        /// <summary>
        /// Gets a value indicating whether the value is truthy.
        /// </summary>
        public bool AsBool { get => !NegativeValues.Contains(this.AsString.ToLower()); }
    }
}
