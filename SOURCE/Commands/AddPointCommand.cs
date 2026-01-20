/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian    ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/    ).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html     or the LICENSE file for full terms.
 */

#nullable enable

using System;
using System.Linq;

namespace KSPCurveBuilder;

/// <summary>
/// Command to add a new point to the curve.
/// </summary>
public sealed class AddPointCommand(CurveEditorService service, FloatString4 point) : ICommand
{
    private readonly CurveEditorService _service = service ?? throw new ArgumentNullException(nameof(service));
    private readonly FloatString4 _point = point ?? throw new ArgumentNullException(nameof(point));
    private int _addedIndex = -1; // Store the final index after sorting
    private bool _wasSorted = false;

    public string Name => "Add Point";

    public void Execute()
    {
        // Store current points before adding
        var pointsBefore = _service.Points.ToList();

        // Add the point
        _service.AddPoint(_point);

        // Store the index after adding (reflects any sorting that occurred)
        _addedIndex = _service.PointsInternal.IndexOf(_point);

        // Check if sorting occurred
        var pointsAfter = _service.Points.ToList();
        _wasSorted = !pointsBefore.Select(p => p.Time).SequenceEqual(pointsAfter.Select(p => p.Time));
    }

    public void Unexecute()
    {
        if (_addedIndex >= 0 && _addedIndex < _service.Points.Count)
        {
            // Verify the point at this index matches what we expect
            var currentPoint = _service.PointsInternal[_addedIndex];
            if (Math.Abs(currentPoint.Time - _point.Time) < 0.001f &&
                Math.Abs(currentPoint.Value - _point.Value) < 0.001f)
            {
                _service.RemovePoint(_addedIndex);
            }
            else
            {
                // Fallback: search for the point by value
                var index = _service.PointsInternal.FindIndex(p =>
                    Math.Abs(p.Time - _point.Time) < 0.001f &&
                    Math.Abs(p.Value - _point.Value) < 0.001f);
                if (index >= 0)
                {
                    _service.RemovePoint(index);
                }
            }
        }
    }
}