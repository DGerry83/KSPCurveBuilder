/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian  ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/  ).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html   or the LICENSE file for full terms.
 */

using System.Windows.Forms;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Calculates drag speed multipliers based on modifier keys.
    /// </summary>
    public static class DragSpeedCalculator
    {
        /// <summary>Gets the speed multiplier based on Shift/Ctrl keys.</summary>
        public static float GetSpeedMultiplier()
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                return 5.0f;
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                return 0.1f;
            return 1.0f;
        }
    }
}