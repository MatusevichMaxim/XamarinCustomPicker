using CoreGraphics;
using CustomPicker.Interfaces;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using YetHealth.IOS.UI;
using PureLayout.Net;


namespace CustomPicker
{
    public partial class ViewController : UIViewController, IHorizontalPickerViewDataSource, IHorizontalPickerViewDelegate
    {
        public List<string> titles = new List<string> { "$$", "$$", "$$", "$$$$", "$$", "$$$$", "$", "$$$", "$", "$$$" };
        public YetPickerView pickerView;



        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            pickerView = new YetPickerView();

            pickerView.DataSource = this;
            pickerView.Delegate = this;

            View.AddSubview(pickerView);

            pickerView.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
            pickerView.AutoPinEdgeToSuperviewEdge(ALEdge.Left);
            pickerView.AutoPinEdgeToSuperviewEdge(ALEdge.Right);
            pickerView.AutoSetDimension(ALDimension.Height, 50);
        }

        public void DidSelectRow(YetPickerView pickerView, int row)
        {
        }

        public int NumberOfRowsInHorizontalPickerView(YetPickerView pickerView)
        {
            return titles.Count;
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
            return null;
        }

        public string TitleForRow(YetPickerView pickerView, int row)
        {
            return titles[row];
        }

        public bool UseTwoLineModeForHorizontalPickerView(YetPickerView pickerView)
        {
            return false;
        }
    }
}