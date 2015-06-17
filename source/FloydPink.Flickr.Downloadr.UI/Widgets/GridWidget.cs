namespace FloydPink.Flickr.Downloadr.UI.Widgets {
    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Gdk;
    using Gtk;
    using log4net;
    using Model;

    [ToolboxItem(true)]
    public partial class GridWidget : Bin {
        private readonly ILog Log;

        private int _numberOfItemsInARow = 5;

        private IEnumerable<IGridWidgetItem> _items;

        public GridWidget() {
            Build();
            Log = LogManager.GetLogger(GetType());
        }

        public event EventHandler OnSelectionChanged;

        public int NumberOfItemsInARow {
            get {
                return _numberOfItemsInARow;
            }
            set {
                _numberOfItemsInARow = value;
            }
        }

        public IEnumerable AllItems { get { return this.vboxContainer.AllChildren; } }

        public IEnumerable<IGridWidgetItem> Items {
            get {
                return _items ?? new List<IGridWidgetItem>();
            }
            set {
                _items = value;
                InitializeGrid();
            }
        }

        private HBox AddItemToRow(HBox hboxRow, int j, IGridWidgetItem item, string rowId) {
            Log.Debug("AddImageToRow");
            Box.BoxChild hboxChild;
            if (item != null) {
                var imageCell = new PhotoWidget();
                imageCell.Name = string.Format("{0}Image{1}", rowId, j);
                imageCell.ImageUrl = item.WidgetThumbnailUrl;
                imageCell.Photo = item;
                imageCell.SelectionChanged += OnSelectionChanged;
                hboxRow.Add(imageCell);
                hboxChild = ((Box.BoxChild)(hboxRow[imageCell]));
            } else {
                var dummyImage = new Gtk.Image();
                dummyImage.Name = string.Format("{0}Image{1}", rowId, j);
                hboxRow.Add(dummyImage);
                hboxChild = ((Box.BoxChild)(hboxRow[dummyImage]));
            }
            hboxRow.Homogeneous = true;
            hboxChild.Position = j;
            return hboxRow;
        }

        private void SetupRow(int i, IEnumerable<IGridWidgetItem> rowItems) {
            Log.Debug("SetupTheImageRow");
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
                var vboxChild = ((Box.BoxChild)(this.vboxContainer[hboxRow]));
                vboxChild.Position = i;
                vboxChild.Padding = 10;
                this.vboxContainer.ShowAll();
            });
        }

        private void InitializeGrid() {
            Log.Debug("SetupTheImageGrid");
            var widgetItemsList = this.Items as IList<IGridWidgetItem> ?? this.Items.ToList();
            var itemsCount = widgetItemsList.Count();
            var numberOfRows = itemsCount / NumberOfItemsInARow;
            if (itemsCount % NumberOfItemsInARow > 0) {
                numberOfRows += 1; // add an additional row for remainder of the images that won't reach full row
            }
            numberOfRows = numberOfRows < 3 ? 3 : numberOfRows; // render a minimum of 3 rows

            foreach (var child in this.vboxContainer.Children) {
                Application.Invoke(delegate {
                    this.vboxContainer.Remove(child);
                });
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

