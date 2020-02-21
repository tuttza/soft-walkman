using System;
using System.Diagnostics;
using Windows.Storage;

namespace Soft_Walkman.Models
{
    class CassetteTapeState
    {
        private readonly ApplicationDataContainer SavedState;
            
        private static readonly string[] StateKeys = { "tapeSaved", "tapePath", "tapePlayPos", "tapeTrack" };

        private uint _tapeTrackIndex;
        public uint GetTapeTrackIndex()
        {
            if (SavedState.Values.ContainsKey(StateKeys[3]))
            {
                _tapeTrackIndex = (uint)SavedState.Values[StateKeys[3]];

                Debug.WriteLine("getting _tapeTrackIndex of {0}", _tapeTrackIndex);

                return _tapeTrackIndex;
            }
            else
            {
                _tapeTrackIndex = 0;

                return _tapeTrackIndex;
            }
        }

        public void SetTapeTrackIndex(uint val)
        {
            _tapeTrackIndex = val;

            Debug.WriteLine("setting _trackIndex to: {0}", _tapeTrackIndex);

            SavedState.Values[StateKeys[3]] = _tapeTrackIndex;
        }

        private bool _tapeSaved;

        public bool GetTapeSaved()
        {
            if (SavedState.Values.ContainsKey(StateKeys[0]))
            {
                return (bool)SavedState.Values[StateKeys[0]];
            } 
            else
            {
                return false;
            }
        }

        public void SetTapeSaved(bool val)
        {            
            _tapeSaved = val;

            SavedState.Values[StateKeys[0]] = _tapeSaved;
        }

        private TimeSpan _tapePositoin;

        public TimeSpan GetTapePostion()
        {
            if (SavedState.Values.ContainsKey(StateKeys[2]))
            {
                _tapePositoin = (TimeSpan)SavedState.Values[StateKeys[2]];

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
            _tapePositoin = val;

            Debug.WriteLine("Setting _tapePostion to value of {0}", _tapePositoin);

            SavedState.Values[StateKeys[2]] = _tapePositoin;
        }

        private string _tapePath;
        public string GetTapePath()
        {
            if (SavedState.Values.ContainsKey(StateKeys[1]))
            {

                _tapePath = SavedState.Values[StateKeys[1]] as string;

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
            _tapePath = val;

            SavedState.Values[StateKeys[1]] = _tapePath;
        }

        public Walkman walkman { get; set; }

        public CassetteTape cassetteTape { get; set; }

        public void ClearTapeState()
        {
            SavedState.Values[StateKeys[1]] = null;
            SavedState.Values[StateKeys[2]] = null;
            SavedState.Values[StateKeys[0]] = false;
            SavedState.Values[StateKeys[3]] = null;
        }

        public void SaveTapeState()
        {
            this.SetTapePath(this.cassetteTape.DirPath.Path);
            this.SetTapePostion(this.walkman.MediaPlayer.PlaybackSession.Position);
            this.SetTapeSaved(true);
            this.SetTapeTrackIndex(this.walkman.MediaPlaybackList.CurrentItemIndex);
        }

        public CassetteTapeState()
        {
            this.SavedState = ApplicationData.Current.LocalSettings;
        }

        public CassetteTapeState(Walkman walkman, CassetteTape cassette)
        {
            this.walkman = walkman;
            this.cassetteTape = cassette;
            this.SavedState = ApplicationData.Current.LocalSettings;
        }
    }
}
