using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.Storage.Search;

namespace Soft_Walkman.Models
{
    public sealed class CassetteTape
    {
        public StorageFolder DirPath { get; set; }
        public string Title { get { return this.DirPath.DisplayName; } }
        public Task<List<StorageFile>> MediaFiles => FindMedia();

        public async Task<int> MediaSizeAsync()
        {
            var media = await this.MediaFiles;
            return media.Count;
        }

        public async Task<List<Track>> Tracks()
        {
            var tracks = new List<Track>();
            int index = 0;

            foreach (var track in await MediaFiles)
            {
                ++index;
                var num = index.ToString() + ". ";
                tracks.Add(new Track { TrackNumber = num, Name = track.Name });
            }
           
            return tracks;
        }

        private async Task<List<StorageFile>> FindMedia()
        {
            List<StorageFile> mediaFiles = new List<StorageFile>();
            string[] supportedAudioExtentions = { ".mp3", ".mp4", ".m4a", ".wav", ".wma", ".flac" };

            var fileQueryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, supportedAudioExtentions)
            {
                FolderDepth = FolderDepth.Deep
            };

            var query = this.DirPath.CreateFileQueryWithOptions(fileQueryOptions);
            IReadOnlyList<StorageFile> storageFolder = await query.GetFilesAsync();

            foreach (StorageFile sf in storageFolder)
            {
                mediaFiles.Add(sf);
            }
            return mediaFiles;
        }
    }
}
