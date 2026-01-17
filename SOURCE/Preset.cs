using System.Collections.Generic;

namespace KSPCurveBuilder
{
    /// <summary>
    /// A saved curve preset with name, description, and keyframe data.
    /// </summary>
    public class Preset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FloatString4> Points { get; set; }

        public Preset()
        {
            Points = new List<FloatString4>();
        }

        /// <summary>
        /// Creates a preset from the current points list.
        /// </summary>
        public static Preset FromPoints(string name, string description, List<FloatString4> points)
        {
            var preset = new Preset
            {
                Name = name,
                Description = description
            };

            foreach (var point in points)
            {
                preset.Points.Add(new FloatString4(point.Time, point.Value, point.InTangent, point.OutTangent));
            }

            return preset;
        }
    }
}