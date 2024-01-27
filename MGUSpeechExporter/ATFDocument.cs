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

        public FileStream GetFileStream()
        {
            if (PathInfo.Exists)
               return PathInfo.OpenRead();
            return null;
        }
    }
}