using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polynomial
{
    public float[] Coefficients { get; set; }

    public Polynomial()
    {
        Coefficients = new float[0];
        Coefficients[0] = 0.0f;
    }

    // Coefficients are an array that correspond as follows: a + bx + cx^2 + dx^3 ...
    public Polynomial(float[] coefficients)
    {
        Coefficients = coefficients;
    }

    public float calculateValue(float x)
    {
        float result = 0;
        for (int i = 0; i < Coefficients.Length; i++)
        {
            result += Coefficients[i] * Mathf.Pow(x, i);
        }
        return result;
    }

    public Polynomial calculateDerivative()
    {
        float[] coeffs = new float[Coefficients.Length - 1];
        for (int i = 0; i < coeffs.Length; i++)
        {
            float coefficient = Coefficients[i + 1];
            coefficient *= (i + 1);
            coeffs[i] = coefficient;
        }

        return new Polynomial(coeffs);
    }

    public Polynomial calculateGeneralIntegral()
    {
        float[] coeffs = new float[Coefficients.Length + 1];
        coeffs[0] = 0.0f;
        for (int i = 1; i < coeffs.Length; i++)
        {
            float coefficient = Coefficients[i - 1];
            coefficient /= (i);
            coeffs[i] = coefficient;
        }

        return new Polynomial(coeffs);
    }

    public Polynomial calculateSpecificIntegral(Vector2 point)
    {
        Polynomial integral = calculateGeneralIntegral();
        float y = integral.calculateValue(point.x);
        integral.Coefficients[0] = point.y - y;

        return integral;
    }
}
