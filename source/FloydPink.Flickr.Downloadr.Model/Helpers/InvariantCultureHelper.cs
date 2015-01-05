namespace FloydPink.Flickr.Downloadr.Model.Helpers {
    using System;
    using System.Globalization;
    using System.Threading;

    public static class InvariantCultureHelper {
        public static void PerformInInvariantCulture(Action action) {
            var before = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                action();
            }

            finally {
                Thread.CurrentThread.CurrentCulture = before;
            }
        }
    }
}
