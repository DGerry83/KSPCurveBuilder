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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Handles rendering the curve visualization to a Graphics surface.
    /// Implements IDisposable to properly clean up GDI+ resources.
    /// </summary>
    public class CurveRenderer : IDisposable  // <-- ADDED: IDisposable
    {
        private readonly Graphics _g;
        private readonly List<FloatString4> _points;
        private readonly FloatCurveStandalone _curve;
        private readonly float _minTime, _maxTime, _minValue, _maxValue;
        private readonly int _width, _height;
        private readonly float _zoomLevel;
        private readonly Font _gridFont;
        private readonly Font _titleFont;
        private readonly Pen _curvePen;
        private readonly Pen _gridPen;
        private readonly Pen _pointPen;
        private readonly Brush _pointBrush;
        private const int GRID_LINES = 10;
        private readonly int _highlightedPointIndex;
        private readonly int _hoveredPointIndex;

        private bool _disposed = false;  // <-- ADDED: Disposal flag

        public CurveRenderer(Graphics g, List<FloatString4> points, FloatCurveStandalone curve,
            float minTime, float maxTime, float minValue, float maxValue,
            int width, int height, float zoomLevel, int highlightedPointIndex = -1,
            int hoveredPointIndex = -1)
        {
            _g = g;
            _points = points;
            _curve = curve;
            _minTime = minTime;
            _maxTime = maxTime;
            _minValue = minValue;
            _maxValue = maxValue;
            _width = width;
            _height = height;
            _zoomLevel = zoomLevel;
            _highlightedPointIndex = highlightedPointIndex;
            _hoveredPointIndex = hoveredPointIndex;

            // Create resources (these must be disposed)
            _gridFont = new Font("Arial", 8);
            _titleFont = new Font("Arial", 10, FontStyle.Bold);
            _curvePen = new Pen(Color.LimeGreen, 2f);
            _gridPen = new Pen(Color.FromArgb(60, 60, 60), 1f);
            _pointPen = new Pen(Color.White, 2f);
            _pointBrush = Brushes.White; // System brush, don't dispose
        }

        // <-- ADDED: IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _gridFont?.Dispose();
                    _titleFont?.Dispose();
                    _curvePen?.Dispose();
                    _gridPen?.Dispose();
                    _pointPen?.Dispose();
                    // _pointBrush is Brushes.White (system brush), do NOT dispose
                }
                _disposed = true;
            }
        }

        // <-- ADDED: Destructor for safety
        ~CurveRenderer()
        {
            Dispose(false);
        }

        /// <summary>Draws grid, curve line, points, and labels to the graphics surface.</summary>
        public void Render()
        {
            if (_points.Count == 0) return;

            DrawGrid();
            DrawCurve();
            DrawPoints();
            DrawHoverLabel();
            DrawLabels();
        }

        private void DrawGrid()
        {
            float timeRange = _maxTime - _minTime;
            float valueRange = _maxValue - _minValue;

            if (timeRange <= 0 || valueRange <= 0) return;

            float timeStep = CalculateNiceStep(timeRange / GRID_LINES);
            float valueStep = CalculateNiceStep(valueRange / GRID_LINES);

            float firstTime = (float)Math.Ceiling(_minTime / timeStep) * timeStep;
            float firstValue = (float)Math.Ceiling(_minValue / valueStep) * valueStep;

            for (float time = firstTime; time <= _maxTime; time += timeStep)
            {
                float x = (time - _minTime) * _width / timeRange;
                if (x >= -50 && x <= _width + 50)
                {
                    _g.DrawLine(_gridPen, x, 0, x, _height);
                    _g.DrawString(time.ToString("F1", CultureInfo.InvariantCulture), _gridFont, Brushes.Gray, x, _height - 15);
                }
            }

            for (float value = firstValue; value <= _maxValue; value += valueStep)
            {
                float y = _height - ((value - _minValue) * _height / valueRange);
                if (y >= -50 && y <= _height + 50)
                {
                    _g.DrawLine(_gridPen, 0, y, _width, y);
                    _g.DrawString(value.ToString("F2", CultureInfo.InvariantCulture), _gridFont, Brushes.Gray, 5, y);
                }
            }
        }

        private void DrawCurve()
        {
            if (_curve == null || _curve.Curve.keys.Length < 2) return;

            List<PointF> curvePoints = new List<PointF>();
            int samples = Math.Min(_width, Constants.MAX_SAMPLES);

            for (int i = 0; i <= samples; i++)
            {
                float time = _minTime + (i * (_maxTime - _minTime) / samples);
                float value = _curve.Evaluate(time);

                float x = (time - _minTime) * _width / (_maxTime - _minTime);
                float y = _height - ((value - _minValue) * _height / (_maxValue - _minValue));

                curvePoints.Add(new PointF(x, y));
            }

            if (curvePoints.Count >= 2)
            {
                _g.DrawCurve(_curvePen, curvePoints.ToArray());
            }
        }

        private void DrawPoints()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                bool isHighlighted = (i == _highlightedPointIndex);
                float x = (_points[i].Time - _minTime) * _width / (_maxTime - _minTime);
                float y = _height - ((_points[i].Value - _minValue) * _height / (_maxValue - _minValue));

                Brush brush = isHighlighted ? Brushes.Yellow : _pointBrush;

                // Draw the point with appropriate pen
                _g.FillEllipse(brush, x - 4, y - 4, 8, 8);

                if (isHighlighted)
                {
                    // Use 'using' ONLY for locally created pen
                    using Pen highlightPen = new(Color.Yellow, 2f);
                    _g.DrawEllipse(highlightPen, x - 4, y - 4, 8, 8);
                }
                else
                {
                    _g.DrawEllipse(_pointPen, x - 4, y - 4, 8, 8);
                }
            }
        }

        private void DrawLabels()
        {
            string title = $"Curve Editor - {_points.Count} point(s)";
            //string range = $"Time: [{_minTime:F1}, {_maxTime:F1}]  Value: [{_minValue:F2}, {_maxValue:F2}]";

            SizeF titleSize = _g.MeasureString(title, _titleFont);
            //SizeF rangeSize = _g.MeasureString(range, _gridFont);
            float boxWidth = titleSize.Width + 5;
            float boxHeight = titleSize.Height + 10;

            _g.FillRectangle(Brushes.Black, 35, 5, boxWidth, boxHeight);
            _g.DrawString(title, _titleFont, Brushes.White, 40, 10);
            //_g.DrawString(range, _gridFont, Brushes.White, 10, 30);

            string zoomText = $"Zoom: {_zoomLevel:F1}x";
            SizeF textSize = _g.MeasureString(zoomText, _gridFont);
            float labelX = _width - textSize.Width - 10;
            float labelY = _height - textSize.Height - 30;
            _g.DrawString(zoomText, _gridFont, Brushes.White, labelX, labelY);
        }

        /// <summary>
        /// Draws a hover info box showing Time, Value, InTangent, OutTangent
        /// for the currently hovered or dragged point.
        /// </summary>
        private void DrawHoverLabel()
        {
            if (_hoveredPointIndex < 0 || _hoveredPointIndex >= _points.Count) return;

            var point = _points[_hoveredPointIndex];

            // Position label near the point (but don't go off-screen)
            float x = (point.Time - _minTime) * _width / (_maxTime - _minTime) + Constants.HOVER_LABEL_OFFSET_X;
            float y = _height - ((point.Value - _minValue) * _height / (_maxValue - _minValue)) + Constants.HOVER_LABEL_OFFSET_Y;

            // Keep label on screen
            x = Math.Max(5, Math.Min(_width - 120, x));
            y = Math.Max(5, Math.Min(_height - 60, y));

            // Create label text
            string label = $"Point {_hoveredPointIndex + 1}\n" +
                           $"Time: {FloatString4.FormatNumber(point.Time, "F2")}\n" +
                           $"Value: {FloatString4.FormatNumber(point.Value, "F3")}\n";

            // Measure and draw background box
            SizeF textSize = _g.MeasureString(label, _gridFont);
            float boxWidth = textSize.Width + 10;
            float boxHeight = textSize.Height + 8;

            _g.FillRectangle(Brushes.Black, x, y, boxWidth, boxHeight);
            _g.DrawRectangle(Pens.White, x, y, boxWidth, boxHeight);

            // Draw text
            _g.DrawString(label, _gridFont, Brushes.White, x + 5, y + 4);
        }

        private float CalculateNiceStep(float rawStep)
        {
            if (rawStep <= 0) return 1f;
            float exponent = (float)Math.Pow(10, Math.Floor(Math.Log10(rawStep)));
            float normalized = rawStep / exponent;

            if (normalized <= 1) return 1f * exponent;
            if (normalized <= 2) return 2f * exponent;
            if (normalized <= 5) return 5f * exponent;
            return 10f * exponent;
        }
    }
}