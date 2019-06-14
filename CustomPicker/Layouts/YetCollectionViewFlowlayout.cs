using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace CustomPicker.Layouts
{
    public static class HorizontalPickerViewConstants
    {
        public static CGSize pathCornerRadii = new CGSize(10, 5);
        public static nfloat maxLabelWidthFactor = 0.5f;    // defines how man width space a single element can occupy as portion of the total width
    }

    public class YetCollectionViewFlowLayout : UICollectionViewFlowLayout
    {
        public nfloat InteritemSpacing { get; set; }
        public nfloat ActiveDistance { get; set; }
        public nfloat MidX { get; set; }
        public int LastElementIndex { get; set; }


        public override void PrepareLayout()
        {
            MinimumInteritemSpacing = InteritemSpacing;
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var array = base.LayoutAttributesForElementsInRect(rect);
            if (array != null && CollectionView != null)
            {
                var visibleRect = new CGRect(CollectionView.ContentOffset, CollectionView.Bounds.Size);
                var attributesCopy = new List<UICollectionViewLayoutAttributes>();

                foreach (var itemAttributes in array)
                {
                    var itemAttributesCopy = itemAttributes.Copy() as UICollectionViewLayoutAttributes;
                    var distance = visibleRect.GetMidX() - itemAttributesCopy.Center.X;
                    var normalizeDistance = distance <= ActiveDistance / 2 ? distance / ActiveDistance : 0.5f;

                    if (Math.Abs(distance) < ActiveDistance / 2)
                    {
                        itemAttributesCopy.Alpha = 1.0f - (nfloat)Math.Abs(normalizeDistance);
                        itemAttributesCopy.Transform = CGAffineTransform.MakeScale(1.5f - (nfloat)Math.Abs(normalizeDistance),
                                                                                   1.5f - (nfloat)Math.Abs(normalizeDistance));
                    }
                    else
                    {
                        itemAttributesCopy.Alpha = 0.5f;
                        itemAttributesCopy.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
                    }
                    attributesCopy.Add(itemAttributesCopy);
                }

                return attributesCopy.ToArray();
            }

            return null;
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds) => true;
    }
}
