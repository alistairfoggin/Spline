using System;
using UnityEngine;
using Utility;

namespace Components
{
    [ExecuteAlways]
    public class Spline : MonoBehaviour
    {
        public Vector3[] points = Array.Empty<Vector3>();
        [Range(1, 50)] public int subdivisions = 5;

        public Vector3[] VisiblePoints => _subPoints;

        private Vector3[] _subPoints;
        private Vector3[] _slopes = Array.Empty<Vector3>();

        private Vector3[] _lastPoints = Array.Empty<Vector3>();
        private int _lastSubdivisions;

        private void Update()
        {
            if (_lastSubdivisions != subdivisions)
            {
                _lastSubdivisions = subdivisions;
                CalculateSubPoints();
            }

            if (_lastPoints.Length != points.Length)
            {
                _lastPoints = points;
                CalculateSubPoints();
            }
            else
            {
                bool isDifferent = false;
                for (int i = 0; i < points.Length; i++)
                {
                    if (!_lastPoints[i].Equals(points[i]))
                    {
                        isDifferent = true;
                        _lastPoints[i] = points[i];
                    }
                }

                if (isDifferent)
                {
                    CalculateSubPoints();
                }
            }
        }

        public void CalculateSubPoints()
        {
            if (_slopes.Length != points.Length)
            {
                _slopes = new Vector3[points.Length];
                _slopes[0] = Vector3.zero;
                _slopes[points.Length - 1] = Vector3.zero;
            }

            _subPoints = new Vector3[(points.Length - 1) * subdivisions + 1];

            for (int i = 0; i < points.Length - 1; i++)
            {
                if (i != 0)
                {
                    _slopes[i] = new Vector3(
                        CalculateSlopeAtIndexForAxis(i, 0),
                        CalculateSlopeAtIndexForAxis(i, 1),
                        CalculateSlopeAtIndexForAxis(i, 2));
                }

                Polynomial fx = CalculatePolynomialOfIndexForAxis(i, 0);
                Polynomial fy = CalculatePolynomialOfIndexForAxis(i, 1);
                Polynomial fz = CalculatePolynomialOfIndexForAxis(i, 2);
                for (int j = 0; j < subdivisions; j++)
                {
                    float t = 1.0f / subdivisions * j;
                    _subPoints[i * subdivisions + j] =
                        new Vector3(fx.CalculateValue(t), fy.CalculateValue(t), fz.CalculateValue(t)) +
                        transform.position;
                }
            }

            _subPoints[_subPoints.Length - 1] = points[points.Length - 1] + transform.position;
        }

        private Polynomial CalculatePolynomialOfIndexForAxis(int index, int axis)
        {
            if (index >= points.Length - 1)
                return null;

            float a = points[index][axis];
            float b = _slopes[index][axis];
            float c = 3 * (points[index + 1][axis] - points[index][axis]) - 2 * _slopes[index][axis] -
                      _slopes[index + 1][axis];
            float d = 2 * (points[index][axis] - points[index + 1][axis]) + _slopes[index][axis] +
                      _slopes[index + 1][axis];
            return new Polynomial(new[] {a, b, c, d});
        }

        private float CalculateSlopeAtIndexForAxis(int index, int axis)
        {
            float x0 = points[index - 1][axis];
            float x2 = points[index + 1][axis];

            float b = (x2 - x0) / 2f;
            return b;
        }
    }
}