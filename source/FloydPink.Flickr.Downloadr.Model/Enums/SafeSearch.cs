namespace FloydPink.Flickr.Downloadr.Model.Enums {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class SafeSearch {
        public static readonly string Safe = "1";
        public static readonly string Moderate = "2";
        public static readonly string Restricted = "3";

        public static List<string> GetItems() {
            return typeof (SafeSearch).GetFields(BindingFlags.Public | BindingFlags.Static).
                                       Select(field => field.Name).ToList();
        }

        public static string GetValue(string fieldName) {
            return typeof (SafeSearch).GetField(fieldName).GetValue(null).ToString();
        }
    }
}
