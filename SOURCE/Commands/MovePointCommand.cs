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

namespace KSPCurveBuilder;

/// <summary>
/// Command to move/drag a point to a new position.
/// </summary>
public sealed class MovePointCommand(CurveEditorService service, int index, FloatString4 newPoint, FloatString4 oldPoint) : ICommand
{
    private readonly CurveEditorService _service = service ?? throw new ArgumentNullException(nameof(service));
    private readonly int _index = index;
    private readonly FloatString4 _newPoint = newPoint ?? throw new ArgumentNullException(nameof(newPoint));
    private readonly FloatString4 _oldPoint = oldPoint ?? throw new ArgumentNullException(nameof(oldPoint));

    public string Name => "Move Point";

    public void Execute() => _service.UpdatePoint(_index, _newPoint);

    public void Unexecute() => _service.UpdatePoint(_index, _oldPoint);
}