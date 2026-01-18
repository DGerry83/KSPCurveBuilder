/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html or the LICENSE file for full terms.
 */

using System.Collections.Generic;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Collection of commonly-used curve presets.
    /// </summary>
    public static class BuiltInPresets
    {
        /// <summary>
        /// Gets all built-in presets.
        /// </summary>
        public static List<Preset> GetAll()
        {
            return new List<Preset>
            {
                Default(),
                Linear(),
                EaseIn(),
                EaseOut(),
                SmoothStart(),
                // Add more here
            };
        }

        /// <summary>
        /// The default curve that loads on startup.
        /// Based on the AmazingCurveEditor mod
        /// </summary>
        public static Preset Default()
        {
            var points = new List<FloatString4>
            {
                new FloatString4(0, 0, 0, 0.02f),
                new FloatString4(100, 1, 0.02f, 0)
            };
            return Preset.FromPoints("Default", "Default curve on startup", points);
        }
        /// <summary>
        /// Linear curve: constant rate of change.
        /// </summary>
        private static Preset Linear()
        {
            var points = new List<FloatString4>
            {
                new FloatString4(0, 0, 0, 0.01f),
                new FloatString4(100, 1, 0.01f, 0)
            };
            return Preset.FromPoints("Linear", "Constant rate of change", points);
        }

        /// <summary>
        /// Ease-in curve: starts slow, accelerates.
        /// </summary>
        private static Preset EaseIn()
        {
            var points = new List<FloatString4>
            {
                new FloatString4(0, 0, 0, 0),
                new FloatString4(100, 1, 0.005f, 0)
            };
            return Preset.FromPoints("Ease In", "Starts slow, speeds up", points);
        }

        /// <summary>
        /// Ease-out curve: starts fast, decelerates.
        /// </summary>
        private static Preset EaseOut()
        {
            var points = new List<FloatString4>
            {
                new FloatString4(0, 0, 0, 0.005f), 
                new FloatString4(100, 1, 0, 0)
            };
            return Preset.FromPoints("Ease Out", "Starts fast, slows down", points);
        }

        /// <summary>
        /// Smooth start curve: very gradual acceleration.
        /// </summary>
        private static Preset SmoothStart()
        {
            var points = new List<FloatString4>
            {
                new FloatString4(0, 0, 0, 0),
                new FloatString4(50, 0.1f, 0.002f, 0.002f), 
                new FloatString4(100, 1, 0.02f, 0) 
            };
            return Preset.FromPoints("Smooth Start", "Very gradual acceleration", points);
        }
    }
}