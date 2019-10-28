using Soft_Walkman.Models;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Shapes;

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

        public void ChangeLightIndicator(Ellipse indicatorElement, string state);
    }
}
