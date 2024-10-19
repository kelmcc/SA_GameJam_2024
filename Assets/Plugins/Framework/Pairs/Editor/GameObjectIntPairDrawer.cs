using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(GameObjectIntPair))]
    public class GameObjectIntPairDrawer : PairDrawer
    {
        protected override string GetFirstFieldName() => "_number";
        protected override string GetSecondFieldName() => "_gameObject";
        protected override float GetFirstFieldWidth() => 60f;
    }
}



