using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class TableView<T>
    {

        public class TableTreeView : TreeView
        {
            public bool ShowScrollview { get => useScrollView; set => useScrollView = value; }
            public float RowHeight { get => rowHeight; set => rowHeight = value; }
            public bool ShowAlternatingRowBackgrounds { get => showAlternatingRowBackgrounds; set => showAlternatingRowBackgrounds = value; }
            public bool ShowBorder { get => showBorder; set => showBorder = value; }
            public bool AllowSorting { get => multiColumnHeader.canSort; set => multiColumnHeader.canSort = value; }
            public int DisplayRowCount { get { EnsureInitialized(); return _displayRows.Count; } }


            public bool AllowDragging = true;
            public bool AllowRenaming = true;
            public bool AllowMultiSelect = true;
            public bool AllowPropertyEditing = true;
            public SearchMatchFunction CustomSearchFilter;
            public Func<T, float> CustomRowHeightFunction;
            public event RenameCallback RenameCallback;
            public event Action<T> DoubleClickCallback;
            public event Action<T> SingleClickCallback;
            public event Action<T> RightClickCallback;
            public event SelectionCallback OnSelectionChanged;
            public event Action OnVisibleColumnsChanged;
            public event Action OnSortingChanged;
            public event Action<string> OnSearchChanged;
            public event Action<SerializedProperty> OnPropertyEdited;

            private List<Column> _columns;
            private List<Row> _rows;
            private List<Row> _displayRows;
            private List<MultiColumnHeaderState.Column> _columnStates = new List<MultiColumnHeaderState.Column>();
            private bool _hasDrawn;
            private MethodInfo _defaultPropertyFieldMethod;
            private Dictionary<int, bool> _columnHasHeader;


            public TableTreeView(TreeViewState state, MultiColumnHeader header, List<Row> rows, List<Column> columns) : base(state, header)
            {
                _rows = rows;
                _columns = columns;

                rowHeight = 20f;
                showAlternatingRowBackgrounds = true;
                showBorder = true;
                useScrollView = true;

                customFoldoutYOffset = (20f - EditorGUIUtility.singleLineHeight) * 0.5f;
                extraSpaceBeforeIconAndLabel = 18f;

                header.sortingChanged += (columnHeader) =>
                {
                    Reload();
                    OnSortingChanged?.Invoke();
                };

                header.visibleColumnsChanged += (columnHeader) => { OnVisibleColumnsChanged?.Invoke(); };
            }

            public void SetData() { }

            void EnsureInitialized()
            {
                if (_displayRows == null)
                {
                    Reload();
                    Repaint();
                }

                if (_defaultPropertyFieldMethod == null)
                {
                    _defaultPropertyFieldMethod = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.NonPublic | BindingFlags.Static);
                }
            }



            public void OnColumnsChanged()
            {

                for (int i = 0; i < _columnStates.Count; i++)
                {
                    _columnStates[i].allowToggleVisibility = i != 0;
                }

                multiColumnHeader.state = new MultiColumnHeaderState(_columnStates.ToArray());

                _displayRows = null;
                _hasDrawn = false;

                Repaint();
            }

            public void OnRowsChanged()
            {
                _displayRows = null;
                _hasDrawn = false;
                Repaint();
            }

            public void AddColumnState(MultiColumnHeaderState.Column state)
            {
                _columnStates.Add(state);
            }

            public void RemoveColumnState(int index)
            {
                _columnStates.RemoveAt(index);
            }

            protected override TreeViewItem BuildRoot()
            {
                return new TreeViewItem { id = 0, depth = -1, displayName = "Root" }; ;
            }


            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {

                if (hasSearch)
                {
                    Filter();
                }
                else if (_displayRows == null)
                {
                    _displayRows = new List<Row>(_rows);
                }

                Sort();

                TreeViewItem[] items = _displayRows.Cast<TreeViewItem>().ToArray();
                SetupParentsAndChildrenFromDepths(root, items);

                return items;
            }

            private Rect bigrect;

            public override void OnGUI(Rect rect)
            {
                EnsureInitialized();
                _columnHasHeader = new Dictionary<int, bool>();

                bigrect = rect;



                base.OnGUI(rect);

                _hasDrawn = true;
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                {
                    Column column = _columns[args.GetColumn(i)];

                    Rect rect = args.GetCellRect(i);
                    CenterRectUsingSingleLineHeight(ref rect);

                    if (!IsCellColumnVisibleInScrollView(rect)) continue;

                    if (column.DrawFunction != null)
                    {
                        column.DrawFunction(_displayRows[args.row].Item, rect);
                    }
                    else if (column.GetPropertyFunction != null)
                    {
                        SerializedProperty property = column.GetPropertyFunction(_displayRows[args.row].Item);

                        if (property != null && property.serializedObject != null)
                        {
                            EditorGUI.BeginDisabledGroup(!AllowPropertyEditing);

                            EditorGUI.BeginChangeCheck();

                            if (!_columnHasHeader.TryGetValue(i, out bool hasHeader))
                            {
                                hasHeader = property.GetAttribute<HeaderAttribute>() != null;
                                _columnHasHeader.Add(i, hasHeader);
                            }

                            if (hasHeader)
                            {
                                _defaultPropertyFieldMethod.Invoke(null, new object[] { rect, property, GUIContent.none });
                            }
                            else
                            {
                                EditorGUI.PropertyField(rect, property, GUIContent.none);
                            }



                            if (EditorGUI.EndChangeCheck())
                            {
                                property.serializedObject.ApplyModifiedProperties();
                                OnPropertyEdited?.Invoke(property);
                            }

                            EditorGUI.EndDisabledGroup();
                        }

                    }
                    else if (column.GetValueFunction != null)
                    {
                        object value = column.GetValueFunction(_displayRows[args.row].Item);

                        if (value != null)
                        {
                            if (column.NumberFormatting == null)
                            {
                                GUI.Label(rect, value.ToString());
                            }
                            else if (value is float floatValue)
                            {
                                GUI.Label(rect, floatValue.ToString(column.NumberFormatting));
                            }
                            else if (value is int intValue)
                            {
                                GUI.Label(rect, intValue.ToString(column.NumberFormatting));
                            }
                            else if (value is double doubleValue)
                            {
                                GUI.Label(rect, doubleValue.ToString(column.NumberFormatting));
                            }
                            else if (value is short shortValue)
                            {
                                GUI.Label(rect, shortValue.ToString(column.NumberFormatting));
                            }
                            else if (value is long longValue)
                            {
                                GUI.Label(rect, longValue.ToString(column.NumberFormatting));
                            }
                            else if (value is byte byteValue)
                            {
                                GUI.Label(rect, byteValue.ToString(column.NumberFormatting));
                            }
                            else
                            {
                                GUI.Label(rect, value.ToString());
                            }
                        }

                    }
                }
            }



            void Filter()
            {
                _displayRows = new List<Row>();

                for (int i = 0; i < _rows.Count; i++)
                {
                    if (CustomSearchFilter != null)
                    {
                        if (CustomSearchFilter(_rows[i].Item, searchString))
                        {
                            _displayRows.Add(_rows[i]);
                        }
                    }
                    else
                    {
                        string name = _columns[0].GetValueFunction(_rows[i].Item).ToString();
                        if (name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            _displayRows.Add(_rows[i]);
                        }
                    }
                }

            }

            void Sort()
            {

                if (!AllowSorting) return;
                if (_displayRows.Count <= 1) return;
                if (multiColumnHeader.sortedColumnIndex == -1) return;
                if (multiColumnHeader.state.sortedColumns.Length == 0) return;

                T[] selectedItems = null;
                if (_hasDrawn)
                {
                    selectedItems = GetSelectedRows();
                }

                int[] sortedColumns = multiColumnHeader.state.sortedColumns;
                List<IComparer<Row>> comparers = new List<IComparer<Row>>();

                for (int i = 0; i < sortedColumns.Length; i++)
                {
                    Column column = _columns[sortedColumns[i]];

                    if (column.CompareFunction != null)
                    {
                        comparers.Add(new RowComparer(column.CompareFunction, multiColumnHeader.IsSortedAscending(sortedColumns[i])));
                    }
                    else if (column.GetPropertyFunction != null)
                    {
                        comparers.Add(new RowPropertyComparer(column.GetPropertyFunction, multiColumnHeader.IsSortedAscending(sortedColumns[i])));
                    }
                    else if (column.GetValueFunction != null)
                    {
                        comparers.Add(new RowValueComparer(column.GetValueFunction, multiColumnHeader.IsSortedAscending(sortedColumns[i])));
                    }
                }

                _displayRows.SortByMultipleCriteria(comparers);

                if (selectedItems != null)
                {
                    SetSelectedRows(selectedItems);
                }
            }

            protected override float GetCustomRowHeight(int row, TreeViewItem item)
            {
                if (CustomRowHeightFunction != null)
                {
                    return CustomRowHeightFunction((item as Row).Item);
                }

                return base.GetCustomRowHeight(row, item);
            }

            protected override IList<int> GetAncestors(int id)
            {
                return new int[0];
            }

            protected override IList<int> GetDescendantsThatHaveChildren(int id)
            {
                return new int[0];
            }

            protected override void SelectionChanged(IList<int> selectedIds)
            {
                base.SelectionChanged(selectedIds);

                OnSelectionChanged?.Invoke(GetSelectedRows());
            }

            protected override bool CanRename(TreeViewItem item)
            {
                return AllowRenaming && GetRenameRect(treeViewRect, 0, item).width > 30;
            }

            protected override void RenameEnded(RenameEndedArgs args)
            {

                if (args.acceptedRename)
                {
                    Row row = FindRowByID(args.itemID);
                    row.displayName = args.newName;

                    if (RenameCallback != null)
                    {
                        RenameCallback(row.Item, args.newName);
                    }
                    else if (row.Item is Object obj)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        if (!string.IsNullOrEmpty(path))
                        {
                            AssetDatabase.RenameAsset(path, args.newName);
                            AssetDatabase.Refresh();
                        }
                        else
                        {
                            obj.name = args.newName;
                            EditorUtility.SetDirty(obj);
                        }


                    }

                    Reload();
                }
            }

            protected override void SingleClickedItem(int id)
            {
                base.SingleClickedItem(id);

                SingleClickCallback?.Invoke(FindItemByID(id));
            }


            protected override void DoubleClickedItem(int id)
            {
                base.DoubleClickedItem(id);

                T item = FindItemByID(id);

                if (DoubleClickCallback != null)
                {
                    DoubleClickCallback(item);
                }
                else if (item is Object obj)
                {
                    Selection.activeObject = obj;
                }
            }

            protected override void ContextClickedItem(int id)
            {
                base.ContextClickedItem(id);

                RightClickCallback?.Invoke(FindItemByID(id));
            }

            protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
            {
                Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
                CenterRectUsingSingleLineHeight(ref cellRect);

                cellRect.xMin -= 30f;

                return base.GetRenameRect(cellRect, row, item);
            }

            protected override bool CanMultiSelect(TreeViewItem item)
            {
                return AllowMultiSelect;
            }

            protected override bool CanStartDrag(CanStartDragArgs args)
            {
                return AllowDragging && !hasSearch;
            }

            protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
            {
                if (hasSearch) return;

                DragAndDrop.PrepareStartDrag();

                List<Row> draggedRows = FindRowsByIDs(args.draggedItemIDs);

                DragAndDrop.SetGenericData("GenericDragColumnDragging", draggedRows);

                if (typeof(Object).IsAssignableFrom(typeof(T)))
                {
                    Object[] objects = new Object[draggedRows.Count];

                    for (int i = 0; i < draggedRows.Count; i++)
                    {
                        objects[i] = draggedRows[i].Item as Object;
                    }

                    DragAndDrop.objectReferences = objects;
                }
                else
                {
                    DragAndDrop.objectReferences = new UnityEngine.Object[] { };
                }

                DragAndDrop.StartDrag(draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >");
            }

            protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
            {

                List<Row> draggedRows = DragAndDrop.GetGenericData("GenericDragColumnDragging") as List<Row>;
                if (draggedRows == null) return DragAndDropVisualMode.None;




                switch (args.dragAndDropPosition)
                {
                    case DragAndDropPosition.UponItem: return DragAndDropVisualMode.None;
                    case DragAndDropPosition.BetweenItems:
                        {
                            if (args.performDrop)
                            {
                                OnDropDraggedElementsAtIndex(draggedRows, args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
                            }

                            return DragAndDropVisualMode.Move;
                        }

                    case DragAndDropPosition.OutsideItems:
                        {
                            if (args.performDrop)
                            {
                                OnDropDraggedElementsAtIndex(draggedRows, _displayRows.Count);
                            }

                            return DragAndDropVisualMode.Move;
                        }

                    default: throw new ArgumentException();
                }
            }

            void OnDropDraggedElementsAtIndex(List<Row> draggedRows, int insertIndex)
            {

                for (int i = 0; i < draggedRows.Count; i++)
                {
                    _displayRows.Move(draggedRows[i], insertIndex);
                }

                List<int> selectedIDs = new List<int>();
                for (int i = 0; i < _displayRows.Count; i++)
                {
                    if (draggedRows.Contains(_displayRows[i]))
                    {
                        selectedIDs.Add(_displayRows[i].id);
                    }
                }

                multiColumnHeader.state.sortedColumns = new int[0];
                multiColumnHeader.state.sortedColumnIndex = -1;

                Reload();

                SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
            }

            List<T> FindItemsByIDs(IList<int> ids)
            {
                List<T> items = new List<T>();

                for (int i = 0; i < _rows.Count; i++)
                {
                    if (ids.Contains(_rows[i].id))
                    {
                        items.Add(_rows[i].Item);
                    }
                }

                return items;
            }


            T FindItemByID(int id)
            {
                Row row = FindRowByID(id);
                if (row != null)
                {
                    return row.Item;
                }

                return default(T);
            }


            List<Row> FindRowsByIDs(IList<int> ids)
            {
                List<Row> rows = new List<Row>();

                for (int i = 0; i < _rows.Count; i++)
                {
                    if (ids.Contains(_rows[i].id))
                    {
                        rows.Add(_rows[i]);
                    }
                }

                return rows;
            }

            Row FindRowByID(int id)
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    if (_rows[i].id == id) return _rows[i];
                }

                return null;
            }

            protected override void SearchChanged(string newSearch)
            {
                base.SearchChanged(newSearch);

                if (!hasSearch)
                {
                    _displayRows = null;
                }

                OnSearchChanged?.Invoke(newSearch);
            }

            public void DeselectAll()
            {
                SetSelection(new int[0], TreeViewSelectionOptions.None);
            }

            public void SelectAll()
            {
                EnsureInitialized();

                int[] selectedIDs = new int[_displayRows.Count];

                for (int i = 0; i < _displayRows.Count; i++)
                {
                    selectedIDs[i] = _displayRows[i].id;
                }

                SetSelection(selectedIDs);
            }

            public T[] GetSelectedRows()
            {
                EnsureInitialized();

                return FindItemsByIDs(GetSelection()).ToArray();
            }

            public void SetSelectedRows(T[] items)
            {
                EnsureInitialized();

                List<int> selectedIDs = new List<int>();

                for (int i = 0; i < _displayRows.Count; i++)
                {
                    if (items.Contains(_displayRows[i].Item))
                    {
                        selectedIDs.Add(_displayRows[i].id);
                    }
                }

                SetSelection(selectedIDs);
            }

            public void FrameRow(T item)
            {
                EnsureInitialized();

                for (int i = 0; i < _displayRows.Count; i++)
                {
                    if (_displayRows[i].Item.Equals(item))
                    {
                        FrameItem(_displayRows[i].id);
                        return;
                    }
                }
            }

            public T[] GetDisplayRowItems()
            {
                EnsureInitialized();

                List<T> items = new List<T>();

                for (int i = 0; i < _displayRows.Count; i++)
                {
                    items.Add(_displayRows[i].Item);
                }

                return items.ToArray();
            }

            public T GetRow(int index)
            {
                EnsureInitialized();

                return _displayRows[index].Item;
            }

            public bool IsRowVisibleInScrollView(T item)
            {
                EnsureInitialized();

                GetFirstAndLastVisibleRows(out int first, out int last);

                for (int i = first; i < last; i++)
                {
                    if (_displayRows[i].Item.Equals(item))
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool IsCellColumnVisibleInScrollView(Rect cellRect)
            {
                return cellRect.xMax >= state.scrollPos.x && cellRect.xMin <= state.scrollPos.x + treeViewRect.width;
            }


            public Rect GetCellRect(T item, Column column)
            {
                Assert.IsNotNull(column);

                if (!_hasDrawn) return new Rect();

                EnsureInitialized();

                int columnIndex = multiColumnHeader.GetVisibleColumnIndex(_columns.IndexOf(column));

                if (columnIndex >= 0)
                {
                    for (int i = 0; i < _displayRows.Count; i++)
                    {
                        if (_displayRows[i].Item.Equals(item))
                        {
                            return multiColumnHeader.GetCellRect(columnIndex, GetRowRect(i));
                        }
                    }
                }

                return new Rect();
            }

            public Rect GetRowRect(T item)
            {
                if (!_hasDrawn) return new Rect();

                EnsureInitialized();

                for (int i = 0; i < _displayRows.Count; i++)
                {
                    if (_displayRows[i].Item.Equals(item))
                    {
                        return GetRowRect(i);
                    }
                }

                return new Rect();
            }

            public int GetColumnIndex(Column column)
            {
                return _columns.IndexOf(column);
            }

            public bool IsColumnVisible(Column column)
            {
                return multiColumnHeader.IsColumnVisible(_columns.IndexOf(column));
            }

            public void SetColumnVisible(Column column, bool visible)
            {
                int index = _columns.IndexOf(column);
                bool isVisible = multiColumnHeader.IsColumnVisible(index);

                if (isVisible != visible)
                {
                    if (isVisible)
                    {
                        List<int> indices = new List<int>(multiColumnHeader.state.visibleColumns);
                        indices.Remove(index);
                        multiColumnHeader.state.visibleColumns = indices.ToArray();
                    }
                    else
                    {
                        List<int> indices = new List<int>(multiColumnHeader.state.visibleColumns);
                        indices.InsertSorted(index);
                        multiColumnHeader.state.visibleColumns = indices.ToArray();
                    }

                    OnVisibleColumnsChanged?.Invoke();
                }


            }



        }
    }
}