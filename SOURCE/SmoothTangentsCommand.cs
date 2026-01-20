/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/ ).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html  or the LICENSE file for full terms.
 */

#nullable enable

using System;
using System.Collections.Generic;

namespace KSPCurveBuilder;

/// <summary>
/// Command to smooth tangents on the entire curve.
/// </summary>
public sealed class SmoothTangentsCommand(CurveEditorService service, List<FloatString4> pointsBefore) : ICommand
{
    private readonly CurveEditorService _service = service ?? throw new ArgumentNullException(nameof(service));
    private readonly List<FloatString4> _pointsBefore = pointsBefore ?? throw new ArgumentNullException(nameof(pointsBefore));

    public string Name => "Smooth Tangents";

    public void Execute() => _service.SmoothTangents();

    public void Unexecute() => _service.LoadFromPoints(_pointsBefore);
}