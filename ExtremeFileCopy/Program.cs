using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeFileCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            string SourceDirectory, DestinationDirectory;

            if (args.Length == 2)
            {
                SourceDirectory = args[0];
                DestinationDirectory = args[1];
            }
            else if (args.Length == 0)
            {
                SourceDirectory = ConfigurationManager.AppSettings["SourceDirectory"];
                DestinationDirectory = ConfigurationManager.AppSettings["DestinationDirectory"];
            }
            else
            {
                Console.WriteLine("Not enough parameters, either two or zero");
                return;
            }

            if (!CheckDirectory(SourceDirectory) || !CheckDirectory(DestinationDirectory))
            {
                return;
            }

            CopyDirectory copy = new CopyDirectory(SourceDirectory, DestinationDirectory);

            copy.Execute();
        }


        private static bool CheckDirectory(string directoryName)
        {
            // check and see if these are directories.
            if (!Directory.Exists(directoryName))
            {
                Console.WriteLine(string.Format("Directory {0} does not exist", directoryName));
                return false;
            }
            return true;
        }
    }
}
