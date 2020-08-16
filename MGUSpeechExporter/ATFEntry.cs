using System;
using System.IO;

namespace MGUSpeechExporter
{
    public struct ATFEntry
    {
        public ATFDocument Document { get; private set; }
        public ATHEntry Header { get; private set; }

        public ATFEntry(ATHEntry header, ATFDocument document)
        {
            Document = document;
            Header = header;
        }

        public MemoryStream ReadWave(FileStream fs)
        {
            if (Header.WaveSize == 0) return null;

            MemoryStream stream = new MemoryStream(Header.WaveSize);

            fs.Seek(Header.WaveOffset, SeekOrigin.Begin);
            CopyStream(fs, stream, Header.WaveSize);

            return stream;
        }

        public MemoryStream ReadText(FileStream fs)
        {
            if (!Header.IsValid) return null;

            MemoryStream stream = new MemoryStream();

            fs.Seek(Header.TextOffset + 8, SeekOrigin.Begin);

            do
            {
                int read = fs.ReadByte();

                if (read == 0xFF)
                    break;

                if (read != 0x00)
                    stream.WriteByte((byte)read);
                else
                {
                    stream.WriteByte(Convert.ToByte('\r'));
                    stream.WriteByte(Convert.ToByte('\n'));

                    fs.Seek(8, SeekOrigin.Current);
                }
            } while (fs.Position < Header.WaveOffset && fs.Position < fs.Length);

            return stream;
        }

        public void SaveDataToDisk(FileStream input, DirectoryInfo folder)
        {
            SaveWaveToDisk(input, folder);
            SaveTextToDisk(input, folder);
        }

        public void SaveWaveToDisk(FileStream input, DirectoryInfo folder)
        {
            if (Header.WaveSize == 0) return;

            string path = Path.Combine(folder.FullName, GetOutputFileName(".wav"));

            using (FileStream output = File.Create(path))
            {
                input.Seek(Header.WaveOffset, SeekOrigin.Begin);
                CopyStream(input, output, Header.WaveSize);
            }
        }

        public void SaveTextToDisk(FileStream input, DirectoryInfo folder)
        {
            if (!Header.IsValid) return;

            string path = Path.Combine(folder.FullName, GetOutputFileName(".txt"));

            using (MemoryStream stream = ReadText(input))
            {
                if (stream.Length <= 0) return;

                using (FileStream output = File.Create(path))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(output);
                }
            }
        }

        public string GetOutputFileName(string extension = "")
        {
            return String.Format("{0}_{1}{2}",
                Path.GetFileNameWithoutExtension(Document.PathInfo.Name), Header.Index + 1, extension);
        }

        // https://stackoverflow.com/questions/13021866/any-way-to-use-stream-copyto-to-copy-only-certain-number-of-bytes
        private void CopyStream(Stream input, Stream output, int bytes)
        {
            byte[] buffer = new byte[32768];
            int read;

            while (bytes > 0 &&
                   (read = input.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
            {
                output.Write(buffer, 0, read);
                bytes -= read;
            }
        }
    }
}