using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace timeshift
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Verify Parameters
            if (args.Length != 3)
            {
                Usage();
                return;
            }

            string source = args[0];
            string output = args[2];
            int milliseconds = 0;

            if (!File.Exists(source))
            {
                Console.WriteLine($"Source .SRT file '{source}' does not exist.");
                return;
            }

            if(File.Exists(output))
            {
                Console.WriteLine($"Output .SRT file '{output}' already exists.");
                return;
            }

            if (!int.TryParse(args[1], out milliseconds))
            {
                Usage();
                return;
            }

            if (milliseconds == 0)
            {
                Console.WriteLine("No time change required as milliseconds param is 0.");
                return;
            }
            #endregion

            TimeShift(source, output, milliseconds);

        }

        static void TimeShift(string source, string output, int milliseconds)
        {
            TimeSpan addtime = TimeSpan.FromMilliseconds(milliseconds);

            // Required to parse TimeSpan in format hh:mm:ss,fff
            var srtTimeFormat = System.Globalization.CultureInfo.GetCultureInfo("hr-HR").DateTimeFormat;

            using (StreamReader reader = new StreamReader(source, Encoding.GetEncoding("iso-8859-1")))
            {
                using (StreamWriter writer = new StreamWriter(File.OpenWrite(output), Encoding.GetEncoding("iso-8859-1")))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (line.Length == 29 && line.Substring(12, 5) == " --> ")
                        {
                            var start = TimeSpan.Parse(line.Substring(0, 12), srtTimeFormat);
                            var end = TimeSpan.Parse(line.Substring(17, 12), srtTimeFormat);

                            // Add milliseconds and create time string for subtitle file
                            line = $"{start.Add(addtime).ToString("hh':'mm':'ss','fff")} --> {end.Add(addtime).ToString("hh':'mm':'ss','fff")}";
                        }

                        writer.WriteLine(line);
                    }
                }
            }
        }

        static void Usage()
        {
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("A small utility to adjust the time codes within an SRT subtitle file. SRT file should have the following format:-");
            Console.WriteLine("2");
            Console.WriteLine("00:01:35,597 --> 00:01:36,209");
            Console.WriteLine("Caption goes here!");
            Console.WriteLine("");
            Console.WriteLine("USAGE:");
            Console.WriteLine("timeshift.exe [source srt filename] [miliseconds] [output srt filename]");
        }

    }
}
