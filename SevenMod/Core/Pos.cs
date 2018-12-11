// <copyright file="Pos.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Represents a position in 3D space.
    /// </summary>
    public struct Pos
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public int Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pos"/> struct.
        /// </summary>
        /// <param name="vector3I">The vector containing the coordinates.</param>
        internal Pos(Vector3i vector3I)
        {
            this.X = vector3I.x;
            this.Y = vector3I.y;
            this.Z = vector3I.z;
        }
    }
}
