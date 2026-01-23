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

namespace KSPCurveBuilder;

public static class Constants
{
    public const float TIME_EPSILON = 0.0001f;
    public const int GRID_LINES = 10;
    public const float DRAG_SENSITIVITY = 0.1f;
    public const float DRAG_REFERENCE = 10000.0f;
    public const float DRAG_MIN_RATE = 0.001f;
    public const float TANGENT_MULTIPLIER = 0.35f;
    public const float MAX_RATE_MULTIPLIER = 300.0f;
    public const float MAX_REASONABLE_VALUE = 1e9f;
    public const int MAX_SAMPLES = 1000;
    public const string FLOAT_FORMAT = "G";
    public const int HOVER_LABEL_OFFSET_X = 10;
    public const int HOVER_LABEL_OFFSET_Y = -20;
    public const int DRAG_EDGE_THRESHOLD = 5;
    public const int DRAG_WARP_DISTANCE = 50;
    public const int DRAG_START_THRESHOLD = 5; // pixels

    public static class Visual
    {
        public const float MAX_RENDER_COORDINATE = 1e6f;
        public const float MIN_RENDER_COORDINATE = -1e6f;
        public const float MIN_RANGE_BEFORE_DIVIDE_BY_ZERO = 0.001f;
        public const float HIT_TEST_RADIUS = 8f;
        public const int POINT_DRAW_SIZE = 8;
        public const int POINT_DRAW_RADIUS = 4;

        // Font constants
        public const string GRID_FONT_NAME = "Arial";
        public const int GRID_FONT_SIZE = 8;
        public const string TITLE_FONT_NAME = "Arial";
        public const int TITLE_FONT_SIZE = 10;
        public const System.Drawing.FontStyle TITLE_FONT_STYLE = System.Drawing.FontStyle.Bold;

        // Pen width constants
        public const float CURVE_PEN_WIDTH = 2f;
        public const float GRID_PEN_WIDTH = 1f;
        public const float POINT_PEN_WIDTH = 2f;

        // Label and title positioning
        public const int TITLE_OFFSET_X = 40;
        public const int TITLE_OFFSET_Y = 10;
        public const int TITLE_BOX_PADDING_X = 5;
        public const int TITLE_BOX_PADDING_Y = 5;
        public const int LABEL_PADDING = 5;
        public const int GRID_LABEL_OFFSET_Y = 15;
        public const int GRID_LABEL_VALUE_OFFSET_X = 5;
    }

    public static class UI
    {
        public const int COLUMN_WIDTH_TIME = 60;
        public const int COLUMN_WIDTH_VALUE = 80;
        public const int COLUMN_WIDTH_TANGENT = 60;
        public const int COLUMN_WIDTH_REMOVE = 70;
    }
}

public static class Formatting
{
    // Display formats: up to 4 significant figures, may use scientific notation for extreme values
    public const string TIME_DISPLAY = "G4";
    public const string VALUE_DISPLAY = "G4";
    public const string TANGENT_DISPLAY = "G4";

    // Serialization formats: full precision fixed-point, never scientific notation
    public const string TIME_SERIALIZATION = "0.#########";
    public const string VALUE_SERIALIZATION = "0.#########";
    public const string TANGENT_SERIALIZATION = "0.#########";

    // Legacy constants for backward compatibility (deprecated)
    //public const string TIME_DECIMAL_PLACES = "G4";
    //public const string VALUE_DECIMAL_PLACES = "G4";
    //public const string TANGENT_SIGNIFICANT_FIGURES = "G4";
}

public enum DataGridColumn
{
    Time = 0,
    Value = 1,
    InTangent = 2,
    OutTangent = 3,
    RemoveButton = 4
}