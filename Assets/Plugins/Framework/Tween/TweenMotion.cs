using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace FrameworkWIP
{
    public abstract class TweenMotion
    {
        public abstract float EvaluateFloat(float startValue, float targetValue, float t);


        public virtual Vector3 EvaluateVector3(Vector3 startValue, Vector3 targetValue, float t)
        {
            return new Vector3(EvaluateFloat(startValue.x, targetValue.x, t), EvaluateFloat(startValue.y, targetValue.y, t), EvaluateFloat(startValue.z, targetValue.z, t));
        }



        public static LinearTweenMotion Linear() => new LinearTweenMotion();

        public static SpringTweenMotion Spring(float frequency = 10f, float dampingSteepness = 0f) => new SpringTweenMotion(frequency, dampingSteepness);
        public static ShakeTweenMotion Shake(float frequency = 10f, float dampingSteepness = 0f) => new ShakeTweenMotion(frequency, dampingSteepness);


        protected float Damping(float t, float steepness)
        {
            if (steepness == 0) return 1f - t;
            if (steepness > 0) return 1f - Mathf.Pow(t, 1f - Mathf.Min(1f, steepness));

            return -Mathf.Pow(t, 1f / (Mathf.Max(-1f, steepness) + 1f)) + 1f;
        }
    }

    public class LinearTweenMotion : TweenMotion
    {
        public override float EvaluateFloat(float startValue, float targetValue, float t)
        {
            return Mathf.LerpUnclamped(startValue, targetValue, t);
        }
    }

    public class ShakeTweenMotion : TweenMotion
    {
        private float _frequency;
        private float _seed;
        private float _dampingSteepness;

        public ShakeTweenMotion(float frequency, float dampingSteepness)
        {
            _frequency = frequency;
            _dampingSteepness = dampingSteepness;
            _seed = Random.value * 1000f;
        }

        public override float EvaluateFloat(float startValue, float targetValue, float t)
        {
            float noise = Noise.GetPerlin(_seed + (t * _frequency));
            return startValue + targetValue * noise * Damping(t, _dampingSteepness);
        }

        public override Vector3 EvaluateVector3(Vector3 startValue, Vector3 targetValue, float t)
        {
            float perlinSeed = _seed + (t * _frequency);
            Vector3 noise = new Vector3(Noise.GetPerlin(perlinSeed), Noise.GetPerlin(perlinSeed + 57387f), Noise.GetPerlin(perlinSeed + 8364f));
            return startValue + new Vector3(noise.x * targetValue.x, noise.y * targetValue.y, noise.z * targetValue.z) * Damping(t, _dampingSteepness);
        }

    }

    public class SpringTweenMotion : TweenMotion
    {
        private float _frequency;
        private float _dampingSteepness;

        public SpringTweenMotion(float frequency, float dampingSteepness)
        {
            _frequency = frequency;
            _dampingSteepness = dampingSteepness;
        }

        public override float EvaluateFloat(float startValue, float targetValue, float t)
        {
            t = Mathf.Sin(t * _frequency) * Damping(t, _dampingSteepness);
            return Mathf.LerpUnclamped(startValue, targetValue, t);
        }
    }

}