using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FileAndFolderLister
{
    // Custom event argument class with Cancel property
    public class FileSystemVisitorEventArgs : EventArgs
    {
        public string Message { get; }
        public bool Cancel { get; set; }
        public bool Exclude { get; set; }

        public FileSystemVisitorEventArgs(string message)
        {
            Message = message;
            Cancel = false;
            Exclude = false;
        }
    }

    public class FileSystemVisitor
    {
        private string _path;
        private string _filter;

        // Event delegates
        public delegate void FileSystemVisitorEventHandler(object sender, FileSystemVisitorEventArgs e);

        // Events
        public event FileSystemVisitorEventHandler Starting;
        public event FileSystemVisitorEventHandler DirectoryFound;
        public event FileSystemVisitorEventHandler FileFound;
        public event FileSystemVisitorEventHandler Finished;

        // Constructor without filter
        public FileSystemVisitor(string path)
        {
            _path = path;
            _filter = "*"; // Default filter to list all files and folders
        }

        // Constructor with filter
        public FileSystemVisitor(string path, string filter)
        {
            _path = path;
            _filter = $"*{filter}*";
        }

        public List<string> ListItems()
        {
            List<string> items = new List<string>();

            if (!Directory.Exists(_path))
            {
                MessageBox.Show($"The directory '{_path}' does not exist.", "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return items;
            }

            // Raise Starting event
            Starting?.Invoke(this, new FileSystemVisitorEventArgs("Starting to list items..."));

            try
            {
                // List directories
                foreach (var dir in Directory.GetDirectories(_path, _filter, SearchOption.TopDirectoryOnly))
                {
                    var args = new FileSystemVisitorEventArgs($"Directory found: {dir}");
                    DirectoryFound?.Invoke(this, args);
                    if (args.Cancel)
                    {
                        break;
                    }
                    if (args.Exclude)
                    {
                        continue;
                    }
                    items.Add(dir);
                }

                // List files
                foreach (var file in Directory.GetFiles(_path, _filter, SearchOption.TopDirectoryOnly))
                {
                    var args = new FileSystemVisitorEventArgs($"File found: {file}");
                    FileFound?.Invoke(this, args);
                    if (args.Cancel)
                    {
                        break;
                    }
                    if (args.Exclude)
                    {
                        continue;
                    }
                    items.Add(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Raise Finished event
            Finished?.Invoke(this, new FileSystemVisitorEventArgs("Finished listing items."));

            return items;
        }
    }
}
