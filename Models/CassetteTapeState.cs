using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Soft_Walkman.Models
{
    class CassetteTapeState
    {
        private static readonly string[] StateKeys = { "tapeSaved", "tapePath", "tapePlayPos", "tapeTrack" };

        private uint _tapeTrackIndex;
        public uint GetTapeTrackIndex()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(StateKeys[3]))
            {
                _tapeTrackIndex = (uint)localSettings.Values[StateKeys[3]];

                Debug.WriteLine("getting _tapeTrackIndex of {0}", _tapeTrackIndex);

                return _tapeTrackIndex;
            }
            else
            {
                _tapeTrackIndex = 1;
                return _tapeTrackIndex;
            }
        }

        public void SetTapeTrackIndex(uint val)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            _tapeTrackIndex = val;

            Debug.WriteLine("setting _trackIndex to: {0}", val);

            localSettings.Values[StateKeys[3]] = _tapeTrackIndex;
        }

        private bool _tapeSaved;

        public bool GetTapeSaved()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(StateKeys[0]))
            {
                return (bool)localSettings.Values[StateKeys[0]];
            } 
            else
            {
                return false;
            }
        }

        public void SetTapeSaved(bool val)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            
            _tapeSaved = val;

            localSettings.Values[StateKeys[0]] = _tapeSaved;
        }

        private TimeSpan _tapePositoin;

        public TimeSpan GetTapePostion()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(StateKeys[2]))
            {
                _tapePositoin = (TimeSpan)localSettings.Values[StateKeys[2]];

                Debug.WriteLine("getting _tapePostion of {0}", _tapePositoin);

                return _tapePositoin;
            }
            else
            {
                Debug.WriteLine("Creating new TimeSpan instance...");
                return new TimeSpan();
            }

        }

        public void SetTapePostion(TimeSpan val)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            _tapePositoin = val;

            Debug.WriteLine("Setting _tapePostion to value of {0}", val);
            localSettings.Values[StateKeys[2]] = _tapePositoin;
        }

        private string _tapePath;
        public string GetTapePath()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(StateKeys[1]))
            {

                _tapePath = localSettings.Values[StateKeys[1]] as string;

                Debug.WriteLine("getting _tapePath of {0}", _tapePath);

                return _tapePath;
            }
            else
            {
                Debug.WriteLine("creating empty string instance...");
                return "";
            }
        }

        public void SetTapePath(string val)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            
            _tapePath = val;

            localSettings.Values[StateKeys[1]] = _tapePath;
        }
        public Walkman walkman { get; set; }

        public CassetteTape cassetteTape { get; set; }

        public void ClearTapeState()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            localSettings.Values[StateKeys[1]] = null;
            localSettings.Values[StateKeys[2]] = null;
            localSettings.Values[StateKeys[0]] = false;
            localSettings.Values[StateKeys[3]] = null;
        }

        public void SaveTapeState()
        {
            this.SetTapePath(this.cassetteTape.DirPath.Path);
            this.SetTapePostion(this.walkman.MediaPlayer.PlaybackSession.Position);
            this.SetTapeSaved(true);
            this.SetTapeTrackIndex(this.walkman.MediaPlaybackList.CurrentItemIndex);
        }

        public CassetteTapeState() { }

        public CassetteTapeState(Walkman walkman, CassetteTape cassette)
        {
            this.walkman = walkman;
            this.cassetteTape = cassette;
        }
    }
}
