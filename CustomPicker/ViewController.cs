using CustomPicker.Interfaces;
using System;
using System.Collections.Generic;
using UIKit;
using YetHealth.IOS.UI;
using PureLayout.Net;


namespace CustomPicker
{
    public partial class ViewController : UIViewController, IHorizontalPickerViewDataSource, IHorizontalPickerViewDelegate
    {
        private List<string> _items = new List<string>();
        private YetPickerView pickerView;


        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            for (int i = 15; i <= 100; i++)
                _items.Add(i.ToString());

            pickerView = new YetPickerView
            {
                MaxCellWidth = 70,
                Delegate = this,
                DataSource = this,
            };

            var leftSeparator = new UIView();
            leftSeparator.BackgroundColor = UIColor.FromRGB(255, 116, 158);
            pickerView.AddSubview(leftSeparator);

            var rightSeparator = new UIView();
            rightSeparator.BackgroundColor = UIColor.FromRGB(255, 116, 158);
            pickerView.AddSubview(rightSeparator);

            View.AddSubview(pickerView);
            pickerView.ReloadAll();

            pickerView.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
            pickerView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 63);
            pickerView.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 63);
            pickerView.AutoSetDimension(ALDimension.Height, 52);

            leftSeparator.AutoAlignAxis(ALAxis.Vertical, pickerView, -35);
            leftSeparator.AutoSetDimension(ALDimension.Width, 2);
            leftSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Top);
            leftSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Bottom);

            rightSeparator.AutoAlignAxis(ALAxis.Vertical, pickerView, 35);
            rightSeparator.AutoSetDimension(ALDimension.Width, 2);
            rightSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Top);
            rightSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Bottom);
        }

        public void DidSelectRow(YetPickerView pickerView, int row)
        {
            // business logic
        }

        public int NumberOfRowsInHorizontalPickerView(YetPickerView pickerView)
        {
            return _items.Count;
        }

        public bool PickerViewShouldMask(YetPickerView pickerView)
        {
            return true;
        }

        public UIColor TextColorForHorizontalPickerView(YetPickerView pickerView)
        {
            return UIColor.DarkTextColor;
        }

        public UIFont TextFontForHorizontalPickerView(YetPickerView pickerView)
        {
            return UIFont.SystemFontOfSize(20, UIFontWeight.Light);
        }

        public string TitleForRow(YetPickerView pickerView, int row)
        {
            return _items[row];
        }

        public bool UseTwoLineModeForHorizontalPickerView(YetPickerView pickerView)
        {
            return false;
        }
    }
}