using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    class ConsoleLogger:ILogger
    {
        public void log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
