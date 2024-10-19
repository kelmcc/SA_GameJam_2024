using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public static class SpringManager
    {

        // Local Scale

        public static Spring3 SpringLocalScale(this Transform transform, float frequency, float damping, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), transform.localScale, transform.localScale, TransformSpringMode.LocalScale);
        }

        public static Spring3 SpringLocalScale(this Transform transform, float frequency, float damping, Vector3 startScale, Vector3 targetScale, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), startScale, targetScale, TransformSpringMode.LocalScale);
        }

        public static void SpringLocalScale(this Transform transform, Spring3 spring)
        {
            AddTransformSpring(transform, spring, spring.CurrentValue, spring.TargetValue, TransformSpringMode.LocalScale);
        }

        public static void SpringLocalScale(this Transform transform, Spring3 spring, Vector3 startScale, Vector3 targetScale)
        {
            AddTransformSpring(transform, spring, startScale, targetScale, TransformSpringMode.LocalScale);
        }

        // Position

        public static Spring3 SpringPosition(this Transform transform, float frequency, float damping, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), transform.position, transform.position, TransformSpringMode.Position);
        }

        public static Spring3 SpringPosition(this Transform transform, float frequency, float damping, Vector3 startScale, Vector3 targetScale, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), startScale, targetScale, TransformSpringMode.Position);
        }

        public static void SpringPosition(this Transform transform, Spring3 spring)
        {
            AddTransformSpring(transform, spring, spring.CurrentValue, spring.TargetValue, TransformSpringMode.Position);
        }

        public static void SpringPosition(this Transform transform, Spring3 spring, Vector3 startScale, Vector3 targetScale)
        {
            AddTransformSpring(transform, spring, startScale, targetScale, TransformSpringMode.Position);
        }

        // Local Position

        public static Spring3 SpringLocalPosition(this Transform transform, float frequency, float damping, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), transform.localPosition, transform.localPosition, TransformSpringMode.LocalPosition);
        }

        public static Spring3 SpringLocalPosition(this Transform transform, float frequency, float damping, Vector3 startScale, Vector3 targetScale, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), startScale, targetScale, TransformSpringMode.LocalPosition);
        }

        public static void SpringLocalPosition(this Transform transform, Spring3 spring)
        {
            AddTransformSpring(transform, spring, spring.CurrentValue, spring.TargetValue, TransformSpringMode.LocalPosition);
        }

        public static void SpringLocalPosition(this Transform transform, Spring3 spring, Vector3 startScale, Vector3 targetScale)
        {
            AddTransformSpring(transform, spring, startScale, targetScale, TransformSpringMode.LocalPosition);
        }

        // Rotation

        public static Spring3 SpringRotation(this Transform transform, float frequency, float damping, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), transform.rotation.eulerAngles, transform.rotation.eulerAngles, TransformSpringMode.Rotation);
        }

        public static Spring3 SpringRotation(this Transform transform, float frequency, float damping, Vector3 startScale, Vector3 targetScale, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), startScale, targetScale, TransformSpringMode.Rotation);
        }

        public static void SpringRotation(this Transform transform, Spring3 spring)
        {
            AddTransformSpring(transform, spring, spring.CurrentValue, spring.TargetValue, TransformSpringMode.Rotation);
        }

        public static void SpringRotation(this Transform transform, Spring3 spring, Vector3 startScale, Vector3 targetScale)
        {
            AddTransformSpring(transform, spring, startScale, targetScale, TransformSpringMode.Rotation);
        }

        // Local Rotation

        public static Spring3 SpringLocalRotation(this Transform transform, float frequency, float damping, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), transform.localRotation.eulerAngles, transform.localRotation.eulerAngles,
                TransformSpringMode.LocalRotation);
        }

        public static Spring3 SpringLocalRotation(this Transform transform, float frequency, float damping, Vector3 startScale, Vector3 targetScale, bool useUnscaledTime = false)
        {
            return AddTransformSpring(transform, new Spring3(frequency, damping, useUnscaledTime), startScale, targetScale, TransformSpringMode.LocalRotation);
        }

        public static void SpringLocalRotation(this Transform transform, Spring3 spring)
        {
            AddTransformSpring(transform, spring, spring.CurrentValue, spring.TargetValue, TransformSpringMode.LocalRotation);
        }

        public static void SpringLocalRotation(this Transform transform, Spring3 spring, Vector3 startScale, Vector3 targetScale)
        {
            AddTransformSpring(transform, spring, startScale, targetScale, TransformSpringMode.LocalRotation);
        }


        enum TransformSpringMode
        {
            Position,
            LocalPosition,
            Rotation,
            LocalRotation,
            LocalScale
        }

        class TransformSpring
        {
            public Spring3 Spring;
            public Transform Transform;
            public TransformSpringMode Mode;
            public float LastUpdateFrame = -1000;

            public void Apply()
            {
                switch (Mode)
                {
                    case TransformSpringMode.Position:
                        Transform.position = Spring.CurrentValue;
                        break;
                    case TransformSpringMode.LocalPosition:
                        Transform.localPosition = Spring.CurrentValue;
                        break;
                    case TransformSpringMode.Rotation:
                        Transform.rotation = Quaternion.Euler(Spring.CurrentValue);
                        break;
                    case TransformSpringMode.LocalRotation:
                        Transform.localRotation = Quaternion.Euler(Spring.CurrentValue);
                        break;
                    case TransformSpringMode.LocalScale:
                        Transform.localScale = Spring.CurrentValue;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static List<TransformSpring> _transformSprings = new List<TransformSpring>();


        static Spring3 AddTransformSpring(Transform transform, Spring3 spring, Vector3 startValue, Vector3 targetValue, TransformSpringMode mode)
        {
            TransformSpring transformSpring = new TransformSpring { Mode = mode, Transform = transform, Spring = spring };
            spring.CurrentValue = startValue;
            spring.TargetValue = targetValue;
            _transformSprings.Add(transformSpring);

            return spring;
        }





        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
            Runtime.UpdateCallback -= Update;
            Runtime.UpdateCallback += Update;
        }

        private static void Update()
        {
            for (int i = 0; i < _transformSprings.Count; i++)
            {
                if (_transformSprings[i].Transform == null)
                {
                    _transformSprings.RemoveAt(i);
                    i--;
                    continue;
                }

                if (!_transformSprings[i].Transform.gameObject.activeInHierarchy) continue;

                if (Time.frameCount > _transformSprings[i].LastUpdateFrame)
                {
                    _transformSprings[i].Spring.Update();
                    _transformSprings[i].LastUpdateFrame = Time.frameCount;

                }

                _transformSprings[i].Apply();
            }
        }
    }
}
