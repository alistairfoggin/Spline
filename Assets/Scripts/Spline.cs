using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class Spline : MonoBehaviour
{
    public Vector3[] points = new Vector3[0];
    public Vector3[] slopes = new Vector3[0];
    public int subdivisions = 5;

    private Vector3[] subPoints;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();

        subPoints = new Vector3[(points.Length - 1) * subdivisions];
        //slopes = new Vector3[points.Length];
        slopes[0] = Vector3.zero;

        for (int i = 0; i < points.Length - 1; i++)
        {
            // only calculate slope for intermediate points
            if (i != 0)
            {
                // slope functions
                Polynomial fsx = new Polynomial(CalcSlopeCoeffsForAxis(0, i));
                Polynomial fsy = new Polynomial(CalcSlopeCoeffsForAxis(1, i));
                Polynomial fsz = new Polynomial(CalcSlopeCoeffsForAxis(2, i));

                float t = i / (float)points.Length;
                float sx = fsx.calculateValue(t);
                float sy = fsy.calculateValue(t);
                float sz = fsz.calculateValue(t);

                slopes[i] = new Vector3(sx, sy, sz);
            }

            calculateSubPointsForIndex(i);

        }
        subPoints[subPoints.Length - 1] = points[points.Length - 1];

        lineRenderer.positionCount = subPoints.Length;
        lineRenderer.SetPositions(AdjustPointsForPosition(subPoints));
    }

    private void calculateSubPointsForIndex(int i)
    {
        // axis functions
        Polynomial fx = new Polynomial(CalcPositionCoeffsForAxis(0, i));
        Polynomial fy = new Polynomial(CalcPositionCoeffsForAxis(1, i));
        Polynomial fz = new Polynomial(CalcPositionCoeffsForAxis(2, i));

        for (int j = 0; j < subdivisions; j++)
        {
            // t interpolates between 0 and 1
            float t = (float)j / (float)subdivisions;
            float x = fx.calculateValue(t);
            float y = fy.calculateValue(t);
            float z = fz.calculateValue(t);
            subPoints[i * subdivisions + j] = new Vector3(x, y, z);
        }
    }


    // axis is 0, 1, or 2 corresponding with x, y, and z
    private float[] CalcPositionCoeffsForAxis(int axis, int index)
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

    // get parabola for x(i-1) x(i) and x(i+1)
    private float[] CalcSlopeCoeffsForAxis(int axis, int index)
    {
        if (index == 0) return new float[] { 0.0f };
        float x0 = points[index - 1][axis];
        float x1 = points[index][axis];
        float x2 = points[index + 1][axis];

        float[] xVals = { x0, x1, x2 };

        float t0 = (index - 1) / (float)points.Length;
        float t1 = index / (float)points.Length;
        float t2 = (index + 1) / (float)points.Length;

        Matrix4x4 thisMatrix = new Matrix4x4(
            new Vector4(1, 1, 1, 0),
            new Vector4(t0, t1, t2, 0),
            new Vector4(t0 * t0, t1 * t1, t2 * t2),
            Vector4.zero);

        Vector3 coeffs = thisMatrix.inverse.MultiplyVector(new Vector3(x0, x1, x2));
        return new float[] { coeffs.x, coeffs.y, coeffs.z };

        float det0 = t1 * t2 * t2 - t2 * t1 * t1;
        float det1 = t2 * t0 * t0 - t0 * t2 * t2;
        float det2 = t0 * t1 * t1 - t1 * t0 * t0;
        float det = det0 + det1 + det2;

        // rows by cols
        float[,] matrix = new float[3, 3] {
            { det0,              det1,              det2 },
            { t1 * t1 - t2 * t2, t2 * t2 - t0 * t0, t0 * t0 - t1 * t1 },
            { t2 - t1,           t0 - t2,           t1 - t0 }
        };
        float[] coefficients = new float[3];

        for (int i = 0; i < 3; i++)
        {
            coefficients[i] = 0.0f;
            for (int j = 0; j < 3; j++)
            {
                coefficients[i] = matrix[i, j] * xVals[j] / det;
            }
        }

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
