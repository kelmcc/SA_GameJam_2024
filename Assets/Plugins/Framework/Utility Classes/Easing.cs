using System.Collections;
using UnityEngine;

namespace Framework
{
    //Great easing cheetsheat: http://easings.net/
    public enum EasingType
    {
        Linear,
        ExponentialIn, ExponentialOut, ExponentialInOut, ExponentialOutIn,
        SineIn, SineOut, SineInOut, SineOutIn,
        CubicIn, CubicOut, CubicInOut, CubicOutIn,
        QuinticIn, QuinticOut, QuinticInOut, QuinticOutIn,
        CircularIn, CircularOut, CircularInOut, CircularOutIn,
        ElasticIn, ElasticOut, ElasticInOut, ElasticOutIn,
        QuadraticIn, QuadraticOut, QuadraticInOut, QuadraticOutIn,
        QuarticIn, QuarticOut, QuarticInOut, QuarticOutIn,
        BackIn, BackOut, BackInOut, BackOutIn,
        BounceIn, BounceOut, BounceInOut, BounceOutIn
    }

    /// <summary>
    /// A helper class to ease or 'tween' motion.
    /// </summary>
    public static class Easing
    {
        /// <summary>
        /// Returns the output of an easing function for a specific time.
        /// </summary>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased output</returns>
        public static float Ease(float t, EasingType easingType)
        {
            switch (easingType)
            {
                case EasingType.ExponentialIn: return Functions.ExponentialIn(t);
                case EasingType.ExponentialOut: return Functions.ExponentialOut(t);
                case EasingType.ExponentialInOut: return Functions.ExponentialInOut(t);
                case EasingType.ExponentialOutIn: return Functions.ExponentialOutIn(t);
                case EasingType.SineIn: return Functions.SineIn(t);
                case EasingType.SineOut: return Functions.SineOut(t);
                case EasingType.SineInOut: return Functions.SineInOut(t);
                case EasingType.SineOutIn: return Functions.SineOutIn(t);
                case EasingType.CubicIn: return Functions.CubicIn(t);
                case EasingType.CubicOut: return Functions.CubicOut(t);
                case EasingType.CubicInOut: return Functions.CubicInOut(t);
                case EasingType.CubicOutIn: return Functions.CubicOutIn(t);
                case EasingType.QuinticIn: return Functions.QuinticIn(t);
                case EasingType.QuinticOut: return Functions.QuinticOut(t);
                case EasingType.QuinticInOut: return Functions.QuinticInOut(t);
                case EasingType.QuinticOutIn: return Functions.QuinticOutIn(t);
                case EasingType.CircularIn: return Functions.CircularIn(t);
                case EasingType.CircularOut: return Functions.CircularOut(t);
                case EasingType.CircularInOut: return Functions.CircularInOut(t);
                case EasingType.CircularOutIn: return Functions.CircularOutIn(t);
                case EasingType.ElasticIn: return Functions.ElasticIn(t);
                case EasingType.ElasticOut: return Functions.ElasticOut(t);
                case EasingType.ElasticInOut: return Functions.ElasticInOut(t);
                case EasingType.ElasticOutIn: return Functions.ElasticOutIn(t);
                case EasingType.QuadraticIn: return Functions.QuadraticIn(t);
                case EasingType.QuadraticOut: return Functions.QuadraticOut(t);
                case EasingType.QuadraticInOut: return Functions.QuadraticInOut(t);
                case EasingType.QuadraticOutIn: return Functions.QuadraticOutIn(t);
                case EasingType.QuarticIn: return Functions.QuarticIn(t);
                case EasingType.QuarticOut: return Functions.QuarticOut(t);
                case EasingType.QuarticInOut: return Functions.QuarticInOut(t);
                case EasingType.QuarticOutIn: return Functions.QuarticOutIn(t);
                case EasingType.BackIn: return Functions.BackIn(t);
                case EasingType.BackOut: return Functions.BackOut(t);
                case EasingType.BackInOut: return Functions.BackInOut(t);
                case EasingType.BackOutIn: return Functions.BackOutIn(t);
                case EasingType.BounceIn: return Functions.BounceIn(t);
                case EasingType.BounceOut: return Functions.BounceOut(t);
                case EasingType.BounceInOut: return Functions.BounceInOut(t);
                case EasingType.BounceOutIn: return Functions.BounceOutIn(t);
                default: return Functions.Linear(t);
            }
        }

        /// <summary>
        /// Returns an easing between two values.
        /// </summary>
        /// <param name="from">The starting value</param>
        /// <param name="to">The finishing value</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased value</returns>
        public static float Ease(float from, float to, float t, EasingType easingType)
        {
            return from + (Ease(t, easingType) * (to - from));
        }

        /// <summary>
        /// Returns an easing between two vectors.
        /// </summary>
        /// <param name="from">The starting vector</param>
        /// <param name="to">The finishing vector</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased vector</returns>
        public static Vector3 Ease(Vector3 from, Vector3 to, float t, EasingType easingType)
        {
            return from + (Ease(t, easingType) * (to - from));
        }

        /// <summary>
        /// Returns an easing between two vectors.
        /// </summary>
        /// <param name="from">The starting vector</param>
        /// <param name="to">The finishing vector</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased vector</returns>
        public static Vector2 Ease(Vector2 from, Vector2 to, float t, EasingType easingType)
        {
            return from + (Ease(t, easingType) * (to - from));
        }

        /// <summary>
        /// Returns an easing between two colours.
        /// </summary>
        /// <param name="from">The starting colour</param>
        /// <param name="to">The finishing colour</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased colour</returns>
        public static Color Ease(Color from, Color to, float t, EasingType easingType)
        {
            return Color.Lerp(from, to, Ease(t, easingType));
        }

        /// <summary>
        /// Returns an easing from one value to another and back again.
        /// </summary>
        /// <param name="from">The starting value</param>
        /// <param name="to">The finishing value</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased value</returns>
        public static float EaseThereAndBack(float from, float to, float t, EasingType easingType)
        {
            return t < 0.5 ? from + (Ease(t * 2, easingType) * (to - from)) : from + (Ease(1f - ((t - 0.5f) * 2), easingType) * (to - from));
        }

        /// <summary>
        /// Returns an easing from one value to another and back again.
        /// </summary>
        /// <param name="from">The starting value</param>
        /// <param name="to">The finishing value</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingTypeThere">The easing function to use for the first half of the easing</param>
        /// <param name="easingTypeBack">The easing function to use for the second half of the easing</param>
        /// <returns>The eased value</returns>
        public static float EaseThereAndBack(float from, float to, float t, EasingType easingTypeThere, EasingType easingTypeBack)
        {
            return t < 0.5 ? from + (Ease(t * 2, easingTypeThere) * (to - from)) : from + (Ease(1f - ((t - 0.5f) * 2), easingTypeBack) * (to - from));
        }

        /// <summary>
        /// Returns an easing from one vector to another and back again.
        /// </summary>
        /// <param name="from">The starting vector</param>
        /// <param name="to">The finishing vector</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased value</returns>
        public static Vector3 EaseThereAndBack(Vector3 from, Vector3 to, float t, EasingType easingType)
        {
            return t < 0.5 ? from + (Ease(t * 2, easingType) * (to - from)) : from + (Ease(1f - ((t - 0.5f) * 2), easingType) * (to - from));
        }

        /// <summary>
        /// Returns an easing from one vector to another and back again.
        /// </summary>
        /// <param name="from">The starting vector</param>
        /// <param name="to">The finishing vector</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingTypeThere">The easing function to use for the first half of the easing</param>
        /// <param name="easingTypeBack">The easing function to use for the second half of the easing</param>
        /// <returns>The eased value</returns>
        public static Vector3 EaseThereAndBack(Vector3 from, Vector3 to, float t, EasingType easingTypeThere, EasingType easingTypeBack)
        {
            return t < 0.5 ? from + (Ease(t * 2, easingTypeThere) * (to - from)) : from + (Ease(1f - ((t - 0.5f) * 2), easingTypeBack) * (to - from));
        }

        /// <summary>
        /// Returns an easing from one vector to another and back again.
        /// </summary>
        /// <param name="from">The starting vector</param>
        /// <param name="to">The finishing vector</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingType">The easing function to use</param>
        /// <returns>The eased value</returns>
        public static Vector2 EaseThereAndBack(Vector2 from, Vector2 to, float t, EasingType easingType)
        {
            return t < 0.5 ? from + (Ease(t * 2, easingType) * (to - from)) : from + (Ease(1f - ((t - 0.5f) * 2), easingType) * (to - from));
        }

        /// <summary>
        /// Returns an easing from one vector to another and back again.
        /// </summary>
        /// <param name="from">The starting vector</param>
        /// <param name="to">The finishing vector</param>
        /// <param name="t">The normalized time</param>
        /// <param name="easingTypeThere">The easing function to use for the first half of the easing</param>
        /// <param name="easingTypeBack">The easing function to use for the second half of the easing</param>
        /// <returns>The eased value</returns>
        public static Vector2 EaseThereAndBack(Vector2 from, Vector2 to, float t, EasingType easingTypeThere, EasingType easingTypeBack)
        {
            return t < 0.5 ? from + (Ease(t * 2, easingTypeThere) * (to - from)) : from + (Ease(1f - ((t - 0.5f) * 2), easingTypeBack) * (to - from));
        }


        /// <summary>
        /// A collection of swappable functions that add flavor to motion. Adapted from: http://robertpenner.com/easing/
        /// </summary>
        public static class Functions
        {

            /// <summary>
            /// Easing equation function for a simple linear tweening, with no easing.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float Linear(float t)
            {
                t = Mathf.Clamp01(t);
                return t;
            }

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ExponentialOut(float t)
            {
                t = Mathf.Clamp01(t);
                return (t == 1) ? 1 : (-Mathf.Pow(2, -10 * t) + 1);
            }

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ExponentialIn(float t)
            {
                t = Mathf.Clamp01(t);
                return (t == 0) ? 0 : Mathf.Pow(2, 10 * (t - 1));
            }

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ExponentialInOut(float t)
            {
                t = Mathf.Clamp01(t);
                if (t == 0) return 0;
                if (t == 1) return 1;

                t *= 2;
                if (t < 1)
                {
                    return 0.5f * Mathf.Pow(2, 10 * (t - 1));
                }

                return 0.5f * (-Mathf.Pow(2, -10 * --t) + 2);
            }

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing out/in: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ExponentialOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return ExponentialOut(t * 2) * 0.5f;
                }
                return 0.5f + (ExponentialIn((t - 0.5f) * 2) * 0.5f);
            }


            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CircularOut(float t)
            {
                t = Mathf.Clamp01(t) - 1;
                return Mathf.Sqrt(1 - (t * t));
            }

            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CircularIn(float t)
            {
                t = Mathf.Clamp01(t);
                return -(Mathf.Sqrt(1 - t * t) - 1);
            }

            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CircularInOut(float t)
            {
                t = Mathf.Clamp01(t) * 2;
                if (t < 1)
                    return -0.5f * (Mathf.Sqrt(1 - t * t) - 1);

                return 0.5f * (Mathf.Sqrt(1 - (t -= 2) * t) + 1);
            }

            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CircularOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return CircularOut(t * 2) * 0.5f;
                }
                return 0.5f + (CircularIn((t - 0.5f) * 2) * 0.5f);
            }



            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuadraticOut(float t)
            {
                t = Mathf.Clamp01(t);
                return -t * (t - 2);
            }

            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuadraticIn(float t)
            {
                t = Mathf.Clamp01(t);
                return t * t;
            }

            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuadraticInOut(float t)
            {
                t = Mathf.Clamp01(t) * 2;
                if (t < 1)
                    return 0.5f * t * t;

                return -0.5f * ((--t) * (t - 2) - 1);
            }

            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing out/in: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuadraticOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return QuadraticOut(t * 2) * 0.5f;
                }
                return 0.5f + (QuadraticIn((t * 2) - 1) * 0.5f);
            }


            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float SineOut(float t)
            {
                t = Mathf.Clamp01(t);
                return Mathf.Sin(t * (Mathf.PI / 2));
            }

            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float SineIn(float t)
            {
                t = Mathf.Clamp01(t);
                return -Mathf.Cos(t * (Mathf.PI / 2)) + 1;
            }

            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float SineInOut(float t)
            {
                t = Mathf.Clamp01(t) * 2;
                if (t < 1)
                    return 0.5f * (Mathf.Sin(Mathf.PI * t / 2));

                return -0.5f * (Mathf.Cos(Mathf.PI * --t / 2) - 2);
            }

            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing in/out: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float SineOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return SineOut(t * 2) * 0.5f;
                }
                return 0.5f + (SineIn((t - 0.5f) * 2) * 0.5f);
            }


            /// <summary>
            /// Easing equation function for a cubic (t^3) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CubicOut(float t)
            {
                t = 1f - Mathf.Clamp01(t);
                return 1 - (t * t * t);
            }

            /// <summary>
            /// Easing equation function for a cubic (t^3) easing in:  accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CubicIn(float t)
            {
                t = Mathf.Clamp01(t);
                return t * t * t;
            }

            /// <summary>
            /// Easing equation function for a cubic (t^3) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CubicInOut(float t)
            {
                t = Mathf.Clamp01(t) * 2;
                if (t < 1)
                    return 0.5f * t * t * t;

                return 0.5f * ((t -= 2) * t * t + 2);
            }

            /// <summary>
            /// Easing equation function for a cubic (t^3) easing out/in: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float CubicOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return CubicOut(t * 2) * 0.5f;
                }
                return 0.5f + (CubicIn((t - 0.5f) * 2) * 0.5f);
            }

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuarticOut(float t)
            {
                t = Mathf.Clamp01(t) - 1f;
                return -(t * t * t * t) + 1;
            }

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuarticIn(float t)
            {
                t = Mathf.Clamp01(t);
                return t * t * t * t;
            }

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuarticInOut(float t)
            {
                t = Mathf.Clamp01(t) * 2;
                if (t < 1)
                    return 0.5f * t * t * t * t;

                return -0.5f * ((t -= 2) * t * t * t - 2);
            }

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing out/in: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuarticOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return QuarticOut(t * 2) * 0.5f;
                }
                return 0.5f + (QuarticIn((t - 0.5f) * 2) * 0.5f);
            }



            /// <summary>
            /// Easing equation function for a quintic (t^5) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuinticOut(float t)
            {
                t = Mathf.Clamp01(t) - 1;
                return (t * t * t * t * t) + 1;
            }

            /// <summary>
            /// Easing equation function for a quintic (t^5) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuinticIn(float t)
            {
                t = Mathf.Clamp01(t);
                return t * t * t * t * t;
            }

            /// <summary>
            /// Easing equation function for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuinticInOut(float t)
            {
                t = Mathf.Clamp01(t) * 2;
                if (t < 1)
                    return 0.5f * t * t * t * t * t;
                return 0.5f * ((t -= 2) * t * t * t * t + 2);
            }

            /// <summary>
            /// Easing equation function for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float QuinticOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return QuinticOut(t * 2) * 0.5f;
                }
                return 0.5f + (QuinticIn((t - 0.5f) * 2) * 0.5f);
            }


            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ElasticOut(float t)
            {
                t = Mathf.Clamp01(t);
                if (t == 1) return 1;

                return (Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) / 0.3f) + 1);
            }

            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ElasticIn(float t)
            {
                t = Mathf.Clamp01(t);
                if (t == 1) return 1;

                return -(Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) / 0.3f));
            }

            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ElasticInOut(float t)
            {
                t = Mathf.Clamp01(t);
                if (t == 1) return 1;

                t *= 2;
                if (t < 1) return -0.5f * (Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t - 0.1125f) * (2 * Mathf.PI) / 0.45f));
                return Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t - 0.1125f) * (2 * Mathf.PI) / 0.45f) * 0.5f + 1;
            }

            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float ElasticOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return ElasticOut(t * 2) * 0.5f;
                }
                return 0.5f + (ElasticIn((t - 0.5f) * 2) * 0.5f);
            }


            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BounceOut(float t)
            {
                t = Mathf.Clamp01(t);
                if (t < 0.3636363636f) return (7.5625f * t * t);
                if (t < 0.7272727272f) return (7.5625f * (t -= 0.5454545454f) * t + 0.75f);
                if (t < (2.5 / 2.75)) return (7.5625f * (t -= 0.8181818181f) * t + 0.9375f);
                return (7.5625f * (t -= 0.96363636363f) * t + 0.984375f);
            }

            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BounceIn(float t)
            {
                t = 1 - Mathf.Clamp01(t);
                if (t < 0.3636363636f) return 1 - (7.5625f * t * t);
                if (t < 0.7272727272f) return 1 - (7.5625f * (t -= 0.5454545454f) * t + 0.75f);
                if (t < (2.5 / 2.75)) return 1 - (7.5625f * (t -= 0.8181818181f) * t + 0.9375f);
                return 1 - (7.5625f * (t -= 0.96363636363f) * t + 0.984375f);
            }

            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BounceInOut(float t)
            {
                t = Mathf.Clamp01(t) * 2;
                if (t < 1) return BounceIn(t) * 0.5f;
                return BounceOut(t - 1) * 0.5f + 1 * 0.5f;
            }

            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BounceOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return BounceOut(t * 2) * 0.5f;
                }
                return 0.5f + (BounceIn((t - 0.5f) * 2) * 0.5f);
            }

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BackOut(float t)
            {
                t = Mathf.Clamp01(t) - 1;
                return t * t * ((1.70158f + 1f) * t + 1.70158f) + 1f;
            }

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BackIn(float t)
            {
                t = Mathf.Clamp01(t);
                return t * t * ((1.70158f + 1) * t - 1.70158f);
            }

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BackInOut(float t)
            {
                float s = 1.70158f;

                t = Mathf.Clamp01(t) * 2;
                if (t < 1) return 0.5f * (t * t * (((s *= (1.525f)) + 1) * t - s));
                return 0.5f * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2);
            }

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Normalised time</param>
            /// <returns>The eased value</returns>
            public static float BackOutIn(float t)
            {
                if (t < 0.5f)
                {
                    return BackOut(t * 2) * 0.5f;
                }
                return 0.5f + (BackIn((t - 0.5f) * 2) * 0.5f);
            }

        }



    }
}