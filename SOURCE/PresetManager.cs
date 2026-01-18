/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/).
 * 
 * This file is part of Curve Editor, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html or the LICENSE file for full terms.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Manages loading and saving curve presets from disk.
    /// </summary>
    public static class PresetManager
    {
        private static readonly string PresetFolder = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "KSPCurveBuilder",
            "Presets"
        );

        /// <summary>
        /// Ensures the preset folder exists.
        /// </summary>
        private static void EnsureFolderExists()
        {
            if (!Directory.Exists(PresetFolder))
                Directory.CreateDirectory(PresetFolder);
        }

        /// <summary>
        /// Saves a preset to file.
        /// </summary>
        public static void SavePreset(Preset preset)
        {
            EnsureFolderExists();
            var filename = Path.Combine(PresetFolder, $"{preset.Name}.curvepreset");
            var lines = preset.Points.Select(p => p.ToKeyString("key")).ToArray();
            File.WriteAllLines(filename, lines);
        }

        /// <summary>
        /// Loads a preset from file.
        /// </summary>
        public static Preset LoadPreset(string presetName)
        {
            var filename = Path.Combine(PresetFolder, $"{presetName}.curvepreset");
            if (!File.Exists(filename)) return null;

            var lines = File.ReadAllLines(filename);
            var preset = ParseFromLines(Path.GetFileNameWithoutExtension(filename), lines);
            return preset;
        }

        /// <summary>
        /// Gets all available preset names.
        /// </summary>
        public static string[] GetAvailablePresets()
        {
            EnsureFolderExists();
            var files = Directory.GetFiles(PresetFolder, "*.curvepreset");
            return files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
        }

        /// <summary>
        /// Deletes a preset.
        /// </summary>
        public static void DeletePreset(string presetName)
        {
            var filename = Path.Combine(PresetFolder, $"{presetName}.curvepreset");
            if (File.Exists(filename))
                File.Delete(filename);
        }

        /// <summary>
        /// Parses a preset from key string lines.
        /// </summary>
        private static Preset ParseFromLines(string name, string[] lines)
        {
            var preset = new Preset { Name = name };

            foreach (var line in lines)
            {
                if (line.StartsWith("key"))
                {
                    var result = CurveParser.TryParseKeyString(line);
                    if (result.Success)
                        preset.Points.Add(result.Point);
                }
            }

            return preset;
        }
    }
}