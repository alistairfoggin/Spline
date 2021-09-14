using Components;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Editors
{
    [CustomEditor(typeof(Spline)), CanEditMultipleObjects]
    public class SplineEditor : Editor
    {
        private void OnSceneGUI()
        {
            Spline splineComponent = (Spline) target;
        
            for (int i = 0; i < splineComponent.points.Length; i++)
            {
                float size = HandleUtility.GetHandleSize(splineComponent.points[i]) * 0.1f;
                
                EditorGUI.BeginChangeCheck();
                Vector3 newPoint = Handles.FreeMoveHandle(i, splineComponent.points[i], Quaternion.identity, size, Vector3.zero, Handles.RectangleHandleCap);
                // Vector3 newPoint = Handles.PositionHandle(splineComponent.points[i], Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(splineComponent, "Change Point Position");
                    splineComponent.points[i] = newPoint;
                    splineComponent.Update();
                }

            }
        }
    }
}
