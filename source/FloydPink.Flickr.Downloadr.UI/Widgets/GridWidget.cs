namespace FloydPink.Flickr.Downloadr.UI.Widgets {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Gtk;
    using log4net;
    using Model;

    [ToolboxItem(true)]
    public partial class GridWidget : Bin {
        private readonly ILog Log;
        private IEnumerable<IGridWidgetItem> _items;
        private int _numberOfItemsInARow = 5;

        public GridWidget() {
            Build();
            this.Log = LogManager.GetLogger(GetType());
        }

        public bool DoNotFireSelectionChanged { get; set; }

        public int NumberOfItemsInARow { get { return this._numberOfItemsInARow; } set { this._numberOfItemsInARow = value; } }

        public IEnumerable AllItems { get { return this.vboxContainer.AllChildren; } }

        public IEnumerable<IGridWidgetItem> Items
        {
            get { return this._items ?? new List<IGridWidgetItem>(); }
            set
            {
                this._items = value;
                InitializeGrid();
            }
        }

        public event EventHandler OnSelectionChanged;

        private void OnSelectionChangedInternal(object sender, EventArgs e) {
            this.Log.Debug("OnSelectionChangedInternal");
            if (!DoNotFireSelectionChanged && OnSelectionChanged != null) {
                OnSelectionChanged.Invoke(sender, e);
            }
        }

        private HBox AddItemToRow(HBox hboxRow, int j, IGridWidgetItem item, string rowId) {
            this.Log.Debug("AddItemToRow");
            Box.BoxChild hboxChild;
            if (item != null) {
                var itemWidget = new PhotoWidget();
                itemWidget.Name = string.Format("{0}Image{1}", rowId, j);
                itemWidget.WidgetItem = item;
                itemWidget.SelectionChanged += OnSelectionChangedInternal;
                hboxRow.Add(itemWidget);
                hboxChild = ((Box.BoxChild) (hboxRow[itemWidget]));
            } else {
                var dummyImage = new Image();
                dummyImage.Name = string.Format("{0}Image{1}", rowId, j);
                hboxRow.Add(dummyImage);
                hboxChild = ((Box.BoxChild) (hboxRow[dummyImage]));
            }
            hboxRow.Homogeneous = true;
            hboxChild.Position = j;
            return hboxRow;
        }

        private void SetupRow(int i, IEnumerable<IGridWidgetItem> rowItems) {
            this.Log.Debug("SetupRow");
            var rowItemsList = rowItems as IList<IGridWidgetItem> ?? rowItems.ToList();
            var rowItemsCount = rowItemsList.Count();

            var rowId = string.Format("hboxRow{0}", i);
            var hboxRow = new HBox();
            hboxRow.Name = rowId;
            hboxRow.Spacing = 6;

            for (var j = 0; j < NumberOfItemsInARow; j++) {
                if (j < rowItemsCount) {
                    hboxRow = AddItemToRow(hboxRow, j, rowItemsList.ElementAt(j), rowId);
                } else {
                    hboxRow = AddItemToRow(hboxRow, j, null, rowId);
                }
            }

            Application.Invoke(delegate {
                                   this.vboxContainer.Add(hboxRow);
                                   var vboxChild = ((Box.BoxChild) (this.vboxContainer[hboxRow]));
                                   vboxChild.Position = i;
                                   vboxChild.Padding = 10;
                                   this.vboxContainer.ShowAll();
                               });
        }

        private void InitializeGrid() {
            this.Log.Debug("InitializeGrid");
            var widgetItemsList = Items as IList<IGridWidgetItem> ?? Items.ToList();
            var itemsCount = widgetItemsList.Count();
            var numberOfRows = itemsCount / NumberOfItemsInARow;
            if (itemsCount % NumberOfItemsInARow > 0) {
                numberOfRows += 1; // add an additional row for remainder of the images that won't reach full row
            }
            numberOfRows = numberOfRows < 3 ? 3 : numberOfRows; // render a minimum of 3 rows

            foreach (var child in this.vboxContainer.Children) {
                Application.Invoke(delegate { this.vboxContainer.Remove(child); });
            }

            if (itemsCount == 0) {
                return;
            }

            for (var i = 0; i < numberOfRows; i++) {
                var rowItems = widgetItemsList.Skip(i * NumberOfItemsInARow).Take(NumberOfItemsInARow);
                SetupRow(i, rowItems);
            }
        }
    }
}
