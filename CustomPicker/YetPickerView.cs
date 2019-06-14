using CoreAnimation;
using CoreGraphics;
using CustomPicker.Cells;
using CustomPicker.Delegates;
using CustomPicker.Helpers;
using CustomPicker.Interfaces;
using CustomPicker.Layouts;
using Foundation;
using PureLayout.Net;
using System;
using UIKit;

namespace YetHealth.IOS.UI
{
    public class YetPickerView : UIView, IYetCollectionProvider
    {
        private YetCollectionViewController _collectionController;
        private YetCollectionViewFlowLayout _collectionViewLayout;
        private UICollectionView _collectionView;
        private bool _isInitialized;

        public nfloat MaxCellWidth { get; set; } = 0;
        public int StartItemIndex { get; set; } = 0;

        private IHorizontalPickerViewDelegate _delegate;
        public IHorizontalPickerViewDelegate Delegate
        {
            get => _delegate;
            set
            {
                _delegate = value;
                Adjust(_delegate, DataSource);
            }
        }

        private IHorizontalPickerViewDataSource _dataSource;
        public IHorizontalPickerViewDataSource DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                Adjust(_delegate, DataSource);
            }
        }


        public YetPickerView()
        {
            Setup();
        }

        private void Setup()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;

            var maxElementWidth = Bounds.Width * HorizontalPickerViewConstants.maxLabelWidthFactor;
            _collectionViewLayout = new YetCollectionViewFlowLayout 
            { 
                InteritemSpacing = 37,
            };
            _collectionController = new YetCollectionViewController(_collectionViewLayout, this, maxElementWidth);
            _collectionController.CollectionView.RegisterClassForCell(typeof(YetCollectionViewCell), nameof(YetCollectionViewCell));
            _collectionController.CollectionView.BackgroundColor = UIColor.Clear;
            _collectionController.CollectionView.ShowsHorizontalScrollIndicator = false;
            _collectionController.ClearsSelectionOnViewWillAppear = false;

            _collectionView = _collectionController.CollectionView;
            _collectionView.AllowsMultipleSelection = false;
            _collectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddSubview(_collectionView);

            _collectionView.AutoPinEdgesToSuperviewEdges();
        }

        public int SelectedRow() => _collectionController.SelectedCellIndexPath.Row;

        public void SelectRow(int rowIndex, bool animated)
        {
            var indexPath = NSIndexPath.FromItemSection(rowIndex, 0);
            _collectionController.ProgrammaticallySet = true;
            _collectionController.SelectRow(indexPath, animated);
        }

        public void ReloadAll()
        {
            _collectionController.CollectionView.ReloadData();
            SelectRow(StartItemIndex, false);
        }

        private UIBezierPath ShapePathForFrame(CGRect frame)
        {
            return UIBezierPath.FromRoundedRect(frame, UIRectCorner.AllCorners, HorizontalPickerViewConstants.pathCornerRadii);
        }

        private void Adjust(IHorizontalPickerViewDelegate del, IHorizontalPickerViewDataSource dataSource)
        {
            if (del != null && dataSource != null && !_isInitialized)
            {
                _collectionController.Font = del?.TextFontForHorizontalPickerView(this) ?? UIFont.PreferredBody;
                _collectionController.TextColor = del?.TextColorForHorizontalPickerView(this) ?? UIColor.LightGray;
                _collectionController.UseTwoLineMode = del?.UseTwoLineModeForHorizontalPickerView(this) ?? false;

                _isInitialized = true;
                if (_collectionView != null && _collectionViewLayout != null)
                {
                    _collectionViewLayout.ActiveDistance = MaxCellWidth > 0 ? MaxCellWidth : (nfloat)Math.Floor(_collectionView.Bounds.Width);
                    _collectionViewLayout.MidX = (nfloat)Math.Ceiling(_collectionView.Bounds.GetMidX());
                    var numberOfElements = dataSource.NumberOfRowsInHorizontalPickerView(this);
                    _collectionViewLayout.LastElementIndex = numberOfElements - 1;

                    var firstElement = del.TitleForRow(this, 0);
                    var lastElement = del.TitleForRow(this, _collectionViewLayout.LastElementIndex);

                    var firstSize = StringHelper.SizeForText(firstElement, _collectionView.Bounds.Size, _collectionController.Font).Width / 2;
                    var lastSize = StringHelper.SizeForText(lastElement, _collectionView.Bounds.Size, _collectionController.Font).Width / 2;
                    _collectionViewLayout.SectionInset = new UIEdgeInsets(_collectionViewLayout.SectionInset.Top, _collectionViewLayout.MidX - firstSize, _collectionViewLayout.SectionInset.Bottom, _collectionViewLayout.MidX - lastSize);
                }
            }
            else
                return;
        }

        public int NumberOfRowsInCollectionViewController(YetCollectionViewController controller)
        {
            return DataSource?.NumberOfRowsInHorizontalPickerView(this) ?? 0;
        }

        public string TitleForRow(YetCollectionViewController controller, int row)
        {
            return Delegate?.TitleForRow(this, row) ?? string.Empty;
        }

        public void DidSelectRow(YetCollectionViewController controller, int row)
        {
            Delegate?.DidSelectRow(this, row);
        }
    }
}
