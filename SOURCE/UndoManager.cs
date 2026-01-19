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

#nullable enable
using System;
using System.Collections.Generic;

namespace KSPCurveBuilder;

/// <summary>
/// Simple undo/redo manager that stores text snapshots of the curve state.
/// </summary>
public sealed class SimpleUndoManager(string initialState)
{
    private readonly Stack<UndoState> _undoStack = new();
    private readonly Stack<UndoState> _redoStack = new();
    private string _currentState = initialState;
    private record UndoState(string Text, string Action);

    public event EventHandler? StateChanged;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public string UndoActionName => _undoStack.TryPeek(out var state) ? state.Action : "Undo";
    public string RedoActionName => _redoStack.TryPeek(out var state) ? state.Action : "Redo";

    /// <summary>Records a new action, saving the current text state to undo stack.</summary>
    public void RecordAction(string newState, string actionName)
    {
        _undoStack.Push(new(_currentState, actionName));
        _currentState = newState;
        _redoStack.Clear();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Undoes the last action and returns the previous text state.</summary>
    public string Undo()
    {
        if (!CanUndo) return _currentState;
        _redoStack.Push(new(_currentState, "Redo"));
        var state = _undoStack.Pop();
        _currentState = state.Text;
        StateChanged?.Invoke(this, EventArgs.Empty);
        return _currentState;
    }

    /// <summary>Redoes the previously undone action and returns the next text state.</summary>
    public string Redo()
    {
        if (!CanRedo) return _currentState;
        _undoStack.Push(new(_currentState, "Undo"));
        var state = _redoStack.Pop();
        _currentState = state.Text;
        StateChanged?.Invoke(this, EventArgs.Empty);
        return _currentState;
    }
}