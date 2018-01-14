using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace ADBMDTS
{
    class ADBMInterface
    {
        public static void RunADBMMacro(string Macro)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            try
            {
                p.StartInfo.FileName = ConfigurationManager.AppSettings["pathADBMWEXE"].ToString();
                p.StartInfo.Arguments = " macro " + Macro;
                p.Start();
            }
            catch (Exception ex7)
            {
                //DisplayServiceMessage(ex7, "Ex7");
            }
            finally
            {
                p.WaitForExit();
            }
        }

        public static List<string> pathToADB()
        {
            var files = new List<string>();

            foreach (DriveInfo d in DriveInfo.GetDrives().Where(x => x.IsReady == true))
            {
                try
                {
                    files.AddRange(Directory.GetFiles(d.RootDirectory.FullName, "ADBMData.ADB", SearchOption.AllDirectories));
                }
                catch (UnauthorizedAccessException) { }
            }
            return files;
        }   
    }
}
