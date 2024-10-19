using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkWIP
{






    public static class TweenExtensions
    {



        public static Vector3Tween<Transform> TweenPosition(this Transform transform, Vector3 target, float duration, TweenMotion motion)
        {
            return new Vector3Tween<Transform>(transform, target, duration, motion,
                t => t.position,
                (t, v) => t.position = v);
        }

        public static Vector3Tween<Transform> TweenScale(this Transform transform, Vector3 target, float duration, TweenMotion motion)
        {
            return new Vector3Tween<Transform>(transform, target, duration, motion,
                t => t.localScale,
                (t, v) => t.localScale = v);
        }

        public static Vector3Tween<Transform> TweenScale(this Transform transform, float target, float duration, TweenMotion motion)
        {
            return new Vector3Tween<Transform>(transform, target * Vector3.one, duration, motion,
                t => t.localScale,
                (t, v) => t.localScale = v);
        }
    }

}

