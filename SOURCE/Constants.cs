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
