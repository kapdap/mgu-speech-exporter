using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGUSpeechExporter
{
    public class Export
    {
        public ExportConfig Config { get; private set; }

        public Export(ExportConfig config) =>
            Config = config;

        public void Run()
        {
            FileInfo[] files = Config.InputPath.GetFiles("*.ath");

            if (files.Length <= 0)
                throw new ExportException("Could not find any supported files in the input folder.");

            if (!Config.OutputPath.Exists)
            {
                DialogResult result = MessageBox.Show("Output folder does not exist. Do you want to create it?", "Create new folder?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    Config.OutputPath.Create();
                else
                    throw new ExportException("Could not create the output folder.");

                Config.OutputPath.Refresh();
            }

            for (int i = 0; i < files.Length; i++)
            {
                ATHDocument athDoc = new ATHDocument(files[i].OpenRead());
                ATFDocument atfDoc = new ATFDocument(athDoc);

                ExportAllData(atfDoc, Config.OutputPath);
            }
        }

        public void RunAsync()
        {
        }

        private void SaveDataToDisk(ATFEntry entry, FileStream input, DirectoryInfo folder)
        {
            SaveWaveToDisk(entry, input, folder);
            SaveTextToDisk(entry, input, folder);
        }

        private void SaveWaveToDisk(ATFEntry entry, FileStream input, DirectoryInfo folder)
        {
            if (entry.Header.WaveSize == 0)
                return;

            string path = Path.Combine(folder.FullName, entry.GetFileName(".wav"));

            using (FileStream output = File.Create(path))
            {
                input.Seek(entry.Header.WaveOffset, SeekOrigin.Begin);
                CopyStream(input, output, entry.Header.WaveSize);
            }
        }

        private void SaveTextToDisk(ATFEntry entry, FileStream input, DirectoryInfo folder, string extension = ".txt")
        {
            if (!entry.Header.IsValid)
                return;

            using (MemoryStream stream = GetSubtitleStream(entry, input))
            {
                if (stream.Length <= 0)
                    return;

                string path = Path.Combine(folder.FullName, entry.GetFileName(extension));

                using (FileStream output = File.Create(path))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(output);
                }
            }
        }

        private MemoryStream GetSubtitleStream(ATFEntry entry, FileStream input)
        {
            MemoryStream stream = new MemoryStream();
            List<SubtitleEntry> subtitles = entry.GetSubtitles(input);

            foreach (SubtitleEntry subtitle in subtitles)
            {

            }

            return stream;
        }

        private void ExportAllData(ATFDocument doc, DirectoryInfo folder)
        {
            using (FileStream fs = doc.GetFileStream())
                for (int i = 0; i < doc.Headers.EntryCount; i++)
                    SaveDataToDisk(doc.Entries[i], fs, folder);
        }

        private void ExportAllText(ATFDocument doc, DirectoryInfo folder)
        {
            using (FileStream fs = doc.GetFileStream())
                for (int i = 0; i < doc.Headers.EntryCount; i++)
                    SaveTextToDisk(doc.Entries[i], fs, folder);
        }

        private void ExportAllWave(ATFDocument doc, DirectoryInfo folder)
        {
            using (FileStream fs = doc.GetFileStream())
                for (int i = 0; i < doc.Headers.EntryCount; i++)
                    SaveWaveToDisk(doc.Entries[i], fs, folder);
        }

        private void ExportData(ATFDocument doc, int index, DirectoryInfo folder)
        {
            using (FileStream fs = doc.GetFileStream())
                SaveDataToDisk(doc.Entries[index], fs, folder);
        }

        private void ExportText(ATFDocument doc, int index, DirectoryInfo folder)
        {
            using (FileStream fs = doc.GetFileStream())
                SaveTextToDisk(doc.Entries[index], fs, folder);
        }

        private void ExportWave(ATFDocument doc, int index, DirectoryInfo folder)
        {
            using (FileStream fs = doc.GetFileStream())
                SaveWaveToDisk(doc.Entries[index], fs, folder);
        }

        // https://stackoverflow.com/questions/13021866/any-way-to-use-stream-copyto-to-copy-only-certain-number-of-bytes
        public static void CopyStream(Stream input, Stream output, int bytes)
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
