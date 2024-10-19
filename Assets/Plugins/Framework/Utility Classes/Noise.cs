using System.Collections.Generic;
using UnityEngine;

// Based on work by:
// https://github.com/keijiro/PerlinNoise
// http://theinstructionlimit.com/fast-uniform-poisson-disk-sampling-in-c

namespace Framework
{
    public static class Noise
    {
        private const int DEFAULT_POINTS_PER_POISSON_ITERATION = 30;
        private const float Y_SEED_MODIFIER = 463f;
        private const float Z_SEED_MODIFIER = 8363f;



        public static Vector3 GetAnimatedPerlin3D(float seed, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            float x = GetPerlin(seed + time);
            float y = GetPerlin(seed + Y_SEED_MODIFIER + time);
            float z = GetPerlin(seed + Z_SEED_MODIFIER + time);
            return new Vector3(x, y, z) * amplitude;
        }

        public static Vector3 GetAnimatedPerlin3D(float x, float y, float z, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            x = GetPerlin(x + time);
            y = GetPerlin(y + time);
            z = GetPerlin(z + time);
            return new Vector3(x, y, z) * amplitude;
        }

        public static Vector3 GetNormalizedAnimatedPerlin3D(float seed, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            float x = NormalizeOutput(GetPerlin(seed + time));
            float y = NormalizeOutput(GetPerlin(seed + Y_SEED_MODIFIER + time));
            float z = NormalizeOutput(GetPerlin(seed + Z_SEED_MODIFIER + time));
            return new Vector3(x, y, z) * amplitude;
        }

        public static Vector3 GetNormalizedAnimatedPerlin3D(float x, float y, float z, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            x = NormalizeOutput(GetPerlin(x + time));
            y = NormalizeOutput(GetPerlin(y + time));
            z = NormalizeOutput(GetPerlin(z + time));
            return new Vector3(x, y, z) * amplitude;
        }

        public static Vector2 GetAnimatedPerlin2D(float seed, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            float x = GetPerlin(seed + time);
            float y = GetPerlin(seed + Y_SEED_MODIFIER + time);

            return new Vector2(x, y) * amplitude;
        }

        public static Vector2 GetAnimatedPerlin2D(float x, float y, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            x = GetPerlin(x + time);
            y = GetPerlin(y + time);
            return new Vector2(x, y) * amplitude;
        }

        public static Vector2 GetNormalizedAnimatedPerlin2D(float seed, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            float x = NormalizeOutput(GetPerlin(seed + time));
            float y = NormalizeOutput(GetPerlin(seed + Y_SEED_MODIFIER + time));
            return new Vector2(x, y) * amplitude;
        }

        public static Vector2 GetNormalizedAnimatedPerlin2D(float x, float y, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            x = NormalizeOutput(GetPerlin(x + time));
            y = NormalizeOutput(GetPerlin(y + time));
            return new Vector2(x, y) * amplitude;
        }

        public static float GetAnimatedPerlin(float seed, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            return GetPerlin(seed + time) * amplitude;
        }

        public static float GetNormalizedAnimatedPerlin(float seed, float frequency, float amplitude, bool unscaledTime = false)
        {
            float time = (unscaledTime ? Time.unscaledTime : Time.time) * frequency;
            return NormalizeOutput(GetPerlin(seed + time)) * amplitude;
        }



        public static float GetNormalizedPerlin(float x, float y, float z)
        {
            return NormalizeOutput(GetPerlin(x, y, z));
        }

        public static float GetNormalizedPerlin(float x, float y)
        {
            return NormalizeOutput(GetPerlin(x, y));
        }

        public static float GetNormalizedPerlin(float x)
        {
            return NormalizeOutput(GetPerlin(x));
        }

        public static float GetNormalizedPerlin(Vector3 coord)
        {
            return NormalizeOutput(GetPerlin(coord.x, coord.y, coord.z));
        }

        public static float GetNormalizedPerlin(Vector2 coord)
        {
            return NormalizeOutput(GetPerlin(coord.x, coord.y));
        }

        public static float GetPerlin(float x)
        {
            x = NormalizeInput(x);

            int X = Mathf.FloorToInt(x) & 0xff;
            x -= Mathf.Floor(x);
            float u = Fade(x);

            return Lerp(u, Grad(_perm[X], x), Grad(_perm[X + 1], x - 1)) * 2;
        }

        public static float GetPerlin(float x, float y)
        {
            x = NormalizeInput(x);
            y = NormalizeInput(y);

            int X = Mathf.FloorToInt(x) & 0xff;
            int Y = Mathf.FloorToInt(y) & 0xff;

            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);

            float u = Fade(x);
            float v = Fade(y);

            int A = (_perm[X] + Y) & 0xff;
            int B = (_perm[X + 1] + Y) & 0xff;

            return Lerp(v, Lerp(u, Grad(_perm[A], x, y), Grad(_perm[B], x - 1, y)), Lerp(u, Grad(_perm[A + 1], x, y - 1), Grad(_perm[B + 1], x - 1, y - 1)));
        }

        public static float GetPerlin(Vector2 coord)
        {
            return GetPerlin(coord.x, coord.y);
        }

        public static float GetPerlin(float x, float y, float z)
        {
            x = NormalizeInput(x);
            y = NormalizeInput(y);
            z = NormalizeInput(z);

            int X = Mathf.FloorToInt(x) & 0xff;
            int Y = Mathf.FloorToInt(y) & 0xff;
            int Z = Mathf.FloorToInt(z) & 0xff;

            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            z -= Mathf.Floor(z);

            float u = Fade(x);
            float v = Fade(y);
            float w = Fade(z);

            int A = (_perm[X] + Y) & 0xff;
            int B = (_perm[X + 1] + Y) & 0xff;
            int AA = (_perm[A] + Z) & 0xff;
            int BA = (_perm[B] + Z) & 0xff;
            int AB = (_perm[A + 1] + Z) & 0xff;
            int BB = (_perm[B + 1] + Z) & 0xff;

            return Lerp(w, Lerp(v, Lerp(u, Grad(_perm[AA], x, y, z), Grad(_perm[BA], x - 1, y, z)), Lerp(u, Grad(_perm[AB], x, y - 1, z), Grad(_perm[BB], x - 1, y - 1, z))), Lerp(v, Lerp(u, Grad(_perm[AA + 1], x, y, z - 1), Grad(_perm[BA + 1], x - 1, y, z - 1)), Lerp(u, Grad(_perm[AB + 1], x, y - 1, z - 1), Grad(_perm[BB + 1], x - 1, y - 1, z - 1))));
        }

        public static float GetPerlin(Vector3 coord)
        {
            return GetPerlin(coord.x, coord.y, coord.z);
        }

        public static float GetFractalBrownianMotion(float x, int octave)
        {
            float f = 0.0f;
            float w = 0.5f;

            for (int i = 0; i < octave; i++)
            {
                f += w * GetPerlin(x);
                x *= 2.0f;
                w *= 0.5f;
            }

            return f;
        }

        public static float GetFractalBrownianMotion(Vector2 coord, int octave)
        {
            float f = 0.0f;
            float w = 0.5f;

            for (int i = 0; i < octave; i++)
            {
                f += w * GetPerlin(coord);
                coord *= 2.0f;
                w *= 0.5f;
            }

            return f;
        }

        public static float GetFractalBrownianMotion(float x, float y, int octave)
        {
            return GetFractalBrownianMotion(new Vector2(x, y), octave);
        }

        public static float GetFractalBrownianMotion(Vector3 coord, int octave)
        {
            float f = 0.0f;
            float w = 0.5f;

            for (int i = 0; i < octave; i++)
            {
                f += w * GetPerlin(coord);
                coord *= 2.0f;
                w *= 0.5f;
            }

            return f;
        }

        public static float GetFractalBrownianMotion(float x, float y, float z, int octave)
        {
            return GetFractalBrownianMotion(new Vector3(x, y, z), octave);
        }

        private static float NormalizeInput(float f)
        {
            return (f - 0.5f) * 2f;
        }

        private static float NormalizeOutput(float f)
        {
            return (f + 1) * 0.5f;
        }

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        private static float Grad(int hash, float x)
        {
            return (hash & 1) == 0 ? x : -x;
        }

        private static float Grad(int hash, float x, float y)
        {
            return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
        }

        private static float Grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        private static int[] _perm = {
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
            151
        };

        public static List<Vector2> SampleUniformPoissonDiscInCircle(Vector2 center, float radius, float minimumDistance)
        {
            return SampleUniformPoissonDiscInCircle(center, radius, minimumDistance, DEFAULT_POINTS_PER_POISSON_ITERATION);
        }

        public static List<Vector2> SampleUniformPoissonDiscInCircle(Vector2 center, float radius, float minimumDistance, int pointsPerIteration)
        {
            Vector2?[,] pointGrid;
            return UniformPoissonDiskSampler.Sample(center - new Vector2(radius, radius), center + new Vector2(radius, radius), radius, minimumDistance, pointsPerIteration, out pointGrid);
        }

        public static List<Vector2> SampleUniformPoissonDiscInRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance)
        {
            return SampleUniformPoissonDiscInRectangle(topLeft, lowerRight, minimumDistance, DEFAULT_POINTS_PER_POISSON_ITERATION);
        }

        public static List<Vector2> SampleUniformPoissonDiscInRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration)
        {
            Vector2?[,] pointGrid;
            return UniformPoissonDiskSampler.Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration, out pointGrid);
        }

        public static List<Vector2> SampleUniformPoissonDiscInCircle(Vector2 center, float radius, float minimumDistance, out Vector2?[,] pointGrid)
        {
            return SampleUniformPoissonDiscInCircle(center, radius, minimumDistance, DEFAULT_POINTS_PER_POISSON_ITERATION, out pointGrid);
        }

        public static List<Vector2> SampleUniformPoissonDiscInCircle(Vector2 center, float radius, float minimumDistance, int pointsPerIteration, out Vector2?[,] pointGrid)
        {
            return UniformPoissonDiskSampler.Sample(center - new Vector2(radius, radius), center + new Vector2(radius, radius), radius, minimumDistance, pointsPerIteration, out pointGrid);
        }

        public static List<Vector2> SampleUniformPoissonDiscInRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, out Vector2?[,] pointGrid)
        {
            return SampleUniformPoissonDiscInRectangle(topLeft, lowerRight, minimumDistance, DEFAULT_POINTS_PER_POISSON_ITERATION, out pointGrid);
        }

        public static List<Vector2> SampleUniformPoissonDiscInRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration, out Vector2?[,] pointGrid)
        {
            return UniformPoissonDiskSampler.Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration, out pointGrid);
        }


        static class UniformPoissonDiskSampler
        {

            struct Settings
            {
                public Vector2 TopLeft, LowerRight, Center;
                public Vector2 Dimensions;
                public float? RejectionSqDistance;
                public float MinimumDistance;
                public float CellSize;
                public int GridWidth, GridHeight;
            }

            struct State
            {
                public Vector2?[,] Grid;
                public List<Vector2> ActivePoints, Points;
            }

            public static List<Vector2> Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration, out Vector2?[,] pointGrid)
            {
                Settings settings = new Settings
                {
                    TopLeft = topLeft,
                    LowerRight = lowerRight,
                    Dimensions = lowerRight - topLeft,
                    Center = (topLeft + lowerRight) / 2,
                    CellSize = minimumDistance / MathUtils.SQUARE_ROOT_TWO,
                    MinimumDistance = minimumDistance,
                    RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance
                };

                settings.GridWidth = (int)(settings.Dimensions.x / settings.CellSize) + 1;
                settings.GridHeight = (int)(settings.Dimensions.y / settings.CellSize) + 1;

                State state = new State
                {
                    Grid = new Vector2?[settings.GridWidth, settings.GridHeight],
                    ActivePoints = new List<Vector2>(),
                    Points = new List<Vector2>()
                };

                AddFirstPoint(ref settings, ref state);

                while (state.ActivePoints.Count != 0)
                {
                    int listIndex = Random.Range(0, state.ActivePoints.Count);

                    Vector2 point = state.ActivePoints[listIndex];
                    bool found = false;

                    for (int k = 0; k < pointsPerIteration; k++)
                        found |= AddNextPoint(point, ref settings, ref state);

                    if (!found)
                        state.ActivePoints.RemoveAt(listIndex);
                }

                pointGrid = state.Grid;

                return state.Points;
            }

            static void AddFirstPoint(ref Settings settings, ref State state)
            {
                bool added = false;
                while (!added)
                {
                    float d = Random.value;
                    float xr = settings.TopLeft.x + settings.Dimensions.x * d;

                    d = Random.value;
                    float yr = settings.TopLeft.y + settings.Dimensions.y * d;

                    Vector2 p = new Vector2((float)xr, (float)yr);
                    if (settings.RejectionSqDistance != null && (settings.Center - p).sqrMagnitude > settings.RejectionSqDistance)
                        continue;
                    added = true;

                    Vector2 index = Denormalize(p, settings.TopLeft, settings.CellSize);

                    state.Grid[(int)index.x, (int)index.y] = p;

                    state.ActivePoints.Add(p);
                    state.Points.Add(p);
                }
            }

            static bool AddNextPoint(Vector2 point, ref Settings settings, ref State state)
            {
                bool found = false;
                Vector2 q = GenerateRandomAround(point, settings.MinimumDistance);

                if (q.x >= settings.TopLeft.x && q.x < settings.LowerRight.x &&
                    q.y > settings.TopLeft.y && q.y < settings.LowerRight.y &&
                    (settings.RejectionSqDistance == null || (settings.Center - q).sqrMagnitude <= settings.RejectionSqDistance))
                {
                    Vector2 qIndex = Denormalize(q, settings.TopLeft, settings.CellSize);
                    bool tooClose = false;

                    for (int i = (int)Mathf.Max(0, qIndex.x - 2); i < Mathf.Min(settings.GridWidth, qIndex.x + 3) && !tooClose; i++)
                        for (int j = (int)Mathf.Max(0, qIndex.y - 2); j < Mathf.Min(settings.GridHeight, qIndex.y + 3) && !tooClose; j++)
                            if (state.Grid[i, j].HasValue && Vector2.Distance(state.Grid[i, j].Value, q) < settings.MinimumDistance)
                                tooClose = true;

                    if (!tooClose)
                    {
                        found = true;
                        state.ActivePoints.Add(q);
                        state.Points.Add(q);
                        state.Grid[(int)qIndex.x, (int)qIndex.y] = q;
                    }
                }
                return found;
            }

            static Vector2 GenerateRandomAround(Vector2 center, float minimumDistance)
            {
                float d = Random.value;
                float radius = minimumDistance + minimumDistance * d;

                d = Random.value;
                float angle = MathUtils.TWO_PI * d;

                float newX = radius * Mathf.Sin(angle);
                float newY = radius * Mathf.Cos(angle);

                return new Vector2((float)(center.x + newX), (float)(center.y + newY));
            }

            static Vector2 Denormalize(Vector2 point, Vector2 origin, double cellSize)
            {
                return new Vector2((int)((point.x - origin.x) / cellSize), (int)((point.y - origin.y) / cellSize));
            }
        }
    }
}