﻿using Common;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CommonControls.Table
{

    public class CellFactory
    {
        List<Func<object, BaseCellItem>> _createMap = new List<Func<object, BaseCellItem>>();
        List<string> _columnNames = new List<string>();
        Dictionary<string, ColoumTypes> _coloumTypes = new Dictionary<string, ColoumTypes>();
        Dictionary<ColoumTypes, string> _resourceMap = new Dictionary<ColoumTypes, string>();

        DataTable _table;


        public enum ColoumTypes
        { 
            Default,
            Bool,
            SubTable,
            ComboBox,
            BitFlag
        }

        public CellFactory(DataTable privateTable)
        {
            _table = privateTable;
            _resourceMap[ColoumTypes.Default] = "BaseTemplate";
            _resourceMap[ColoumTypes.Bool] = "BoolTemplate";
            _resourceMap[ColoumTypes.SubTable] = "ButtonTemplate";
            _resourceMap[ColoumTypes.ComboBox] = "ComboBoxTemplate";
            _resourceMap[ColoumTypes.BitFlag] = "";
        }

        internal string GetCellTemplate(string propertyName)
        {
            var type = _coloumTypes[propertyName];
            return _resourceMap[type];
        }

        public void CreateColoumn(string coloumnName, ColoumTypes coloumType, Func<object, BaseCellItem> createCell)
        {
            _columnNames = new List<string>();
            _createMap.Add(createCell);
            _table.Columns.Add(coloumnName, typeof(BaseCellItem));
            _coloumTypes[coloumnName] = coloumType;
        }

        BaseCellItem CreateColumnValue(int coloumIndex, object value)
        {
            return _createMap[coloumIndex](value);
        }

        public List<BaseCellItem> CreateRowInstance(params object[] values)
        {
            List<BaseCellItem> output = new List<BaseCellItem>();
            for(int i = 0; i < values.Length; i++)
                output.Add(CreateColumnValue(i, values[i]));

            return output;
        }
    }


    public abstract class BaseCellItem : NotifyPropertyChangedImpl, IConvertible
    {
        bool _isValid = true;
        public bool IsValid { get => _isValid; set => SetAndNotify(ref _isValid, value); }

        bool _isEditable = true;
        public bool IsEditable { get => _isEditable; set => SetAndNotify(ref _isEditable, value); }

        string _errorText = null;
        public string ErrorText { get => _errorText; set => SetAndNotify(ref _errorText, value); }

        public abstract string ToString(IFormatProvider provider);


        public abstract BaseCellItem Duplicate();

        protected void CopyInto(BaseCellItem target)
        {
            target.IsValid = IsValid;
            target.IsEditable = IsEditable;
            target.ErrorText = ErrorText;
        }

        #region IConvertable dont care
        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }


        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public delegate bool ValidateDelegate<T>(T cell, out string errorText);
    public class ValueCellItem<T> : BaseCellItem
    {
        public ValidateDelegate<T> ValidateFunc { get; set; }

        protected virtual void Validate()
        {
            if (ValidateFunc != null)
            {
                IsValid = ValidateFunc(Data, out var errorStr);
                ErrorText = errorStr;
            }
        }

        public ValueCellItem(T value = default, ValidateDelegate<T> validateFunc = null)
        {
            Data = value;
            ValidateFunc = validateFunc;
        }

        T _data;
        public T Data { get => _data; set { SetAndNotifyWhenChanged(ref _data, value); Validate(); } }


        public override string ToString(IFormatProvider provider)
        {
            return Data?.ToString();
        }

        public override string ToString()
        {
            return Data?.ToString();
        }

        public override BaseCellItem Duplicate()
        {
            var copy = new ValueCellItem<T>();
            copy.Data = Data;
            CopyInto(copy);
            return copy;
        }
    }

    public class BoolCellItem : BaseCellItem
    {
        public BoolCellItem(bool value)
        {
            Data = value;
        }

        bool _data;
        public bool Data { get => _data; set => SetAndNotify(ref _data, value); }

        public override string ToString(IFormatProvider provider)
        {
            return Data.ToString();
        }

        public override BaseCellItem Duplicate()
        {
            var copy = new BoolCellItem(Data);
            copy.Data = Data;
            CopyInto(copy);
            return copy;
        }
    }

    public class TypedComboBoxCellItem<T> : ValueCellItem<T>, ComboBoxCellItem
    {
        ObservableCollection<T> _allPossibleValues;
        ObservableCollection<T> _possibleValues;
        public ObservableCollection<T> PossibleValues { get => _possibleValues; set => SetAndNotify(ref _possibleValues, value); }
        public bool ValidateAsEnums { get; set; } = true;


        //int _selectedIndex = -1;
        //public int SelectedIndex { get => _selectedIndex; set => SetAndNotify(ref _selectedIndex, value); }

        public TypedComboBoxCellItem(T selectedValue = default, ObservableCollection<T> possibleValues = null)
        {
            Data = selectedValue;
            PossibleValues = possibleValues;
            _allPossibleValues = possibleValues;
            if(Data != null)
                _SearchText = Data.ToString();
            //_selectedIndex = PossibleValues.IndexOf(selectedValue);
        }

        public override string ToString(IFormatProvider provider)
        {
            return Data?.ToString();
        }

        private string _SearchText = "";
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                SetAndNotify(ref _SearchText, value);
                PossibleValues = new ObservableCollection<T> (_allPossibleValues.Where(x=>x.ToString().Contains(SearchText, StringComparison.InvariantCultureIgnoreCase)).ToList()) ;

                Data = _allPossibleValues.FirstOrDefault(x => x.ToString() == SearchText);
            }
        }

        protected override void Validate()
        {
            if (ValidateAsEnums && PossibleValues != null)
            {
                IsValid = PossibleValues.Contains(Data);
                ErrorText = IsValid ? null : "Not a valid value";
            }
            else
            {
                base.Validate();
            }
        }

        public override BaseCellItem Duplicate()
        {
            var copy = new TypedComboBoxCellItem<T>(Data, _allPossibleValues);
            copy.ValidateAsEnums = ValidateAsEnums;
            CopyInto(copy);
            return copy;
        }
    }

    public interface ComboBoxCellItem
    { }


    public class ButtonCellItem : BaseCellItem
    {
        public delegate void ExplorCellButtonPressed(BaseCellItem cell, int rowIndex);
        ExplorCellButtonPressed _explorCellButtonCallback;
        public ExplorCellButtonPressed ExplorCellButtonCallback { get => _explorCellButtonCallback; set { _explorCellButtonCallback = value; } }

        public ICommand ClickCommand { get; set; }

        TableViewModel _subTable;

        public ButtonCellItem(TableViewModel subTable)
        {
            ClickCommand = new RelayCommand<DataGridCellInfo>(Click);
            _subTable = subTable;
            //ExplorCellButtonCallback = clickCallback;
        }

        public override BaseCellItem Duplicate()
        {
            var copy = new ButtonCellItem(_subTable.Clone());
            CopyInto(copy);
            return copy;
        }

        void Click(DataGridCellInfo cellInfo)
        {
            var owner = cellInfo.Item as System.Data.DataRowView;
            var table = owner.DataView.Table;
            var index = table.Rows.IndexOf(owner.Row);

            TableWindow window = new TableWindow();
            window.DataContext = _subTable;
            window.Show();

            ExplorCellButtonCallback?.Invoke(this, index);
        }

        public override string ToString(IFormatProvider provider)
        {
            return "";
        }
    }

}
