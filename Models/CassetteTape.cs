using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Soft_Walkman.Interfaces;

namespace Soft_Walkman.Models
{
    public sealed class CassetteTape: ICassetteTape
    {
        public StorageFolder DirPath { get; set; }
        public string Title { get { return this.DirPath.DisplayName; } }
        public Task<string> CoverArtPath => FindCoverArt();
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

                string formattedName = Lib.StringFormatter.RemoveNumbersFromBeginningOfString(track.Name);
                formattedName = Lib.StringFormatter.RemoveExtension(formattedName);
                formattedName = Lib.StringFormatter.RemoveSpecialCharNearBeginningOfString(formattedName, '-');
                formattedName = Lib.StringFormatter.RemoveSpecialCharNearBeginningOfString(formattedName, '.');

                string name = index.ToString() + $". {formattedName}";

                tracks.Add(new Track { Name = name });
            }
           
            return tracks;
        }

        public async Task<string> FindCoverArt()
        {
            string coverArtPath = "";

            string[] supportedImageExtentions = { ".jpg", ".jpeg", ".png", ".bmp"};

            var fileQueryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, supportedImageExtentions)
            {
                FolderDepth = FolderDepth.Deep
            };

            var query = this.DirPath.CreateFileQueryWithOptions(fileQueryOptions);

            IReadOnlyList<StorageFile> storageFolder = await query.GetFilesAsync();

            if (storageFolder.Count <= 0)
            {
                return coverArtPath;
            }

            foreach(StorageFile sf in storageFolder)
            {
                coverArtPath = sf.Path.ToString();
            }

            return coverArtPath;
        }

        public async Task<List<StorageFile>> FindMedia()
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
