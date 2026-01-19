/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/ ).
 * 
 * This file is part of Curve Editor, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html  or the LICENSE file for full terms.
 */

using System;

namespace KSPCurveBuilder;

public static class CurveValidator
{
    public static float ValidateFloat(float value, string paramName)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be NaN or Infinity");

        if (Math.Abs(value) > Constants.MAX_REASONABLE_VALUE)
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} too large: {value}");

        return value;
    }
}