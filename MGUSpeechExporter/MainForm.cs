using System;
using System.IO;
using System.Windows.Forms;

namespace MGUSpeechExporter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private DirectoryInfo SelectFolder(string path = "")
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = path;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !String.IsNullOrWhiteSpace(dialog.SelectedPath))
                    return new DirectoryInfo(dialog.SelectedPath);
            }

            return null;
        }

        private void SetFolder(TextBox txt)
        {
            DirectoryInfo info = SelectFolder(txt.Text);

            if (info != null && info.Exists)
                txt.Text = info.FullName;
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            SetFolder(txtInput);
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            SetFolder(txtOutput);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                Export();
            }
            catch (ExportException ex)
            {
                MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Export()
        {
            if (txtInput.Text == String.Empty)
                throw new ExportException("Please select an input folder.");

            if (txtOutput.Text == String.Empty)
                throw new ExportException("Please select an output folder.");

            DirectoryInfo input = new DirectoryInfo(txtInput.Text);

            if (!input.Exists)
                throw new ExportException("Could not find the input folder.");

            FileInfo[] files = input.GetFiles("*.ath");

            if (files.Length <= 0)
                throw new ExportException("Could not find any supported files in the input folder.");

            DirectoryInfo output = new DirectoryInfo(txtOutput.Text);

            if (!output.Exists)
            {
                DialogResult result = MessageBox.Show("Output folder does not exist. Do you want to create it?", "Create new folder?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    output.Create();
                else
                    throw new ExportException("Could not create the output folder.");

                output.Refresh();
            }

            for (int i = 0; i < files.Length; i++)
            {
                ATHDocument athDoc = new ATHDocument(files[i].OpenRead());
                ATFDocument atfDoc = new ATFDocument(athDoc);

                atfDoc.SaveAllData(output);
            }

            MessageBox.Show("Export is complete.", "MGU Sound Exporter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}