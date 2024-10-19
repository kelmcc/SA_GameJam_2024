using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{


    public class WorldSpaceGrid3D<T>
    {

        private T[,,] _grid;
        private Vector3Int _origin;
        private Vector3Int _dimensions;

        public WorldSpaceGrid3D(Vector3 worldSpaceOrigin, Vector3 dimensions)
        {
            _origin = new Vector3Int(Mathf.FloorToInt(worldSpaceOrigin.x), Mathf.FloorToInt(worldSpaceOrigin.y), Mathf.FloorToInt(worldSpaceOrigin.z));
            _dimensions = new Vector3Int(Mathf.CeilToInt(dimensions.x), Mathf.CeilToInt(dimensions.y), Mathf.CeilToInt(dimensions.z));

            _grid = new T[_dimensions.x, _dimensions.y, _dimensions.z];
        }

        public WorldSpaceGrid3D(Bounds bounds)
        {
            _origin = new Vector3Int(Mathf.FloorToInt(bounds.min.x), Mathf.FloorToInt(bounds.min.y), Mathf.FloorToInt(bounds.min.z));
            _dimensions = new Vector3Int(Mathf.CeilToInt(bounds.size.x), Mathf.CeilToInt(bounds.size.y), Mathf.CeilToInt(bounds.size.z));

            _grid = new T[_dimensions.x, _dimensions.y, _dimensions.z];
        }

        public WorldSpaceGrid3D(IEnumerable<Vector3> encapsulatedPoints)
        {
            Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

            foreach (Vector3 position in encapsulatedPoints)
            {
                min.x = Mathf.Min(min.x, Mathf.FloorToInt(position.x));
                min.y = Mathf.Min(min.y, Mathf.FloorToInt(position.y));
                min.z = Mathf.Min(min.z, Mathf.FloorToInt(position.z));

                max.x = Mathf.Max(max.x, Mathf.FloorToInt(position.x));
                max.y = Mathf.Max(max.y, Mathf.FloorToInt(position.y));
                max.z = Mathf.Max(max.z, Mathf.FloorToInt(position.z));
            }

            _origin = min;
            _dimensions = new Vector3Int(max.x - min.x + 1, max.y - min.y + 1, max.z - min.z + 1);

            _grid = new T[_dimensions.x, _dimensions.y, _dimensions.z];
        }

        public void SetValue(Vector3 worldPoint, T value)
        {
            Vector3Int index = GetIndex(worldPoint);

            _grid[index.x, index.y, index.z] = value;
        }

        public T GetValue(Vector3 worldPoint)
        {
            Vector3Int index = GetIndex(worldPoint);

            return _grid[index.x, index.y, index.z];
        }


        Vector3Int GetIndex(Vector3 worldPoint)
        {
            return new Vector3Int(Mathf.FloorToInt(worldPoint.x - _origin.x), Mathf.FloorToInt(worldPoint.y - _origin.y), Mathf.FloorToInt(worldPoint.z - _origin.z));
        }

    }

}

