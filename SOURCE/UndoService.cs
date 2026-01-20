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
/// Manages undo/redo using the Command Pattern.
/// </summary>
public sealed class UndoService
{
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();
    private readonly CurveEditorService _editorService;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public string UndoActionName => _undoStack.TryPeek(out var cmd) ? $"Undo {cmd.Name}" : "Undo";
    public string RedoActionName => _redoStack.TryPeek(out var cmd) ? $"Redo {cmd.Name}" : "Redo";

    public event EventHandler? StateChanged;

    public UndoService(CurveEditorService editorService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
    }

    /// <summary>Executes a command and adds it to the undo stack.</summary>
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Undoes the last command.</summary>
    public void Undo()
    {
        if (_undoStack.Count == 0) return;

        var command = _undoStack.Pop();
        command.Unexecute();
        _redoStack.Push(command);
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Redoes the previously undone command.</summary>
    public void Redo()
    {
        if (_redoStack.Count == 0) return;

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Clears all undo/redo history.</summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}