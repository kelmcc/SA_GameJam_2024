using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Framework
{

    /// <summary>
    /// Grid-like collection that stores elements by X and Y coordinate. Isn't fixed size and doesn't store values that haven't been set. Good for sparsely populated grids, grids of unknown size, and grids where the dimensions might change.
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public class GridCollection<T>
    {
        SortedList<Column<T>> _columns = new SortedList<Column<T>>();

        public void SetValue(int x, int y, T value)
        {
            Column<T> column = GetColumn(x);
            if (column == null)
            {
                column = new Column<T> { Index = x, Rows = new SortedList<Row<T>>() };
                _columns.Add(column);
            }

            Row<T> row = column.GetRow(y);
            if (row != null)
            {
                row.Value = value;
            }
            else
            {
                column.Rows.Add(new Row<T> { Index = y, Value = value });
            }
        }

        public bool RemoveValue(int x, int y)
        {
            Column<T> column = GetColumn(x);
            if (column != null)
            {
                Row<T> row = column.GetRow(y);
                if (row != null)
                {
                    column.Rows.Remove(row);
                    return true;
                }
            }

            return false;
        }

        public T GetValue(int x, int y)
        {
            Column<T> column = GetColumn(x);
            if (column != null)
            {
                Row<T> row = column.GetRow(y);
                if (row != null)
                {
                    return row.Value;
                }
            }

            return default;
        }

        public bool TryGetValue(int x, int y, out T value)
        {
            Column<T> column = GetColumn(x);
            if (column != null)
            {
                Row<T> row = column.GetRow(y);
                if (row != null)
                {
                    value = row.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool HasValue(int x, int y)
        {
            Column<T> column = GetColumn(x);
            if (column != null)
            {
                Row<T> row = column.GetRow(y);
                if (row != null) return true;
            }

            return false;
        }

        private Column<T> GetColumn(int index)
        {
            int min = 0;
            int max = _columns.Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (index == _columns[mid].Index)
                {
                    return _columns[mid];
                }

                if (index < _columns[mid].Index)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            return null;
        }

        private class Column<CT> : IComparable<Column<CT>>
        {
            public int Index;
            public SortedList<Row<CT>> Rows = new SortedList<Row<CT>>();

            public int CompareTo(Column<CT> other) => Index.CompareTo(other.Index);

            public Row<CT> GetRow(int index)
            {
                int min = 0;
                int max = Rows.Count - 1;
                while (min <= max)
                {
                    int mid = (min + max) / 2;
                    if (index == Rows[mid].Index)
                    {
                        return Rows[mid];
                    }

                    if (index < Rows[mid].Index)
                    {
                        max = mid - 1;
                    }
                    else
                    {
                        min = mid + 1;
                    }
                }

                return null;
            }
        }

        private class Row<RT> : IComparable<Row<RT>>
        {
            public int Index;
            public RT Value;

            public int CompareTo(Row<RT> other) => Index.CompareTo(other.Index);
        }
    }

}
