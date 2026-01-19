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

namespace KSPCurveBuilder;

/// <summary>
/// Encapsulates undo/redo functionality.
/// </summary>
public class UndoService
{
    private readonly SimpleUndoManager _undoManager;
    private string _lastRecordedState;

    public bool CanUndo => _undoManager.CanUndo;
    public bool CanRedo => _undoManager.CanRedo;
    public string UndoActionName => _undoManager.UndoActionName;
    public string RedoActionName => _undoManager.RedoActionName;

    public event EventHandler? StateChanged;

    public UndoService(string initialState)
    {
        _undoManager = new SimpleUndoManager(initialState);
        _lastRecordedState = initialState;
        _undoManager.StateChanged += (s, e) => StateChanged?.Invoke(s, EventArgs.Empty);
    }

    public void RecordAction(string newState, string actionName)
    {
        if (newState != _lastRecordedState)
        {
            _undoManager.RecordAction(newState, actionName);
            _lastRecordedState = newState;
        }
    }

    public string? Undo()
    {
        var state = _undoManager.Undo();
        if (state != null)
            _lastRecordedState = state;
        return state;
    }

    public string? Redo()
    {
        var state = _undoManager.Redo();
        if (state != null)
            _lastRecordedState = state;
        return state;
    }
}