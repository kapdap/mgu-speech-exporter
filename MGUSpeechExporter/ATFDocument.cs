using System.IO;

namespace MGUSpeechExporter
{
    public class ATFDocument
    {
        public ATHDocument Headers { get; private set; }
        public ATFEntry[] Entries { get; private set; }

        public FileInfo PathInfo { get; private set; }

        public ATFDocument(ATHDocument headers)
        {
            Headers = headers;
            Entries = new ATFEntry[Headers.EntryCount];

            for (int i = 0; i < Headers.EntryCount; i++)
                Entries[i] = new ATFEntry(Headers.Entries[i], this);

            PathInfo = new FileInfo(Path.ChangeExtension(Headers.PathInfo.FullName, "atf"));
        }

        public void SaveAllData(string folder)
        {
            ExportAllData(new DirectoryInfo(folder));
        }

        public void SaveAllData(DirectoryInfo folder)
        {
            ExportAllData(folder);
        } 

        public void SaveAllText(string folder)
        {
            ExportAllText(new DirectoryInfo(folder));
        }

        public void SaveAllText(DirectoryInfo folder)
        {
            ExportAllText(folder);
        }

        public void SaveAllWave(string folder)
        {
            ExportAllWave(new DirectoryInfo(folder));
        }

        public void SaveAllWave(DirectoryInfo folder)
        {
            ExportAllWave(folder);
        }

        public void SaveData(int index, string folder)
        {
            ExportData(index, new DirectoryInfo(folder));
        }

        public void SaveData(int index, DirectoryInfo folder)
        {
            ExportData(index, folder);
        }

        public void SaveText(int index, string folder)
        {
            ExportText(index, new DirectoryInfo(folder));
        }

        public void SaveText(int index, DirectoryInfo folder)
        {
            ExportText(index, folder);
        }

        public void SaveWave(int index, string folder)
        {
            ExportWave(index, new DirectoryInfo(folder));
        }

        public void SaveWave(int index, DirectoryInfo folder)
        {
            ExportWave(index, folder);
        }

        private void ExportAllData(DirectoryInfo folder)
        {
            using (FileStream fs = GetFileStream())
                for (int i = 0; i < Headers.EntryCount; i++)
                    Entries[i].SaveDataToDisk(fs, folder);
        }

        private void ExportAllText(DirectoryInfo folder)
        {
            using (FileStream fs = GetFileStream())
                for (int i = 0; i < Headers.EntryCount; i++)
                    Entries[i].SaveTextToDisk(fs, folder);
        }

        private void ExportAllWave(DirectoryInfo folder)
        {
            using (FileStream fs = GetFileStream())
                for (int i = 0; i < Headers.EntryCount; i++)
                    Entries[i].SaveWaveToDisk(fs, folder);
        }

        private void ExportData(int index, DirectoryInfo folder)
        {
            using (FileStream fs = GetFileStream())
                Entries[index].SaveDataToDisk(fs, folder);
        }

        private void ExportText(int index, DirectoryInfo folder)
        {
            using (FileStream fs = GetFileStream())
                Entries[index].SaveTextToDisk(fs, folder);
        }

        private void ExportWave(int index, DirectoryInfo folder)
        {
            using (FileStream fs = GetFileStream())
                Entries[index].SaveWaveToDisk(fs, folder);
        }

        private FileStream GetFileStream()
        {
            if (PathInfo.Exists)
               return PathInfo.OpenRead();

            return null;
        }
    }
}