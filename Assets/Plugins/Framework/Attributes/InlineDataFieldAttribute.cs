using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class InlineDataFieldAttribute : PropertyAttribute
    {
        public string Label = null;
        public float Width = -1;

        public InlineDataFieldAttribute(string label, float width)
        {
            Label = label;
            Width = width;
        }

        public InlineDataFieldAttribute(float width)
        {
            Width = width;
        }

        public InlineDataFieldAttribute(string label)
        {
            Label = label;
        }
    }
}
