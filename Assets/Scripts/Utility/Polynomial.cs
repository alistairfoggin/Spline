using System;
using UnityEngine;

namespace Utility
{
    public class Polynomial
    {
        public float[] Coefficients { get; set; }

        public Polynomial()
        {
            Coefficients = Array.Empty<float>();
            Coefficients[0] = 0.0f;
        }

        // Coefficients are an array that correspond as follows: a + bx + cx^2 + dx^3 ...
        public Polynomial(float[] coefficients)
        {
            Coefficients = coefficients;
        }

        public float CalculateValue(float x)
        {
            float result = 0;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                result += Coefficients[i] * Mathf.Pow(x, i);
            }
            return result;
        }

        public Polynomial CalculateDerivative()
        {
            float[] newCoefficients = new float[Coefficients.Length - 1];
            for (int i = 0; i < newCoefficients.Length; i++)
            {
                float coefficient = Coefficients[i + 1];
                coefficient *= (i + 1);
                newCoefficients[i] = coefficient;
            }

            return new Polynomial(newCoefficients);
        }

        public Polynomial CalculateGeneralIntegral()
        {
            float[] newCoefficients = new float[Coefficients.Length + 1];
            newCoefficients[0] = 0.0f;
            for (int i = 1; i < newCoefficients.Length; i++)
            {
                float coefficient = Coefficients[i - 1];
                coefficient /= (i);
                newCoefficients[i] = coefficient;
            }

            return new Polynomial(newCoefficients);
        }

        public Polynomial CalculateSpecificIntegral(Vector2 point)
        {
            Polynomial integral = CalculateGeneralIntegral();
            float y = integral.CalculateValue(point.x);
            integral.Coefficients[0] = point.y - y;

            return integral;
        }
    }
}
