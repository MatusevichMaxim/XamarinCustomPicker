using CoreAnimation;
using CoreGraphics;
using CustomPicker.Cells;
using CustomPicker.Delegates;
using CustomPicker.Interfaces;
using CustomPicker.Layouts;
using Foundation;
using PureLayout.Net;
using System;
using System.Threading.Tasks;
using UIKit;

namespace YetHealth.IOS.UI
{
    public class YetPickerView : UIView, IYetCollectionProvider
    {
        private YetCollectionViewController _collectionController;
        private YetCollectionViewFlowlayout _collectionViewLayout;
        private CAShapeLayer _shapeLayer;
        private UICollectionView _collectionView;
        private bool _isInitialized;

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

            var layout = new YetCollectionViewFlowlayout();
            var maxElementWidth = Bounds.Width * HorizontalPickerViewConstants.maxLabelWidthFactor;
            _collectionController = new YetCollectionViewController(layout, this, maxElementWidth);
            _collectionController.CollectionView.RegisterClassForCell(typeof(YetCollectionViewCell), nameof(YetCollectionViewCell));
            _collectionController.CollectionView.BackgroundColor = UIColor.Clear;
            _collectionController.CollectionView.ShowsHorizontalScrollIndicator = false;
            _collectionController.ClearsSelectionOnViewWillAppear = false;

            _collectionView = _collectionController.CollectionView;
            _collectionView.AllowsMultipleSelection = false;
            _collectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddSubview(_collectionView);
            _collectionView.AutoPinEdgesToSuperviewEdges();

            _collectionViewLayout = _collectionController.Layout as YetCollectionViewFlowlayout;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Delegate?.PickerViewShouldMask(this) ?? false)
            {
                _shapeLayer = new CAShapeLayer
                {
                    Frame = Bounds,
                    ContentsScale = UIScreen.MainScreen.Scale
                };

                Layer.Mask = _shapeLayer;
                _shapeLayer.Path = ShapePathForFrame(Bounds).CGPath;
            }
        }

        public int SelectedRow() => _collectionController.selectedCellIndexPath.Row;

        public void SelectRow(int rowIndex, bool animated)
        {
            var indexPath = NSIndexPath.FromItemSection(rowIndex, 0);
            _collectionController.programmaticallySet = true;
            _collectionController.SelectRow(indexPath, animated);
        }

        public void ReloadAll()
        {
            _collectionController.CollectionView.ReloadData();
        }

        private UIBezierPath ShapePathForFrame(CGRect frame)
        {
            return UIBezierPath.FromRoundedRect(frame, UIRectCorner.AllCorners, HorizontalPickerViewConstants.pathCornerRadii);
        }

        private async void Adjust(IHorizontalPickerViewDelegate del, IHorizontalPickerViewDataSource dataSource)
        {
            if (del != null && dataSource != null && !_isInitialized)
            {
                _collectionController.font = del?.TextFontForHorizontalPickerView(this) ?? UIFont.PreferredBody;
                _collectionController.textColor = del?.TextColorForHorizontalPickerView(this) ?? UIColor.LightGray;
                _collectionController.useTwoLineMode = del?.UseTwoLineModeForHorizontalPickerView(this) ?? false;

                _isInitialized = true;
                var view = _collectionView;
                var layout = _collectionViewLayout;
                if (view != null && layout != null)
                {
                    layout.activeDistance = (nfloat)Math.Floor(view.Bounds.Width / 2);
                    layout.midX = (nfloat)Math.Ceiling(view.Bounds.GetMidX());
                    var numberOfElements = dataSource.NumberOfRowsInHorizontalPickerView(this);
                    layout.lastElementIndex = numberOfElements - 1;

                    var firstElement = del.TitleForRow(this, 0);
                    var lastElement = del.TitleForRow(this, layout.lastElementIndex);

                    var firstSize = _collectionController.SizeForText(firstElement, view.Bounds.Size).Width / 2;
                    var lastSize = _collectionController.SizeForText(lastElement, view.Bounds.Size).Width / 2;
                    layout.SectionInset = new UIEdgeInsets(layout.SectionInset.Top, layout.midX - firstSize, layout.SectionInset.Bottom, layout.midX - lastSize);

                    await Task.Delay(250);
                    view.SelectItem(_collectionController.selectedCellIndexPath, false, UICollectionViewScrollPosition.CenteredHorizontally);
                }
            }
            else
            {
                return;
            }
        }

        // HPCollectionVCProvider

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
