using System.IO;

namespace MGUSpeechExporter
{
    public struct ExportConfig
    {
        public DirectoryInfo InputPath;
        public DirectoryInfo OutputPath;

        public bool SaveText;
        public bool SaveSubRip;
        public bool SaveWebVTT;

        public bool SaveWave;
        public bool SaveMP3;
        public bool SaveMP3Lyrics;

        public bool MergeAudio;
    }
}