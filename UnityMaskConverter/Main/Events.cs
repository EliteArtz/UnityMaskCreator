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
    }
}
