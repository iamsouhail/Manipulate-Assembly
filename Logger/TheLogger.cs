using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public static class TheLogger
    {
        private static ILogger logger;

        static TheLogger()
        {
            logger  = new FileLogger("log.txt");
        }


        public static void log(string msg)
        {
            logger.log(msg);
        }
    }
}
