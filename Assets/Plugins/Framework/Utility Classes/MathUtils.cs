using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    /// <summary>
    /// Utility class for maths, particularly 3D geometry stuff. A lot of this was adapted from code by Bit Barrel Media on the Unity Community wiki.
    /// </summary>
    public static class MathUtils
    {
        // Some calculations cannot be exact due to floating point rounding errors, this value accounts for those.
        public const float ROUNDING_ERROR_TOLERENCE = 0.001f;
        public const float TWO_PI = 6.28318530718f;
        public const float PI_OVER_TWO = 1.570796326795f;
        public const float PI_OVER_FOUR = 0.78539816339f;
        public const float PI_THREE_OVER_FOUR = 2.35619449019f;
        public const float SQUARE_ROOT_TWO = 1.41421356237f;
        public const float E = 2.7182818284f;

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return a.sqrMagnitude >= b.sqrMagnitude ? a : b;
        }

        public static Vector3 Max(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = a.sqrMagnitude >= b.sqrMagnitude ? a : b;
            return ab.sqrMagnitude >= c.sqrMagnitude ? ab : c;
        }

        public static Vector3 Max(params Vector3[] vectors)
        {
            float max = vectors[0].sqrMagnitude;
            Vector3 result = vectors[0];

            for (int i = 1; i < vectors.Length; i++)
            {
                float mag = vectors[i].sqrMagnitude;
                if (mag > max)
                {
                    max = mag;
                    result = vectors[i];
                }
            }

            return result;
        }

        public static float DistanceXZ(Vector3 pointA, Vector3 pointB)
        {
            return Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));
        }

        public static Vector3 Floor(Vector3 vector)
        {
            return new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), Mathf.Floor(vector.z));
        }

        public static Vector3 Ceil(Vector3 vector)
        {
            return new Vector3(Mathf.Ceil(vector.x), Mathf.Ceil(vector.y), Mathf.Ceil(vector.z));
        }

        public static Vector3 Abs(Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static float GetHDRColourIntensityMultiplier(float unityIntensityValue)
        {
            return Mathf.Pow(2.0f, unityIntensityValue - 0.4169f);
        }

        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            if (a == b) return 0;

            Vector3 AB = b - a;
            Vector3 AV = value - a;

            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }

        public static float ExponentialLerp(float min, float max, float t, float exponent)
        {
            return min + Mathf.Pow(t, exponent) * (max - min);
        }

        public static int Get1DArrayIndex(int x, int y, int width)
        {
            return x + (y * width);
        }

        public static Vector2Int Get2DArrayIndex(int index, int width)
        {
            return new Vector2Int(index % width, index / width);
        }

        public static int Get2DArrayXIndex(int index, int width)
        {
            return index % width;
        }
        public static int Get2DArrayYIndex(int index, int width)
        {
            return index / width;
        }

        public static int Get1DArrayIndex(int x, int y, int z, int width, int height)
        {
            return (z * width * height) + (y * width) + x;
        }

        public static Vector3Int Get3DArrayIndex(int index, int width, int height)
        {
            int z = index / (width * height);
            index -= (z * width * height);
            int y = index / width;
            int x = index % width;

            return new Vector3Int(x, y, z);
        }

        public static int Get3DArrayIndexX(int index, int width, int height)
        {
            int z = index / (width * height);
            index -= (z * width * height);

            return index % width;
        }

        public static int Get3DArrayIndexY(int index, int width, int height)
        {
            int z = index / (width * height);
            index -= (z * width * height);

            return index / width;
        }

        public static int Get3DArrayIndexZ(int index, int width, int height)
        {
            return index / (width * height);
        }


        public static bool Approximately(Quaternion a, Quaternion b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z) && Mathf.Approximately(a.w, b.w);
        }

        public static bool Approximately(Vector4 a, Vector4 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z) && Mathf.Approximately(a.w, b.w);
        }

        public static bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }

        public static bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }

        public static float OppositeFromHypotenuseAndAngle(float hypotenuse, float degrees)
        {
            return hypotenuse * Mathf.Sin(degrees * Mathf.Deg2Rad);
        }

        public static float OppositeFromAdjacentAndAngle(float adjacent, float degrees)
        {
            return adjacent * Mathf.Tan(degrees * Mathf.Deg2Rad);
        }

        public static float AdjacentFromHypotenuseAndAngle(float hypotenuse, float degrees)
        {
            return hypotenuse * Mathf.Cos(degrees * Mathf.Deg2Rad);
        }

        public static float AdjacentFromOppositeAndAngle(float opposite, float degrees)
        {
            return opposite / Mathf.Tan(degrees * Mathf.Deg2Rad);
        }

        public static float HypotenuseFromOppositeAndAdjacent(float opposite, float adjacent)
        {
            return Mathf.Sqrt(adjacent * adjacent + opposite * opposite);
        }

        public static float HypotenuseFromOppositeAndAngle(float opposite, float degrees)
        {
            return opposite / Mathf.Sin(degrees * Mathf.Deg2Rad);
        }

        public static float HypotenuseFromAdjacentAndAngle(float adjacent, float degrees)
        {
            return adjacent / Mathf.Cos(degrees * Mathf.Deg2Rad);
        }

        public static float AngleFromOppositeAndHypotenuse(float opposite, float hypotenuse)
        {
            return Mathf.Asin(opposite / hypotenuse) * Mathf.Rad2Deg;
        }

        public static float AngleFromAdjacentAndHypotenuse(float adjacent, float hypotenuse)
        {
            return Mathf.Acos(adjacent / hypotenuse) * Mathf.Rad2Deg;
        }

        public static float AngleFromOppositeAndAdjacent(float opposite, float adjacent)
        {
            return Mathf.Atan(opposite / adjacent) * Mathf.Rad2Deg;
        }

        public static void GetComponentAngles(Vector3 fromDirection, Vector3 toDirection, out float horizontalAngle, out float verticalAngle)
        {
            if (fromDirection == Vector3.zero || toDirection == Vector3.zero || fromDirection == toDirection)
            {
                horizontalAngle = 0f;
                verticalAngle = 0f;
            }
            else
            {
                Vector3 local = Quaternion.Inverse(Quaternion.LookRotation(fromDirection)) * toDirection;
                horizontalAngle = Mathf.Abs(Mathf.Atan(local.x / local.z) * Mathf.Rad2Deg);
                verticalAngle = Mathf.Abs(Mathf.Atan(local.y / local.WithY(0).magnitude) * Mathf.Rad2Deg);
            }
        }


        public static void GetComponentAnglesSigned(Vector3 fromDirection, Vector3 toDirection, out float horizontalAngle, out float verticalAngle)
        {
            if (fromDirection == Vector3.zero || toDirection == Vector3.zero || fromDirection == toDirection)
            {
                horizontalAngle = 0f;
                verticalAngle = 0f;
            }
            else
            {
                Vector3 local = Quaternion.Inverse(Quaternion.LookRotation(fromDirection)) * toDirection;
                horizontalAngle = Mathf.Atan(local.x / local.z) * Mathf.Rad2Deg;
                verticalAngle = Mathf.Atan(local.y / local.WithY(0).magnitude) * Mathf.Rad2Deg;
            }
        }

        public static float Sin2(float x)
        {
            return 0.5f * (1f - Mathf.Cos(2f * x));
        }

        public static float Cos2(float x)
        {
            return 0.5f * (1f + Mathf.Cos(2f * x));
        }

        public static float Tan2(float x)
        {

            return (1f - Mathf.Cos(2f * x)) / (1f + Mathf.Cos(2f * x));
        }


        public static float SinH(float x)
        {
            return 0.5f * (Mathf.Exp(x) - Mathf.Exp(-x));
        }

        public static float CosH(float x)
        {
            return 0.5f * (Mathf.Exp(x) + Mathf.Exp(-x));
        }

        public static float TanH(float x)
        {
            float exp2x = Mathf.Exp(2f * x);
            return (exp2x - 1f) / (exp2x + 1f);
        }

        public static float Sec(float x)
        {
            return 1f / Mathf.Cos(x);
        }

        public static float Cot(float x)
        {
            return 1f / Mathf.Tan(x);
        }

        public static float Cosec(float x)
        {
            return 1f / Mathf.Sin(x);
        }

        public static float SecH(float x)
        {
            return 1f / CosH(x);
        }

        public static float CotH(float x)
        {
            return 1f / TanH(x);
        }

        public static float CosecH(float x)
        {
            return 1f / SinH(x);
        }

        public static float ASinH(float x)
        {
            return Mathf.Log(x + Mathf.Sqrt(x * x + 1));
        }

        public static float ACosH(float x)
        {
            return Mathf.Log(x + Mathf.Sqrt(x * x - 1));
        }

        public static float ATanH(float x)
        {

            return Mathf.Log((1f / x + 1f) / (1f / x - 1f)) / 2f;
        }

        public static float ASec(float x)
        {
            return Mathf.Log((Mathf.Sqrt(1f - x * x) + 1f) / x);
        }

        public static float ACot(float x)
        {
            return Mathf.Log((x + 1f) / (x - 1f)) / 2f;
        }

        public static float ACosec(float x)
        {
            return Mathf.Log((Mathf.Sign(x) * Mathf.Sqrt(1 + x * x) + 1) / x);
        }

        public static float ASecH(float x)
        {
            return ACosH(1 / x);
        }

        public static float ACotH(float x)
        {
            return ATanH(1 / x);
        }

        public static float ACosecH(float x)
        {
            return ASinH(1 / x);
        }

        public static float MoveTowards(float from, float to, float speed)
        {
            if (to > from)
            {
                return Mathf.Min(to, from + speed);
            }

            return Mathf.Max(to, from - speed);
        }

        public static int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static float ManhattanDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static void SimpleHarmonicMotion(ref Vector3 currentValue, ref Vector3 velocity, Vector3 targetValue, float dampingRatio, float angularFrequency)
        {
            CalculateDampedSpringMotionParameters(dampingRatio, angularFrequency, Time.deltaTime, out float pp, out float pv, out float vp, out float vv);

            float velocityX = velocity.x;
            float stateX = currentValue.x;
            float targetX = targetValue.x;

            SimpleHarmonicMotion(ref stateX, ref velocityX, targetX, pp, pv, vp, vv);

            float velocityY = velocity.y;
            float stateY = currentValue.y;
            float targetY = targetValue.y;

            SimpleHarmonicMotion(ref stateY, ref velocityY, targetY, pp, pv, vp, vv);

            float velocityZ = velocity.z;
            float stateZ = currentValue.z;
            float targetZ = targetValue.z;

            SimpleHarmonicMotion(ref stateZ, ref velocityZ, targetZ, pp, pv, vp, vv);

            velocity = new Vector3(velocityX, velocityY, velocityZ);
            currentValue = new Vector3(stateX, stateY, stateZ);
        }

        public static void SimpleHarmonicMotion(ref Vector2 currentValue, ref Vector2 velocity, Vector2 targetValue, float dampingRatio, float angularFrequency)
        {
            CalculateDampedSpringMotionParameters(dampingRatio, angularFrequency, Time.deltaTime, out float pp, out float pv, out float vp, out float vv);

            float velocityX = velocity.x;
            float stateX = currentValue.x;
            float targetX = targetValue.x;

            SimpleHarmonicMotion(ref stateX, ref velocityX, targetX, pp, pv, vp, vv);

            float velocityY = velocity.y;
            float stateY = currentValue.y;
            float targetY = targetValue.y;

            SimpleHarmonicMotion(ref stateY, ref velocityY, targetY, pp, pv, vp, vv);

            velocity = new Vector2(velocityX, velocityY);
            currentValue = new Vector2(stateX, stateY);
        }

        public static void SimpleHarmonicMotion(ref float currentValue, ref float velocity, float targetValue, float dampingRatio, float angularFrequency)
        {
            CalculateDampedSpringMotionParameters(dampingRatio, angularFrequency, Time.deltaTime, out float pp, out float pv, out float vp, out float vv);
            SimpleHarmonicMotion(ref currentValue, ref velocity, targetValue, pp, pv, vp, vv);
        }

        public static void SimpleHarmonicMotion(ref float currentValue, ref float velocity, float targetValue, float pp, float pv, float vp, float vv)
        {
            float oldPosition = currentValue - targetValue;
            float oldVelocity = velocity;

            currentValue = oldPosition * pp + oldVelocity * pv + targetValue;
            velocity = oldPosition * vp + oldVelocity * vv;
        }

        // Adpapted from https://www.ryanjuckett.com/damped-springs/
        public static void CalculateDampedSpringMotionParameters(float dampingRatio, float angularFrequency, float deltaTime, out float pp, out float pv, out float vp, out float vv)
        {
            const float epsilon = 0.0001f;

            // force values into legal range
            if (dampingRatio < 0.0f) dampingRatio = 0.0f;
            if (angularFrequency < 0.0f) angularFrequency = 0.0f;

            // if there is no angular frequency, the spring will not move and we can
            // return identity
            if (angularFrequency < epsilon)
            {
                pp = 1.0f;
                pv = 0.0f;
                vp = 0.0f;
                vv = 1.0f;
            }
            else if (dampingRatio > 1.0f + epsilon)
            {
                // over-damped
                float za = -angularFrequency * dampingRatio;
                float zb = angularFrequency * Mathf.Sqrt(dampingRatio * dampingRatio - 1.0f);
                float z1 = za - zb;
                float z2 = za + zb;

                float e1 = Mathf.Exp(z1 * deltaTime);
                float e2 = Mathf.Exp(z2 * deltaTime);

                float invTwoZb = 1.0f / (2.0f * zb); // = 1 / (z2 - z1)

                float e1_Over_TwoZb = e1 * invTwoZb;
                float e2_Over_TwoZb = e2 * invTwoZb;

                float z1e1_Over_TwoZb = z1 * e1_Over_TwoZb;
                float z2e2_Over_TwoZb = z2 * e2_Over_TwoZb;

                pp = e1_Over_TwoZb * z2 - z2e2_Over_TwoZb + e2;
                pv = -e1_Over_TwoZb + e2_Over_TwoZb;
                vp = (z1e1_Over_TwoZb - z2e2_Over_TwoZb + e2) * z2;
                vv = -z1e1_Over_TwoZb + z2e2_Over_TwoZb;
            }
            else if (dampingRatio < 1.0f - epsilon)
            {
                // under-damped
                float omegaZeta = angularFrequency * dampingRatio;
                float alpha = angularFrequency * Mathf.Sqrt(1.0f - dampingRatio * dampingRatio);

                float expTerm = Mathf.Exp(-omegaZeta * deltaTime);
                float cosTerm = Mathf.Cos(alpha * deltaTime);
                float sinTerm = Mathf.Sin(alpha * deltaTime);

                float invAlpha = 1.0f / alpha;

                float expSin = expTerm * sinTerm;
                float expCos = expTerm * cosTerm;
                float expOmegaZetaSin_Over_Alpha = expTerm * omegaZeta * sinTerm * invAlpha;


                pp = expCos + expOmegaZetaSin_Over_Alpha;
                pv = expSin * invAlpha;
                vp = -expSin * alpha - omegaZeta * expOmegaZetaSin_Over_Alpha;
                vv = expCos - expOmegaZetaSin_Over_Alpha;

            }
            else
            {
                // critically damped
                float expTerm = Mathf.Exp(-angularFrequency * deltaTime);
                float timeExp = deltaTime * expTerm;
                float timeExpFreq = timeExp * angularFrequency;

                pp = timeExpFreq + expTerm;
                pv = timeExp;
                vp = -angularFrequency * timeExpFreq;
                vv = -timeExpFreq + expTerm;
            }
        }

        public static Quaternion SlerpApproach(Quaternion from, Quaternion to, float speed)
        {
            return Quaternion.Slerp(from, to, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static Color HCLApproach(Color from, Color to, float speed)
        {
            return HCLColour.Lerp(from, to, 1 - Mathf.Exp(-speed * Time.deltaTime)).ToRGB();
        }

        public static Color Approach(Color from, Color to, float speed)
        {
            return Color.Lerp(from, to, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static Vector2 Approach(Vector2 from, Vector2 to, float speed)
        {
            return Vector2.Lerp(from, to, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static Vector3 Approach(Vector3 from, Vector3 to, float speed)
        {
            return Vector3.Lerp(from, to, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static float Approach(float from, float to, float speed)
        {
            return Mathf.Lerp(from, to, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }

        public static Quaternion SlerpApproachUnscaled(Quaternion from, Quaternion to, float speed)
        {
            return Quaternion.Slerp(from, to, 1 - Mathf.Exp(-speed * Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime)));
        }

        public static Color ApproachUnscaled(Color from, Color to, float speed)
        {
            return Color.Lerp(from, to, 1 - Mathf.Exp(-speed * Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime)));
        }

        public static Vector2 ApproachUnscaled(Vector2 from, Vector2 to, float speed)
        {
            return Vector2.Lerp(from, to, 1 - Mathf.Exp(-speed * Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime)));
        }

        public static Vector3 ApproachUnscaled(Vector3 from, Vector3 to, float speed)
        {
            return Vector3.Lerp(from, to, 1 - Mathf.Exp(-speed * Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime)));
        }

        public static float ApproachUnscaled(float from, float to, float speed)
        {
            return Mathf.Lerp(from, to, 1 - Mathf.Exp(-speed * Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime)));
        }

        public static float Map(float value, FloatRange inputRange, FloatRange outputRange)
        {
            return Mathf.Lerp(outputRange.Min, outputRange.Max, Mathf.InverseLerp(inputRange.Min, inputRange.Max, value));
        }

        public static float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax)
        {
            return Mathf.Lerp(outputMin, outputMax, Mathf.InverseLerp(inputMin, inputMax, value));
        }

        public static float MapUnclamped(float value, float inputMin, float inputMax, float outputMin, float outputMax)
        {
            return Mathf.LerpUnclamped(outputMin, outputMax, Mathf.InverseLerp(inputMin, inputMax, value));
        }

        public static float MapUnclamped(float value, FloatRange inputRange, FloatRange outputRange)
        {
            return Mathf.LerpUnclamped(outputRange.Min, outputRange.Max, Mathf.InverseLerp(inputRange.Min, inputRange.Max, value));
        }

        public static float NormalizedSin(float t)
        {
            return (Mathf.Sin(t) + 1) * 0.5f;
        }

        public static float NormalizedCos(float t)
        {
            return (Mathf.Cos(t) + 1) * 0.5f;
        }


        public static float PowerLerp(float a, float b, float t, float midpointValue = 1f)
        {
            float k = Mathf.Log((midpointValue - a) / (b - a), 0.5f);
            return (b - a) * Mathf.Pow(t, k) + a;
        }

        public static float MidpointLerp(float min, float midpoint, float max, float t)
        {
            if (t < 0.5f)
            {
                return Mathf.Lerp(min, midpoint, t * 2f);
            }

            return Mathf.Lerp(midpoint, max, (t - 0.5f) * 2f);
        }

        public static Vector3 ClampVectorToDirection(Vector3 vector, Vector3 clampDirection, float maxAngleDifference)
        {
            if (Vector3.Angle(vector, clampDirection) > maxAngleDifference)
            {
                return (Quaternion.AngleAxis(-maxAngleDifference, Vector3.Cross(vector, clampDirection)) * clampDirection).normalized * vector.magnitude;
            }

            return vector;
        }

        public static Vector3 GetDirectionOnPlane(float angle, Vector3 normal, Vector3 forward)
        {
            return Quaternion.LookRotation(Vector3.forward, Vector3.up) * Quaternion.Euler(0, angle, 0) * Vector3.forward;
        }

        public static Vector3 GetPointOnCircle(Vector3 center, float radius, Vector3 normal, float angle)
        {
            return center + Quaternion.LookRotation(normal) * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radius, Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0);
        }

        public static Vector3 GetPointOnCircle(Vector3 center, float radius, float angle)
        {
            return center + new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radius, Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0);
        }

        public static Vector3 GetPointOnUnitCircle(float angle, Vector3 normal)
        {
            return Quaternion.LookRotation(normal) * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        public static Vector3 GetEulerDirection(Vector3 eulerAngles)
        {
            eulerAngles *= Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(eulerAngles.y) * Mathf.Cos(eulerAngles.x), Mathf.Sin(eulerAngles.y) * Mathf.Cos(eulerAngles.x), Mathf.Sin(eulerAngles.x)).normalized;
        }

        public static Vector3 GetEulerDirection(float eulerX, float eulerY, float eulerZ)
        {
            eulerX *= Mathf.Deg2Rad;
            eulerY *= Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(eulerY) * Mathf.Cos(eulerX), Mathf.Sin(eulerY) * Mathf.Cos(eulerX), Mathf.Sin(eulerX)).normalized;
        }

        public static bool IsValueNear(float value, float target, float tolerance)
        {
            return Mathf.Abs(value - target) <= tolerance;
        }

        public static float GetLogarithmicVolume(float linearVolume)
        {
            return Mathf.Pow(Mathf.Clamp01(linearVolume), 2.017033339299f);
        }

        public static float GetLinearVolume(float logarthmicVolume)
        {
            return Mathf.Pow(Mathf.Clamp01(logarthmicVolume), 0.4957776257419f);
        }

        public static float LinearToDecibel(float linearVolume)
        {
            if (linearVolume != 0)
            {
                return 20f * Mathf.Log10(linearVolume);
            }

            return -144f;
        }

        public static float DecibelToLinear(float decibelVolume)
        {
            return Mathf.Pow(10f, decibelVolume / 20f);
        }

        public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t)
        {
            Quaternion deltaRotation = b * Quaternion.Inverse(a);
            Quaternion swing = Quaternion.identity;
            Quaternion twist = Quaternion.identity;

            Vector3 r = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);

            // singularity: rotation by 180 degree
            if (r.sqrMagnitude < ROUNDING_ERROR_TOLERENCE)
            {
                Vector3 rotatedTwistAxis = deltaRotation * twistAxis;
                Vector3 swingAxis =
                    Vector3.Cross(twistAxis, rotatedTwistAxis);

                if (swingAxis.sqrMagnitude > ROUNDING_ERROR_TOLERENCE)
                {
                    float swingAngle = Vector3.Angle(twistAxis, rotatedTwistAxis);
                    swing = Quaternion.AngleAxis(swingAngle, swingAxis);
                }
                else
                {
                    // more singularity: 
                    // rotation axis parallel to twist axis
                    swing = Quaternion.identity; // no swing
                }

                // always twist 180 degree on singularity
                twist = Quaternion.AngleAxis(180.0f, twistAxis);

            }
            else
            {

                // meat of swing-twist decomposition
                Vector3 p = Vector3.Project(r, twistAxis);
                twist = new Quaternion(p.x, p.y, p.z, deltaRotation.w).Normalized();
                swing = deltaRotation * Quaternion.Inverse(twist);
            }

            swing = Quaternion.Slerp(Quaternion.identity, swing, t);
            twist = Quaternion.Slerp(Quaternion.identity, twist, t);

            return twist * swing;

        }

        /*  public static Quaternion ApproachSingleAxis(Quaternion from, Quaternion to, RotationAxis axis, float speed)
          {
              float GetApproachTarget(float current, float target)
              {
                  float diff = Mathf.Abs(current - target);

                  if (current < target)
                  {
                      if (diff <= 180) return Approach(current, current + diff, speed);
                      return Approach(current, current - diff, speed);
                  }

                  if (diff <= 180) return Approach(current, current - 360f + diff, speed);
                  return Approach(current, current - 360f - diff, speed);
              }

              switch (axis)
              {
                  case RotationAxis.X: return Quaternion.Euler(from.eulerAngles.WithX(GetApproachTarget(from.eulerAngles.x, to.eulerAngles.x)));
                  case RotationAxis.Y: return Quaternion.Euler(from.eulerAngles.WithY(GetApproachTarget(from.eulerAngles.y, to.eulerAngles.y)));
                  case RotationAxis.Z: return Quaternion.Euler(from.eulerAngles.WithZ(GetApproachTarget(from.eulerAngles.z, to.eulerAngles.z)));
              }

              throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
          }
        */

        public static Quaternion ApproachSingleAxis(Quaternion from, Quaternion to, RotationAxis axis, float speed)
        {
            float GetApproachTarget(float current, float target)
            {
                return Mathf.Lerp(current + 360f, target + 360f, 1 - Mathf.Exp(-speed * Time.deltaTime)) - 360f;
            }

            switch (axis)
            {
                case RotationAxis.X: return Quaternion.Euler(from.eulerAngles.WithX(GetApproachTarget(from.eulerAngles.x, to.eulerAngles.x)));
                case RotationAxis.Y: return Quaternion.Euler(from.eulerAngles.WithY(GetApproachTarget(from.eulerAngles.y, to.eulerAngles.y)));
                case RotationAxis.Z: return Quaternion.Euler(from.eulerAngles.WithZ(GetApproachTarget(from.eulerAngles.z, to.eulerAngles.z)));
            }

            throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }

        public static Quaternion LookRotation(Vector3 direction, RotationAxis directionAxis, Vector3 upwards)
        {
            switch (directionAxis)
            {
                case RotationAxis.X: return Quaternion.LookRotation(direction, upwards) * Quaternion.Euler(0f, -90f, 0f);
                case RotationAxis.Y: return Quaternion.LookRotation(direction, upwards) * Quaternion.Euler(-90f, 180f, 0f);
                case RotationAxis.Z: return Quaternion.LookRotation(direction, upwards);
            }

            throw new ArgumentOutOfRangeException(nameof(directionAxis), directionAxis, null);
        }

        public static Quaternion LookRotation(Vector3 direction, RotationAxis directionAxis)
        {
            switch (directionAxis)
            {
                case RotationAxis.X: return Quaternion.LookRotation(direction) * Quaternion.Euler(0f, -90f, 0f);
                case RotationAxis.Y: return Quaternion.LookRotation(direction) * Quaternion.Euler(-90f, 180f, 0f);
                case RotationAxis.Z: return Quaternion.LookRotation(direction);
            }

            throw new ArgumentOutOfRangeException(nameof(directionAxis), directionAxis, null);
        }


        /// <summary>
        /// Wraps a value between the range of 0 and 1.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <returns>The wrapped value</returns>
        public static float Wrap01(float value)
        {
            return value < 0 ? 1 - (value - (float)Math.Truncate(value)) : value - (float)Math.Truncate(value);
        }

        /// <summary>
        /// Wraps a value between the range of 0 and 1.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <returns>The wrapped value</returns>
        public static int Wrap01(int value)
        {
            return value < 0 ? value + 2 * (-value / 2 + 1) % 2 : value % 2;
        }

        /// <summary>
        /// Wraps a value between some range.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="lowerBound">The lower bound</param>
        /// <param name="upperBound">The upper bound</param>
        /// <returns>The wrapped value</returns>
        public static float Wrap(float value, float lowerBound, float upperBound)
        {
            float distance = upperBound - lowerBound;
            float times = (float)Math.Floor((value - lowerBound) / distance);

            return value - (times * distance);
        }

        /// <summary>
        /// Wraps a value between some range. Upper bound is inclusive. 
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="lowerBound">The lower bound</param>
        /// <param name="upperBound">The upper bound</param>
        /// <returns>The wrapped value</returns>
        public static int Wrap(int value, int lowerBound, int upperBound)
        {
            int distance = upperBound - lowerBound + 1;
            if (value < lowerBound) value += distance * ((lowerBound - value) / distance + 1);

            return lowerBound + (value - lowerBound) % distance;

        }

        /// <summary>
        /// Wraps a value between 0 and an exclusive upper bound. 
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="numElements">The upper bound</param>
        /// <returns>The wrapped value</returns>
        public static int WrapIndex(int value, int numElements)
        {
            if (value < 0) value += numElements * (-(value / numElements) + 1);

            return value % numElements;
        }

        /// <summary>
        /// Converts an angle (relative to Vector2.up) to a direction vector.
        /// </summary>
        /// <param name="angle">The angle to convert</param>
        /// <returns>A normalized direction Vector</returns>
        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;
        }

        /// <summary>
        /// Finds the smallest distance between two angles.
        /// </summary>
        /// <param name="angleA">First angle</param>
        /// <param name="angleB">Second angle</param>
        /// <returns>The smallest distance between the angles (singed)</returns>
        public static float AngleDistanceSigned(float angleA, float angleB)
        {
            float angle = angleA - angleB;

            if (angle > 180)
            {
                angle -= 360;
            }
            if (angle < -180)
            {
                angle += 360;
            }

            return -angle;
        }

        public static float RotateAngleTowards(float angle, float target, float maxDelta)
        {
            angle = angle - (float)Math.Floor(angle / 360f) * 360;
            target = target - (float)Math.Floor(target / 360f) * 360;

            float distance = AngleDistanceSigned(angle, target);
            if (Mathf.Abs(distance) <= maxDelta) return target;

            return distance < 0 ? angle - maxDelta : angle + maxDelta;
        }

        public static float ConvertAngleTo180Range(float angle)
        {
            angle = angle - (float)Math.Floor(angle / 360f) * 360;

            return angle > 180 ? angle - 360f : angle;
        }

        public static float ConvertAngleFrom180Range(float angle)
        {
            Assert.IsFalse(angle < -180f || angle > 180f);

            return angle < 0 ? angle + 360f : angle;
        }

        /// <summary>
        /// Finds the smallest distance between two angles.
        /// </summary>
        /// <param name="angleA">First angle</param>
        /// <param name="angleB">Second angle</param>
        /// <returns>The smallest distance between the angles</returns>
        public static float AngleDistance(float angleA, float angleB)
        {
            return Mathf.Abs(AngleDistanceSigned(angleA, angleB));
        }

        /// <summary>
        /// Returns a float that is reduced in value by a specified amount, regardless of the float's sign.
        /// </summary>
        /// <param name="value">The value to reduce</param>
        /// <param name="amount">The amount to reduce by</param>
        /// <returns>The reduced float</returns>
        public static float ReduceLength(float value, float amount)
        {
            return value - (amount * Mathf.Sign(value));
        }

        /// <summary>
        /// Returns a float that is increased in value by a specified amount, regardless of the float's sign.
        /// </summary>
        /// <param name="value">The value to increase</param>
        /// <param name="amount">The amount to increase by</param>
        /// <returns>The increase float</returns>
        public static float IncreaseLength(float value, float amount)
        {
            return value + (amount * Mathf.Sign(value));
        }

        /// <summary>
        /// Rounds a value to the nearest interval.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="interval">The interval</param>
        /// <returns>The interval nearest the value</returns>
        public static float RoundToNearest(float value, float interval)
        {
            return Mathf.Round(value / interval) * interval;
        }

        public static Vector3 RoundToNearest(Vector3 vector, float interval)
        {
            vector.x = Mathf.Round(vector.x / interval) * interval;
            vector.y = Mathf.Round(vector.y / interval) * interval;
            vector.z = Mathf.Round(vector.z / interval) * interval;

            return vector;
        }

        public static Vector2 RoundToNearest(Vector2 vector, float interval)
        {
            vector.x = Mathf.Round(vector.x / interval) * interval;
            vector.y = Mathf.Round(vector.y / interval) * interval;

            return vector;
        }

        /// <summary>
        /// Rounds a value to the nearest interval.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="interval">The interval</param>
        /// <returns>The interval nearest the value</returns>
        public static int RoundToNearest(int value, int interval)
        {
            return Mathf.RoundToInt(Mathf.Round((float)value / interval) * interval);
        }

        public static float FloorToNearest(float value, float interval)
        {
            return Mathf.Floor(value / interval) * interval;
        }

        public static int FloorToNearest(int value, int interval)
        {
            return Mathf.FloorToInt(Mathf.FloorToInt((float)value / interval) * interval);
        }

        public static float CeilToNearest(float value, float interval)
        {
            return Mathf.Ceil(value / interval) * interval;
        }

        public static int CeilToNearest(int value, int interval)
        {
            return Mathf.CeilToInt(Mathf.CeilToInt((float)value / interval) * interval);
        }

        public static Vector3 GetOrbitalPosition(Vector3 focalPoint, float distance, float horizontalAngle, float verticalAngle)
        {
            return focalPoint + Quaternion.Euler(Mathf.Clamp(verticalAngle, -90f, 90f), horizontalAngle, 0) * new Vector3(0, 0, -distance);
        }

        public static float VectorMagnitudeInDirection(Vector3 vector, Vector3 direction)
        {
            return vector.magnitude * Vector3.Dot(direction.normalized, vector.normalized);
        }

        public static float CalculateMaxProjectileRange(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity)
        {
            return CalculateMaxProjectileRange(launchPosition, targetPosition, launchVelocity, Physics.gravity.y);
        }

        public static float CalculateMaxProjectileRange(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravity)
        {
            if (launchPosition.y == targetPosition.y)
            {
                return (launchVelocity * launchVelocity) / Mathf.Abs(gravity);
            }

            float v = launchVelocity;
            float g = Mathf.Abs(gravity);
            float y = launchPosition.y - targetPosition.y;

            float v2 = v * v;
            float gy = 2f * g * y;

            float ta = gy + v2;
            float tb = gy + 2 * v2;
            float theta = Mathf.Acos(Mathf.Sqrt(ta / tb));

            float a = (v * Mathf.Cos(theta)) / g;
            float b = v * Mathf.Sin(theta);
            float c = v2 * Sin2(theta);

            return a * (b + Mathf.Sqrt(c + gy));
        }


        public static float CalculateMaxRangeProjectileAngle(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravity)
        {
            if (launchPosition.y <= targetPosition.y) return 45f;


            float v2 = launchVelocity * launchVelocity;
            float gy = 2f * Mathf.Abs(gravity) * (launchPosition.y - targetPosition.y);

            float ta = gy + v2;
            float tb = gy + 2 * v2;

            return Mathf.Acos(Mathf.Sqrt(ta / tb)) * Mathf.Rad2Deg;
        }

        public static void CalculatePosibleProjectileLaunchAngles(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravity, out float firstSoloution, out float secondSolution)
        {
            Vector3 toTarget = targetPosition - launchPosition;

            float g = Mathf.Abs(gravity);
            float x = toTarget.WithY(0).magnitude;
            float gx = g * x;
            float y = toTarget.y;
            float v2 = launchVelocity * launchVelocity;
            float v4 = v2 * v2;
            float x2 = x * x;
            float sqrt = Mathf.Sqrt(v4 - g * (g * x2 + 2 * y * v2));

            firstSoloution = Mathf.Atan2(v2 + sqrt, gx) * Mathf.Rad2Deg;
            secondSolution = Mathf.Atan2(v2 - sqrt, gx) * Mathf.Rad2Deg;
        }

        public static void CalculatePossibleProjectileLaunchDirections(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravity, out Vector3 firstSoloution, out Vector3 secondSolution)
        {
            CalculatePosibleProjectileLaunchAngles(launchPosition, targetPosition, launchVelocity, gravity, out float angle1, out float angle2);

            Vector3 toTarget = (targetPosition - launchPosition).WithY(0).normalized;
            Vector3 axis = Vector3.Cross(toTarget, Vector3.up);

            firstSoloution = Quaternion.AngleAxis(angle1, axis) * toTarget;
            secondSolution = Quaternion.AngleAxis(angle2, axis) * toTarget;
        }

        public static float CalculateBestProjectileLaunchAngle(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravity)
        {
            float range = (targetPosition - launchPosition).WithY(0).magnitude;
            float maxRange = CalculateMaxProjectileRange(launchPosition, targetPosition, launchVelocity, gravity);

            if (range < maxRange)
            {
                CalculatePosibleProjectileLaunchAngles(launchPosition, targetPosition, launchVelocity, gravity, out float angle1, out float angle2);

                if (float.IsNaN(angle1) && float.IsNaN(angle2))
                {
                    Debug.LogError("Impossible projectile launch!\nlaunchPosition: " + launchPosition + "\ntargetPosition: " + targetPosition + "\nlaunchVelocity: " + launchVelocity + "\ngravity: " + gravity);
                    return 0f;
                }

                if (float.IsNaN(angle1)) return angle2;
                if (float.IsNaN(angle2)) return angle1;

                return Mathf.Min(angle1, angle2);
            }

            return CalculateMaxRangeProjectileAngle(launchPosition, targetPosition, launchVelocity, gravity);
        }

        public static Vector3 CalculateBestProjectileLaunchDirection(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravity)
        {
            Vector3 toTarget = (targetPosition - launchPosition).WithY(0).normalized;
            Vector3 axis = Vector3.Cross(toTarget, Vector3.up);

            return Quaternion.AngleAxis(CalculateBestProjectileLaunchAngle(launchPosition, targetPosition, launchVelocity, gravity), axis) * toTarget;
        }


        public static Vector3 CalculateBestProjectileLaunchDirection(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity)
        {
            return CalculateBestProjectileLaunchDirection(launchPosition, targetPosition, launchVelocity, Physics.gravity.y);
        }

        public static Vector3 CalculateMaxRangeProjectileLaunchDirection(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity)
        {
            return CalculateMaxRangeProjectileLaunchDirection(launchPosition, targetPosition, launchVelocity, Physics.gravity.y);
        }

        public static Vector3 CalculateMaxRangeProjectileLaunchDirection(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravity)
        {
            Vector3 toTarget = (targetPosition - launchPosition).WithY(0).normalized;
            Vector3 axis = Vector3.Cross(toTarget, Vector3.up);
            return Quaternion.AngleAxis(CalculateMaxRangeProjectileAngle(launchPosition, targetPosition, launchVelocity, gravity), axis) * toTarget;
        }


        public static Vector3 CalculateProjectilePositionAtTime(Vector3 launchPosition, Vector3 launchVelocity, float time)
        {
            return launchPosition + (launchVelocity * time) + (0.5f * Physics.gravity * time * time);
        }

        public static Vector3 CalculateProjectilePositionAtTime(Vector3 launchPosition, Vector3 launchVelocity, float time, float gravity)
        {
            return launchPosition + (launchVelocity * time) + (0.5f * new Vector3(0, gravity, 0) * time * time);
        }

        public static Vector3[] CalculateProjectileArc(Vector3 launchPosition, Vector3 launchVelocity, float maxTime)
        {
            return CalculateProjectileArc(launchPosition, launchVelocity, Time.fixedDeltaTime, maxTime, Physics.gravity.y);
        }

        public static Vector3[] CalculateProjectileArc(Vector3 launchPosition, Vector3 launchVelocity, float timeStep, float maxTime)
        {
            return CalculateProjectileArc(launchPosition, launchVelocity, timeStep, maxTime, Physics.gravity.y);
        }

        public static Vector3[] CalculateProjectileArc(Vector3 launchPosition, Vector3 launchVelocity, float timeStep, float maxTime, float gravity)
        {
            List<Vector3> points = new List<Vector3>();
            float t = 0f;
            while (t < maxTime)
            {
                points.Add(launchPosition + (launchVelocity * t) + (0.5f * new Vector3(0, gravity, 0) * t * t));
                t += timeStep;
            }
            return points.ToArray();
        }

        public static Vector3[] CalculateProjectileArc(Vector3 launchPosition, Vector3 targetPosition, Vector3 launchVelocity)
        {
            return CalculateProjectileArc(launchPosition, targetPosition, launchVelocity, Time.fixedDeltaTime, Physics.gravity.y);
        }

        public static Vector3[] CalculateProjectileArc(Vector3 launchPosition, Vector3 targetPosition, Vector3 launchVelocity, float timeStep)
        {
            return CalculateProjectileArc(launchPosition, targetPosition, launchVelocity, timeStep, Physics.gravity.y);
        }

        public static Vector3[] CalculateProjectileArc(Vector3 launchPosition, Vector3 targetPosition, Vector3 launchVelocity, float timeStep, float gravity)
        {
            float targetDistance = launchPosition.To(targetPosition).WithY(0).sqrMagnitude;
            float currentDistance = 0f;
            float t = 0f;

            List<Vector3> points = new List<Vector3>();

            do
            {
                Vector3 point = launchPosition + (launchVelocity * t) + (0.5f * new Vector3(0, gravity, 0) * t * t);
                currentDistance = launchPosition.To(point).WithY(0).sqrMagnitude;

                points.Add(point);
                t += timeStep;

            } while (currentDistance < targetDistance);


            // points[points.Count - 1] = targetPosition;
            return points.ToArray();
        }

        public static Vector3[] CalculateCatenaryPoints(Vector3 start, Vector3 end, float length, int segments)
        {
            List<Vector3> points = new List<Vector3>();
            CalculateCatenaryPoints(start, end, length, segments, points);
            return points.ToArray();
        }

        public static void CalculateCatenaryPoints(Vector3 start, Vector3 end, float length, int segments, List<Vector3> points)
        {
            points.Clear();

            if (length < Vector3.Distance(start, end))
            {
                points.Add(start);
                points.Add(end);
                return;
            }

            Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.FromToRotation((end - start).WithY(0), Vector3.right)) * Matrix4x4.Translate(new Vector3(-start.x, 0, -start.z));
            Matrix4x4 inverse = matrix.inverse;

            Vector3 A = matrix.MultiplyPoint3x4(start);
            Vector3 B = matrix.MultiplyPoint3x4(end);

            float zTarget = Mathf.Sqrt((length * length) - ((B.y - A.y) * (B.y - A.y))) / (B.x - A.x);


            /*  float z = 0.001f;
              while (SinH(z) / z < zTarget)
              {
                  z += 0.001f;
              }
            */

            Func<float, float> f = x => (SinH(x) / x) - zTarget;
            Func<float, float> derivative = x => (CosH(x) / x) - (SinH(x) / (x * x));

            float z = Mathf.Abs(NewtonsMethod(f, derivative, 1f));

            float a = (B.x - A.x) / 2f / z;
            float p = (A.x + B.x - a * Mathf.Log((length + B.y - A.y) / (length - B.y + A.y))) / 2f;
            float q = (B.y + A.y - length * CosH(z) / SinH(z)) / 2f;
            float interval = Mathf.Abs(A.x - B.x) / Mathf.Max(segments, 1);

            for (int i = 0; i < Mathf.Max(segments, 1); i++)
            {
                float x = A.x + (interval * i);
                points.Add(inverse.MultiplyPoint3x4(new Vector3(x, a * CosH((x - p) / a) + q)));
            }

            points.Add(end);
        }

        public static float NewtonsMethod(Func<float, float> function, Func<float, float> derivative, float initialGuess, float tolerance = ROUNDING_ERROR_TOLERENCE)
        {
            float x = initialGuess;

            float h = function(x) / derivative(x);
            while (Mathf.Abs(h) > tolerance)
            {
                h = function(x) / derivative(x);
                x = x - h;
            }

            return x;
        }

        public static float BisectionMethod(Func<float, float> function, float start, float end, float tolerance = ROUNDING_ERROR_TOLERENCE)
        {
            Assert.IsTrue(function(start) * function(end) < 0);

            float avg, interval, sign;

            avg = (start + end) / 2f;
            do
            {
                interval = Mathf.Abs(function(end) - function(start));

                sign = function(start) * function(avg);
                if (sign < 0)
                {
                    end = avg;
                }
                else if (sign > 0)
                {
                    start = avg;
                }
                else
                {
                    if (function(start) == 0) return start;

                    return end;
                }

                avg = (start + end) / 2;

            } while (interval > tolerance);

            return end;
        }


        /// <summary>
        /// Find the line of intersection between two planes.
        /// </summary>
        /// <param name="planeA">The first plane</param>
        /// <param name="planeB">The second plane</param>
        /// <param name="intersectingRay">The line of intersection, if it exists</param>
        /// <returns>True if the planes intersect (they are not parallel)</returns>
        public static bool CheckPlanePlaneIntersection(Plane planeA, Plane planeB, out Ray intersectingRay)
        {

            intersectingRay = new Ray(Vector3.zero, Vector3.up);

            //We can get the direction of the line of intersection of the two planes by calculating the 
            //cross product of the normals of the two planes. Note that this is just a direction and the line
            //is not fixed in space yet. We need a point for that to go with the line vector.
            Vector3 lineVec = Vector3.Cross(planeA.normal, planeB.normal);

            //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
            //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
            //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
            //the cross product of the normal of plane2 and the lineDirection.		
            Vector3 direction = Vector3.Cross(planeB.normal, lineVec);

            float denominator = Vector3.Dot(planeA.normal, direction);

            //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
            if (Mathf.Abs(denominator) > ROUNDING_ERROR_TOLERENCE)
            {

                Vector3 plane1ToPlane2 = planeA.GetOrigin() - planeB.GetOrigin();
                float t = Vector3.Dot(planeA.normal, plane1ToPlane2) / denominator;
                intersectingRay = new Ray(planeB.GetOrigin() + t * direction, direction);

                return true;
            }

            //output not valid
            return false;

        }

        /// <summary>
        /// Check if a ray intersects a plane.
        /// </summary>
        /// <param name="ray">The ray</param>
        /// <param name="plane">The plane</param>
        /// <param name="intersectionPoint">The point at which the ray intersects the plane (if it exists)</param>
        /// <returns>True if the ray intersect the plane</returns>
        public static bool CheckRayPlaneIntersection(Ray ray, Plane plane, out Vector3 intersectionPoint)
        {

            intersectionPoint = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            float dotNumerator = Vector3.Dot((plane.GetOrigin() - ray.origin), plane.normal);
            float dotDenominator = Vector3.Dot(ray.direction, plane.normal);

            //line and plane are not parallel
            if (dotDenominator != 0.0f)
            {

                //create a vector from the linePoint to the intersection point and get the coordinates of the line-plane intersection point
                intersectionPoint = ray.origin + ray.direction.WithMagnitude(dotNumerator / dotDenominator);

                return true;
            }

            //output not valid
            return false;
        }

        /// <summary>
        /// Checks whether two rays intersect. Note that in 3d, two rays do not intersect most of the time. So if the two rays are not in the same plane, use MathUtils.CheckClosestPointsOnTwoRays() instead.
        /// </summary>
        /// <param name="rayA">The first ray</param>
        /// <param name="rayB">The second ray</param>
        /// <param name="intersectionPoint">The point at which the ray intersect (if it exists)</param>
        /// <returns>True if the lines intersect</returns>
        public static bool CheckRayRayIntersection(Ray rayA, Ray rayB, out Vector3 intersectionPoint)
        {
            intersectionPoint = Vector3.zero;

            Vector3 lineVec3 = rayB.origin - rayA.origin;
            Vector3 crossVec1and2 = Vector3.Cross(rayA.direction, rayB.direction);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, rayB.direction);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //Lines are not coplanar. Take into account rounding errors.
            if ((planarFactor >= ROUNDING_ERROR_TOLERENCE) || (planarFactor <= -ROUNDING_ERROR_TOLERENCE))
            {
                return false;
            }

            //Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;

            if ((s >= 0.0f) && (s <= 1.0f))
            {

                intersectionPoint = rayA.origin + (rayA.direction * s);
                return true;
            }


            return false;
        }

        /// <summary>
        /// If two rays are not parallel, there exists a point on each ray that is closest to the other ray. This function finds those points. If the rays intersect, the points will be the same.
        /// </summary>
        /// <param name="rayA">The first ray</param>
        /// <param name="rayB">The second ray</param>
        /// <param name="closestPointOnRayA">The point on ray A that is closest to ray B</param>
        /// <param name="closestPointOnRayB">The point on ray B that is closest to ray A</param>
        /// <returns>True if the rays are not parallel, false if they are</returns>
        public static bool CheckClosestPointsOnTwoRays(Ray rayA, Ray rayB, out Vector3 closestPointOnRayA, out Vector3 closestPointOnRayB)
        {

            closestPointOnRayA = Vector3.zero;
            closestPointOnRayB = Vector3.zero;

            float a = Vector3.Dot(rayA.direction, rayA.direction);
            float b = Vector3.Dot(rayA.direction, rayB.direction);
            float e = Vector3.Dot(rayB.direction, rayB.direction);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = rayA.origin - rayB.origin;
                float c = Vector3.Dot(rayB.direction, r);
                float f = Vector3.Dot(rayB.direction, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointOnRayA = rayA.origin + rayB.direction * s;
                closestPointOnRayB = rayB.origin + rayB.direction * t;

                return true;
            }


            return false;
        }

        /// <summary>
        /// Finds a point which is a projection from a point to a ray.
        /// </summary>
        /// <param name="ray">The ray to project to</param>
        /// <param name="point">The point to project</param>
        /// <returns>The projected point</returns>
        public static Vector3 ProjectPointOnRay(Ray ray, Vector3 point)
        {

            //get vector from point on line to point in space
            Vector3 linePointToPoint = point - ray.origin;

            float t = Vector3.Dot(linePointToPoint, ray.direction);

            return ray.origin + ray.direction * t;
        }

        /// <summary>
        /// Finds a point which is a projection from a point to a line segment. If the projected point lies outside of the line segment, the projected point will be clamped to the appropriate line end.
        /// </summary>
        /// <param name="line">The line to project to</param>
        /// <param name="point">The point to project</param>
        /// <returns>The projected point</returns>
        public static Vector3 ProjectPointOnLineSegment(Line3 line, Vector3 point)
        {

            Vector3 projectedPoint = ProjectPointOnRay(line.ToRay(), point);

            int side = CheckPointRelativeToLineSegment(line, projectedPoint);

            //The projected point is on the line segment
            if (side == 0) return projectedPoint;
            if (side == -1) return line.Start;
            if (side == 1) return line.End;

            //output is invalid
            throw new Exception();
        }

        /// <summary>
        /// Finds the point which is a projection from a point to a plane.
        /// </summary>
        /// <param name="plane">The plane to project to</param>
        /// <param name="point">The point to project</param>
        /// <returns>The projected point</returns>
        public static Vector3 ProjectPointOnPlane(Plane plane, Vector3 point)
        {

            //First calculate the distance from the point to the plane:
            float distance = plane.GetDistanceToPoint(point);

            //Reverse the sign of the distance
            distance *= -1;

            //Translate the point to form a projection
            return point + plane.normal.WithMagnitude(distance);
        }

        /// <summary>
        /// Projects a vector onto a plane.
        /// </summary>
        /// <param name="plane">The plane to project to</param>
        /// <param name="vector">The vector to project</param>
        /// <returns>The projected vector (not normalized)</returns>
        public static Vector3 ProjectVectorOnPlane(Plane plane, Vector3 vector)
        {
            return vector - (Vector3.Dot(vector, plane.normal) * plane.normal);
        }


        /// <summary>
        /// Calculates a signed (normal dot products are ambiguous) dot product between two vectors by using a normal vector as a reference. Usually used to figure out if one vector is the left of right of another.
        /// </summary>
        /// <param name="vectorA">The first vector</param>
        /// <param name="vectorB">The second vector</param>
        /// <param name="normal">A reference normal vector</param>
        /// <returns>The signed dot product. Positive if VectorA is "to the right" of VectorB</returns>
        public static float DotProductSigned(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
        {

            //Use the normal and one of the input vectors to calculate the perpendicular vector
            Vector3 perpVector = Vector3.Cross(normal, vectorA);

            //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
            return Vector3.Dot(perpVector, vectorB);

        }

        /// <summary>
        /// Calculates the signed angle between two vectors by using a third vector as a reference.
        /// </summary>
        /// <param name="vectorA">The first vector</param>
        /// <param name="vectorB">The second vector</param>
        /// <param name="normal">The reference normal vector</param>
        /// <returns>The angle between the vectors. Positive if vector B is "clockwise" to vector A (relative to the normal)</returns>
        public static float VectorAngleSigned(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
        {
            float angle = Mathf.Acos((Vector3.Dot(vectorA.normalized, vectorB.normalized)));
            Vector3 cross = Vector3.Cross(vectorA, vectorB);
            return angle * Mathf.Sign(Vector3.Dot(normal, cross));
        }

        /// <summary>
        /// Calculates the angle between a vector and a plane.
        /// </summary>
        /// <param name="vector">The vector</param>
        /// <param name="plane">The plane</param>
        /// <returns>Tha angle between the vector and the plane. In radians.</returns>
        public static float VectorPlaneAngle(Vector3 vector, Plane plane)
        {

            //calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
            float dot = Vector3.Dot(vector, plane.normal);

            return 1.570796326794897f - Mathf.Acos(dot); //90 degrees - angle
        }

        /// <summary>
        /// Calculates the dot product between two vectors as an angle.
        /// </summary>
        /// <param name="vectorA">The first vector</param>
        /// <param name="vectorB">The second vector</param>
        /// <returns>The radian angle dot product of the two vectors</returns>
        public static float DotProductAngle(Vector3 vectorA, Vector3 vectorB)
        {

            //get the dot product
            double dot = Vector3.Dot(vectorA, vectorB);

            //Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
            if (dot < -1.0f)
            {
                dot = -1.0f;
            }
            if (dot > 1.0f)
            {
                dot = 1.0f;
            }

            //Calculate the angle. The output is in radians
            //This step can be skipped for optimization...
            return (float)Math.Acos(dot);
        }

        /// <summary>
        /// Checks where a point is relative to a line segment. The point is assumed to be on the line, but not necessarily on the line segment. If the point is not on the line, MathUtils.ProjectPointOnLineSegment() can be used.
        /// </summary>
        /// <param name="line">The line segment to check against</param>
        /// <param name="point">The point to check</param>
        /// <returns>0 if the point is on the line segment. -1 if the point is beyond the start of the line. 1 if the point is beyond the end of the line.</returns>
        public static int CheckPointRelativeToLineSegment(Line3 line, Vector3 point)
        {

            Vector3 lineVec = line.End - line.Start;
            Vector3 pointVec = point - line.Start;

            float dot = Vector3.Dot(pointVec, lineVec);

            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0)
            {

                //point is on the line segment
                if (pointVec.magnitude <= lineVec.magnitude)
                {
                    return 0;
                }

                //point is not on the line segment and it is on the side of linePoint2
                return 1;
            }

            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            return -1;
        }

        /// <summary>
        /// Checks whether a point is on a line segment.
        /// </summary>
        /// <param name="line">The line segment</param>
        /// <param name="point">The point</param>
        /// <returns>True if the point lies on the line segment</returns>
        public static bool IsPointOnLineSegment(Line3 line, Vector3 point)
        {
            float fromA = (point - line.Start).magnitude;
            float fromB = (point - line.End).magnitude;

            return Mathf.Abs(line.Length - fromA - fromB) < ROUNDING_ERROR_TOLERENCE;
        }

        /// <summary>
        /// Checks whether a point is on a line segment.
        /// </summary>
        /// <param name="line">The line segment</param>
        /// <param name="point">The point</param>
        /// <returns>True if the point lies on the line segment</returns>
        public static bool IsPointOnLineSegment(Line2 line, Vector2 point)
        {
            float fromA = (point - line.Start).magnitude;
            float fromB = (point - line.End).magnitude;

            return Mathf.Abs(line.Length - fromA - fromB) < ROUNDING_ERROR_TOLERENCE;
        }

        /// <summary>
        /// Checks whether a point is on a ray.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="ray">The ray</param>
        /// <returns>True if the point lies on the ray</returns>
        public static bool IsPointOnRay(Vector3 point, Ray ray)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude < ROUNDING_ERROR_TOLERENCE;
        }

        /// <summary>
        /// Calculates the perpendicular distance from a point to a ray.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="ray">The ray</param>
        /// <returns>The shortest distance to the ray</returns>
        public static float ShortestDistanceFromPointToRay(Vector3 point, Ray ray)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        /// <summary>
        /// This is an extended version of Quaternion.LookRotation that can use custom forward and up vectors.
        /// </summary>
        /// <param name="forward">The forward direction of the rotation (world space)</param>
        /// <param name="up">The upward direction of the rotation (world space)</param>
        /// <param name="worldForward">Custom world forward direction (object space)</param>
        /// <param name="worldUp">Custom world up direction (object space)</param>
        /// <returns>The new look rotation</returns>
        public static Quaternion LookRotationExtended(Vector3 forward, Vector3 up, Vector3 worldForward, Vector3 worldUp)
        {

            //Set the rotation of the destination
            Quaternion rotationA = Quaternion.LookRotation(forward, up);

            //Set the rotation of the custom normal and up vectors. 
            //When using the default LookRotation function, this would be hard coded to the forward and up vector.
            Quaternion rotationB = Quaternion.LookRotation(worldForward, worldUp);

            //Calculate the rotation
            return rotationA * Quaternion.Inverse(rotationB);

        }

        public static int CheckRayCircleIntersection(Ray2D ray, Vector2 center, float radius, out Vector2 firstIntersection, out Vector2 secondIntersection)
        {

            float a, b, c, t, dx, dy;

            Vector2 p1 = ray.origin;
            Vector2 p2 = ray.origin + ray.direction;

            dx = p2.x - p1.x;
            dy = p2.y - p1.y;
            a = dx * dx + dy * dy;
            b = 2 * (dx * (p1.x - center.x) + dy * (p1.y - center.y));
            c = center.x * center.x + center.y * center.y;
            c += p1.x * p1.x + p1.y * p1.y;
            c -= 2 * (center.x * p1.x + center.y * p1.y);
            c -= radius * radius;

            float determinate = b * b - 4 * a * c;

            if (a <= float.Epsilon || determinate < -float.Epsilon)
            {
                firstIntersection = Vector2.zero;
                secondIntersection = Vector2.zero;

                return 0;
            }

            if (determinate < float.Epsilon && determinate > -float.Epsilon)
            {
                t = -b / (2 * a);
                firstIntersection = new Vector2(p1.x + t * dx, p1.y + t * dy);
                secondIntersection = Vector2.zero;

                return 1;
            }

            t = (float)((-b + Math.Sqrt(determinate)) / (2 * a));
            firstIntersection = new Vector2(p1.x + t * dx, p1.y + t * dy);

            t = (float)((-b - Math.Sqrt(determinate)) / (2 * a));
            secondIntersection = new Vector2(p1.x + t * dx, p1.y + t * dy);

            return 2;
        }

        public static int CheckLineCircleIntersection(Line2 line, Vector2 center, float radius, out Vector2 firstIntersection, out Vector2 secondIntersection)
        {
            int numRayIntersections = CheckRayCircleIntersection(line.ToRay(), center, radius, out firstIntersection, out secondIntersection);

            if (numRayIntersections == 1)
            {
                if (!IsPointOnLineSegment(line, firstIntersection))
                {
                    firstIntersection = Vector2.zero;
                    return 0;
                }

                return 1;
            }

            if (numRayIntersections == 2)
            {
                bool firstOn = IsPointOnLineSegment(line, firstIntersection);
                bool secondOn = IsPointOnLineSegment(line, secondIntersection);

                if (firstOn && secondOn) return 2;

                if (!firstOn && !secondOn)
                {
                    firstIntersection = Vector2.zero;
                    secondIntersection = Vector2.zero;
                    return 0;
                }

                if (firstOn && !secondOn)
                {
                    secondIntersection = Vector2.zero;
                    return 1;
                }

                firstIntersection = secondIntersection;
                secondIntersection = Vector2.zero;
                return 1;
            }

            return 0;
        }


        public static bool IsPointInsideCircle(Vector2 point, Vector2 center, float radius)
        {
            return (center - point).sqrMagnitude < radius * radius;
        }


        /// <summary>
        /// Tests whether two 2D lines segements intersect.
        /// </summary>
        /// <param name="lineA">The first line segment</param>
        /// <param name="lineB">The second line segment</param>
        /// <param name="intersectionPoint">The point at which the lines intersect, (if they do not intersect this will be the origin)</param>
        /// <param name="considerCollinearOverlapAsIntersection">Whether or not to consider lines that are collinear and overlapping as intersecting (note that in this case, the intersection point will be returned as 0,0)</param>
        /// <returns>True if the two line segements intersect</returns>
        public static bool LineSegementsIntersect(Line2 lineA, Line2 lineB, out Vector2 intersectionPoint, bool considerCollinearOverlapAsIntersection = false)
        {
            intersectionPoint = new Vector2();

            // lineA = p, lineB = q

            Vector2 r = lineA.ToVector();
            Vector2 s = lineB.ToVector();
            Vector2 pq = lineB.Start - lineA.Start;
            Vector2 qp = lineA.Start - lineB.Start;
            float rxs = r.Cross(s);
            float qpxr = pq.Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (Mathf.Approximately(rxs, 0) && Mathf.Approximately(qpxr, 0))
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersection)
                {

                    if ((0 <= Vector2.Dot(pq, r) && Vector2.Dot(pq, r) <= Vector2.Dot(r, r)) || (0 <= Vector2.Dot(qp, s) && Vector2.Dot(qp, s) <= Vector2.Dot(s, s)))
                    {
                        return true;
                    }
                }

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (Mathf.Approximately(rxs, 0) && !Mathf.Approximately(qpxr, 0))
                return false;

            // t = (q - p) x s / (r x s)
            float t = pq.Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            float u = pq.Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!Mathf.Approximately(rxs, 0) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersectionPoint = lineA.Start + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        public static bool LineSegementsIntersect(Line2 lineA, Line2 lineB, bool considerCollinearOverlapAsIntersection = false)
        {
            return LineSegementsIntersect(lineA, lineB, out Vector2 temp, considerCollinearOverlapAsIntersection);
        }

        public static bool TryFitPlaneToPoints(Vector3[] points, out Plane plane)
        {
            plane = new Plane();

            if (points.Length < 3) return false;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < points.Length; i++)
            {
                sum += points[i];
            }

            Vector3 centroid = sum / points.Length;

            // Calc full 3x3 covariance matrix, excluding symmetries:
            float xx = 0f, xy = 0f, xz = 0f, yy = 0f, yz = 0f, zz = 0f;

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 r = points[i] - centroid;
                xx += r.x * r.x;
                xy += r.x * r.y;
                xz += r.x * r.z;
                yy += r.y * r.y;
                yz += r.y * r.z;
                zz += r.z * r.z;
            }

            float det_x = yy * zz - yz * yz;
            float det_y = xx * zz - xz * xz;
            float det_z = xx * yy - xy * xy;

            float det_max = Mathf.Max(det_x, det_y, det_z);

            if (det_max <= 0.0) return false; // Points are not on a plane

            // Pick path with best conditioning:
            Vector3 normal = Vector3.zero;

            if (det_max == det_x)
            {
                normal = new Vector3(det_x, xz * yz - xy * zz, xy * yz - xz * yy);
            }
            else if (det_max == det_y)
            {
                normal = new Vector3(xz * yz - xy * zz, det_y, xy * xz - yz * xx);
            }
            else
            {
                normal = new Vector3(xy * yz - xz * yy, xy * xz - yz * xx, det_z);
            }

            plane = new Plane(normal.normalized, centroid);
            return true;
        }

    }
}


