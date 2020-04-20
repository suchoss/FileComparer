using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 4)
            {
                WriteHelp();
                Environment.Exit(0);
            }

            string p = "";
            string o = "";
            string[] g1 = null;
            string[] g2 = null;

            try
            {
                foreach (string arg in args)
                {

                    switch (arg.Split("=")[0])
                    {
                        case "-p":
                            p = arg.Split("=")[1];
                            break;
                        case "-o":
                            o = arg.Split("=")[1];
                            break;
                        case "-g1":
                            g1 = SplitGroup(arg.Split("=")[1]);
                            break;
                        case "-g2":
                            g2 = SplitGroup(arg.Split("=")[1]);
                            break;

                        default:
                            WriteHelp();
                            Environment.Exit(0);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                WriteHelp();
                Environment.Exit(0);
            }

            HashSet<string> firstGroup = LoadFilesToHashSet(p, g1);
            HashSet<string> secondGroup = LoadFilesToHashSet(p, g2);

            File.WriteAllLines(Path.Combine(o,"missingInFirst"), firstGroup.Except(secondGroup));
            File.WriteAllLines(Path.Combine(o, "missingInSecond"), secondGroup.Except(firstGroup));

            Console.WriteLine("Done!");

        }

        private static void WriteHelp()
        {
            Console.WriteLine(@"Please run this app with following arguments:
  1) -p path to folder where your files are stored
  2) -g1 semicolon (;) separated list of filenames to load as the first group
  3) -g2 semicolon (;) separated list of filenames to load as the second group
  4) -o output folder where results are going to be stored.

  example: filecomparer -p=c:\folder -g1=filename1;filename2 -g2=filename3;filename4 -o=c:\out
----
This app loads unique rows from the first group and compares them with unique rows from the second group.
Output contains two files:
missingInFirst - rows which are not contained in first group
missingInSecond - rows which are not contained in second group");
        }

        private static string[] SplitGroup(string input)
        {
            return input.Split(";");
        }

        private static HashSet<string> LoadFilesToHashSet(string folderPath, string[] filenames)
        {
            HashSet<string> hs = new HashSet<string>();
            foreach (string filename in filenames)
            {
                var fileContent = File.ReadAllLines(Path.Combine(folderPath, filename));
                hs.UnionWith(fileContent);
            }
            return hs;
        }
    }
}
