using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    //Amazing hex grid reference : http://www.redblobgames.com/grids/hexagons
    /// <summary>
    /// Data structure to store generic cells that can be accessed by hex grid coordinates.
    /// </summary>
    /// <typeparam name="T">The cell type to store</typeparam>
    public class HexGrid<T> : IEnumerable<T> where T : class
    {
        /// <summary>
        /// The number of columns in the grid.
        /// </summary>
        public int Width => _width - 1;

        /// <summary>
        /// The number of rows in the grid.
        /// </summary>
        public int Height => _height;

        private int _width;
        private int _height;
        private HexCoordinate _gridOffset;

        //We store cells with odd-r offset coordinates
        private T[,] _cells;

        /// <summary>
        /// Creates a new new HexGrid at a specfic position.
        /// </summary>
        /// <param name="width">The number of columns</param>
        /// <param name="height">The number of rows</param>
        /// <param name="position">The center of the grid in cartesian space</param>>
        /// <param name="hexDimensions">Hex cell dimensions, x = width in world units, y = squash factor eg. 0.66 means hex height is 66% what it would be for a cell of that width</param>
        public HexGrid(int width, int height, Vector2 position, Vector2 hexDimensions)
        {
            _width = width + 1;
            _height = height;

            _gridOffset = HexCoordinate.FromCartesian(position - new Vector2(hexDimensions.x * _width * 0.5f, hexDimensions.y * _height * 0.5f));
            _cells = new T[_width, _height];
        }


        /// <summary>
        /// Creates a new new HexGrid.
        /// </summary>
        /// <param name="width">The number of columns</param>
        /// <param name="height">The number of rows</param>
        public HexGrid(int width, int height)
        {
            _width = width + 1;
            _height = height;

            _gridOffset = HexCoordinate.FromCartesian(-new Vector2(_width * 0.5f, 0.66f * _height * 0.5f));
            _cells = new T[_width, _height];
        }

        /// <summary>
        /// Clear all the cells in the grid.
        /// </summary>
        public void ClearGrid()
        {
            _cells = new T[_width, _height];
        }

        /// <summary>
        /// Finds all the stored cells neighbouring a particular coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate to check</param>
        /// <returns>A list of cells neighbouring the coordinate, starting east and going in a clockwise direction</returns>
        public List<T> GetNeighbours(HexCoordinate coordinate)
        {
            Assert.IsTrue(IsOnGrid(coordinate), "Invalid coordinate " + coordinate);

            List<T> neighbours = new List<T>(6);

            T cell = GetCell(coordinate + HexCoordinate.East);
            if (cell != null) neighbours.Add(cell);

            cell = GetCell(coordinate + HexCoordinate.SouthEast);
            if (cell != null) neighbours.Add(cell);

            cell = GetCell(coordinate + HexCoordinate.SouthWest);
            if (cell != null) neighbours.Add(cell);

            cell = GetCell(coordinate + HexCoordinate.West);
            if (cell != null) neighbours.Add(cell);

            cell = GetCell(coordinate + HexCoordinate.NorthWest);
            if (cell != null) neighbours.Add(cell);

            cell = GetCell(coordinate + HexCoordinate.NorthEast);
            if (cell != null) neighbours.Add(cell);

            return neighbours;
        }

        /// <summary>
        /// Checks whether there is a cell stored at a specific coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate to check</param>
        /// <returns>True if there is no cell stored at the coordinate</returns>
        public bool IsCellEmpty(HexCoordinate coordinate)
        {
            Assert.IsTrue(IsOnGrid(coordinate), "Invalid coordinate " + coordinate);
            return _cells[GetColumn(coordinate), GetRow(coordinate)] == null;
        }

        /// <summary>
        /// Stores a cell at a coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate to store at</param>
        /// <param name="cell">The cell to store</param>
        public T SetCell(HexCoordinate coordinate, T cell)
        {
            Assert.IsTrue(IsOnGrid(coordinate), "Invalid coordinate " + coordinate);
            _cells[GetColumn(coordinate), GetRow(coordinate)] = cell;
            return cell;
        }

        /// <summary>
        /// Clears the cell at a specific coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate to clear</param>
        public void ClearCell(HexCoordinate coordinate)
        {
            Assert.IsTrue(IsOnGrid(coordinate), "Invalid coordinate " + coordinate);
            _cells[GetColumn(coordinate), GetRow(coordinate)] = null;
        }

        /// <summary>
        /// Finds all the stored cells that are within a radius of a coordinate.
        /// </summary>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The number to cells outwards from the center to check</param>
        /// <returns>A list of cells stored around the center coordinate within the radius</returns>
        public List<T> GetCellsInRadius(HexCoordinate center, int radius)
        {
            Assert.IsTrue(IsOnGrid(center), "Invalid coordinate " + center);
            Assert.IsTrue(radius >= 0);

            List<HexCoordinate> coordinates = center.GetCoordinatesInRadius(radius);
            List<T> cells = new List<T>(coordinates.Count);

            for (int i = 0; i < coordinates.Count; i++)
            {
                T cell = GetCell(coordinates[i]);
                if (cell != null)
                {
                    cells.Add(cell);
                }
            }

            return cells;
        }

        /// <summary>
        /// Gets the cell stored at a particular coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate to check</param>
        /// <returns>The cell at the coordinate if it exists, otherwise null</returns>
        public T GetCell(HexCoordinate coordinate)
        {
            int column = GetColumn(coordinate);
            int row = GetRow(coordinate);

            if (CheckValidOffset(column, row))
            {
                return _cells[column, row];
            }
            return null;
        }

        /// <summary>
        /// Checks whether a coordinate is a valid coordinate on this grid.
        /// </summary>
        /// <param name="coordinate">The coordinate to check</param>
        /// <returns>True if the coorinate is within the grid's bounds</returns>
        public bool IsOnGrid(HexCoordinate coordinate)
        {
            return CheckValidOffset(GetColumn(coordinate), GetRow(coordinate));
        }

        /// <summary>
        /// Gets an enumerator that enumerates through all the stored cells in the grid.
        /// </summary>
        /// <returns>The stored cell enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    if (i < _width - (j & 1) && j < _height)
                    {
                        if (_cells[i, j] != null)
                        {
                            yield return _cells[i, j];
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool CheckValidOffset(int column, int row)
        {
            if (row < _height && row >= 0)
            {
                if (column < _width - (row & 1) && column >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        int GetRow(HexCoordinate coordinate)
        {
            return coordinate.Z - _gridOffset.Z;
        }

        int GetColumn(HexCoordinate coordinate)
        {
            return (coordinate.X - _gridOffset.X) + ((coordinate.Z - _gridOffset.Z) - ((coordinate.Z - _gridOffset.Z) & 1)) / 2;
        }
    }

    /// <summary>
    /// A cubic coordinate representing a single cell in a 2D hex grid, pointy side up only.
    /// </summary>
    public struct HexCoordinate : IEquatable<HexCoordinate>
    {

        /// <summary>
        /// The directions that neighbouring cells can be in, starting east and moving in a clockwise direction.
        /// </summary>
        public enum Direction
        {
            East,
            SouthEast,
            SouthWest,
            West,
            NorthWest,
            NorthEast
        }

        /// <summary>
        /// The identity coordinate in the south west direction.
        /// </summary>
        public static HexCoordinate SouthWest => new HexCoordinate(0, 1, -1);

        /// <summary>
        /// The identity coordinate in the westerly direction.
        /// </summary>
        public static HexCoordinate West => new HexCoordinate(-1, 1, 0);

        /// <summary>
        /// The identity coordinate in the north west direction.
        /// </summary>
        public static HexCoordinate NorthWest => new HexCoordinate(-1, 0, 1);

        /// <summary>
        /// The identity coordinate in the north east direction.
        /// </summary>
        public static HexCoordinate NorthEast => new HexCoordinate(0, -1, 1);

        /// <summary>
        /// The identity coordinate in the easterly direction.
        /// </summary>
        public static HexCoordinate East => new HexCoordinate(1, -1, 0);

        /// <summary>
        /// The identity coordinate in the south east direction.
        /// </summary>
        public static HexCoordinate SouthEast => new HexCoordinate(1, 0, -1);

        /// <summary>
        /// The origin coordinate.
        /// </summary>
        public static HexCoordinate Zero => new HexCoordinate(0, 0, 0);

        /// <summary>
        /// The cubic position on the X axis.
        /// </summary>
        public int X => _x;

        /// <summary>
        /// The cubic position on the Y axis.
        /// </summary>
        public int Y => _y;

        /// <summary>
        /// The cubic position on the Z axis.
        /// </summary>
        public int Z => _z;

        private int _x;
        private int _y;
        private int _z;

        //This is needed becuase of the relationship between the height and width of a hex cell.
        private const float SIZE_FACTOR = 0.5773502691896f;

        /// <summary>
        /// Creates a new hex coordinate representing a cell at a specific cubic position.
        /// </summary>
        /// <param name="x">The cubic position on the X axis</param>
        /// <param name="y">The cubic position on the Y axis</param>
        /// <param name="z">The cubic position on the Z axis</param>
        public HexCoordinate(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Creates a new hex coordinate at the same position as an existing one.
        /// </summary>
        /// <param name="coordinate">The coordinate to duplicate</param>
        public HexCoordinate(HexCoordinate coordinate)
        {
            _x = coordinate._x;
            _y = coordinate._y;
            _z = coordinate._z;
        }

        public static Vector2 SnapToHex(Vector2 cartesianPosition)
        {
            return FromCartesian(cartesianPosition).GetCartesian();
        }

        /// <summary>
        /// Returns the center of this hex cell in cartesian (world) space based on the dimensions of the hex cells in the grid.
        /// </summary>
        /// <param name="hexDimensions">Hex cell dimensions, x = width in world units, y = squash factor eg. 0.66 means hex height is 66% what it would be for a cell of that width</param>
        /// <returns>The center of the cell in world space</returns>
        public Vector2 GetCartesian(Vector2 hexDimensions)
        {
            int column = _x + (_z - (_z & 1)) / 2;
            int row = _z;
            return new Vector2(Mathf.Sqrt(3f) * (float)(column + 0.5 * (row & 1)) * hexDimensions.x, 3f / 2f * row * hexDimensions.y) * SIZE_FACTOR;
        }

        /// <summary>
        /// Returns the center of this hex cell in cartesian (world) space based on the default dimensions of hex cells (1, 0.66f).
        /// </summary>
        /// <param name="hexDimensions">Hex cell dimensions, x = width in world units, y = squash factor eg. 0.66 means hex height is 66% what it would be for a cell of that width</param>
        /// <returns>The center of the cell in world space</returns>
        public Vector2 GetCartesian()
        {
            return GetCartesian(new Vector2(1, 0.66f));
        }

        /// <summary>
        /// Finds the hex coordinate that is nearest to a world space position based on the dimensions of the hex cells in the grid.
        /// </summary>
        /// <param name="position">2D point in cartesian (world) space</param>
        /// <param name="hexDimensions">Hex cell dimensions, x = width in world units, y = squash factor eg. 0.66 means hex height is 66% what it would be for a cell of that width</param>
        /// <returns>The nearest hex coordinate to the position</returns>
        public static HexCoordinate FromCartesian(Vector2 position, Vector2 hexDimensions)
        {

            float x = ((position.x / hexDimensions.x) * Mathf.Sqrt(3) / 3 - (position.y / 3 / hexDimensions.y)) / SIZE_FACTOR;
            float z = position.y * 2f / 3f / SIZE_FACTOR / hexDimensions.y;
            float y = -x - z;

            float rx = Mathf.Round(x);
            float ry = Mathf.Round(y);
            float rz = Mathf.Round(z);

            if (Mathf.Abs(rx - x) > Mathf.Abs(ry - y) && Mathf.Abs(rx - x) > Mathf.Abs(rz - z))
            {
                rx = -ry - rz;
            }
            else if (Mathf.Abs(ry - y) > Mathf.Abs(rz - z))
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new HexCoordinate((int)rx, (int)ry, (int)rz);
        }

        /// <summary>
        /// Finds the hex coordinate that is nearest to a world space position based on the default dimensions of hex cells (1, 0.66f).
        /// </summary>
        /// <param name="position">2D point in cartesian (world) space</param>
        /// <returns>The nearest hex coordinate to the position</returns>
        public static HexCoordinate FromCartesian(Vector2 position)
        {
            return FromCartesian(position, new Vector2(1, 0.66f));
        }

        /// <summary>
        /// Finds the hex coordinates neighbouring this one.
        /// </summary>
        /// <returns>An array of the six neighbouring coordinates, starting at the east and moving in a clockwise fashion</returns>
        public HexCoordinate[] GetNeighbours()
        {
            HexCoordinate[] neighbours = new HexCoordinate[6];

            neighbours[0] = this + East;
            neighbours[1] = this + SouthEast;
            neighbours[2] = this + SouthWest;
            neighbours[3] = this + West;
            neighbours[4] = this + NorthWest;
            neighbours[5] = this + NorthEast;

            return neighbours;
        }

        /// <summary>
        /// Finds the neighbouring coordinate in a specific direction.
        /// </summary>
        /// <param name="direction">The direction to check</param>
        /// <returns>The neighbouring coordinate in the direction</returns>
        public HexCoordinate GetCoordinateInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.East: return this + East;
                case Direction.West: return this + West;
                case Direction.NorthEast: return this + NorthEast;
                case Direction.NorthWest: return this + NorthWest;
                case Direction.SouthEast: return this + SouthEast;
                case Direction.SouthWest: return this + SouthWest;
            }

            throw new UnityException("Direction does not exist? What is life?");
        }

        /// <summary>
        /// Finds all the hex coordinates within a radius from this one.
        /// </summary>
        /// <param name="radius">The radius to check (number of cells)</param>
        /// <returns>A list of the hex coordinates within the radius, including this one</returns>
        public List<HexCoordinate> GetCoordinatesInRadius(int radius)
        {
            Assert.IsTrue(radius >= 0);

            List<HexCoordinate> results = new List<HexCoordinate>(6 * radius);
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = Mathf.Max(-radius, -dx - radius); dy <= Mathf.Min(radius, -dx + radius); dy++)
                {
                    results.Add(this + new HexCoordinate(dx, dy, -dx - dy));
                }
            }

            return results;
        }

        /// <summary>
        /// Finds the distance between this hex coordinate and another one.
        /// </summary>
        /// <param name="coordinate">The coordinate to check</param>
        /// <returns>The distance to the point (number of cells)</returns>
        public int DistanceTo(HexCoordinate coordinate)
        {
            return Mathf.Max(Mathf.Abs(_x - coordinate._x), Mathf.Abs(_y - coordinate._y), Mathf.Abs(_z - coordinate._z));
        }

        /// <summary>
        /// Returns a coordinate that is this coordinate, but rotated around another coordinate by a certain number of rotations in the clickwise direction.
        /// </summary>
        /// <param name="point">The coordinate to rotate around</param>
        /// <param name="rotations">The number of rotations to perform (6 rotations = 360 degree rotation for a hex cell)</param>
        /// <returns></returns>
        public HexCoordinate RotatedAround(HexCoordinate point, int rotations)
        {
            HexCoordinate coordinate = this - point;

            for (int i = 0; i < Mathf.Abs(rotations); i++)
            {
                if (rotations > 0)
                {
                    coordinate = new HexCoordinate(-coordinate.Y, -coordinate.Z, -coordinate.X);
                }

                if (rotations < 0)
                {
                    coordinate = new HexCoordinate(-coordinate.Z, -coordinate.X, -coordinate.Y);
                }
            }

            return coordinate + point;
        }


        public override string ToString()
        {
            return "(" + _x + ", " + _y + ", " + _z + ")";
        }

        public static bool operator ==(HexCoordinate a, HexCoordinate b)
        {
            return a._x == b._x && a._y == b._y && a._z == b._z;
        }

        public static bool operator !=(HexCoordinate a, HexCoordinate b)
        {
            return a._x != b._x || a._y != b._y || a._z != b._z;
        }

        public static HexCoordinate operator +(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a._x + b._x, a._y + b._y, a._z + b._z);
        }

        public static HexCoordinate operator -(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a._x - b._x, a._y - b._y, a._z - b._z);
        }

        public static HexCoordinate operator *(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a._x * b._x, a._y * b._y, a._z * b._z);
        }

        public static HexCoordinate operator /(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a._x / b._x, a._y / b._y, a._z / b._z);
        }

        public static HexCoordinate operator *(HexCoordinate a, int b)
        {
            return new HexCoordinate(a._x * b, a._y * b, a._z * b);
        }

        public static HexCoordinate operator /(HexCoordinate a, int b)
        {
            return new HexCoordinate(a._x / b, a._y / b, a._z / b);
        }

        public bool Equals(HexCoordinate other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public override bool Equals(object obj)
        {
            if (obj is HexCoordinate)
            {
                HexCoordinate other = (HexCoordinate)obj;
                return _x == other._x && _y == other._y && _z == other._z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ _x.GetHashCode();
                hash = hash * 16777619 ^ _y.GetHashCode();
                hash = hash * 16777619 ^ _z.GetHashCode();
                return hash;
            }
        }


    }
}