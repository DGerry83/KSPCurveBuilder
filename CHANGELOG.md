# V 1.0.0
## Initial Release

**Main Interface**
- Fixed-size dialog window with non-resizable layout
- Black PictureBox displaying real-time curve visualization
- DataGridView with 5 columns: Time, Value, InTangent, OutTangent, Remove
- Multiline TextBox showing raw keyframe data in "key = time value inTangent outTangent" format

**DataGrid Editing**
- Direct cell editing for all numeric values
- "X" button column to remove points with confirmation
- Auto-sort checkbox to maintain time-order
- Real-time drag-to-edit: click and drag vertically on cells to adjust values
- Input validation with red error text for invalid numbers

**Preset System**
- Dropdown combo with built-in presets: Default, Linear, EaseIn, EaseOut, SmoothStart
- Save button to store current curve as named preset
- Delete button to remove user presets (built-ins protected)
- Presets stored in %AppData%\KSPCurveBuilder\Presets\

**Text-Based Workflow**
- Copy button to export curve text to clipboard
- Paste button to import clipboard text with validation
- Manual text editing capability

**Mouse Controls on Graph**
- Mouse wheel zoom (0.1x to 10x range)
- Left-click drag to pan view
- Cursor changes during interactions (SizeAll when panning)

**Button Functions**
- New Curve: clears all points immediately
- Smooth: applies tangent smoothing to all keyframes
- Add Node: appends new point at last time +10 with zero tangents
- Reset Zoom: returns to 1.0x zoom and centered view

# V 2.0.0
## Feature Upgrade and Refactoring

**New Interactive Features**
- **Direct point manipulation**: Click and drag any curve point directly on the graph visualization
- **Right-click to add points**: Right-click anywhere on the graph to insert a new point at that location
- **Hover information labels**: Hover over points to see tooltip with index, time, and value
- **Smart zoom centering**: Mouse wheel zoom keeps the data point under cursor stationary, making it easier to zoom in on a specific point
- **Auto-pan during drag**: Cursor warps at screen edges while dragging on the spreadsheet view, enabling "infinite" drag operations.

**Enhanced Existing Interactions**
- **DataGrid drag-to-edit**: New modifier keys
  - **Shift**: 5× faster dragging
  - **Ctrl**: 0.1× slower precision dragging
- **Textbox sync**: Changes apply on **Enter key** or focus loss (previously immediate)
- **Preset management**: Async loading with "Saving..."/"Deleting..." feedback, auto-selects first preset after deletion

**Behavioral Changes**
- **Sorting**: When "Sort" checkbox is enabled, points sort immediately after time column edits
- **Sort by default**: Sort checkbox is enabled by default
- **Error handling**: User-friendly message boxes instead of silent failures