using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Soft_Walkman.Models;
using Windows.UI.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Soft_Walkman
{
    /// <summary>
    /// The Main and only page for the Walkman.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        private Models.CassetteTape cassetteTape;
        private Models.Walkman walkman;
        private DispatcherTimer cassetteNameScrollTimer;

        const double DEFAULT_MEDIAPLAYER_VOLUME_LEVEL = 5.0;
        const int DEFAULT_FF_RR_VALUE = 3;

        public MainPage()
        {
            this.InitializeComponent();
            this.cassetteTapeGif.AutoPlay = false;
            this.cassetteTrackListButton.IsEnabled = false;
            this.cassetteAlbumArtButton.IsEnabled = false;

            volumeSlider.Value = DEFAULT_MEDIAPLAYER_VOLUME_LEVEL;

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            ResumeTape();
        }

        private async void ResumeTape()
        {
            CassetteTapeState tapeState = new CassetteTapeState();

            if (tapeState.GetTapeSaved())
            {
                DisableFlyoutButtons();
                EnableUIButtons(false);

                walkman = new Models.Walkman();
                walkman.Volume(DEFAULT_MEDIAPLAYER_VOLUME_LEVEL);

                cassetteTape            = new Models.CassetteTape();
                cassetteTape.DirPath    = await StorageFolder.GetFolderFromPathAsync(tapeState.GetTapePath());
                cassetteTitleLabel.Text = cassetteTape.Title;

                await walkman.LoadCassetteTape(cassetteTape);

                walkman.MediaPlaybackList.MoveTo(tapeState.GetTapeTrackIndex());

                walkman.MediaPlayer.PlaybackSession.Position = tapeState.GetTapePostion();

                CassetteTrackListView.ItemsSource = await cassetteTape.Tracks();
                cassetteTrackListButton.IsEnabled = true;

                await DisplayAlbumArtAsync();

                EnableUIButtons(true);
            }
        }

        private void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            if (walkman != null && cassetteTape != null)
            {
                Debug.WriteLine("Saving tape state on exit...");
                CassetteTapeState cts = new CassetteTapeState(walkman, cassetteTape);
                cts.SaveTapeState();
            }
        }


        private async void OpenCassetteButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Eject tape. Clear cassette and reset walkman state.
            if (cassetteTape != null && walkman != null)
            {
                CassetteTapeState tapeState = new CassetteTapeState();
                tapeState.ClearTapeState();

                walkman.PlaySound("eject");
                walkman.Reset();
                cassetteTapeGif.Stop();

                DisableFlyoutButtons();

                if (cassetteNameScrollTimer != null)
                {
                    cassetteNameScrollTimer.Stop();
                }

                cassetteTitleLabel.Text = string.Empty;
            }

            walkman = new Models.Walkman();
            walkman.Volume(DEFAULT_MEDIAPLAYER_VOLUME_LEVEL);
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
                await walkman.LoadCassetteTape(cassetteTape);

                // Populate Track listview:
                CassetteTrackListView.ItemsSource = await cassetteTape.Tracks();
                cassetteTrackListButton.IsEnabled = true;

                await DisplayAlbumArtAsync();

                EnableUIButtons(false);
            }

            EnableUIButtons(true);
        }

        private void DisableFlyoutButtons()
        {
            AlbumArtImage.Source = null;
            cassetteAlbumArtButton.IsEnabled = false;

            CassetteTrackListView.ItemsSource = null;
            cassetteTrackListButton.IsEnabled = false;
        }

        private async Task DisplayAlbumArtAsync()
        {
            string albumCoverPath = await cassetteTape.FindCoverArt();

            if (albumCoverPath != null && albumCoverPath.Length > 0 && albumCoverPath != "" && albumCoverPath != " ")
            {
                StorageFile coverStorageFile = await StorageFile.GetFileFromPathAsync(albumCoverPath);

                using (var stream = await coverStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmap = new BitmapImage();

                    bitmap.SetSource(stream);

                    AlbumArtImage.Source = bitmap;
                }

                this.cassetteAlbumArtButton.IsEnabled = true;
            }

        }

        private void EnableUIButtons(bool enabled_state)
        {
            openButton.IsEnabled        = enabled_state;
            fastFowardButton.IsEnabled  = enabled_state;
            rewindButton.IsEnabled      = enabled_state;
            pauseButton.IsEnabled       = enabled_state;
            stopButton.IsEnabled        = enabled_state;
            play.IsEnabled              = enabled_state;
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
                    cassetteTapeGif.Play();
                    ScrollTapeName();
                }

                walkman.ChangeLightIndicator(lightIndicator, "play");

                EnableUIButtons(false);

                // Give some time for the Walkman sound to play before actually playing the tape.
                var delay = Task.Delay(2750);

                await delay;

                while (delay.Status == TaskStatus.Running)
                {
                    EnableUIButtons(false);
                }

                if (delay.Status == TaskStatus.RanToCompletion)
                {
                    walkman.Play();
                    EnableUIButtons(true);
                }
                else
                {
                    EnableUIButtons(true);
                }
            }
        }

        private void fastFowardButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null && cassetteTape != null)
            {
                walkman.FastForward(DEFAULT_FF_RR_VALUE);
            }
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null && cassetteTape != null)
            {
                walkman.Rewind(DEFAULT_FF_RR_VALUE);
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null && cassetteNameScrollTimer != null)
            {
                walkman.ChangeLightIndicator(lightIndicator, "pause");
                walkman.Pause();
                cassetteNameScrollTimer.Stop();
                this.cassetteTapeGif.Stop();

                CassetteTapeState tapeState = new CassetteTapeState(walkman, cassetteTape);
                tapeState.SaveTapeState();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (walkman != null && cassetteNameScrollTimer != null)
            {
                walkman.ChangeLightIndicator(lightIndicator, "stop");
                cassetteTape = null;
                walkman.Reset();
                cassetteTapeGif.Stop();
                cassetteNameScrollTimer.Stop();
                cassetteTitleLabel.Text = string.Empty;

                DisableFlyoutButtons();

                CassetteTapeState tapeState = new CassetteTapeState();
                tapeState.ClearTapeState();
            }
        }

        private void ChangeWalkmanVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (walkman != null && cassetteTape != null)
            {
                walkman.Volume(volumeSlider.Value);
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

            await cassetteTapeLoadErrorDialog.ShowAsync();
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
                    walkman.Volume(DEFAULT_MEDIAPLAYER_VOLUME_LEVEL);
                    walkman.PlaySound("open");

                    StorageFolder directoryPath = (StorageFolder)items[0];

                    if (directoryPath != null)
                    {
                        cassetteTape = new Models.CassetteTape
                        {
                            DirPath = directoryPath
                        };
                        cassetteTitleLabel.Text = cassetteTape.Title;
                        await walkman.LoadCassetteTape(cassetteTape);
                        CassetteTrackListView.ItemsSource = await cassetteTape.Tracks();
                        this.cassetteTrackListButton.IsEnabled = true;

                        await cassetteTape.FindCoverArt();
                    }
                }
            }

        }

        private void ScrollTapeName()
        {
            const int TICK_COUNT = 100;
            const int MAX_LABEL_LENGTH = 23;

            cassetteNameScrollTimer = new DispatcherTimer();

            if (cassetteTitleLabel.Text.Length > MAX_LABEL_LENGTH)
            {
                cassetteNameScrollTimer.Tick += async (ss, ee) =>
                {
                    if (cassetteNameScrollTimer.Interval.Ticks == TICK_COUNT)
                    {
                        //each time set the offset to scrollviewer.HorizontalOffset + 5
                        scrollviewer.ChangeView(scrollviewer.HorizontalOffset + 0.5, null, null, true);
                        //if the scrollviewer scrolls to the end, scroll it back to the start.
                        if (scrollviewer.HorizontalOffset == scrollviewer.ScrollableWidth)
                        {
                            await Task.Delay(700);
                            scrollviewer.ChangeView(-1, null, null, true);
                        }
                    }
                };

                cassetteNameScrollTimer.Interval = new TimeSpan(TICK_COUNT);

                cassetteNameScrollTimer.Start();
            }
        }

        private void TapeNameScrollViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            cassetteNameScrollTimer.Stop();
        }

        private void fastFowardButton_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            Debug.WriteLine("ff button is being held down...");
            if (walkman != null && cassetteTape != null)
            {
                walkman.FastForward(1);
            }

        }

        private void VolumeIncreased_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            if (walkman != null && cassetteTape != null)
            {
                walkman.Volume(volumeSlider.Value += 0.5);
            }
        }

        private void VolumeDecreased_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            if (walkman != null && cassetteTape != null)
            {
                walkman.Volume(volumeSlider.Value -= 0.5);
            }
        }
    }
}
