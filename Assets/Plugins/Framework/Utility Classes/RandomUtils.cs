using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;



namespace Framework
{
    /// <summary>
    /// Helper class for making random choices.
    /// </summary>
    public static class RandomUtils
    {
        private static string _numbers = "0123456789";
        private static string _lowercaseLetters = "abcedefghijklmnopqrstuvwxyz";
        private static string _uppercaseLetters = "ABCEDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Randomizes the Unity seed naievely based on the system time.
        /// </summary>
        public static void RandomizeSeed()
        {
            Random.InitState(new System.Random((int)DateTime.Now.Ticks & 0x0000FFFF).Next());
        }

        public static Random.State GetNewRandomStateFromSeed(int seed)
        {
            Random.State originalState = Random.state;
            Random.InitState(seed);
            Random.State newState = Random.state;
            Random.state = originalState;

            return newState;
        }

        public static Quaternion GetRandomYRotation()
        {
            return Quaternion.Euler(0, Random.value * 360, 0);
        }

        public static string GetRandomCharacters(int numChars, bool allowNumbers, bool allowLowercaseLetters, bool allowUppercaseLetters)
        {
            string charSet = null;

            if (allowNumbers && !allowLowercaseLetters && !allowUppercaseLetters)
            {
                charSet = _numbers;
            }
            else if (!allowNumbers && allowLowercaseLetters && !allowUppercaseLetters)
            {
                charSet = _lowercaseLetters;
            }
            else if (!allowNumbers && !allowLowercaseLetters && allowUppercaseLetters)
            {
                charSet = _uppercaseLetters;
            }
            else
            {
                if (allowNumbers) charSet += _numbers;
                if (allowLowercaseLetters) charSet += _lowercaseLetters;
                if (allowUppercaseLetters) charSet += _lowercaseLetters;
            }

            if (string.IsNullOrEmpty(charSet)) return null;

            char[] chars = new char[numChars];

            for (int i = 0; i < numChars; i++)
            {
                chars[i] = charSet[Random.Range(0, charSet.Length)];
            }

            return new string(chars);
        }

        public static float GetGassianValue(float mean = 0f, float standardDeviation = 1f)
        {
            return GaussianSampler.NextSample(mean, standardDeviation);
        }

        public static float GetGaussianRange(float min, float max, float mean = 0f, float standardDeviation = 1f)
        {

            float x;
            do
            {
                x = GaussianSampler.NextSample(mean, standardDeviation);
            } while (x < min || x > max);

            return x;
        }

        public static Vector2 GetRandomVector2()
        {
            return new Vector2(Random.value, Random.value);
        }

        public static Vector3 GetRandomVector3()
        {
            return new Vector3(Random.value, Random.value, Random.value);
        }

        public static Vector4 GetRandomVector4()
        {
            return new Vector4(Random.value, Random.value, Random.value, Random.value);
        }

        public static Vector3 GetRandomDirectionXZ()
        {
            return Random.insideUnitCircle.ToVector3XZ().normalized;
        }

        /// <summary>
        /// Returns a random point on a unit circle projected onto a plane.
        /// </summary>
        /// <param name="normal">The normal of the plane</param>
        /// <returns>A random point on a unit circle on the plane.</returns>
        public static Vector3 OnUnitCircleOnPlane(Vector3 normal)
        {
            return Vector3.Cross(Random.insideUnitSphere, normal).normalized;
        }

        /// <summary>
        /// Returns a random point inside a unit circle projected onto a plane.
        /// </summary>
        /// <param name="normal">The normal of the plane</param>
        /// <returns>A random point inside a unit circle on the plane.</returns>
        public static Vector3 InsideUnitCircleOnPlane(Vector3 normal)
        {
            return Vector3.Cross(Random.insideUnitSphere, normal).normalized * Random.value;
        }

        public static Vector3 InsideCircle(Vector3 center, float radius, Vector3 normal)
        {
            return center + (Vector3.Cross(Random.insideUnitSphere, normal).normalized * Random.value * radius);
        }

        public static Vector3 OnCircle(Vector3 center, float radius, Vector3 normal)
        {
            return center + (Vector3.Cross(Random.insideUnitSphere, normal).normalized * radius);
        }

        /// <summary>
        /// Returns a random point on a unit sphere, but constrained to a cap defined by a cone.
        /// </summary>
        /// <param name="coneDirection">The cone direction</param>
        /// <param name="coneAngle">The angle of the cone, in degrees between 0 and 180</param>
        /// <returns>A vector of unit length with a random direction defined by the cone</returns>
        public static Vector3 OnUnitSphereCap(Vector3 coneDirection, float coneAngle)
        {
            float radians = Random.Range(0, Mathf.Clamp(coneAngle * 0.5f, 0f, 180f)) * Mathf.Deg2Rad;
            Vector3 pointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(radians);

            return Quaternion.LookRotation(coneDirection) * new Vector3(pointOnCircle.x, pointOnCircle.y, Mathf.Cos(radians));
        }

        /// <summary>
        /// Returns a random point inside some specific bounds.
        /// </summary>
        /// <param name="bounds">The bounds that point must be inside</param>
        /// <returns>A random point inside the bounds</returns>
        public static Vector3 InsideBounds(Bounds bounds)
        {
            return bounds.center + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).MultiplyComponentWise(bounds.extents);
        }

        public static Vector3 InsideBox(Box3 box)
        {
            return box.TransformPoint(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
        }

        /// <summary>
        /// Has a 50/50 chance of returning true or false.
        /// </summary>
        /// <returns>Whether or not the coin landed on heads</returns>
        public static bool CoinToss()
        {
            return Random.value < 0.5f;
        }

        public static T CoinToss<T>(T choiceA, T choiceB)
        {
            return Random.value < 0.5f ? choiceA : choiceB;
        }

        public static float Sign()
        {
            return Random.value < 0.5f ? -1f : 1f;
        }


        /// <summary>
        /// Returns an integer between 1 and 6 inclusive.
        /// </summary>
        /// <returns>The result of the dice roll</returns>
        public static int DiceRoll()
        {
            return Random.Range(1, 7);
        }

        /// <summary>
        /// Has a probalistic chance of returning true.
        /// </summary>
        /// <param name="probability">A normalized probablity</param>
        /// <returns>Whether or not this chance was taken</returns>
        public static bool Chance(float probability)
        {
            return Random.value < Mathf.Clamp01(probability);
        }

        /// <summary>
        /// Returns a random element from an IList with uniform probability.
        /// </summary>
        /// <param name="choices">The list of potential choices</param>
        /// <returns>The choice</returns>
        public static T Choose<T>(IList<T> choices)
        {
            Assert.IsNotNull(choices);
            Assert.IsTrue(choices.Count > 0, "Empty list or array provided to choose algorithm.");
            return choices[Random.Range(0, choices.Count)];
        }

        public static int ChooseInt(int numChoices)
        {
            return Random.Range(0, numChoices);
        }

        public static int ChooseIndex(IList list)
        {
            Assert.IsTrue(list.Count > 0, "Empty list or array provided to choose algorithm.");
            return Random.Range(0, list.Count);
        }

        /// <summary>
        /// Retruns a random element from a IList that is not also a member of a set of excluded values.
        /// </summary>
        /// <param name="choices">The list of potential choices</param>
        /// <param name="excludedChoices">The list of disallowed choices</param>
        /// <returns>The choice</returns>
        public static T ChooseExluding<T>(IList<T> choices, params T[] excludedChoices)
        {
            Assert.IsNotNull(choices);
            Assert.IsNotNull(excludedChoices);
            Assert.IsTrue(choices.Count > 0, "Empty list or array provided to choose algorithm.");

            T choice = choices[Random.Range(0, choices.Count)];
            while (excludedChoices.Contains(choice))
            {
                choice = choices[Random.Range(0, choices.Count)];
            }
            return choice;
        }

        /// <summary>
        /// Returns a random element from a set with uniform probability.
        /// </summary>
        /// <param name="choices">The set of potential choices</param>
        /// <returns>The choice</returns>
        public static T ChooseOne<T>(params T[] choices)
        {
            Assert.IsNotNull(choices);
            Assert.IsTrue(choices.Length > 0, "Empty list or array provided to choose algorithm.");

            return choices[Random.Range(0, choices.Length)];
        }

        /// <summary>
        /// Returns a random element from a list using a probalistic weight for each element.
        /// </summary>
        /// <param name="choices">The list of potential choices</param>
        /// <param name="weights">The list of weights corresponding to the list of choices
        /// <returns>The choice</returns>
        public static T Choose<T>(IList<T> choices, IList<float> weights)
        {
            Assert.IsNotNull(choices);
            Assert.IsNotNull(weights);
            Assert.IsTrue(choices.Count == weights.Count, "Number of weights (" + weights.Count + ") does not match number of choices (" + choices.Count + ").");

            float totalWeight = 0;
            for (int i = 0; i < choices.Count; i++)
            {
                weights[i] = Mathf.Max(0, weights[i]);
                totalWeight += weights[i];
            }

            if (totalWeight == 0)
            {
                throw new UnityException("Error choosing value: All probability weights are zero.");
            }

            float choice = Random.value * totalWeight;
            totalWeight = 0;

            for (int i = 0; i < choices.Count; i++)
            {
                if (weights[i] == 0)
                {
                    continue;
                }
                if (totalWeight + weights[i] > choice)
                {
                    return choices[i];
                }
                totalWeight += weights[i];
            }

            throw new UnityException("Choose algorithm is broken?");
        }

        public static T[] Choose<T>(int numChoices, IList<T> choices)
        {
            T[] results = new T[numChoices];
            ShuffleBag<T> bag = new ShuffleBag<T>(choices);

            for (int i = 0; i < numChoices; i++)
            {
                results[i] = bag.GetNext();
            }

            return results;
        }

        /// <summary>
        /// Returns a random enum value using a probalistic weight for each enum value.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="weights">The list of weights corresponding to the list of choices
        /// <returns>The chosen enum value</returns>
        public static T ChooseEnumValue<T>(IList<float> weights) where T : struct, IComparable, IConvertible, IFormattable
        {

            DebugUtils.AssertIsEnumType<T>();
            Assert.IsNotNull(weights);
            T[] choices = Enum.GetValues(typeof(T)) as T[];

            Assert.IsTrue(choices.Length == weights.Count, "Number of weights (" + weights.Count + ") does not match number of enum values (" + choices.Length + ").");

            float totalWeight = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                weights[i] = Mathf.Max(0, weights[i]);
                totalWeight += weights[i];
            }

            if (totalWeight == 0)
            {
                throw new UnityException("Error choosing value: All probability weights are zero.");
            }

            float choice = Random.value * totalWeight;
            totalWeight = 0;

            for (int i = 0; i < choices.Length; i++)
            {
                if (weights[i] == 0)
                {
                    continue;
                }
                if (totalWeight + weights[i] > choice)
                {
                    return choices[i];
                }
                totalWeight += weights[i];
            }

            throw new UnityException("Choose algorithm is broken?");
        }


        /// <summary>
        /// Returns a random enum value with uniform probability.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>The chosen enum value</returns>
        public static T ChooseEnumValue<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();
            T[] choices = Enum.GetValues(typeof(T)) as T[];
            return choices[Random.Range(0, choices.Length)];
        }

        /// <summary>
        /// Returns a random enum value that is not also a member of a set of excluded values.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="excludedChoices">The list of disallowed choices</param>
        /// <returns>The chosen enum value</returns>
        public static T ChooseEnumValueExluding<T>(params T[] excludedChoices) where T : struct, IComparable, IConvertible, IFormattable
        {
            Assert.IsNotNull(excludedChoices);
            Assert.IsFalse(excludedChoices.Length >= EnumUtils.GetCount<T>());
            DebugUtils.AssertIsEnumType<T>();

            List<T> choices = new List<T>(Enum.GetValues(typeof(T)) as T[]);

            for (int i = 0; i < excludedChoices.Length; i++)
            {
                choices.Remove(excludedChoices[i]);
            }

            return choices[Random.Range(0, choices.Count)];

        }

        /// <summary>
        /// Chooses a number of elements at random from a list of possible choices, each element can only be selected once.
        /// </summary>
        /// <param name="numberOfChoices">The number of elemtns to choose</param>
        /// <param name="choices">The list of potential choices</param>
        /// <returns>An array of the choices picked</returns>
        public static T[] ChooseSet<T>(int numberOfChoices, IList<T> choices)
        {
            Assert.IsNotNull(choices);
            Assert.IsTrue(numberOfChoices <= choices.Count, "Cannot select " + numberOfChoices + " elements from " + choices.Count + " choices.");
            Assert.IsTrue(numberOfChoices > 0);

            T[] result = new T[numberOfChoices];
            choices = new List<T>(choices);

            for (int i = 0; i < numberOfChoices; i++)
            {
                int choice = Random.Range(0, choices.Count);
                result[i] = choices[choice];
                choices.RemoveAt(choice);
            }

            return result;
        }



        // Ziggurat gaussian sampler adapted from GNUGPL code by Colin Green (sharpneat@gmail.com)
        static class GaussianSampler
        {

            /// <summary>
            /// Number of blocks.
            /// </summary>
            const int __blockCount = 128;

            /// <summary>
            /// Right hand x coord of the base rectangle, thus also the left hand x coord of the tail 
            /// (pre-determined/computed for 128 blocks).
            /// </summary>
            const float __R = 3.442619855899f;

            /// <summary>
            /// Area of each rectangle (pre-determined/computed for 128 blocks).
            /// </summary>
            const float __A = 9.91256303526217e-3f;

            /// <summary>
            /// Scale factor for converting a UInt with range [0,0xffffffff] to a double with range [0,1].
            /// </summary>
            const float __UIntToU = 1f / (float)uint.MaxValue;

            // _x[i] and _y[i] describe the top-right position ox rectangle i.
            static readonly float[] _x;
            static readonly float[] _y;

            // The proprtion of each segment that is entirely within the distribution, expressed as uint where 
            // a value of 0 indicates 0% and uint.MaxValue 100%. Expressing this as an integer allows some floating
            // points operations to be replaced with integer ones.
            static readonly uint[] _xComp;

            // Useful precomputed values.
            // Area A divided by the height of B0. Note. This is *not* the same as _x[i] because the area 
            // of B0 is __A minus the area of the distribution tail.
            static readonly float _A_Div_Y0;


            static GaussianSampler()
            {
                // Initialise rectangle position data. 
                // _x[i] and _y[i] describe the top-right position ox Box i.

                // Allocate storage. We add one to the length of _x so that we have an entry at _x[_blockCount], this avoids having 
                // to do a special case test when sampling from the top box.
                _x = new float[__blockCount + 1];
                _y = new float[__blockCount];

                // Determine top right position of the base rectangle/box (the rectangle with the Gaussian tale attached). 
                // We call this Box 0 or B0 for short.
                // Note. x[0] also describes the right-hand edge of B1. (See diagram).
                _x[0] = __R;
                _y[0] = GaussianPdfDenorm(__R);

                // The next box (B1) has a right hand X edge the same as B0. 
                // Note. B1's height is the box area divided by its width, hence B1 has a smaller height than B0 because
                // B0's total area includes the attached distribution tail.
                _x[1] = __R;
                _y[1] = _y[0] + (__A / _x[1]);

                // Calc positions of all remaining rectangles.
                for (int i = 2; i < __blockCount; i++)
                {
                    _x[i] = GaussianPdfDenormInv(_y[i - 1]);
                    _y[i] = _y[i - 1] + (__A / _x[i]);
                }

                // For completeness we define the right-hand edge of a notional box 6 as being zero (a box with no area).
                _x[__blockCount] = 0f;

                // Useful precomputed values.
                _A_Div_Y0 = __A / _y[0];
                _xComp = new uint[__blockCount];

                // Special case for base box. _xComp[0] stores the area of B0 as a proportion of __R 
                // (recalling that all segments have area __A, but that the base segment is the combination of B0 and the distribution tail).
                // Thus -xComp[0[ is the probability that a sample point is within the box part of the segment.
                _xComp[0] = (uint)(((__R * _y[0]) / __A) * (float)uint.MaxValue);

                for (int i = 1; i < __blockCount - 1; i++)
                {
                    _xComp[i] = (uint)((_x[i + 1] / _x[i]) * (float)uint.MaxValue);
                }
                _xComp[__blockCount - 1] = 0; // Shown for completeness.

                // Sanity check. Test that the top edge of the topmost rectangle is at y=1.0.
                // Note. We expect there to be a tiny drift away from 1.0 due to the inexactness of floating
                // point arithmetic.
                Assert.IsTrue(Math.Abs(1.0 - _y[__blockCount - 1]) < 1e-10);
            }


            /// <summary>
            /// Get the next sample value from the gaussian distribution.
            /// </summary>
            public static float NextSample()
            {
                for (; ; )
                {
                    // Select box at random.
                    byte u = (byte)Random.Range(0, 256);
                    int i = (int)(u & 0x7F);
                    float sign = ((u & 0x80) == 0) ? -1f : 1f;

                    // Generate uniform random value with range [0,0xffffffff].
                    uint u2 = (uint)Math.Floor(Random.value * uint.MaxValue);

                    // Special case for the base segment.
                    if (0 == i)
                    {
                        if (u2 < _xComp[0])
                        {
                            // Generated x is within R0.
                            return u2 * __UIntToU * _A_Div_Y0 * sign;
                        }
                        // Generated x is in the tail of the distribution.
                        return SampleTail() * sign;
                    }

                    // All other segments.
                    if (u2 < _xComp[i])
                    {
                        // Generated x is within the rectangle.
                        return u2 * __UIntToU * _x[i] * sign;
                    }

                    // Generated x is outside of the rectangle.
                    // Generate a random y coordinate and test if our (x,y) is within the distribution curve.
                    // This execution path is relatively slow/expensive (makes a call to Math.Exp()) but relatively rarely executed,
                    // although more often than the 'tail' path (above).
                    float x = u2 * __UIntToU * _x[i];
                    if (_y[i - 1] + ((_y[i] - _y[i - 1]) * Random.value) < GaussianPdfDenorm(x))
                    {
                        return x * sign;
                    }
                }
            }

            /// <summary>
            /// Get the next sample value from the gaussian distribution.
            /// </summary>
            /// <param name="mu">The distribution's mean.</param>
            /// <param name="sigma">The distribution's standard deviation.</param>
            public static float NextSample(float mu = 0f, float sigma = 1f)
            {
                return mu + (NextSample() * sigma);
            }

            /// <summary>
            /// Sample from the distribution tail (defined as having x >= __R).
            /// </summary>
            /// <returns></returns>
            private static float SampleTail()
            {
                float x, y;
                do
                {
                    // Note. we use NextDoubleNonZero() because Log(0) returns NaN and will also tend to be a very slow execution path (when it occurs, which is rarely).
                    x = -Mathf.Log(NonZeroRandom()) / __R;
                    y = -Mathf.Log(NonZeroRandom());
                } while (y + y < x * x);
                return __R + x;
            }

            /// <summary>
            /// Gaussian probability density function, denormailised, that is, y = e^-(x^2/2).
            /// </summary>
            private static float GaussianPdfDenorm(float x)
            {
                return Mathf.Exp(-(x * x / 2f));
            }

            /// <summary>
            /// Inverse function of GaussianPdfDenorm(x)
            /// </summary>
            private static float GaussianPdfDenormInv(float y)
            {
                // Operates over the y range (0,1], which happens to be the y range of the pdf, 
                // with the exception that it does not include y=0, but we would never call with 
                // y=0 so it doesn't matter. Remember that a Gaussian effectively has a tail going
                // off into x == infinity, hence asking what is x when y=0 is an invalid question
                // in the context of this class.
                return Mathf.Sqrt(-2f * Mathf.Log(y));
            }

            static float NonZeroRandom()
            {
                float value = 0f;
                do
                {
                    value = Random.value;
                } while (value == 0f);

                return value;
            }

        }


    }
}
