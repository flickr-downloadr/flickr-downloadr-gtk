namespace FloydPink.Flickr.Downloadr.Model {
    using System.Collections.Generic;

    public class PhotosResponse {
        public PhotosResponse(int page, int pages, int perPage, int total, IEnumerable<Photo> photos) {
            Page = page;
            Pages = pages;
            PerPage = perPage;
            Total = total;
            Photos = photos;
        }

        public int Page { get; private set; }
        public int Pages { get; private set; }
        public int PerPage { get; private set; }
        public int Total { get; private set; }
        public IEnumerable<Photo> Photos { get; private set; }
    }
}
