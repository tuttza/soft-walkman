using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.ViewManagement;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Soft_Walkman
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Models.CassetteTape cassetteTape;
        private Models.Walkman walkman;
        private DispatcherTimer cassetteNameScrollTimer;

        public MainPage()
        {
            this.InitializeComponent();
            this.cassetteTapeGif.AutoPlay = false;
            this.cassetteTrackListButton.IsEnabled = false;
        }

        private async void OpenCassetteButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Eject tape. Clear cassette and reset walkman state.
            if (cassetteTape != null && walkman != null)
            {
                walkman.PlaySound("eject");
                walkman.Reset();
                this.cassetteTapeGif.Stop();
            }

            walkman = new Models.Walkman(); 
            walkman.PlaySound("open");

            FolderPicker fp = new FolderPicker
            {
                CommitButtonText = "Load Cassette Tape",
                SuggestedStartLocation = PickerLocationId.MusicLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            fp.FileTypeFilter.Add(".");
            StorageFolder directoryPath = await fp.PickSingleFolderAsync();

            if (directoryPath != null)
            {
                cassetteTape = new Models.CassetteTape();
                cassetteTape.DirPath = directoryPath;
                cassetteTitleLabel.Text = cassetteTape.Title;
                walkman.LoadCassetteTape(cassetteTape);
                CassetteTrackListView.ItemsSource = await cassetteTape.Tracks();
                this.cassetteTrackListButton.IsEnabled = true;
            }
        }

        private async void Play_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (cassetteTape == null || walkman == null)
            {
                ErrorDialog("Have you loaded your tape yet?", "Click the open button(+) to load your cassette tape.");
            }
            else if (cassetteTape != null && await cassetteTape.MediaSizeAsync() > 0)
            {
                //
                // Make sure walkman system sounds can't play while audio is already playing.
                //
                if (walkman.MediaPlayer.PlaybackSession.PlaybackState != Windows.Media.Playback.MediaPlaybackState.Playing)
                {
                    walkman.PlaySound("play");
                    this.cassetteTapeGif.Play();
                    ScrollTapeName();
                }

                walkman.ChangeLightIndicator(lightIndicator, "play");               
                await Task.Delay(2750);
                walkman.Play();
            }
        }

        private void fastFowardButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null)
            {
                walkman.FastForward(10);
            }
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null)
            {
                walkman.Rewind(10);
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null)
            {
                walkman.ChangeLightIndicator(lightIndicator, "pause");
                walkman.Pause();
                this.cassetteTapeGif.Stop();
                cassetteNameScrollTimer.Stop();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null)
            {
                walkman.ChangeLightIndicator(lightIndicator, "stop");
                cassetteTape = null;
                walkman.Reset();
                cassetteTapeGif.Stop();
            }
        }

        private void ChangeWalkmanVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            Debug.WriteLine($"volume changed to {volumeSlider.Value}");

            if (walkman != null)
            {
                Debug.WriteLine($"Current MediaPlayer Volume set at: {walkman.MediaPlayer.Volume.ToString()}");
                walkman.MediaPlayer.Volume = volumeSlider.Value;
            }
        }

        private async void ErrorDialog(string title, string content)
        {
            ContentDialog cassetteTapeLoadErrorDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await cassetteTapeLoadErrorDialog.ShowAsync();
        }

        private void Tape_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }

        private async void Tape_DragDrop(object sender, DragEventArgs e)
        {
            if (cassetteTape != null)
            {
                return;
            }

            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    walkman = new Models.Walkman();
                    walkman.PlaySound("open");

                    StorageFolder directoryPath = (StorageFolder)items[0];

                    if (directoryPath != null)
                    {
                        cassetteTape = new Models.CassetteTape();
                        cassetteTape.DirPath = directoryPath;
                        cassetteTitleLabel.Text = cassetteTape.Title;
                        walkman.LoadCassetteTape(cassetteTape);
                        CassetteTrackListView.ItemsSource = await cassetteTape.Tracks();
                        this.cassetteTrackListButton.IsEnabled = true;
                    }
                }
            }

        }

        private void ScrollTapeName()
        {
            const int TICK_NUMBER = 350;
            cassetteNameScrollTimer = new DispatcherTimer();

            if (cassetteTitleLabel.Text.Length > 23)
            {
                cassetteNameScrollTimer.Tick += (ss, ee) =>
                {
                    if (cassetteNameScrollTimer.Interval.Ticks == TICK_NUMBER)
                    {
                        //each time set the offset to scrollviewer.HorizontalOffset + 1
                        scrollviewer.ChangeView(scrollviewer.HorizontalOffset + 1, null, null, true);
                        //if the scrollviewer scrolls to the end, scroll it back to the start.
                        if (scrollviewer.HorizontalOffset == scrollviewer.ScrollableWidth)
                        {
                            scrollviewer.ChangeView(0, null, null, true);
                        }

                    }
                };
                cassetteNameScrollTimer.Interval = new TimeSpan(TICK_NUMBER);
                cassetteNameScrollTimer.Start();
            }
        }

    }
}
