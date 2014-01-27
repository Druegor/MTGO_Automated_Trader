using System.Drawing;
using BusinessLogicLayer;

namespace AutoItBot.Updater
{
    public class CollectionExporter
    {
        private int _currentSet = (int)CardSet.ScarsOfMirrodin;
        private Point CollectionTab = new Point(261, 1144);
        private Point SetSelector = new Point(802, 17);
        private Point ContextMenu = new Point(532, 182);
        private Point SelectAllOption = new Point(597, 190);
        private Point ExportCsv = new Point(637, 216);

        public void Export(CardSet cardSet)
        {
            AutoItX.MouseClick(CollectionTab);
            AutoItX.Sleep(2000);

            this._currentSet = Selector.PickFilterOption(this._currentSet, (int)cardSet, SetSelector);

            AutoItX.Sleep(2000);
            AutoItX.MouseClick("right", ContextMenu.X, ContextMenu.Y);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(SelectAllOption);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick("right", ContextMenu.X, ContextMenu.Y);
            AutoItX.Sleep(1000);
            AutoItX.MouseClick(ExportCsv);
            AutoItX.Sleep(1000);
            AutoItX.Send(cardSet.ToString());
            AutoItX.Sleep(1000);
            AutoItX.Send("{ENTER}");
        }
    }
}
