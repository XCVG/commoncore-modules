using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandstormMPHelper1
{
    class Program
    {
        public static void Main()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            /*
            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine("  Drive type: {0}", d.DriveType);
                if (d.IsReady == true)
                {
                    Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                    Console.WriteLine("  File system: {0}", d.DriveFormat);
                    Console.WriteLine(
                        "  Available space to current user:{0, 15} bytes",
                        d.AvailableFreeSpace);

                    Console.WriteLine(
                        "  Total available space:          {0, 15} bytes",
                        d.TotalFreeSpace);

                    Console.WriteLine(
                        "  Total size of drive:            {0, 15} bytes ",
                        d.TotalSize);
                }
            }

            Console.WriteLine();

            string driveName = @"F:\";

            var files = Directory.EnumerateFiles(driveName);
            foreach(var file in files)
            {
                Console.WriteLine(file);
            }
            */

            string driveName;
            List<DriveInfo> possibleDrives = new List<DriveInfo>();
            foreach(DriveInfo d in allDrives)
            {
                if (d.IsReady && d.DriveType == DriveType.CDRom)
                    possibleDrives.Add(d);
            }

            if(possibleDrives.Count == 0)
            {
                Console.WriteLine("0");
            }
            else
            {
                //search for drives with .cda files
                for(int i = possibleDrives.Count-1; i >= 0; i--)
                {
                    var d = possibleDrives[i];
                    var files = Directory.EnumerateFiles(d.Name);
                    if(files.Count() > 30)
                    {
                        possibleDrives.RemoveAt(i);
                        continue;
                    }

                    bool foundCda = false;
                    foreach(var file in files)
                    {
                        if (string.Equals(Path.GetExtension(file), ".cda", StringComparison.OrdinalIgnoreCase))
                        {
                            foundCda = true;
                            break;
                        }
                    }

                    if(!foundCda)
                    {
                        possibleDrives.RemoveAt(i);
                    }
                }

                //reject again or take the remaining drive
                if (possibleDrives.Count == 0)
                {
                    Console.WriteLine("0");
                }
                else
                {
                    int fileCount = 0;
                    foreach(var file in Directory.EnumerateFiles(possibleDrives[0].Name))
                    {
                        if (string.Equals(Path.GetExtension(file), ".cda", StringComparison.OrdinalIgnoreCase))
                            fileCount++;
                    }

                    Console.WriteLine($"{fileCount},{possibleDrives[0].Name[0]}");
                }


            }

            //Console.WriteLine();
        }
    }
}
