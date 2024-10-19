using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(FloatPair))]
    public class FloatPairDrawer : PairDrawer
    {
        protected override string GetFirstFieldName() => "First";
        protected override string GetSecondFieldName() => "Second";
    }

    [CustomPropertyDrawer(typeof(IntPair))]
    public class IntPairDrawer : PairDrawer
    {
        protected override string GetFirstFieldName() => "First";
        protected override string GetSecondFieldName() => "Second";
    }

    [CustomPropertyDrawer(typeof(Vector3Pair))]
    public class Vector3PairDrawer : PairDrawer
    {
        protected override string GetFirstFieldName() => "First";
        protected override string GetSecondFieldName() => "Second";
        protected override float GetMargin() => 15f;
    }

    [CustomPropertyDrawer(typeof(Vector2Pair))]
    public class Vector2PairDrawer : PairDrawer
    {
        protected override string GetFirstFieldName() => "First";
        protected override string GetSecondFieldName() => "Second";
        protected override float GetMargin() => 15f;
    }
}



