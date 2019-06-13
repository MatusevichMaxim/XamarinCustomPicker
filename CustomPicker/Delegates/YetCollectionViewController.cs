using CoreGraphics;
using CustomPicker.Cells;
using CustomPicker.Interfaces;
using Foundation;
using System;
using System.Linq;
using UIKit;

namespace CustomPicker.Delegates
{
    public class YetCollectionViewController : UICollectionViewController, IUICollectionViewDelegateFlowLayout, IYetCollectionViewCellDelegate
    {
        private readonly IYetCollectionProvider _provider;
        private readonly nfloat _maxElementWidth = 0;

        public NSIndexPath selectedCellIndexPath = NSIndexPath.FromItemSection(0, 0);
        public UIFont font = UIFont.PreferredTitle1;
        public UIColor textColor = UIColor.LightGray;
        public bool useTwoLineMode = true;
        public bool programmaticallySet;


        public int SelectedRow => selectedCellIndexPath.Row;


        public YetCollectionViewController(UICollectionViewFlowLayout layout, IYetCollectionProvider provider, nfloat maxElementWidth)
            : base(layout)
        {
            _provider = provider;
            _maxElementWidth = maxElementWidth;
        }

        public void SelectRow(NSIndexPath indexPath, bool animated)
        {
            var collectionView = CollectionView;
            if (collectionView != null)
            {
                selectedCellIndexPath = indexPath;
                ScrollToIndex((int)indexPath.Item, animated);
                ChangeSelectionForCell(indexPath, collectionView, animated);
            }
        }

        // UICollectionViewDelegate/UICollectionViewDataSource

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _provider?.NumberOfRowsInCollectionViewController(this) ?? 0;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (YetCollectionViewCell)collectionView.DequeueReusableCell(nameof(YetCollectionViewCell), indexPath);
            cell.SetupCell();
            ConfigureCollectionViewCell(cell, indexPath);

            return cell;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            SelectRow(indexPath, true);
        }

        // UICollectionViewDelegateFlowLayout

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CoreGraphics.CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var text = _provider.TitleForRow(this, indexPath.Row);
            var maxHeight = collectionView.Bounds.Height - collectionView.ContentInset.Top - collectionView.ContentInset.Bottom;
            return SizeForText(text, new CGSize(_maxElementWidth, maxHeight));
        }

        // UIScrollviewDelegate

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            ScrollToPosition(scrollView);
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (!willDecelerate)
                ScrollToPosition(scrollView);
        }

        // Helpers

        private void ScrollToPosition(UIScrollView scrollView)
        {
            var collectionView = (UICollectionView)scrollView;
            var item = IndexPathForCenterCellFromCollectionview(collectionView);
            if (collectionView != null && item != null)
            {
                ScrollToIndex(item.Row, true);
                ChangeSelectionForCell(item, collectionView, true);
            }
        }

        private NSIndexPath IndexPathForCenterCellFromCollectionview(UICollectionView collectionView)
        {
            var point = collectionView.ConvertPointFromView(collectionView.Center, collectionView.Superview);
            var indexPath = collectionView.IndexPathForItemAtPoint(point);
            if (indexPath != null)
            {
                return indexPath;
            }
            else
            {
                return collectionView.IndexPathsForVisibleItems.First();
            }
        }

        public CGSize SizeForText(string text, CGSize maxSize)
        {
            var attr = new UIStringAttributes { Font = font };
            var frame = ((NSString)text).GetBoundingRect(maxSize, NSStringDrawingOptions.UsesLineFragmentOrigin, attr, new NSStringDrawingContext());
            frame = frame.Integral();
            frame.Width += 10;
            frame.Width = (nfloat)Math.Max(frame.Width, 30);
            frame.Size = new CGSize(frame.Size.Width + 10, maxSize.Height);
            return frame.Size;
        }

        private void ConfigureCollectionViewCell(YetCollectionViewCell cell, NSIndexPath indexPath)
        {
            var prov = _provider;
            if (prov != null)
            {
                cell.Text = prov.TitleForRow(this, indexPath.Row);
                cell.Selected = selectedCellIndexPath == indexPath;
                cell.Delegate = this;
            }
        }

        private void ScrollToIndex(int index, bool animated)
        {
            var indexPath = NSIndexPath.FromItemSection(index, 0);
            var cv = CollectionView;
            var attributes = cv.GetLayoutAttributesForItem(indexPath);
            if (cv != null && attributes != null)
            {
                var halfWidth = cv.Frame.Width / 2;
                var offset = new CGPoint(attributes.Frame.GetMidX() - halfWidth, 0);
                cv.SetContentOffset(offset, animated);
            }
            else
            {
                return;
            }
        }

        private void ChangeSelectionForCell(NSIndexPath indexPath, UICollectionView collectionView, bool animated)
        {
            collectionView.SelectItem(indexPath, animated, UICollectionViewScrollPosition.CenteredHorizontally);

            if (!programmaticallySet)
                _provider.DidSelectRow(this, (int)indexPath.Item);
            else
                programmaticallySet = false;
        }

        // HPCollectionViewCellDelegate

        public UIFont FontForCollectionViewCell(YetCollectionViewCell cvCell)
        {
            return font;
        }

        public UIColor TextColorForCollectionViewCell(YetCollectionViewCell cvCell)
        {
            return textColor;
        }

        public bool UseTwolineModeForCollectionViewCell(YetCollectionViewCell cvCell)
        {
            return useTwoLineMode;
        }
    }
}
