using CoreAnimation;
using CustomPicker.Interfaces;
using System;
using UIKit;
using PureLayout.Net;

namespace CustomPicker.Cells
{
    public class YetCollectionViewCell : UICollectionViewCell
    {
        private UIColor _textColor = UIColor.LightGray;
        private UILabel _label;

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _label.Text = value ?? string.Empty;
            }
        }

        private IYetCollectionViewCellDelegate _delegate;
        public IYetCollectionViewCellDelegate Delegate
        {
            get => _delegate;
            set
            {
                _delegate = value;

                if (_delegate != null)
                {
                    _label.Font = Delegate.FontForCollectionViewCell(this);
                    _textColor = Delegate.TextColorForCollectionViewCell(this);
                    _label.TextColor = _textColor;

                    var useTwoLineMode = Delegate.UseTwolineModeForCollectionViewCell(this);
                    _label.Lines = useTwoLineMode ? 2 : 1;
                    _label.LineBreakMode = useTwoLineMode ? UILineBreakMode.WordWrap : UILineBreakMode.TailTruncation;
                }
            }
        }

        public override bool Selected 
        { 
            get => base.Selected;
            set
            {
                base.Selected = value;
                _label.TextColor = base.Selected ? TintColor : _textColor;
            } 
        }


        public YetCollectionViewCell(IntPtr IntPtr)
            : base(IntPtr)
        { 
        }

        public void SetupCell()
        {
            BackgroundColor = UIColor.Clear;

            if (_label == null)
            {
                _label = new UILabel
                {
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                    BackgroundColor = UIColor.Clear,
                    TextAlignment = UITextAlignment.Center
                };
                ContentView.AddSubview(_label);

                _label.AutoPinEdgesToSuperviewEdges();
            }
        }
    }
}
