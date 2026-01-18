/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html or the LICENSE file for full terms.
 */

namespace KSPCurveBuilder
{
    public static class Constants
    {
        public const float TIME_EPSILON = 0.0001f;
        public const int GRID_LINES = 10;
        public const float DRAG_SENSITIVITY = 0.1f;
        public const float DRAG_REFERENCE = 10000.0f;
        public const float DRAG_MIN_RATE = 0.00001f;
        public const float TANGENT_MULTIPLIER = 0.35f;
        public const float MAX_RATE_MULTIPLIER = 300.0f;
        public const float MAX_REASONABLE_VALUE = 1e9f;
        public const int MAX_SAMPLES = 1000;
        public const string FLOAT_FORMAT = "G";
        public const int HOVER_LABEL_OFFSET_X = 10;
        public const int HOVER_LABEL_OFFSET_Y = -20;
        public const int DRAG_EDGE_THRESHOLD = 5;  // Pixels from screen edge to trigger wrap
        public const int DRAG_WARP_DISTANCE = 50;   // Distance to warp cursor back

        public static class Visual
        {
            // GDI+ coordinate overflow prevention limit (DO NOT increase)
            public const float MAX_RENDER_COORDINATE = 1e6f;
            public const float MIN_RENDER_COORDINATE = -1e6f;
            public const float MIN_RANGE_BEFORE_DIVIDE_BY_ZERO = 0.001f;
        }
    }
    public static class Formatting
    {
        public const string TIME_DECIMAL_PLACES = "F1";
        public const string VALUE_DECIMAL_PLACES = "F1";
        public const string TANGENT_SIGNIFICANT_FIGURES = "G4";
    }


    public enum DataGridColumn
    {
        Time = 0,
        Value = 1,
        InTangent = 2,
        OutTangent = 3,
        RemoveButton = 4
    }
}
