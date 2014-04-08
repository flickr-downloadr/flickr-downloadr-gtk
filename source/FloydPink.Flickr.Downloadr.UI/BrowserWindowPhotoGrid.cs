using System;
using FloydPink.Flickr.Downloadr.Model;
using System.Collections.Generic;
using System.Linq;
using Gtk;

namespace FloydPink.Flickr.Downloadr
{
	public partial class BrowserWindow
	{
		private const int NUMBER_OF_PHOTOS_IN_A_ROW = 5;

		void OnSelectionChanged (object sender, EventArgs e)
		{
			if (_doNotFireOnSelectionChanged) {
				return;
			}
			var cachedImage = (CachedImage)sender;

			if (!AllSelectedPhotos.ContainsKey (Page)) {
				AllSelectedPhotos [Page] = new Dictionary<string, Photo> ();
			}

			if (cachedImage.IsSelected) {
				AllSelectedPhotos [Page].Add (cachedImage.Photo.Id, cachedImage.Photo);
			} else {
				AllSelectedPhotos [Page].Remove (cachedImage.Photo.Id);
			}

			UpdateButtons ();
		}

		HBox AddImageToRow (HBox hboxPhotoRow, int j, Photo photo, string rowId)
		{
			Box.BoxChild hboxChild;
			if (photo != null) {
				var imageCell = new CachedImage ();
				imageCell.Name = string.Format ("{0}Image{1}", rowId, j.ToString ());
				imageCell.ImageUrl = photo.LargeSquare150X150Url;
				imageCell.Photo = photo;
				imageCell.SelectionChanged += OnSelectionChanged;
				hboxPhotoRow.Add (imageCell);
				hboxChild = ((Box.BoxChild)(hboxPhotoRow [imageCell]));
			} else {
				var dummyImage = new Image ();
				dummyImage.Name = string.Format ("{0}Image{1}", rowId, j.ToString ());
				hboxPhotoRow.Add (dummyImage);
				hboxChild = ((Box.BoxChild)(hboxPhotoRow [dummyImage]));
			}
			hboxPhotoRow.Homogeneous = true;
			hboxChild.Position = j;
			return hboxPhotoRow;
		}

		void SetupTheImageRow (int i, IEnumerable<Photo> rowPhotos)
		{
			var rowPhotosCount = rowPhotos.Count ();

			var rowId = string.Format ("hboxPhotoRow{0}", i.ToString ());
			var hboxPhotoRow = new global::Gtk.HBox ();
			hboxPhotoRow.Name = rowId;
			hboxPhotoRow.Spacing = 6;

			for (int j = 0; j < NUMBER_OF_PHOTOS_IN_A_ROW; j++) {
				if (j < rowPhotosCount) {
					hboxPhotoRow = AddImageToRow (hboxPhotoRow, j, rowPhotos.ElementAt (j), rowId);
				} else {
					hboxPhotoRow = AddImageToRow (hboxPhotoRow, j, null, rowId);
				}
			}

			this.vboxPhotos.Add (hboxPhotoRow);
			Box.BoxChild vboxChild = ((Box.BoxChild)(this.vboxPhotos [hboxPhotoRow]));
			vboxChild.Position = i;
			vboxChild.Padding = (uint)10;
			vboxPhotos.ShowAll ();
		}

		void SetupTheImageGrid (IEnumerable<Photo> photos)
		{
			var numberOfRows = photos.Count () / NUMBER_OF_PHOTOS_IN_A_ROW;
			if (photos.Count () % NUMBER_OF_PHOTOS_IN_A_ROW > 0) {
				numberOfRows += 1;	// add an additional row for remainder of the images that won't reach full row
			}
			numberOfRows = numberOfRows < 3 ? 3 : numberOfRows;	// render a minimum of 3 rows

			foreach (Widget child in this.vboxPhotos.Children) {
				this.vboxPhotos.Remove (child);
			}

			for (int i = 0; i < numberOfRows; i++) {
				var rowPhotos = photos.Skip (i * NUMBER_OF_PHOTOS_IN_A_ROW).Take (NUMBER_OF_PHOTOS_IN_A_ROW);
				SetupTheImageRow (i, rowPhotos);
			}
		}

		void SetSelectionOnAllImages (bool selected)
		{
			foreach (var box in vboxPhotos.AllChildren) {
				var hbox = box as HBox;
				if (hbox == null) {
					continue;
				}
				foreach (var image in hbox.AllChildren) {
					var cachedImage = image as CachedImage;
					if (cachedImage != null) {
						cachedImage.IsSelected = selected;
					}
				}
			}
		}

		void FindAndSelectPhoto (Photo photo)
		{
			foreach (var box in vboxPhotos.AllChildren) {
				var hbox = box as HBox;
				if (hbox == null) {
					continue;
				}
				foreach (var image in hbox.AllChildren) {
					var cachedImage = image as CachedImage;
					if (cachedImage != null && cachedImage.Photo.Id == photo.Id) {
						cachedImage.IsSelected = true;
						return;
					}
				}
			}
		}

		void SelectPhotos (List<Photo> photos)
		{
			foreach (var photo in photos) {
				FindAndSelectPhoto (photo);
			}
		}
	}
}

