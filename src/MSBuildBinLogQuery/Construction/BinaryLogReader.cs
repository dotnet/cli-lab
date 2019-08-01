using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Build.Framework;

namespace Microsoft.Build.Logging.Query.Construction
{
    public class BinaryLogReader : IDisposable
    {
        private readonly FileStream _fileStream;
        private readonly GZipStream _gZipStream;
        private readonly BinaryReader _binaryReader;
        private readonly int _fileFormatVersion;
        private IEnumerable<BuildEventArgs> _buildEvents;

        public BinaryLogReader(string path)
        {
            _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _gZipStream = new GZipStream(_fileStream, CompressionMode.Decompress, leaveOpen: true);
            _binaryReader = new BinaryReader(_gZipStream);
            _fileFormatVersion = _binaryReader.ReadInt32();
        }

        public void Dispose()
        {
            _fileStream.Dispose();
            _gZipStream.Dispose();
            _binaryReader.Dispose();
        }

        public IEnumerable<BuildEventArgs> ReadEvents()
        {
            if (_buildEvents != null)
            {
                return _buildEvents;
            }

            var buildEventArgsReader = new BuildEventArgsReader(_binaryReader, _fileFormatVersion);
            var buildEvents = new List<BuildEventArgs>();
            
            while (true)
            {
                var buildEventArgs = buildEventArgsReader.Read();

                if (buildEventArgs == null)
                {
                    break;
                }

                buildEvents.Add(buildEventArgs);
            }

            _buildEvents = buildEvents;
            return _buildEvents;
        }
    }
}
