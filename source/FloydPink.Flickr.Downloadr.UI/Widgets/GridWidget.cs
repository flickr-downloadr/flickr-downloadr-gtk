using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FloydPink.Flickr.Downloadr.Model;
using Gtk;
using log4net;

namespace FloydPink.Flickr.Downloadr.UI.Widgets
{
  [ToolboxItem(true)]
  public partial class GridWidget : Bin
  {
    private readonly ILog Log;
    private IEnumerable<IGridWidgetItem> _items;

    public GridWidget()
    {
      Build();
      Log = LogManager.GetLogger(GetType());
    }

    public bool DoNotFireSelectionChanged { get; set; }

    public int NumberOfItemsInARow { get; set; } = 5;

    public IEnumerable AllItems
    {
      get
      {
        return vboxContainer.AllChildren;
      }
    }

    public IEnumerable<IGridWidgetItem> Items
    {
      get
      {
        return _items ?? new List<IGridWidgetItem>();
      }
      set
      {
        _items = value;
        InitializeGrid();
      }
    }

    public event EventHandler OnSelectionChanged;

    private void OnSelectionChangedInternal(object sender, EventArgs e)
    {
      Log.Debug("OnSelectionChangedInternal");
      if (!DoNotFireSelectionChanged && (OnSelectionChanged != null))
      {
        OnSelectionChanged.Invoke(sender, e);
      }
    }

    private HBox AddItemToRow(HBox hboxRow, int j, IGridWidgetItem item, string rowId)
    {
      Log.Debug("AddItemToRow");
      Box.BoxChild hboxChild;
      if (item != null)
      {
        var itemWidget = new PhotoWidget();
        itemWidget.Name = string.Format("{0}Image{1}", rowId, j);
        itemWidget.WidgetItem = item;
        itemWidget.SelectionChanged += OnSelectionChangedInternal;
        hboxRow.Add(itemWidget);
        hboxChild = (Box.BoxChild) hboxRow[itemWidget];
      }
      else
      {
        var dummyImage = new Image();
        dummyImage.Name = string.Format("{0}Image{1}", rowId, j);
        hboxRow.Add(dummyImage);
        hboxChild = (Box.BoxChild) hboxRow[dummyImage];
      }
      hboxRow.Homogeneous = true;
      hboxChild.Position = j;
      return hboxRow;
    }

    private void SetupRow(int i, IEnumerable<IGridWidgetItem> rowItems)
    {
      Log.Debug("SetupRow");
      var rowItemsList = rowItems as IList<IGridWidgetItem> ?? rowItems.ToList();
      var rowItemsCount = rowItemsList.Count();

      var rowId = string.Format("hboxRow{0}", i);
      var hboxRow = new HBox();
      hboxRow.Name = rowId;
      hboxRow.Spacing = 6;

      for (var j = 0; j < NumberOfItemsInARow; j++)
      {
        if (j < rowItemsCount)
        {
          hboxRow = AddItemToRow(hboxRow, j, rowItemsList.ElementAt(j), rowId);
        }
        else
        {
          hboxRow = AddItemToRow(hboxRow, j, null, rowId);
        }
      }

      Application.Invoke(delegate
      {
        vboxContainer.Add(hboxRow);
        var vboxChild = (Box.BoxChild) vboxContainer[hboxRow];
        vboxChild.Position = i;
        vboxChild.Padding = 10;
        vboxContainer.ShowAll();
      });
    }

    private void InitializeGrid()
    {
      Log.Debug("InitializeGrid");
      var widgetItemsList = Items as IList<IGridWidgetItem> ?? Items.ToList();
      var itemsCount = widgetItemsList.Count();
      var numberOfRows = itemsCount/NumberOfItemsInARow;
      if (itemsCount%NumberOfItemsInARow > 0)
      {
        numberOfRows += 1; // add an additional row for remainder of the images that won't reach full row
      }
      numberOfRows = numberOfRows < 3 ? 3 : numberOfRows; // render a minimum of 3 rows

      foreach (var child in vboxContainer.Children)
      {
        Application.Invoke(delegate { vboxContainer.Remove(child); });
      }

      if (itemsCount == 0)
      {
        return;
      }

      for (var i = 0; i < numberOfRows; i++)
      {
        var rowItems = widgetItemsList.Skip(i*NumberOfItemsInARow).Take(NumberOfItemsInARow);
        SetupRow(i, rowItems);
      }
    }
  }
}
