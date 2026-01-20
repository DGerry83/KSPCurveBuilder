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

using System;

namespace KSPCurveBuilder;

public sealed class PointRemovedEventArgs : EventArgs
{
    public int Index { get; }
    public PointRemovedEventArgs(int index) => Index = index;
}

public sealed class GridCellEditedEventArgs : EventArgs
{
    public int RowIndex { get; }
    public FloatString4 OldPoint { get; }
    public FloatString4 NewPoint { get; }

    public GridCellEditedEventArgs(int rowIndex, FloatString4 oldPoint, FloatString4 newPoint)
    {
        RowIndex = rowIndex;
        OldPoint = oldPoint;
        NewPoint = newPoint;
    }
}

public sealed class PointDraggedEventArgs : EventArgs
{
    public int Index { get; }
    public FloatString4 OldPoint { get; }
    public FloatString4 NewPoint { get; }

    public PointDraggedEventArgs(int index, FloatString4 oldPoint, FloatString4 newPoint)
    {
        Index = index;
        OldPoint = oldPoint;
        NewPoint = newPoint;
    }
}