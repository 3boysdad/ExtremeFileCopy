
namespace ExtremeFileCopy
{
    using System;
    using System.IO;
    using System.Linq;


    public class CopyDirectory
    {
        private string Source;
        private string Destination;

        public CopyDirectory(string SourceDirectory, string DestinationDirectory)
        {
            this.Source = SourceDirectory;
            this.Destination = DestinationDirectory;

            var something = this.Destination.Reverse().ToArray();

            if (something[0] != '\\')
            {
                this.Destination = this.Destination + "\\";
            }
        }

        public void Execute()
        {
            this.ProcessDirectory(this.Source);
        }

        private void ProcessDirectory(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            var files = directoryInfo.GetFiles().ToList().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));
            foreach (FileInfo fileInfo in files)
            {
                string targetFile = this.GetTargetFileName(fileInfo);

                if (!File.Exists(targetFile))
                {
                    // will copy the file it doens't exist on target.
                    Console.WriteLine(string.Format("Copying {0}", targetFile));
                    File.Copy(fileInfo.FullName, targetFile);
                }
                else
                {
                    // check the size - if they aren't the same - assume corruption and dlelete/copy.
                    FileInfo targetFileInfo = new FileInfo(targetFile);
                    if (targetFileInfo.Length != fileInfo.Length)
                    {
                        Console.WriteLine(string.Format("Ovewritting {0}", targetFile));
                        try
                        {
                            File.Delete(targetFile);
                            File.Copy(fileInfo.FullName, targetFile);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

            var directories = directoryInfo.GetDirectories().ToList().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));
            foreach (DirectoryInfo info in directories)
            {
                string targetDirectory = this.GetTargetDirectoryName(info);

                if (!Directory.Exists(targetDirectory))
                {
                    Console.WriteLine(string.Format("Creating {0}", targetDirectory));
                    // Missing a new directory - create it.
                    Directory.CreateDirectory(targetDirectory);
                }

                this.ProcessDirectory(info.FullName);
            }
        }

        private string GetTargetFileName(FileInfo fileInfo)
        {
            string drive = Path.GetPathRoot(fileInfo.FullName);
            int directoryPos = fileInfo.FullName.IndexOf(drive) + drive.Length;

            string relativeDirectory = Path.GetDirectoryName(fileInfo.FullName)
                .Substring(directoryPos, Path.GetDirectoryName(fileInfo.FullName).Length - directoryPos);

            return this.Destination + relativeDirectory + "\\" + fileInfo.Name;
        }

        private string GetTargetDirectoryName(DirectoryInfo dirInfo)
        {
            string drive = Path.GetPathRoot(dirInfo.FullName);
            int directoryPos = dirInfo.FullName.IndexOf(drive) + drive.Length;

            string relativeDirectory = Path.GetDirectoryName(dirInfo.FullName)
                    .Substring(directoryPos, Path.GetDirectoryName(dirInfo.FullName).Length - directoryPos);

            return this.Destination + relativeDirectory + "\\" + dirInfo.Name;
        }
    }

}
