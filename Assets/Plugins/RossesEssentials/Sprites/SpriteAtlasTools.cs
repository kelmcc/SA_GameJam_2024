using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.U2D;

namespace UnityEditor
{
    public static class SpriteAtlasTools 
    {
        public static Texture2D GetAtlas(Sprite sprite)
        {
            if (sprite.packed)
            {
                return sprite.texture;
            }
            return null;
        }
    }
}