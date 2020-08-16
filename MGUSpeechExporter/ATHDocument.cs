using System.IO;
using System.Text;

namespace MGUSpeechExporter
{
    public class ATHDocument
    {
        public const long EntryCountPtr = 0x04;
        public const long DataBeginPtr = 0x54;

        public FileInfo PathInfo { get; private set; }

        public int EntryCount { get; private set; }
        public ATHEntry[] Entries { get; private set; }

        public ATHDocument(FileStream fs)
        {
            PathInfo = new FileInfo(fs.Name);

            using (BinaryReader br = new BinaryReader(fs))
            {
                fs.Seek(EntryCountPtr, SeekOrigin.Begin);

                EntryCount = br.ReadByte();
                Entries = new ATHEntry[EntryCount];

                fs.Seek(DataBeginPtr, SeekOrigin.Begin);

                for (int i = 0; i < EntryCount; i++)
                    Entries[i] = new ATHEntry(br, this, i);
            }
        }
    }
}