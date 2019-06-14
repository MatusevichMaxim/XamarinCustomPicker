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
        public nfloat ActiveDistance { get; set; } = 0;
        public nfloat MidX { get; set; } = 0;
        public int LastElementIndex { get; set; } = 0;


        public override void PrepareLayout()
        {
            MinimumInteritemSpacing = 37;
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
                    var normalizeDistance = distance / ActiveDistance;
                    if (Math.Abs(distance) < ActiveDistance)
                    {
                        itemAttributesCopy.Alpha = 1.0f - (nfloat)Math.Abs(normalizeDistance * 2);
                        itemAttributesCopy.Transform = CGAffineTransform.MakeScale(1.5f - (nfloat)Math.Abs(normalizeDistance * 2),
                                                                                   1.5f - (nfloat)Math.Abs(normalizeDistance * 2));
                    }
                    else
                    {
                        itemAttributesCopy.Alpha = 0.9f;
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
