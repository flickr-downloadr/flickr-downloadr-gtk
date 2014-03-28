namespace FloydPink.Flickr.Downloadr.Model.Enums
{
    public static class PhotoSize
    {
        /*
            s	small square 75x75
            q	large square 150x150
            t	thumbnail, 100 on longest side
            m	small, 240 on longest side
            n	small, 320 on longest side
            -	medium, 500 on longest side
            z	medium 640, 640 on longest side
            c	medium 800, 800 on longest side†
            b	large, 1024 on longest side*
         */

        public static readonly string SmallSquare75X75 = "s";
        public static readonly string LargeSquare150X150 = "q";
        public static readonly string Thumbnail100 = "t";
        public static readonly string Small240 = "m";
        public static readonly string Small320 = "n";
        public static readonly string Medium500 = "-";
        public static readonly string Medium640 = "z";
        public static readonly string Medium800 = "c";
        public static readonly string Large1024 = "b";
    }
}