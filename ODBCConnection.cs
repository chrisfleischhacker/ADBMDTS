using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ADBMDTS
{
    public class ODBCConnection
    {
        public static string adbmdsn = ConfigurationManager.AppSettings["adbmdsn"].ToString();
    }
}
