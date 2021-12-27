using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static UnityMaskCreator.Channels.Channels;

namespace UnityMaskCreator
{
    public partial class Main
    {
        private bool selectionChanged = false;

        public Main()
        {
            InitializeComponent();
            ChannelComboBoxes().ForEach(ChannelComboBox => ChannelComboBox.SelectionChanged += Channel_SelectionChanged);
        }

        private List<ComboBox> ChannelComboBoxes()
        {
            return new List<ComboBox>() { _metallicChannel, _aoChannel, _heightChannel, _snChannel };
        }


        /* Set Image for the channels separately */
        private void _metallicTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(sender, _metallicImageBox, (Channel)_metallicChannel.SelectedItem);
        private void _aoTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(sender, _aoImageBox, (Channel)_aoChannel.SelectedItem);
        private void _heightTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(sender, _heightImageBox, (Channel)_heightChannel.SelectedItem);
        private void _snTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => FileDialog(sender, _snImageBox, (Channel)_snChannel.SelectedItem);



        /* Clear the Inputs specifically */
        private void _clearMetallic_Click(object sender, RoutedEventArgs e) => ClearInput(_metallicTextBox, _metallicImageBox, (Channel)_metallicChannel.SelectedItem);
        private void _clearAO_Click(object sender, RoutedEventArgs e) => ClearInput(_aoTextBox, _aoImageBox, (Channel)_aoChannel.SelectedItem);
        private void _clearHeight_Click(object sender, RoutedEventArgs e) => ClearInput(_heightTextBox, _heightImageBox, (Channel)_heightChannel.SelectedItem);
        private void _clearSN_Click(object sender, RoutedEventArgs e) => ClearInput(_snTextBox, _snImageBox, (Channel)_snChannel.SelectedItem);


        // Settings Window
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e) => new Settings().Show();


        // Color channel destinations handling
        private void Channel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!selectionChanged) //Prevent to rerun when the other combobox got changed
            {
                selectionChanged = true;

                ComboBox comboBox = (ComboBox)sender;
                List<ComboBox> comboBoxes = ChannelComboBoxes();

                Channel[] channels = (Channel[])Enum.GetValues(typeof(Channel)); //Every enum Color Channel possible
                Channel unusedChannel = new Channel();

                foreach (Channel channel in channels) // get the unused color channel, aka. the previous color channel selected
                {
                    if (Array.FindIndex(comboBoxes.ToArray(), comboBox1 => (Channel)comboBox1.SelectedItem == channel) != -1) continue;

                    unusedChannel = channel;
                    break;
                }

                foreach (ComboBox comboBox1 in comboBoxes) // change color destination for opposing combobox that already had the new color channel selected
                {
                    if (!(comboBox.Name != comboBox1.Name && (Channel)comboBox.SelectedItem == (Channel)comboBox1.SelectedItem)) continue;

                    comboBox1.SelectedItem = unusedChannel;

                    SwitchColorChannelDestinations(images, unusedChannel, (Channel)comboBox.SelectedItem);

                }
            } 
            else selectionChanged = false;
        }


        /* Final Trigger - Create the mask texture */
        private async void _createMask_Click(object sender, RoutedEventArgs e)
        {
            if (images.Count < 1)
            {
                MessageBoxExt.Show("Please select at least one image.", Title);
                return;
            }

            CreateMask.IsEnabled = active = !CreateMask.IsEnabled;
            await Task.Run(() =>
            {
                for (int i = 0; i < images.Count; i++) 
                    CreateMaskImg(images[i], i);
                GC.Collect();
            });
            CreateMask.IsEnabled = active = !CreateMask.IsEnabled;

            // Open the folder where it got saved to spare some search time.
            _ = Process.Start("explorer.exe", $@"{Directory.GetCurrentDirectory()}\output\");

            // Finally clear data for the next mask to create.
            DisposeData();
        }
    }
}
