// <copyright file="ConVar.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.ConVar
{
    using System;
    using SevenMod.Core;

    /// <summary>
    /// Represents a console variable.
    /// </summary>
    public class ConVar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConVar"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that created the console variable.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="defaultValue">The default value of the variable as a string.</param>
        /// <param name="description">The description for the variable.</param>
        /// <param name="hasMin">A value indicating whether the variable has a minimum value.</param>
        /// <param name="min">The minimum value of the variable if <paramref name="hasMin"/> is <c>true</c>.</param>
        /// <param name="hasMax">A value indicating whether the variable has a maximum value.</param>
        /// <param name="max">The maximum value of the variable if <paramref name="hasMax"/> is <c>true</c>.</param>
        internal ConVar(IPlugin plugin, string name, string defaultValue, string description, bool hasMin, float min, bool hasMax, float max)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
            this.Description = description;
            this.HasMin = hasMin;
            this.MinValue = min;
            this.HasMax = hasMax;
            this.MaxValue = max;
            this.Value = new ConVarValue(this, defaultValue);
            this.Plugin = plugin;
        }

        /// <summary>
        /// Occurs when the value of the variable is changed.
        /// </summary>
        public event EventHandler<ConVarChangedEventArgs> ValueChanged;

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the variable.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether the variable has a minimum value.
        /// </summary>
        public bool HasMin { get; }

        /// <summary>
        /// Gets the minimum value of the variable if <see cref="HasMin"/> is <c>true</c>.
        /// </summary>
        public float MinValue { get; }

        /// <summary>
        /// Gets a value indicating whether the variable has a maximum value.
        /// </summary>
        public bool HasMax { get; }

        /// <summary>
        /// Gets the maximum value of the variable if <see cref="HasMax"/> is <c>true</c>.
        /// </summary>
        public float MaxValue { get; }

        /// <summary>
        /// Gets the <see cref="ConVarValue"/> object representing the value of the variable.
        /// </summary>
        public ConVarValue Value { get; }

        /// <summary>
        /// Gets the default value of the variable.
        /// </summary>
        public string DefaultValue { get; }

        /// <summary>
        /// Gets the plugin that created the console variable.
        /// </summary>
        internal IPlugin Plugin { get; }

        /// <summary>
        /// Resets the value to the default.
        /// </summary>
        public void Reset()
        {
            this.Value.Value = this.DefaultValue;
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="oldVal">The old value as a string.</param>
        /// <param name="newVal">The new value as a string.</param>
        internal void OnConVarChanged(string oldVal, string newVal)
        {
            if (this.ValueChanged != null)
            {
                var args = new ConVarChangedEventArgs(this, oldVal, newVal);
                foreach (EventHandler<ConVarChangedEventArgs> d in this.ValueChanged.GetInvocationList())
                {
                    try
                    {
                        d.Invoke(this, args);
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        if (d.Target is IPlugin p)
                        {
                            p.Container.SetFailState(e.Message);
                        }
                        else
                        {
                            SMLog.Error(e.Message);
                        }
                    }
                }
            }
        }
    }
}
