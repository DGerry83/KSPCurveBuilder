/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian     ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/     ).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html     or the LICENSE file for full terms.
 */

#nullable enable

using System;

namespace KSPCurveBuilder;

/// <summary>
/// Command to remove a point from the curve.
/// </summary>
public sealed class RemovePointCommand : ICommand
{
    private readonly CurveEditorService _service;
    private readonly int _originalIndex;
    private FloatString4? _removedPoint;

    public string Name => "Remove Point";

    public RemovePointCommand(CurveEditorService service, int index)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _originalIndex = index;
    }

    public void Execute()
    {
        if (_originalIndex >= 0 && _originalIndex < _service.Points.Count)
        {
            _removedPoint = _service.Points[_originalIndex];
            _service.RemovePoint(_originalIndex);
        }
    }

    public void Unexecute()
    {
        if (_removedPoint != null)
        {
            // Insert at the original index if it's still valid
            if (_originalIndex >= 0 && _originalIndex <= _service.Points.Count)
            {
                _service.InsertPoint(_originalIndex, _removedPoint);
            }
            else
            {
                // Fallback: add to end
                _service.AddPoint(_removedPoint);
            }
        }
    }
}