using System;

namespace KSPCurveBuilder
{
    public static class CurveValidator
    {
        public static float ValidateFloat(float value, string paramName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be NaN or Infinity");

            if (Math.Abs(value) > Constants.MAX_REASONABLE_VALUE)
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} too large: {value}");

            return value;
        }
    }
}