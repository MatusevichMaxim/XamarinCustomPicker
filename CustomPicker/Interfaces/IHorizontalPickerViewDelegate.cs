using System;
using UIKit;
using YetHealth.IOS.UI;

namespace CustomPicker.Interfaces
{
    public interface IHorizontalPickerViewDelegate
    {
        string TitleForRow(YetPickerView pickerView, int row);
        void DidSelectRow(YetPickerView pickerView, int row);

        UIFont TextFontForHorizontalPickerView(YetPickerView pickerView);
        UIColor TextColorForHorizontalPickerView(YetPickerView pickerView);
        bool UseTwoLineModeForHorizontalPickerView(YetPickerView pickerView);
    }
}
