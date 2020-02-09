using System;
using System.IO;
using System.Text;

namespace CalendarOptimizer
{
    public class Main
    {
        public Main()
        {
            //Get file

            //Translate hours into minutes


            var path = @"/Users/carlopeluso/Desktop/file.rtf";

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

        }
    }
}
