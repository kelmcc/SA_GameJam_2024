using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{


    public class WorldSpaceGrid2D<T>
    {

        private T[,] _grid;
        private Vector2Int _origin;
        private Vector2Int _dimensions;

        public WorldSpaceGrid2D(Vector3 worldSpaceOrigin, float width, float depth)
        {
            _origin = new Vector2Int(Mathf.FloorToInt(worldSpaceOrigin.x), Mathf.FloorToInt(worldSpaceOrigin.z));
            _dimensions = new Vector2Int(Mathf.CeilToInt(width), Mathf.CeilToInt(depth));

            _grid = new T[_dimensions.x, _dimensions.y];
        }


        public WorldSpaceGrid2D(IEnumerable<Vector3> positions)
        {
            Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);

            foreach (Vector3 position in positions)
            {
                min.x = Mathf.Min(min.x, Mathf.FloorToInt(position.x));
                min.y = Mathf.Min(min.y, Mathf.FloorToInt(position.z));

                max.x = Mathf.Max(max.x, Mathf.FloorToInt(position.x));
                max.y = Mathf.Max(max.y, Mathf.FloorToInt(position.z));
            }

            _origin = min;
            _dimensions = new Vector2Int(max.x - min.x + 1, max.y - min.y + 1);

            _grid = new T[_dimensions.x, _dimensions.y];
        }


        public void SetValue(Vector3 worldPoint, T value)
        {
            int x = Mathf.FloorToInt(worldPoint.x - _origin.x);
            int y = Mathf.FloorToInt(worldPoint.z - _origin.y);

            _grid[x, y] = value;
        }

        public T GetValue(Vector3 worldPoint)
        {
            int x = Mathf.FloorToInt(worldPoint.x - _origin.x);
            int y = Mathf.FloorToInt(worldPoint.z - _origin.y);

            if (x < 0 || x >= _dimensions.x) return default;
            if (y < 0 || y >= _dimensions.y) return default;

            return _grid[x, y];
        }


    }

}

