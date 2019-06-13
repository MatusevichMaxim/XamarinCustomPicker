using CustomPicker.Delegates;

namespace CustomPicker.Interfaces
{
    public interface IYetCollectionProvider
    {
        int NumberOfRowsInCollectionViewController(YetCollectionViewController controller);
        string TitleForRow(YetCollectionViewController controller, int row);
        void DidSelectRow(YetCollectionViewController controller, int row);
    }
}
