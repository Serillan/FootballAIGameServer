﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.CustomDataTypes
{
    /// <summary>
    /// The vector class used for representing two-dimensional vectors or points.
    /// </summary>
    public class Vector
    {
        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public double Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// </summary>
        public Vector() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets the vector length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public double Length =>
            Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// Gets the distances between the given vectors.
        /// </summary>
        /// <param name="firstVector">The first vector.</param>
        /// <param name="secondVector">The second vector.</param>
        /// <returns>The distance between the given vectors.</returns>
        public static double DistanceBetween(Vector firstVector, Vector secondVector)
        {
            return Math.Sqrt(Math.Pow(firstVector.X - secondVector.X, 2) + Math.Pow(firstVector.Y - secondVector.Y, 2));
        }

        /// <summary>
        /// Gets the dot product of the given vectors.
        /// </summary>
        /// <param name="firstVector">The first vector.</param>
        /// <param name="secondVector">The second vector.</param>
        /// <returns>The dot product between the given vectors.</returns>
        public static double DotProduct(Vector firstVector, Vector secondVector)
        {
            return firstVector.X*secondVector.X + firstVector.Y*secondVector.Y;
        }
    }
}