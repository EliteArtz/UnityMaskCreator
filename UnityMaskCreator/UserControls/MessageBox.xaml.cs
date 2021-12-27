using System.Windows;
using System.Windows.Controls;

namespace UnityMaskCreator
{
    public enum DialogButton
    {
        Ok,
        OkCancel
    };
    public partial class MessageBox : UserControl
    {
        public Window window;
        public MessageBox() => InitializeComponent();


        private void Ok_Btn_Click(object sender, RoutedEventArgs e) => window.DialogResult = true;
        private void Cancel_Btn_Click(object sender, RoutedEventArgs e) => window.DialogResult = false;
    }
    public static class MessageBoxExt
    {
        static readonly MessageBox MsgBox = new MessageBox();

        public static bool? Show(string msg, string title = "",DialogButton dialogButton=DialogButton.Ok)
        {
            if (dialogButton == DialogButton.OkCancel)
                MsgBox.Cancel_Btn.Visibility = Visibility.Visible;

            MsgBox.Message.Text = msg;
            MsgBox.window = new Window()
            {
                Title = title,
                Content = MsgBox,
                MinWidth = MsgBox.MinWidth,
                MinHeight = MsgBox.MinHeight,

                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStyle = WindowStyle.SingleBorderWindow,
                ResizeMode = ResizeMode.NoResize,

                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            return MsgBox.window.ShowDialog();
        }
    }

}
