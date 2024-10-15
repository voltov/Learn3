using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileAndFolderLister
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            string path = textBoxPath.Text;
            string filter = textBoxFilter.Text;
            FileSystemVisitor fileSystemVisitor;

            if (string.IsNullOrEmpty(filter))
            {
                fileSystemVisitor = new FileSystemVisitor(path);
            }
            else
            {
                fileSystemVisitor = new FileSystemVisitor(path, filter);
            }

            richTextBoxStatus.Clear();
            richTextBoxResults.Clear();

            // Subscribe to events
            fileSystemVisitor.Starting += (visitor, visitorArgs) => richTextBoxStatus.AppendText(visitorArgs.Message + Environment.NewLine);
            fileSystemVisitor.DirectoryFound += (visitor, visitorArgs) =>
            {
                richTextBoxStatus.AppendText(visitorArgs.Message + Environment.NewLine);
                if (visitorArgs.Message.Contains("System"))
                {
                    visitorArgs.Cancel = true;
                }
                if (visitorArgs.Message.Contains("Exclude"))
                {
                    visitorArgs.Exclude = true;
                }
            };
            fileSystemVisitor.FileFound += (visitor, visitorArgs) =>
            {
                richTextBoxStatus.AppendText(visitorArgs.Message + Environment.NewLine);
                if (visitorArgs.Message.Contains("System.txt"))
                {
                    visitorArgs.Cancel = true;
                }
                if (visitorArgs.Message.Contains("Exclude.txt"))
                {
                    visitorArgs.Exclude = true;
                }
            };
            fileSystemVisitor.Finished += (visitor, visitorArgs) => richTextBoxStatus.AppendText(visitorArgs.Message + Environment.NewLine);

            List<string> items = fileSystemVisitor.ListItems();
            foreach (var item in items)
            {
                richTextBoxResults.AppendText(item + Environment.NewLine);
            }
        }
    }
}
