using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(GameObjectFloatPair))]
    public class GameObjectFloatPairDrawer : PairDrawer
    {
        protected override string GetFirstFieldName() => "_number";
        protected override string GetSecondFieldName() => "_gameObject";
        protected override float GetFirstFieldWidth() => 60f;
    }
}



