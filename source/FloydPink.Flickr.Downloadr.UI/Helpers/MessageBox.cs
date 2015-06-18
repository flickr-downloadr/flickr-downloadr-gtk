namespace FloydPink.Flickr.Downloadr.UI.Helpers {
    using Gtk;

    public class MessageBox {
        public static ResponseType Show(Window window, string message, ButtonsType buttons, MessageType type) {
            var md = new MessageDialog(window, DialogFlags.DestroyWithParent, type, buttons, false, message);
            var result = (ResponseType) md.Run();
            md.Destroy();
            return result;
        }
    }
}
