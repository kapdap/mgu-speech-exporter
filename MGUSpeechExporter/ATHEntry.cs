using System.IO;

namespace MGUSpeechExporter
{
    public struct ATHEntry
    {
        public ATHDocument Document { get; private set; }
        public int Index { get; private set; }

        public long TextOffset { get; private set; }
        public long WaveOffset { get; private set; }
        public int WaveSize { get; private set; }

        public bool IsValid { get; private set; }

        public ATHEntry(FileStream fs, ATHDocument document, int index)
        {
            Document = document;
            Index = index;

            TextOffset = 0;
            WaveOffset = 0;
            WaveSize = 0;

            IsValid = false;

            SetEntry(fs);
        }

        public ATHEntry(BinaryReader br, ATHDocument document, int index)
        {
            Document = document;
            Index = index;

            TextOffset = 0;
            WaveOffset = 0;
            WaveSize = 0;

            IsValid = false;

            SetEntry(br);
        }

        public void SetEntry(FileStream fs)
        {
            SetEntry(new BinaryReader(fs));
        }

        public void SetEntry(BinaryReader br)
        {
            TextOffset = br.ReadInt32();
            WaveOffset = br.ReadInt32();
            WaveSize = br.ReadInt32();

            IsValid = TextOffset != -1;
        }
    }
}