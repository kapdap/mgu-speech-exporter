using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MGUSpeechExporter
{
    public struct ATFEntry
    {
        public static Dictionary<short, string> Characters => new Dictionary<short, string>()
            {
                { 0, "MOOD" },
                { 1, "Martin Karne" },
                { 2, "Kenzo Uji" },
                { 3, "Diane Matlock" },
                { 4, "Ben Gunn" },
                { 5, "Computer" },
                { 6, "Recording" },
                { 7, "Rock Harding" },
                { 8, "Gloria Feist" },
                { 9, "Professor Achtung" },
                { 10, "British Solider" }
            };

        public ATFDocument Document { get; private set; }
        public ATHEntry Header { get; private set; }

        public ATFEntry(ATHEntry header, ATFDocument document)
        {
            Document = document;
            Header = header;
        }

        public MemoryStream ReadWave(FileStream fs)
        {
            if (Header.WaveSize == 0)
                return null;

            MemoryStream stream = new MemoryStream(Header.WaveSize);

            fs.Seek(Header.WaveOffset, SeekOrigin.Begin);
            Export.CopyStream(fs, stream, Header.WaveSize);

            return stream;
        }

        public List<SubtitleEntry> ReadText(FileStream fs)
        {
            List<SubtitleEntry> entries = new List<SubtitleEntry>();

            if (!Header.IsValid)
                return entries;

            BinaryReader br = new BinaryReader(fs);
            fs.Seek(Header.TextOffset, SeekOrigin.Begin);

            do
            {
                int timestamp = br.ReadInt32();
                if (timestamp == -1)
                    break;

                SubtitleEntry entry = new SubtitleEntry(
                    timestamp, 
                    br.ReadInt16(), 
                    br.ReadInt16()
                );

                byte read;
                while ((read = br.ReadByte()) != 0x00)
                    entry.Text += (char)read;

                entries.Add(entry);
            } while (fs.Position < Header.WaveOffset && fs.Position < fs.Length);

            return entries;
        }

        public MemoryStream GetAudio(FileStream fs) =>
            ReadWave(fs);

        public List<SubtitleEntry> GetSubtitles(FileStream fs) =>
            ReadText(fs);

        public string GetFileName(string extension = "") =>
            String.Format("{0}_{1}{2}",
                Path.GetFileNameWithoutExtension(Document.PathInfo.Name), Header.Index + 1, extension);
    }
}