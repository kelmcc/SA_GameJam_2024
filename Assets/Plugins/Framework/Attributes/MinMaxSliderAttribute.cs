using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public class MinMaxSliderAttribute : PropertyAttribute
    {

        public float Min => _min;
        public float Max => _max;

        private float _min;
        private float _max;

        public MinMaxSliderAttribute(float min, float max)
        {
            _min = min;
            _max = max;
        }
    }

}
