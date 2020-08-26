using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class Spline : MonoBehaviour
{
    public Vector3[] points = new Vector3[0];
    public Vector3[] slopes = new Vector3[0];
    public int subdivisions = 5;

    private LineRenderer lineRenderer;
    private Vector3[] subPoints;

    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        subPoints = new Vector3[(points.Length - 1) * subdivisions];
        for (int i = 0; i < points.Length - 1; i++)
        {
            Polynomial fx = new Polynomial(CalcCoeffsForAxis(0, i));
            Polynomial fy = new Polynomial(CalcCoeffsForAxis(1, i));
            Polynomial fz = new Polynomial(CalcCoeffsForAxis(2, i));

            for (int j = 0; j < subdivisions; j++)
            {
                float t = (float)j / (float)subdivisions;
                float x = fx.calculateValue(t);
                float y = fy.calculateValue(t);
                float z = fz.calculateValue(t);
                subPoints[i * subdivisions + j] = new Vector3(x, y, z);
            }
        }
        subPoints[subPoints.Length - 1] = points[points.Length - 1];

        lineRenderer.positionCount = subPoints.Length;
        lineRenderer.SetPositions(AdjustPointsForPosition(subPoints));
    }


    // axis is 0, 1, or 2 corresponding with x, y, and z
    float[] CalcCoeffsForAxis(int axis, int index)
    {
        // x = a + bt + ct^2 + dt^3
        // s = b + 2ct + 3dt^2
        float[] coefficients = new float[4];
        float x0 = points[index][axis];
        float x1 = points[index + 1][axis];
        float s0 = slopes[index][axis];
        float s1 = slopes[index + 1][axis];

        coefficients[0] = x0;
        coefficients[1] = s0;
        coefficients[2] = 3 * (x1 - x0) - 2 * s0 - s1;
        coefficients[3] = 2 * (x0 - x1) + s0 + s1;
        return coefficients;
    }

    private Vector3[] AdjustPointsForPosition(Vector3[] thesePoints)
    {
        Vector3[] newPoints = new Vector3[thesePoints.Length];

        for (int i = 0; i < thesePoints.Length; i++)
        {
            float x = transform.position.x + thesePoints[i].x;
            float y = transform.position.y + thesePoints[i].y;
            float z = transform.position.z + thesePoints[i].z;
            newPoints[i] = new Vector3(x, y, z);
        }

        return newPoints;
    }
}
