using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SimpleParticleSystem
{
    [Serializable]
    public class RandomNumber<T> where T : struct, IComparable
    {
        [SerializeField] private RandomType type;
        [SerializeField] private T constantValue;
        [SerializeField] private T min;
        [SerializeField] private T max;
        [SerializeField] private AnimationCurve curve;


        private System.Random _random = new();

        public T GetValue()
        {
            switch (type)
            {
                case RandomType.Constant:
                    return constantValue;

                case RandomType.Range:
                    if (typeof(T) == typeof(int))
                    {
                        var minValue = Convert.ToInt32(min);
                        var maxValue = Convert.ToInt32(max);
                        var randomInt = _random.Next(minValue, maxValue + 1);
                        return (T)(object)randomInt;
                    }

                    if (typeof(T) == typeof(float))
                    {
                        var minValue = Convert.ToSingle(min);
                        var maxValue = Convert.ToSingle(max);
                        var randomFloat = Random.Range(minValue, maxValue);
                        return (T)(object)randomFloat;
                    }

                    if (typeof(T) == typeof(double))
                    {
                        var minValue = Convert.ToDouble(min);
                        var maxValue = Convert.ToDouble(max);
                        var randomDouble = minValue + _random.NextDouble() * (maxValue - minValue);
                        return (T)(object)randomDouble;
                    }

                    throw new InvalidOperationException($"Unsupported type for Range: {typeof(T)}");

                case RandomType.Curve:
                    if (typeof(T) == typeof(float))
                    {
                        var time = Random.Range(0f, 1f);
                        var curveValue = curve.Evaluate(time);
                        return (T)(object)curveValue;
                    }

                    throw new InvalidOperationException("Curve type only supports float.");

                default:
                    throw new InvalidOperationException("Unknown RandomType.");
            }
        }
    }
}