namespace FloydPink.Flickr.Downloadr.Model {
    using System.Collections.Generic;

    public class PhotosetsResponse {
        public PhotosetsResponse(int page, int pages, int perPage, int total, IEnumerable<Photoset> photosets) {
            Page = page;
            Pages = pages;
            PerPage = perPage;
            Total = total;
            Photosets = photosets;
        }

        public int Page { get; private set; }
        public int Pages { get; private set; }
        public int PerPage { get; private set; }
        public int Total { get; private set; }
        public IEnumerable<Photoset> Photosets { get; set; }
    }
}
