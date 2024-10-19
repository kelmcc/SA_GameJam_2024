using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    [Serializable]
    public struct FloatPair
    {
        public float Start
        {
            get { return First; }
            set { First = value; }
        }

        public float Finish
        {
            get { return Second; }
            set { Second = value; }
        }

        public float First;
        public float Second;

        public float Lerp(float t) => Mathf.Lerp(First, Second, t);
    }

    [Serializable]
    public struct IntPair
    {
        public int Start
        {
            get { return First; }
            set { First = value; }
        }

        public int Finish
        {
            get { return Second; }
            set { Second = value; }
        }

        public int First;
        public int Second;
    }

    [Serializable]
    public struct Vector3Pair
    {
        public Vector3 Start
        {
            get { return First; }
            set { First = value; }
        }

        public Vector3 Finish
        {
            get { return Second; }
            set { Second = value; }
        }

        public Vector3 First;
        public Vector3 Second;

        public Vector3 Lerp(float t) => Vector3.Lerp(First, Second, t);
    }

    [Serializable]
    public struct Vector2Pair
    {
        public Vector2 Start
        {
            get { return First; }
            set { First = value; }
        }

        public Vector2 Finish
        {
            get { return Second; }
            set { Second = value; }
        }

        public Vector2 First;
        public Vector2 Second;

        public Vector2 Lerp(float t) => Vector2.Lerp(First, Second, t);
    }

}
