/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;

namespace vrroom.Dynaimic.Common
{
    /// <summary>
    /// Base class for singletons
    /// </summary>
    /// <remarks>
    /// Code from https://csharpindepth.com/articles/singleton
    /// </remarks>
    public class Singleton<T> where T: Singleton<T>, new()
    {
        /// <summary>
        /// Singleton instance with lazy initialization
        /// </summary>
        private static readonly Lazy<T> lazy = new Lazy<T>(() => new T());

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static T Instance { get { return lazy.Value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Singleton()
        {
        }
    }

}