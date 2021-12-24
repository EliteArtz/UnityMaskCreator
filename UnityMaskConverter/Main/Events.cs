using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static UnityMaskCreator.Channels.Channels;

namespace UnityMaskCreator
{
    public partial class Main
    {
        public Main() => InitializeComponent();


        /* Set Image for the channels separately */
        private void _metallicTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(_metallicTextBox, _metallicImageBox, LayerChannel("metallic")); //(Channel)_metallicChannel.SelectedItem
        private void _aoTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(_aoTextBox, _aoImageBox, LayerChannel("ao"));
        private void _heightTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(_heightTextBox, _heightImageBox, LayerChannel("height"));
        private void _snTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(_snTextBox, _snImageBox, LayerChannel("smoothness"));



        /* Clear the Inputs specifically */
        private void _clearMetallic_Click(object sender, RoutedEventArgs e) => ClearInput(_metallicTextBox, _metallicImageBox, LayerChannel("metallic"));
        private void _clearAO_Click(object sender, RoutedEventArgs e) => ClearInput(_aoTextBox, _aoImageBox, LayerChannel("ao"));
        private void _clearHeight_Click(object sender, RoutedEventArgs e) => ClearInput(_heightTextBox, _heightImageBox, LayerChannel("height"));
        private void _clearSN_Click(object sender, RoutedEventArgs e) => ClearInput(_snTextBox, _snImageBox, LayerChannel("smoothness"));


        // Settings Window
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Settings().Show();
        }




        /* Final Trigger - Create the mask texture */
        private async void _createMask_Click(object sender, RoutedEventArgs e)
        {
            if (images.Count < 1)
            {
                _ = MessageBox.Show("Please select at least one image", System.Windows.Forms.Application.ProductName);
                return;
            }

            CreateMask.IsEnabled = active = !CreateMask.IsEnabled;
            await Task.Run(() =>
            {
                for (int i = 0; i < images.Count; i++) { _ = CreateMaskImg(images[i], i); }
                GC.Collect();
            });
            CreateMask.IsEnabled = active = !CreateMask.IsEnabled;

            // Open the folder where it got saved to spare some search time.
            _ = Process.Start("explorer.exe", $@"{Directory.GetCurrentDirectory()}\output\");

            // Finally clear data for the next mask to create.
            Array.Clear(_pixelsFinal, 0, _pixelsFinal.Length);
            Array.Clear(_colorData, 0, _colorData.Length);
        }
    }
}
