using Soft_Walkman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Soft_Walkman.Interfaces
{
    interface ICassetteTape
    {

        // Properties:
        public StorageFolder DirPath { get; set; }

        public string Title { get; }

        public Task<string> CoverArtPath { get; }

        public Task<List<StorageFile>> MediaFiles { get; }

        // Methods:
        public Task<int> MediaSizeAsync();

        public Task<List<Track>> Tracks();

        public Task<string> FindCoverArt();

        public Task<List<StorageFile>> FindMedia();


    }
}
