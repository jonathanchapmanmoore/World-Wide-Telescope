// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a <see cref="T:System.Windows.Controls.DataGrid" /> column.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "IComparable in Jolt has only one method, probably a false positive in FxCop.")]
    [StyleTypedProperty(Property = "CellStyle", StyleTargetType = typeof(DataGridCell))]
    [StyleTypedProperty(Property = "DragIndicatorStyle", StyleTargetType = typeof(ContentControl))]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(DataGridColumnHeader))]
    public abstract class DataGridColumn : DependencyObject
    {
        #region Constants

        internal const int DATAGRIDCOLUMN_maximumWidth = 65536;
        private const bool DATAGRIDCOLUMN_defaultIsReadOnly = false;

        #endregion Constants

        #region Data

        private List<string> _bindingPaths;
        private Style _cellStyle; 
        private int _displayIndexWithFiller;
        private double _desiredWidth; // Implicitly default to 0, keep it that way
        private Style _dragIndicatorStyle;
        private object _header;
        private DataGridColumnHeader _headerCell;
        private Style _headerStyle; 
        private List<BindingInfo> _inputBindings;
        private double? _maxWidth;
        private double? _minWidth;
        private bool? _isReadOnly;
        private DataGridLength? _width; // Null by default, null means inherit the Width from the DataGrid
        private Visibility _visibility;

        #endregion Data

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridColumn" /> class.
        /// </summary>
        protected internal DataGridColumn()
        {
            this._visibility = Visibility.Visible;
            this._displayIndexWithFiller = -1;
        }

        #region Dependency Properties

        #endregion

        #region Public Properties

        /// <summary>
        /// Actual visible width after Width, MinWidth, and MaxWidth setting at the Column level and DataGrid level
        /// have been taken into account
        /// </summary>
        public double ActualWidth
        {
            get
            {
                if (this.OwningGrid == null)
                {
                    return 0;
                }
                double targetWidth = this.EffectiveWidth.IsAbsolute ? this.EffectiveWidth.Value : this.DesiredWidth;
                return Math.Min(Math.Max(targetWidth, this.ActualMinWidth), this.ActualMaxWidth);
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering cells in the column.
        /// </summary>
        /// <returns>
        /// The style that is used when rendering cells in the column. The default is null.
        /// </returns>
        public Style CellStyle
        {
            get
            {
                return this._cellStyle;
            }
            set
            {
                if (_cellStyle != value)
                {
                    Style previousStyle = this._cellStyle;
                    this._cellStyle = value;
                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnCellStyleChanged(this, previousStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can change the column display position by 
        /// dragging the column header.
        /// </summary>
        /// <returns>
        /// true if the user can drag the column header to a new position; otherwise, false. The default is the current <see cref="P:System.Windows.Controls.DataGrid.CanUserReorderColumns" /> property value.
        /// </returns>
        public bool CanUserReorder
        {
            get
            {
                if (this.CanUserReorderInternal.HasValue)
                {
                    return this.CanUserReorderInternal.Value;
                }
                else if (this.OwningGrid != null)
                {
                    return this.OwningGrid.CanUserReorderColumns;
                }
                else
                {
                    return DataGrid.DATAGRID_defaultCanUserResizeColumns;
                }
            }
            set
            {
                this.CanUserReorderInternal = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can adjust the column width using the mouse.
        /// </summary>
        /// <returns>
        /// true if the user can resize the column; false if the user cannot resize the column. The default is the current <see cref="P:System.Windows.Controls.DataGrid.CanUserResizeColumns" /> property value.
        /// </returns>
        public bool CanUserResize
        {
            get 
            {
                if (this.CanUserResizeInternal.HasValue)
                {
                    return this.CanUserResizeInternal.Value;
                }
                else if (this.OwningGrid != null)
                {
                    return this.OwningGrid.CanUserResizeColumns;
                }
                else
                {
                    return DataGrid.DATAGRID_defaultCanUserResizeColumns;
                }
            }
            set 
            { 
                this.CanUserResizeInternal = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can sort the column by clicking the column header.
        /// </summary>
        /// <returns>
        /// true if the user can sort the column; false if the user cannot sort the column. The default is the current <see cref="P:System.Windows.Controls.DataGrid.CanUserSortColumns" /> property value.
        /// </returns>
        public bool CanUserSort
        {
            get
            {
                if (this.CanUserSortInternal.HasValue)
                {
                    return this.CanUserSortInternal.Value;
                }
                else if (this.OwningGrid != null)
                {
                    string propertyPath = GetSortPropertyName();
                    Type propertyType = this.OwningGrid.DataConnection.DataType.GetNestedPropertyType(propertyPath);

                    // if the type is nullable, then we will compare the non-nullable type
                    if (TypeHelper.IsNullableType(propertyType))
                    {
                        propertyType = TypeHelper.GetNonNullableType(propertyType);
                    }

                    // return whether or not the property type can be compared
                    return (typeof(IComparable).IsAssignableFrom(propertyType)) ? true : false;
                }
                else
                {
                    return DataGrid.DATAGRID_defaultCanUserSortColumns;
                }
            }
            set
            {
                this.CanUserSortInternal = value;
            }
        }

        /// <summary>
        /// Gets or sets the display position of the column relative to the other columns in the <see cref="T:System.Windows.Controls.DataGrid" />.
        /// </summary>
        /// <returns>
        /// The zero-based position of the column as it is displayed in the associated <see cref="T:System.Windows.Controls.DataGrid" />. The default is the index of the corresponding <see cref="P:System.Collections.ObjectModel.Collection`1.Item(System.Int32)" /> in the <see cref="P:System.Windows.Controls.DataGrid.Columns" /> collection.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// When setting this property, the specified value is less than -1 or equal to <see cref="F:System.Int32.MaxValue" />.
        /// 
        /// -or-
        /// 
        /// When setting this property on a column in a <see cref="T:System.Windows.Controls.DataGrid" />, the specified value is less than zero or greater than or equal to the number of columns in the <see cref="T:System.Windows.Controls.DataGrid" />.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// When setting this property, the <see cref="T:System.Windows.Controls.DataGrid" /> is already making <see cref="P:System.Windows.Controls.DataGridColumn.DisplayIndex" /> adjustments. For example, this exception is thrown when you attempt to set <see cref="P:System.Windows.Controls.DataGridColumn.DisplayIndex" /> in a <see cref="E:System.Windows.Controls.DataGrid.ColumnDisplayIndexChanged" /> event handler.
        /// 
        /// -or-
        /// 
        /// When setting this property, the specified value would result in a frozen column being displayed in the range of unfrozen columns, or an unfrozen column being displayed in the range of frozen columns.
        /// </exception>
        public int DisplayIndex
        {
            get
            {
                if (this.OwningGrid != null && this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
                {
                    return _displayIndexWithFiller - 1;
                }
                else
                {
                    return _displayIndexWithFiller;
                }
            }
            set
            {
                if (value == Int32.MaxValue)
                {
                    throw DataGridError.DataGrid.ValueMustBeLessThan("value", "DisplayIndex", Int32.MaxValue);
                }
                if (this.OwningGrid != null)
                {
                    if (this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
                    {
                        value++;
                    }
                    if (this._displayIndexWithFiller != value)
                    {
                        if (value < 0 || value >= this.OwningGrid.ColumnsItemsInternal.Count)
                        {
                            throw DataGridError.DataGrid.ValueMustBeBetween("value", "DisplayIndex", 0, true, this.OwningGrid.Columns.Count, false);
                        }
                        // Will throw an error if a visible frozen column is placed inside a non-frozen area or vice-versa.
                        this.OwningGrid.OnColumnDisplayIndexChanging(this, value);
                        this._displayIndexWithFiller = value;
                        try
                        {
                            this.OwningGrid.InDisplayIndexAdjustments = true;
                            this.OwningGrid.OnColumnDisplayIndexChanged(this);
                            this.OwningGrid.OnColumnDisplayIndexChanged_PostNotification();
                        }
                        finally
                        {
                            this.OwningGrid.InDisplayIndexAdjustments = false;
                        }
                    }
                }
                else
                {
                    if (value < -1)
                    {
                        throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "DisplayIndex", -1);
                    }
                    this._displayIndexWithFiller = value;
                }
            }
        }

        public Style DragIndicatorStyle
        {
            get
            {
                return _dragIndicatorStyle;
            }
            set
            {
                if (_dragIndicatorStyle != value)
                {
                    _dragIndicatorStyle = value;
                }
            }
        }

        public Style HeaderStyle
        {
            get
            {
                return this._headerStyle;
            }
            set
            {
                if (this._headerStyle != value)
                {
                    Style previousStyle = this._headerStyle;
                    this._headerStyle = value;
                    if (this._headerCell != null)
                    {
                        this._headerCell.EnsureStyle(previousStyle);
                    }
                }
            }
        }

        public object Header
        {
            get
            {
                return this._header;
            }
            set
            {
                if (this._header != value)
                {
                    this._header = value;
                    if (this._headerCell != null)
                    {
                        this._headerCell.Content = this._header;
                    }
                }
            }
        }

        public bool IsAutoGenerated
        {
            get;
            internal set;
        }
        
        public bool IsFrozen
        {
            get;
            internal set;
        }

        public bool IsReadOnly
        {
            get
            {
                if (this.OwningGrid == null)
                {
                    return this._isReadOnly ?? DATAGRIDCOLUMN_defaultIsReadOnly;
                }
                if (this._isReadOnly != null)
                {
                    return this._isReadOnly.Value || this.OwningGrid.IsReadOnly;
                }
                return this.OwningGrid.GetColumnReadOnlyState(this, DATAGRIDCOLUMN_defaultIsReadOnly);
            }
            set
            {
                if (value != this._isReadOnly)
                {
                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnReadOnlyStateChanging(this, value);
                    }
                    this._isReadOnly = value;
                }
            }
        }

        public double MaxWidth
        {
            get
            {
                if (_maxWidth.HasValue)
                {
                    return _maxWidth.Value;
                }
                return double.PositiveInfinity;
            }
            set
            {
                if (value < 0)
                {
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", 0);
                }
                if (value < this.ActualMinWidth)
                {
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", "MinWidth");
                }
                if (!_maxWidth.HasValue || _maxWidth.Value != value)
                {
                    _maxWidth = value;
                    if (this.OwningGrid != null && _maxWidth.Value < this.ActualWidth)
                    {
                        this.OwningGrid.OnColumnWidthChanged(this);
                    }
                }
            }
        }

        public double MinWidth
        {
            get
            {
                if (_minWidth.HasValue)
                {
                    return _minWidth.Value;
                }
                return 0;
            }
            set
            {
                if (double.IsNaN(value))
                {
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MinWidth");
                }
                if (value < 0)
                {
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MinWidth", 0);
                }
                if (double.IsPositiveInfinity(value))
                {
                    throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("MinWidth");
                }
                if (value > this.ActualMaxWidth)
                {
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "MinWidth", "MaxWidth");
                }
                if (!_minWidth.HasValue || _minWidth.Value != value)
                {
                    _minWidth = value;
                    if (this.OwningGrid != null && _minWidth > this.ActualWidth)
                    {
                        this.OwningGrid.OnColumnWidthChanged(this);
                    }
                }
            }
        }

        /// <summary>
        /// Holds the name of the member to use for sorting, if not using the default.
        /// </summary>
        public string SortMemberPath
        {
            get;
            set;
        }

        public Visibility Visibility
        {
            get
            {
                return this._visibility;
            }
            set
            {
                if (value != this.Visibility)
                {
                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnVisibleStateChanging(this);
                    }

                    this._visibility = value;

                    if (this._headerCell != null)
                    {
                        this._headerCell.Visibility = this._visibility;
                    }

                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnVisibleStateChanged(this);
                    }
                }
            }
        }

        public DataGridLength Width
        {
            get
            {
                if (_width.HasValue)
                {
                    return _width.Value;
                }
                else if (this.OwningGrid != null)
                {
                    return this.OwningGrid.ColumnWidth;
                }
                else
                {
                    // We don't have a good choice here because we don't want to make this property nullable, see DevDiv Bugs 196581
                    return DataGridLength.Auto;
                }
            }
            set
            {
                if (!_width.HasValue || _width.Value != value)
                {
                    _width = value;
                    if (value.IsAbsolute)
                    {
                        this.DesiredWidth = value.Value;
                    }
                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnWidthChanged(this);
                    }
                }
            }
        }

        #endregion Public Properties

        #region Internal Properties

        internal bool ActualCanUserResize
        {
            get
            {
                if (this.OwningGrid == null || this.OwningGrid.CanUserResizeColumns == false || this is DataGridFillerColumn)
                {
                    return false;
                }
                return this.CanUserResizeInternal ?? true;
            }
        }

        // MaxWidth from local setting or DataGrid setting
        internal double ActualMaxWidth
        {
            get
            {
                return _maxWidth ?? (this.OwningGrid != null ? this.OwningGrid.MaxColumnWidth : double.PositiveInfinity);
            }
        }

        // MinWidth from local setting or DataGrid setting
        internal double ActualMinWidth
        {
            get
            {
                return _minWidth ?? (this.OwningGrid != null ? this.OwningGrid.MinColumnWidth : 0);
            }
        }

        internal List<string> BindingPaths
        {
            get
            {
                if (this._bindingPaths != null)
                {
                    return this._bindingPaths;
                }
                this._bindingPaths = CreateBindingPaths();
                return this._bindingPaths;
            }
        }

        internal bool? CanUserReorderInternal
        {
            get;
            set;
        }

        internal bool? CanUserResizeInternal
        {
            get;
            set;
        }

        internal bool? CanUserSortInternal
        {
            get;
            set;
        }

        // Desired pixel width of the Column without coercion
        internal double DesiredWidth
        {
            get
            {
                return _desiredWidth;
            }
            set
            {
                if (_desiredWidth != value)
                {
                    double oldActualWidth = this.ActualWidth;
                    _desiredWidth = value;
                    if (this.OwningGrid != null && oldActualWidth != this.ActualWidth)
                    {
                        this.OwningGrid.OnColumnWidthChanged(this);
                    }
                }
            }
        }

        internal bool DisplayIndexHasChanged
        {
            get;
            set;
        }

        internal int DisplayIndexWithFiller
        {
            get
            {
                return this._displayIndexWithFiller;
            }
            set
            {
                Debug.Assert(value >= -1);
                Debug.Assert(value < Int32.MaxValue);

                this._displayIndexWithFiller = value;
            }
        }

        // Width of the Column set locally or inherited from the DataGrid without any coercion
        internal DataGridLength EffectiveWidth
        {
            get
            {
                Debug.Assert(this.OwningGrid != null);
                return _width ?? this.OwningGrid.ColumnWidth;
            }
        }

        internal bool HasHeaderCell
        {
            get
            {
                return this._headerCell != null;
            }
        }

        internal DataGridColumnHeader HeaderCell
        {
            get
            {
                if (this._headerCell == null)
                {
                    this._headerCell = CreateHeader();
                }
                return this._headerCell;
            }
        }
        
        internal int Index
        {
            get;
            set;
        }

        internal DataGrid OwningGrid
        {
            get;
            set;
        }

        #endregion Internal Properties

        #region Public Methods

        public FrameworkElement GetCellContent(DataGridRow dataGridRow)
        {
            if (dataGridRow == null)
            {
                throw new ArgumentNullException("dataGridRow");
            }
            if (this.OwningGrid == null)
            {
                throw DataGridError.DataGrid.NoOwningGrid(this.GetType());
            }
            if (dataGridRow.OwningGrid == this.OwningGrid)
            {
                Debug.Assert(this.Index >= 0);
                Debug.Assert(this.Index < this.OwningGrid.ColumnsItemsInternal.Count);
                DataGridCell dataGridCell = dataGridRow.Cells[this.Index];
                if (dataGridCell != null)
                {
                    return dataGridCell.Content as FrameworkElement;
                }
            }
            return null;
        }

        public FrameworkElement GetCellContent(object dataItem)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException("dataItem");
            }
            if (this.OwningGrid == null)
            {
                throw DataGridError.DataGrid.NoOwningGrid(this.GetType());
            }
            Debug.Assert(this.Index >= 0);
            Debug.Assert(this.Index < this.OwningGrid.ColumnsItemsInternal.Count);
            DataGridRow dataGridRow = this.OwningGrid.GetRowFromItem(dataItem);
            if (dataGridRow == null)
            {
                return null;
            }
            return GetCellContent(dataGridRow);
        }

        /// <summary>
        /// Returns the column which contains the given element
        /// </summary>
        /// <param name="element">element contained in a column</param>
        /// <returns>Column that contains the element, or null if not found
        /// </returns>
        public static DataGridColumn GetColumnContainingElement(FrameworkElement element)
        {
            // Walk up the tree to find the DataGridCell or DataGridColumnHeader that contains the element
            DependencyObject parent = element;
            while (parent != null)
            {
                DataGridCell cell = parent as DataGridCell;
                if (cell != null)
                {
                    return cell.OwningColumn;
                }
                DataGridColumnHeader columnHeader = parent as DataGridColumnHeader;
                if (columnHeader != null)
                {
                    return columnHeader.OwningColumn;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// When overridden in a derived class, causes the column cell being edited to revert to the unedited value.
        /// </summary>
        /// <param name="editingElement">
        /// The element that the column displays for a cell in editing mode.
        /// </param>
        /// <param name="uneditedValue">
        /// The previous, unedited value in the cell being edited.
        /// </param>
        protected virtual void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        { }

        /// <summary>
        /// When overridden in a derived class, gets an editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <param name="cell">
        /// The cell that will contain the generated element.
        /// </param>
        /// <param name="dataItem">
        /// The data item represented by the row that contains the intended cell.
        /// </param>
        /// <returns>
        /// A new editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </returns>
        protected abstract FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem);

        /// <summary>
        /// When overridden in a derived class, gets a read-only element that is bound to the column's 
        /// <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <param name="cell">
        /// The cell that will contain the generated element.
        /// </param>
        /// <param name="dataItem">
        /// The data item represented by the row that contains the intended cell.
        /// </param>
        /// <returns>
        /// A new, read-only element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </returns>
        protected abstract FrameworkElement GenerateElement(DataGridCell cell, object dataItem);

        /// <summary>
        /// Called by a specific column type when one of its properties changed, 
        /// and its current cells need to be updated.
        /// </summary>
        /// <param name="propertyName">Indicates which property changed and caused this call</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.OwningGrid == null)
            {
                return;
            }
            this.OwningGrid.RefreshColumnElements(this, propertyName);
        }

        /// <summary>
        /// When overridden in a derived class, called when a cell in the column enters editing mode.
        /// </summary>
        /// <param name="editingElement">
        /// The element that the column displays for a cell in editing mode.
        /// </param>
        /// <param name="editingEventArgs">
        /// Information about the user gesture that is causing a cell to enter editing mode.
        /// </param>
        /// <returns>
        /// The unedited value.
        /// </returns>
        protected abstract object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs);

        /// <summary>
        /// Called by the DataGrid control when a column asked for its
        /// elements to be refreshed, typically because one of its properties changed.
        /// </summary>
        /// <param name="element">Indicates the element that needs to be refreshed</param>
        /// <param name="propertyName">Indicates which property changed and caused this call</param>
        protected internal virtual void RefreshCellContent(FrameworkElement element, string propertyName)
        {
        }

        #endregion Protected Methods

        #region Internal Methods

        internal void CancelCellEditInternal(FrameworkElement editingElement, object uneditedValue)
        {
            CancelCellEdit(editingElement, uneditedValue);
        }

        internal virtual List<string> CreateBindingPaths()
        {
            List<string> bindingPaths = new List<string>();
            List<BindingInfo> bindings = null;
            if (this._inputBindings != null)
            {
                Debug.Assert(this.OwningGrid != null);
                Debug.Assert(this.OwningGrid.EditingColumnIndex == this.Index);

                // Use the editing bindings if they've already been created
                bindings = this._inputBindings;
            }
            else
            {
                // Otherwise we'll have inspect all of the bindings of the non-editing element
                if (this.OwningGrid != null)
                {
                    DataGridRow row = this.OwningGrid.EditingRow;
                    if (row != null)
                    {
                        bindings = CreateBindings(GetCellContent(row), row.DataContext, false /*twoWay*/);
                    }
                }
            }
            if (bindings != null)
            {
                // We're going to return the path of every active binding
                foreach (BindingInfo binding in bindings)
                {
                    if (binding != null &&
                        binding.BindingExpression != null &&
                        binding.BindingExpression.ParentBinding != null &&
                        binding.BindingExpression.ParentBinding.Path != null)
                    {
                        bindingPaths.Add(binding.BindingExpression.ParentBinding.Path.Path);
                    }
                }
            }
            return bindingPaths;
        }

        internal virtual List<BindingInfo> CreateBindings(FrameworkElement element, object dataItem, bool twoWay)
        {
            return element.GetBindingInfo(dataItem, twoWay, false /*useBlockList*/, true /*searchChildren*/, typeof(DataGrid));
        }

        internal virtual DataGridColumnHeader CreateHeader()
        {
            DataGridColumnHeader result = new DataGridColumnHeader();
            result.OwningColumn = this;
            result.Content = this._header;
            result.EnsureStyle(null);

            return result;
        }

        internal FrameworkElement GenerateEditingElementInternal(DataGridCell cell, object dataItem)
        {
            FrameworkElement editingElement = GenerateEditingElement(cell, dataItem);
            this._inputBindings = CreateBindings(editingElement, dataItem, true /*twoWay*/);

            // Setup all of the active input bindings to support validation
            foreach (BindingInfo bindingData in this._inputBindings)
            {
                if (bindingData.BindingExpression != null
                    && bindingData.BindingExpression.ParentBinding != null
                    && bindingData.BindingExpression.ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                {
                    Binding binding = ValidationUtil.CloneBinding(bindingData.BindingExpression.ParentBinding);
                    if (binding != null)
                    {
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
                        bindingData.Element.SetBinding(bindingData.BindingTarget, binding);
                        bindingData.BindingExpression = bindingData.Element.GetBindingExpression(bindingData.BindingTarget);
                    }
                }
            }

            return editingElement;
        }

        internal FrameworkElement GenerateElementInternal(DataGridCell cell, object dataItem)
        {
            this._inputBindings = null;
            return GenerateElement(cell, dataItem);
        }

        internal List<BindingInfo> GetInputBindings(FrameworkElement element, object dataItem)
        {
            if (this._inputBindings != null)
            {
                return this._inputBindings;
            }
            return CreateBindings(element, dataItem, true /*twoWay*/);
        }

        /// <summary>
        /// We get the sort description from the data source.  We don't worry whether we can modify sort -- perhaps the sort description
        /// describes an unchangeable sort that exists on the data.
        /// </summary>
        /// <returns></returns>
        internal SortDescription? GetSortDescription()
        {
            if (this.OwningGrid != null
                && this.OwningGrid.DataConnection != null
                && this.OwningGrid.DataConnection.SortDescriptions != null)
            {
                string propertyName = GetSortPropertyName();

                SortDescription sort = (new List<SortDescription>(this.OwningGrid.DataConnection.SortDescriptions))
                    .FirstOrDefault(s => s.PropertyName == propertyName);

                if (sort.PropertyName != null)
                {
                    return sort;
                }

                return null;
            }


            return null;
        }

        internal string GetSortPropertyName()
        {
            string result = this.SortMemberPath;

            if (String.IsNullOrEmpty(result))
            {
                DataGridBoundColumn boundColumn = this as DataGridBoundColumn;

                if (boundColumn != null && boundColumn.Binding != null && boundColumn.Binding.Path != null)
                {
                    result = boundColumn.Binding.Path.Path;
                }
            }

            return result;
        }

        internal object PrepareCellForEditInternal(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return PrepareCellForEdit(editingElement, editingEventArgs);
        }

        internal void ResetBindingPaths()
        {
            this._bindingPaths = null;
        }

        #endregion Internal Methods

        #region Private Methods

        #endregion Private Methods

    }
}
