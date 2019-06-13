using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace CustomPicker.Helpers
{
    public static class StringHelper
    {
        public static CGSize SizeForText(string text, CGSize maxSize, UIFont font)
        {
            var attr = new UIStringAttributes { Font = font };
            var frame = ((NSString)text).GetBoundingRect(maxSize, NSStringDrawingOptions.UsesLineFragmentOrigin, attr, new NSStringDrawingContext());
            frame = frame.Integral();
            frame.Width += 10;
            frame.Width = (nfloat)Math.Max(frame.Width, 30);
            frame.Size = new CGSize(frame.Size.Width + 10, maxSize.Height);
            return frame.Size;
        }
    }
}
