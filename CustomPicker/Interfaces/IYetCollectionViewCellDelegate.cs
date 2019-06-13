using CustomPicker.Cells;
using UIKit;

namespace CustomPicker.Interfaces
{
    public interface IYetCollectionViewCellDelegate
    {
        UIFont FontForCollectionViewCell(YetCollectionViewCell cvCell);
        UIColor TextColorForCollectionViewCell(YetCollectionViewCell cvCell);
        bool UseTwolineModeForCollectionViewCell(YetCollectionViewCell cvCell);
    }
}
