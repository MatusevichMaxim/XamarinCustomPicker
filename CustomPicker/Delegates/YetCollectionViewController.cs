using CoreGraphics;
using CustomPicker.Cells;
using CustomPicker.Helpers;
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

        public NSIndexPath SelectedCellIndexPath { get; set; } = NSIndexPath.FromItemSection(0, 0);
        public UIFont Font { get; set; } = UIFont.PreferredTitle1;
        public UIColor TextColor { get; set; } = UIColor.LightGray;
        public bool UseTwoLineMode { get; set; }
        public bool ProgrammaticallySet { get; set; }
        public int SelectedRow => SelectedCellIndexPath.Row;


        public YetCollectionViewController(UICollectionViewFlowLayout layout, IYetCollectionProvider provider, nfloat maxElementWidth)
            : base(layout)
        {
            _provider = provider;
            _maxElementWidth = maxElementWidth;
        }

        public void SelectRow(NSIndexPath indexPath, bool animated)
        {
            if (CollectionView != null)
            {
                SelectedCellIndexPath = indexPath;
                ScrollToIndex((int)indexPath.Item, animated);
                ChangeSelectionForCell(indexPath, CollectionView, animated);
            }
        }

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
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var text = _provider?.TitleForRow(this, indexPath.Row) ?? string.Empty;
            var maxHeight = collectionView.Bounds.Height - collectionView.ContentInset.Top - collectionView.ContentInset.Bottom;
            return StringHelper.SizeForText(text, new CGSize(_maxElementWidth, maxHeight), Font);
        }

        // UIScrollViewDelegate
        
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
            var collectionView = scrollView as UICollectionView;
            var item = IndexPathForCenterCellFromCollectionView(collectionView);
            if (collectionView != null && item != null)
            {
                ScrollToIndex(item.Row, true);
                ChangeSelectionForCell(item, collectionView, true);
            }
            else
                return;
        }

        private NSIndexPath IndexPathForCenterCellFromCollectionView(UICollectionView collectionView)
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

        private void ConfigureCollectionViewCell(YetCollectionViewCell cell, NSIndexPath indexPath)
        {
            if (_provider != null)
            {
                cell.Text = _provider.TitleForRow(this, indexPath.Row);
                cell.Selected = SelectedCellIndexPath == indexPath;
                cell.Delegate = this;
            }
        }

        private void ScrollToIndex(int index, bool animated)
        {
            var indexPath = NSIndexPath.FromItemSection(index, 0);
            var attributes = CollectionView.GetLayoutAttributesForItem(indexPath);
            if (CollectionView != null && attributes != null)
            {
                var halfWidth = CollectionView.Frame.Width / 2;
                var offset = new CGPoint(attributes.Frame.GetMidX() - halfWidth, 0);
                CollectionView.SetContentOffset(offset, animated);
            }
            else
                return;
        }

        private void ChangeSelectionForCell(NSIndexPath indexPath, UICollectionView collectionView, bool animated)
        {
            collectionView.SelectItem(indexPath, animated, UICollectionViewScrollPosition.CenteredHorizontally);
            if (!ProgrammaticallySet)
                _provider?.DidSelectRow(this, (int)indexPath.Item);
            else
                ProgrammaticallySet = false;
        }

        // IYetCollectionViewCellDelegate

        public UIFont FontForCollectionViewCell(YetCollectionViewCell cvCell) => Font;
        public UIColor TextColorForCollectionViewCell(YetCollectionViewCell cvCell) => TextColor;
        public bool UseTwolineModeForCollectionViewCell(YetCollectionViewCell cvCell) => UseTwoLineMode;
    }
}
