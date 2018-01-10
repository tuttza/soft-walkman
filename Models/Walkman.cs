﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Soft_Walkman.Models
{
    public sealed class Walkman
    {
        public MediaSource MediaSource { get; }
        public MediaPlayer MediaPlayer { get; }
        public MediaPlaybackList MediaPlaybackList { get; }
        public MediaPlaybackItem MediaPlaybackItem;
        public CassetteTape CassetteTape { get; set; }

        private MediaElement WalkmanSound { get; }
        private StorageFolder AppDirectory;

        public enum WalkmanState : ushort {Playing, Paused, Stopped};

        public Walkman()
        {
            MediaPlayer = new MediaPlayer();
            MediaPlaybackList = new MediaPlaybackList();
            WalkmanSound = new MediaElement();
            AppDirectory = Windows.ApplicationModel.Package.Current.InstalledLocation;

            // Disable SystemMediaTransportControls
            MediaPlayer.CommandManager.IsEnabled = false;
        }

        public async void LoadCassetteTape(CassetteTape ct)
        {
            CassetteTape = ct;

            foreach (StorageFile sf in await CassetteTape.MediaFiles)
            {
                MediaPlaybackItem = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(sf));
                MediaPlaybackList.Items.Add(MediaPlaybackItem);
            }

            MediaPlayer.Source = MediaPlaybackList;
        }

        public void ChangeTape()
        {
            MediaPlayer.Dispose();
        }

        public void Play()
        {
            MediaPlayer.Play();
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public void FastForward(int seconds)
        {
            var session = MediaPlayer.PlaybackSession;
            session.Position = session.Position + TimeSpan.FromSeconds(seconds);
        }

        public void Rewind(int seconds)
        {
            var session = MediaPlayer.PlaybackSession;
            session.Position = session.Position - TimeSpan.FromSeconds(seconds);
        }

        public void Reset()
        {
            if (CassetteTape != null)
            {
                MediaPlayer.Dispose();
            }
        }

        public async void PlaySound(string sound)
        {
            switch(sound.ToLower())
            {
                case "eject":
                    StorageFile ejectSound = await AppDirectory.GetFileAsync(@"Assets\Sounds\eject_cassette.wav");
                    var ejectSoundFile = await ejectSound.OpenAsync(FileAccessMode.Read);
                    WalkmanSound.SetSource(ejectSoundFile, ejectSound.FileType);
                    WalkmanSound.Play();
                    break;

                case "open":
                    StorageFile openSound = await AppDirectory.GetFileAsync(@"Assets\Sounds\open_walkman.wav");
                    var openSoundFile = await openSound.OpenAsync(FileAccessMode.Read);
                    WalkmanSound.SetSource(openSoundFile, openSound.FileType);
                    WalkmanSound.Play();
                    break;

                case "play":
                    StorageFile playButtonSound = await AppDirectory.GetFileAsync(@"Assets\Sounds\walkman_play_button.wav");
                    var openPlayButtonSound = await playButtonSound.OpenAsync(FileAccessMode.Read);
                    WalkmanSound.SetSource(openPlayButtonSound, playButtonSound.FileType);
                    WalkmanSound.Play();
                    break;
            }
        }

        public void ChangeLightIndicator(Windows.UI.Xaml.Shapes.Ellipse indicatorElement, string state)
        {
            switch(state.ToLower())
            {
                case "play":
                    
                    indicatorElement.Fill = new SolidColorBrush(Windows.UI.Colors.LightGreen);
                    break;
                case "pause":
                    indicatorElement.Fill = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
                case "stop":
                    indicatorElement.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;     
             }
        }

        public void Volume(double vol = 0.5)
        {
            Debug.WriteLine($"Changing volume to: {vol}");
            MediaPlayer.Volume = vol;
        }
    }
}
