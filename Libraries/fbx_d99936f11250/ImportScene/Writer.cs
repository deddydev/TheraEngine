using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImportScene
{
    public  class Writer
    {
        static StreamWriter w;
         static Writer()
        {
            w = new StreamWriter("output.txt", false);
        }
        public static void Write(string str)
        {
            w.Write(str);
        }
        public static void WriteLine(string str)
        {
            w.WriteLine(str);
        }
        public static void flush()
        {
            w.Flush();
        }
        public static void close()
        {
            w.Close();
        }
    };
}
