using System;
using YetHealth.IOS.UI;

namespace CustomPicker.Interfaces
{
    public interface IHorizontalPickerViewDataSource
    {
        int NumberOfRowsInHorizontalPickerView(YetPickerView pickerView);
    }
}
