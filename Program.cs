using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInRow
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Methods
    {
        public static void dsa()
        {
            ConfigurationManager.AppSettings.Get("apikey");
        }
    }

}
