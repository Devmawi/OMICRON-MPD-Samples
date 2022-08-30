using mtronix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMICRON.MpdConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var app = new OmicronApp();
            app.GetStatus();
            app.StartReplay();
            Console.ReadKey();

        }
    }
}
