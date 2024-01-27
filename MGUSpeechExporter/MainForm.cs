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

        private void btnInput_Click(object sender, EventArgs e) =>
            SetFolder(txtInput);

        private void btnOutput_Click(object sender, EventArgs e) =>
            SetFolder(txtOutput);

        private void btnExport_Click(object sender, EventArgs e) =>
            Export();

        private void Export()
        {
            try
            {
                if (txtInput.Text == String.Empty)
                    throw new ExportException("Please select an input folder.");

                if (txtOutput.Text == String.Empty)
                    throw new ExportException("Please select an output folder.");

                ExportConfig config = new ExportConfig();
                config.InputPath = new DirectoryInfo(txtInput.Text);
                config.OutputPath = new DirectoryInfo(txtOutput.Text);

                Export export = new Export(config);
                export.Run();

                MessageBox.Show("Export is complete.", "MGU Speech Exporter", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}