using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Framework
{
    public partial class TableView<T>
    {

        public class Column
        {
            public GUIContent Header
            {
                get => _state.headerContent;
                set => _state.headerContent = value;
            }

            public TextAlignment HeaderAlignment
            {
                get => _state.headerTextAlignment;
                set => _state.headerTextAlignment = value;
            }

            public TextAlignment SortingArrowAlignment
            {
                get => _state.sortingArrowAlignment;
                set => _state.sortingArrowAlignment = value;
            }

            public bool SortedAscending
            {
                get => _state.sortedAscending;
                set => _state.sortedAscending = value;
            }

            public bool AllowAutoResize
            {
                get => _state.autoResize;
                set => _state.autoResize = value;
            }

            public bool AllowSorting
            {
                get => _state.canSort;
                set => _state.canSort = value;
            }

            public float Width
            {
                get => _state.width;
                set => _state.width = value;
            }

            public float MinWidth
            {
                get => _state.minWidth;
                set => _state.minWidth = value;
            }

            public float MaxWidth
            {
                get => _state.maxWidth;
                set => _state.maxWidth = value;
            }

            public bool IsVisible
            {
                get => _treeView != null ? _treeView.IsColumnVisible(this) : false;
                set
                {
                    if (_treeView != null) _treeView.SetColumnVisible(this, value);
                }
            }

            public DrawCellFunction DrawFunction;
            public CompareFunction CompareFunction;
            public GetValueFunction GetValueFunction;
            public GetPropertyFunction GetPropertyFunction;
            public string NumberFormatting;

            private MultiColumnHeaderState.Column _state;
            private TableTreeView _treeView;


            public Column(GUIContent header)
            {
                _state = GetDefaultState(header);
            }


            public Column(GUIContent header, DrawCellFunction drawCellFunction, CompareFunction compareFunction)
            {
                _state = GetDefaultState(header);
                DrawFunction = drawCellFunction;
                CompareFunction = compareFunction;
            }

            public Column(GUIContent header, GetValueFunction getValueFunction, string numberFormatting = null)
            {
                _state = GetDefaultState(header);
                GetValueFunction = getValueFunction;
                NumberFormatting = numberFormatting;
            }

            public Column(GUIContent header, GetPropertyFunction getPropertyFunction)
            {
                _state = GetDefaultState(header);
                GetPropertyFunction = getPropertyFunction;
            }

            public Column(GUIContent header, GetValueFunction getValueFunction, CompareFunction compareFunction)
            {
                _state = GetDefaultState(header);
                GetValueFunction = getValueFunction;
                CompareFunction = compareFunction;
            }

            public Column(GUIContent header, GetPropertyFunction getPropertyFunction, CompareFunction compareFunction)
            {
                _state = GetDefaultState(header);
                GetPropertyFunction = getPropertyFunction;
                CompareFunction = compareFunction;
            }


            MultiColumnHeaderState.Column GetDefaultState(GUIContent header)
            {
                return new MultiColumnHeaderState.Column
                {
                    headerContent = header,
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = GUI.skin.GetStyle("box").CalcSize(header).x + 5f,
                    minWidth = 60,
                    autoResize = true
                };
            }

            public void OnAddedToTable(TableView<T> table)
            {
                _treeView = table._treeView;
                _treeView.AddColumnState(_state);
            }

            public void OnRemovedFromTable()
            {
                _treeView = null;
            }

        }

        public class Row : TreeViewItem
        {
            public T Item;

            public Row(T item, int index)
            {
                if (item is Object obj)
                {
                    displayName = obj.name;
                }
                else
                {
                    displayName = item.ToString();
                }

                Item = item;
                id = index;
                depth = 0;
            }
        }


        class RowComparer : IComparer<Row>
        {
            private CompareFunction _compareFunction;
            private bool _ascending;

            public RowComparer(CompareFunction compareFunction, bool ascending)
            {
                _compareFunction = compareFunction;
                _ascending = ascending;
            }

            public int Compare(Row x, Row y)
            {
                return _ascending ? _compareFunction(x.Item, y.Item) : -_compareFunction(x.Item, y.Item);
            }
        }

        class RowValueComparer : IComparer<Row>
        {
            private GetValueFunction _getValueFunction;
            private int _order;

            public RowValueComparer(GetValueFunction getValueFunction, bool ascending)
            {
                _getValueFunction = getValueFunction;
                _order = ascending ? 1 : -1;
            }

            public int Compare(Row x, Row y)
            {
                object a = _getValueFunction(x.Item);
                object b = _getValueFunction(y.Item);

                if (a == null && b == null) return 0;
                if (b == null) return _order;
                if (a == null) return -_order;

                if (a is IComparable ica && b is IComparable icb)
                {
                    return ica.CompareTo(icb) * _order;
                }

                Type typeA = a.GetType();
                Type typeB = b.GetType();

                if (typeA.Equals(typeB))
                {
                    return a.ToString().CompareTo(b.ToString()) * _order;
                }

                return typeA.ToString().CompareTo(typeB.ToString()) * _order;
            }
        }

        class RowPropertyComparer : IComparer<Row>
        {
            private GetPropertyFunction _getPropertyFunction;
            private int _order;

            public RowPropertyComparer(GetPropertyFunction getPropertyFunction, bool ascending)
            {
                _getPropertyFunction = getPropertyFunction;
                _order = ascending ? 1 : -1;
            }

            public int Compare(Row x, Row y)
            {
                SerializedProperty propertyA = _getPropertyFunction(x.Item);
                SerializedProperty propertyB = _getPropertyFunction(y.Item);

                object a = propertyA != null ? propertyA.GetValue() : null;
                object b = propertyB != null ? propertyB.GetValue() : null;

                if (a == null && b == null) return 0;
                if (b == null) return _order;
                if (a == null) return -_order;

                if (a is IComparable ica && b is IComparable icb)
                {
                    return ica.CompareTo(icb) * _order;
                }

                Type typeA = a.GetType();
                Type typeB = b.GetType();

                if (typeA.Equals(typeB))
                {
                    return a.ToString().CompareTo(b.ToString()) * _order;
                }

                return typeA.ToString().CompareTo(typeB.ToString()) * _order;
            }
        }

        public delegate void DrawCellFunction(T item, Rect rect);

        public delegate int CompareFunction(T a, T b);

        public delegate object GetValueFunction(T item);

        public delegate SerializedProperty GetPropertyFunction(T item);

        public delegate void RenameCallback(T item, string newName);

        public delegate void SelectionCallback(T[] selectedItems);

        public delegate bool SearchMatchFunction(T item, string search);

        public bool ShowSearchbar
        {
            get => _searchField != null;
            set => _searchField = value ? new SearchField() : null;
        }

        public bool ShowScrollView
        {
            get => _treeView.ShowScrollview;
            set => _treeView.ShowScrollview = value;
        }

        public bool AllowSorting
        {
            get => _treeView.AllowSorting;
            set => _treeView.AllowSorting = value;
        }

        public bool AllowDragging
        {
            get => _treeView.AllowDragging;
            set => _treeView.AllowDragging = value;
        }

        public bool AllowRenaming
        {
            get => _treeView.AllowRenaming;
            set => _treeView.AllowRenaming = value;
        }

        public bool AllowMultiSelect
        {
            get => _treeView.AllowMultiSelect;
            set => _treeView.AllowMultiSelect = value;
        }

        public bool AllowPropertyEditing
        {
            get => _treeView.AllowPropertyEditing;
            set => _treeView.AllowPropertyEditing = value;
        }

        public float RowHeight
        {
            get => _treeView.RowHeight;
            set => _treeView.RowHeight = value;
        }

        public bool ShowAlternatingRowBackgrounds
        {
            get => _treeView.ShowAlternatingRowBackgrounds;
            set => _treeView.ShowAlternatingRowBackgrounds = value;
        }

        public bool ShowBorder
        {
            get => _treeView.ShowBorder;
            set => _treeView.ShowBorder = value;
        }

        public SearchMatchFunction SearchFilter
        {
            get => _treeView.CustomSearchFilter;
            set => _treeView.CustomSearchFilter = value;
        }

        public Func<T, float> CustomRowHeightFunction
        {
            get => _treeView.CustomRowHeightFunction;
            set => _treeView.CustomRowHeightFunction = value;
        }

        public int CurrentRowCount => _treeView.DisplayRowCount;
        public int UnfilteredRowCount => _rows.Count;
        public Column GetPrimaryColumn => _columns.Count > 0 ? _columns[0] : null;

        public void DeselectAllRows() => _treeView.DeselectAll();
        public void SelectAllRows() => _treeView.SelectAll();
        public T[] GetSelectedRows() => _treeView.GetSelectedRows();
        public void SetSelectedRows(T[] items) => _treeView.SetSelectedRows(items);
        public void FrameRow(T item) => _treeView.FrameRow(item);
        public bool IsRowVisibleInScrollView(T item) => _treeView.IsRowVisibleInScrollView(item);
        public T[] GetCurrentRows() => _treeView.GetDisplayRowItems();
        public T GetRow(int index) => _treeView.GetRow(index);
        public Rect GetCellRect(T item, Column column) => _treeView.GetCellRect(item, column);
        public Rect GetRowRect(T item) => _treeView.GetRowRect(item);
        public void Focus() => _treeView.SetFocus();

        public event RenameCallback OnItemRenamed
        {
            add => _treeView.RenameCallback += value;
            remove => _treeView.RenameCallback -= value;
        }

        public event Action<T> OnItemClicked
        {
            add => _treeView.SingleClickCallback += value;
            remove => _treeView.SingleClickCallback -= value;
        }

        public event Action<T> OnItemDoubleClicked
        {
            add => _treeView.DoubleClickCallback += value;
            remove => _treeView.DoubleClickCallback -= value;
        }

        public event Action<T> OnItemRightClicked
        {
            add => _treeView.RightClickCallback += value;
            remove => _treeView.RightClickCallback -= value;
        }

        public event SelectionCallback OnSelectionChanged
        {
            add => _treeView.OnSelectionChanged += value;
            remove => _treeView.OnSelectionChanged -= value;
        }

        public event Action OnSortingChanged
        {
            add => _treeView.OnSortingChanged += value;
            remove => _treeView.OnSortingChanged -= value;
        }

        public event Action OnVisibleColumnsChanged
        {
            add => _treeView.OnVisibleColumnsChanged += value;
            remove => _treeView.OnVisibleColumnsChanged -= value;
        }

        public event Action<string> OnSearchChanged
        {
            add => _treeView.OnSearchChanged += value;
            remove => _treeView.OnSearchChanged -= value;
        }

        public event Action<SerializedProperty> OnPropertyEdited
        {
            add => _treeView.OnPropertyEdited += value;
            remove => _treeView.OnPropertyEdited -= value;
        }

        private TableTreeView _treeView;
        private SearchField _searchField;
        private List<Column> _columns = new List<Column>();
        private List<Row> _rows = new List<Row>();
        private int _nextID = 0;

        private const float SEARCH_FIELD_HEIGHT = 20f;
        private const float MARGIN = 5f;

        public TableView()
        {
            _treeView = new TableTreeView(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[] { new MultiColumnHeaderState.Column() })), _rows, _columns);
            _searchField = new SearchField();

        }



        public void Draw(Rect rect)
        {
            if (_searchField != null)
            {
                _treeView.searchString = _searchField.OnGUI(new Rect(rect.x, rect.y, rect.width, SEARCH_FIELD_HEIGHT), _treeView.searchString);
                _treeView.OnGUI(new Rect(rect.x, rect.y + SEARCH_FIELD_HEIGHT + MARGIN, rect.width, rect.height - SEARCH_FIELD_HEIGHT - MARGIN));
            }
            else
            {
                _treeView.OnGUI(rect);
            }

        }

        public void Draw(Rect rect, Rect searchBarRect)
        {
            if (_searchField != null)
            {
                _treeView.searchString = _searchField.OnGUI(searchBarRect, _treeView.searchString);
                _treeView.OnGUI(rect);
            }
            else
            {
                _treeView.OnGUI(rect);
            }
        }

        public void AddRow(T item)
        {
            _rows.Add(new Row(item, _nextID));
            _nextID++;
            _treeView.OnRowsChanged();
        }

        public bool RemoveRow(T item)
        {

            for (int i = 0; i < _rows.Count; i++)
            {
                if (_rows[i].Item.Equals(item))
                {
                    _rows.RemoveAt(i);
                    _treeView.OnRowsChanged();
                    return true;
                }
            }

            return false;
        }



        public Column AddColumn(Column column)
        {
            _columns.Add(column);
            column.OnAddedToTable(this);
            _treeView.OnColumnsChanged();

            return column;
        }

        public bool RemoveColumn(Column column)
        {
            int index = _columns.IndexOf(column);

            if (index >= 0)
            {
                column.OnRemovedFromTable();
                _columns.RemoveAt(index);
                _treeView.RemoveColumnState(index);

                _treeView.OnColumnsChanged();

                return true;
            }

            return false;
        }

        public Column[] GetColumns()
        {
            return _columns.ToArray();
        }

        public Column GetColumn(int index)
        {
            return _columns[index];
        }

        public Column GetColumn(string headerText)
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                if (_columns[i].Header.text == headerText) return _columns[i];
            }

            return null;
        }


        public Rect GetColumnRect(Column column)
        {
            int index = _treeView.multiColumnHeader.state.visibleColumns.IndexOf(_columns.IndexOf(column));

            if (index >= 0)
            {
                return _treeView.multiColumnHeader.GetColumnRect(index);
            }

            return new Rect();
        }

        public Column[] GetVisibleColumns()
        {
            List<Column> visibleColumns = new List<Column>();
            for (int i = 0; i < _columns.Count; i++)
            {
                if (_treeView.multiColumnHeader.IsColumnVisible(i))
                {
                    visibleColumns.Add(_columns[i]);
                }
            }

            return visibleColumns.ToArray();
        }

        public T[] GetUnfilteredRows()
        {
            List<T> items = new List<T>();
            for (int i = 0; i < _rows.Count; i++)
            {
                items.Add(_rows[i].Item);
            }

            return items.ToArray();
        }

        public Column[] GetSortingOrder()
        {
            List<Column> columns = new List<Column>();

            for (int i = 0; i < _treeView.multiColumnHeader.state.sortedColumns.Length; i++)
            {
                columns.Add(_columns[_treeView.multiColumnHeader.state.sortedColumns[i]]);
            }

            return columns.ToArray();
        }

        public void SetSortingOrder(IList<Column> columns)
        {
            int[] indices = new int[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                indices[i] = _columns.IndexOf(columns[i]);
            }

            _treeView.multiColumnHeader.state.sortedColumns = indices;
        }

        public void SetSortingOrder(Column column)
        {
            _treeView.multiColumnHeader.state.sortedColumns = new[]
            {
                _columns.IndexOf(column)
            };
        }

        public void ResizeColumnsToFit()
        {
            _treeView.multiColumnHeader.ResizeToFit();
        }

    }


}



