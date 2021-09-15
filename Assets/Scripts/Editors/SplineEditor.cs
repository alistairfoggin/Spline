using System;
using Components;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Editors
{
    [CustomEditor(typeof(Spline)), CanEditMultipleObjects]
    public class SplineEditor : Editor
    {
        private int _selectedIndex = -1;
        private Spline _splineComponent;

        private void OnSceneGUI()
        {
            _splineComponent = (Spline) target;

            for (int i = 0; i < _splineComponent.points.Length; i++)
            {
                HandleIndex(i);
            }

            Handles.DrawPolyLine(_splineComponent.VisiblePoints);
        }

        private void HandleIndex(int index)
        {
            float size = HandleUtility.GetHandleSize(_splineComponent.points[index]) * 0.1f;

            if (Handles.Button(_splineComponent.points[index], Quaternion.identity, size, size * 2f,
                Handles.CubeHandleCap))
                _selectedIndex = index;

            if (_selectedIndex != index)
                return;

            EditorGUI.BeginChangeCheck();
            // Vector3 newPoint = Handles.PositionHandle(index, _splineComponent.points[index], Quaternion.identity, size, Vector3.zero, Handles.CubeHandleCap);
            Vector3 newPoint = Handles.PositionHandle(_splineComponent.points[index], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_splineComponent, "Change Point Position");
                _splineComponent.points[index] = newPoint;
                _splineComponent.CalculateSubPoints();
            }
        }
    }
}