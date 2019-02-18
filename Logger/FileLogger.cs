using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class FileLogger:ILogger
    {
        private string fileName;

        public FileLogger(string fileName)
        {
            this.fileName = fileName;
            if (File.Exists(this.fileName) == false)
            {
                File.Create(fileName);
            }
        }
        public void log(string msg)
        {
            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine(msg);
            }
        }
    }
}
