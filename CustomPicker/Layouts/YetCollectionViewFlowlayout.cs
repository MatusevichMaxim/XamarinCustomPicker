using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using CoreAnimation;

namespace CustomPicker.Layouts
{
    public static class HorizontalPickerViewConstants
    {
        public static CGSize pathCornerRadii = new CGSize(10, 5);
        public static nfloat maxLabelWidthFactor = 0.5f;    // defines how man width space a single element can occupy as portion of the total width
        public static float maxRotationAngle = 0.0f;      // elements are rotated around the y axis depending on the distance from the center
    }

    public class YetCollectionViewFlowlayout : UICollectionViewFlowLayout
    {
        public nfloat activeDistance = 0;
        public nfloat midX = 0;
        public int lastElementIndex = 0;
        public nfloat maxAngle = HorizontalPickerViewConstants.maxRotationAngle;


        public override void PrepareLayout()
        {
            MinimumInteritemSpacing = 0;
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var array = base.LayoutAttributesForElementsInRect(rect);
            var cv = CollectionView;
            if (array != null && cv != null)
            {
                var visibleRect = new CGRect(cv.ContentOffset, cv.Bounds.Size);
                var attributesCopy = new List<UICollectionViewLayoutAttributes>();

                foreach (var itemAttributes in array)
                {
                    var itemAttributesCopy = itemAttributes.Copy() as UICollectionViewLayoutAttributes;
                    var distance = visibleRect.GetMidX() - itemAttributesCopy.Center.X;
                    var normalizeDistance = distance / activeDistance;
                    if (Math.Abs(distance) < activeDistance)
                    {
                        itemAttributesCopy.Alpha = 1.0f - (nfloat)Math.Abs(normalizeDistance);
                        var rotationAngle = maxAngle * normalizeDistance;
                        //itemAttributesCopy.Transform3D = CATransform3D.MakeRotation(rotationAngle, 0, 1, 0);
                    }
                    else
                    {
                        //itemAttributesCopy.Transform3D = CATransform3D.MakeRotation(maxAngle, 0, 1, 0);
                        itemAttributesCopy.Alpha = 0.1f;
                    }
                    attributesCopy.Add(itemAttributesCopy);
                }

                return attributesCopy.ToArray();
            }
            else
            {
                return null;
            }
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds) => true;
    }
}
