using Soft_Walkman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Soft_Walkman.Interfaces
{
    public interface IWalkman
    {
        public MediaSource MediaSource { get; }

        public MediaPlayer MediaPlayer { get; }

        public MediaPlaybackList MediaPlaybackList { get; }

        public CassetteTape CassetteTape { get; set; }

        // Methods:
        public Task LoadCassetteTape(CassetteTape ct);

        public void Play();

        public void Pause();

        public void FastForward(int seconds);

        public void Rewind(int seconds);

        public void Reset();

        public void PlaySound(string sound);

        public void ChangeLightIndicator(Windows.UI.Xaml.Shapes.Ellipse indicatorElement, string state);

    }
}
