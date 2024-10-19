using UnityEngine;

namespace Framework
{
    public class ScrollingNoise
    {
        private SeedList _seedList;
        private bool _normalize;

        public ScrollingNoise(bool normalized)
        {
            _seedList = new SeedList(1000f);
            _normalize = normalized;
        }

        public float GetValue(float scrollSpeed, float multiplier, int seedIndex = 0)
        {
            if (_normalize)
            {
                return Noise.GetNormalizedPerlin(_seedList[seedIndex] + (Time.time * scrollSpeed)) * multiplier;
            }

            return Noise.GetPerlin(_seedList[seedIndex] + (Time.time * scrollSpeed)) * multiplier;
        }

        public float GetValue(float scrollSpeed, float multiplier)
        {
            return GetValue(scrollSpeed, multiplier, 0);
        }
    }
}
