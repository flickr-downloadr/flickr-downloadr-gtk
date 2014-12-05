using System;
using System.Collections.Generic;
using System.Linq;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.UI.Widgets;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Windows {
    // This is another partial class module for the BrowserWindow class
    public partial class BrowserWindow {
        private const int NumberOfPhotosInARow = 5;

        private void OnSelectionChanged(object sender, EventArgs e) {
            Log.Debug("OnSelectionChanged");
            if (_doNotFireOnSelectionChanged) {
                return;
            }
            var cachedImage = (PhotoWidget) sender;

            if (!AllSelectedPhotos.ContainsKey(Page)) {
                AllSelectedPhotos[Page] = new Dictionary<string, Photo>();
            }

            if (cachedImage.IsSelected) {
                AllSelectedPhotos[Page].Add(cachedImage.Photo.Id, cachedImage.Photo);
            } else {
                AllSelectedPhotos[Page].Remove(cachedImage.Photo.Id);
            }

            UpdateSelectionButtons();
        }

        private HBox AddImageToRow(HBox hboxPhotoRow, int j, Photo photo, string rowId) {
            Log.Debug("AddImageToRow");
            Box.BoxChild hboxChild;
            if (photo != null) {
                var imageCell = new PhotoWidget();
                imageCell.Name = string.Format("{0}Image{1}", rowId, j);
                imageCell.ImageUrl = photo.LargeSquare150X150Url;
                imageCell.Photo = photo;
                imageCell.SelectionChanged += OnSelectionChanged;
                hboxPhotoRow.Add(imageCell);
                hboxChild = ((Box.BoxChild) (hboxPhotoRow[imageCell]));
            } else {
                var dummyImage = new Image();
                dummyImage.Name = string.Format("{0}Image{1}", rowId, j);
                hboxPhotoRow.Add(dummyImage);
                hboxChild = ((Box.BoxChild) (hboxPhotoRow[dummyImage]));
            }
            hboxPhotoRow.Homogeneous = true;
            hboxChild.Position = j;
            return hboxPhotoRow;
        }

        private void SetupTheImageRow(int i, IEnumerable<Photo> rowPhotos) {
            Log.Debug("SetupTheImageRow");
            var rowPhotosCount = rowPhotos.Count();

            var rowId = string.Format("hboxPhotoRow{0}", i);
            var hboxPhotoRow = new HBox();
            hboxPhotoRow.Name = rowId;
            hboxPhotoRow.Spacing = 6;

            for (var j = 0; j < NumberOfPhotosInARow; j++) {
                if (j < rowPhotosCount) {
                    hboxPhotoRow = AddImageToRow(hboxPhotoRow, j, rowPhotos.ElementAt(j), rowId);
                } else {
                    hboxPhotoRow = AddImageToRow(hboxPhotoRow, j, null, rowId);
                }
            }

            Application.Invoke(delegate {
                                   this.vboxPhotos.Add(hboxPhotoRow);
                                   var vboxChild = ((Box.BoxChild) (this.vboxPhotos[hboxPhotoRow]));
                                   vboxChild.Position = i;
                                   vboxChild.Padding = 10;
                                   this.vboxPhotos.ShowAll();
                               });
        }

        private void SetupTheImageGrid(IEnumerable<Photo> photos) {
            Log.Debug("SetupTheImageGrid");
            var numberOfRows = photos.Count() / NumberOfPhotosInARow;
            if (photos.Count() % NumberOfPhotosInARow > 0) {
                numberOfRows += 1; // add an additional row for remainder of the images that won't reach full row
            }
            numberOfRows = numberOfRows < 3 ? 3 : numberOfRows; // render a minimum of 3 rows

            foreach (var child in vboxPhotos.Children) {
                Application.Invoke(delegate { this.vboxPhotos.Remove(child); });
            }

            for (var i = 0; i < numberOfRows; i++) {
                var rowPhotos = photos.Skip(i * NumberOfPhotosInARow).Take(NumberOfPhotosInARow);
                SetupTheImageRow(i, rowPhotos);
            }
        }

        private void SetSelectionOnAllImages(bool selected) {
            Log.Debug("SetSelectionOnAllImages");
            foreach (var box in vboxPhotos.AllChildren) {
                var hbox = box as HBox;
                if (hbox == null) {
                    continue;
                }
                foreach (var image in hbox.AllChildren) {
                    var cachedImage = image as PhotoWidget;
                    if (cachedImage != null) {
                        cachedImage.IsSelected = selected;
                    }
                }
            }
        }

        private void FindAndSelectPhoto(Photo photo) {
            Log.Debug("FindAndSelectPhoto");
            foreach (var box in vboxPhotos.AllChildren) {
                var hbox = box as HBox;
                if (hbox == null) {
                    continue;
                }
                foreach (var image in hbox.AllChildren) {
                    var cachedImage = image as PhotoWidget;
                    if (cachedImage != null && cachedImage.Photo.Id == photo.Id) {
                        cachedImage.IsSelected = true;
                        return;
                    }
                }
            }
        }

        private void SelectPhotos(List<Photo> photos) {
            Log.Debug("SelectPhotos");
            foreach (var photo in photos) {
                FindAndSelectPhoto(photo);
            }
        }
    }
}
