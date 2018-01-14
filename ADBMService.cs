using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Data.Odbc;
using WinSCP;
using System.Configuration;
using ServiceDebuggerHelper;

namespace ADBMService
{
    partial class ADBMService : ServiceBase, IDebuggableService
    {
        public ADBMService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartUp();
        }

        protected override void OnStop()
        {
            ClearTransferFolders();
        }
        
        protected override void OnPause()
        {
        }

        protected override void OnContinue()
        {
        }

        #region IDebuggableService Members

        public void Start(string[] args)
        {
            OnStart(args);
        }

        public void StopService()
        {
            OnStop();
        }

        public void Pause()
        {
            OnPause();
        }

        public void Continue()
        {
            OnContinue();
        }

        #endregion

        protected void StartUp()
        {
            if (!InternetAvailable.PingNetwork("adbmftp.ADBM.com")) { Sleep(Convert.ToInt32(60000 * Convert.ToDecimal(ConfigurationManager.AppSettings["SyncSleepMinutes"]))); }
            //List<string> xxx = ADBMInterface.pathToADB();
            //ClearTransferFolders(); 
            //OnDemand();
            SyncChangedAttachmentFolders();
            //SyncThisFolder(1, 2);
            Sleep(Convert.ToInt32(60000 * Convert.ToDecimal(ConfigurationManager.AppSettings["SyncSleepMinutes"])));  //milliseconds: 1000 = 1 second, 60000 = 1 minute, 3,600,000 = 1 hour
        }

        private string GetRemotePathToThisAttachment(int p1, int p2)
        {
            string path;
            path = ConfigurationManager.AppSettings["pathRemoteAttachments"].ToString();
            if (p1 != 0) { path += @"\By_Log\" + p1; } else { path += @"\By_ERefNo\" + p2; }
            return path;
        }

        private string GetHostPathToThisAttachment(int p1, int p2)
        {
            string path;
            path = ConfigurationManager.AppSettings["pathHostAttachments"].ToString();
            if (p1 != 0) { path += @"/By_Log/" + p1; } else { path += @"/By_ERefNo/" + p2; }
            return path;
        }

        protected void SyncChangedAttachmentFolders()
        {
            string sel, hostpath, remotepath;
            //SFTPInterface sftpi = new SFTPInterface();
            //sel = @"select LogX, ERefNo = null from _Attachments where changed = 1 union select LogX = null, ERefNo from _Attachments where changed = 1";
            sel = @"select LogX, ERefNo from _Attachments where changed = 1";
            AttachmentCollection changed = Attachments.GetCollection(sel);
            foreach (Attachment a in changed)
            {
                hostpath = GetHostPathToThisAttachment(a.Logx, a.ERefNo);
                remotepath = GetRemotePathToThisAttachment(a.Logx, a.ERefNo);
                int xxx = 0;
                xxx = SFTPInterface.SynchronizeAttachmentFolder(remotepath, hostpath);
                if (xxx == 0) { Attachment.ClearAttachmentChangedFlag(a); }
            }
        }

        private void Sleep(int numberofmilliseconds)
        {
            //sleeptime = Convert.ToInt32(60000 * Convert.ToDecimal(ConfigurationManager.AppSettings["SyncSleepMinutes"]));
            Thread.Sleep(numberofmilliseconds); // 1000 = 1 second, 60000 = 1 minute, 3,600,000 = 1 hour
            StartUp();
        }






        private void SyncThisFolder(int logx, int erefno)
        {
            int xxx = 0;
            xxx = SFTPInterface.SynchronizeAttachmentFolder(@"C:\ADBMW\ADBM\ADBMSalesManagement\Attachments\By_ERefNo\29411", @"/Attachments/By_ERefNo/29411");
        }

        private void SyncExisting()
        {
            throw new NotImplementedException();
            //export local
            //transfer local attachments
            //upload and import to host
            //expot host
            //download and import to local
            //download host attachments
        }

        protected void ExportRemoteData()
        {
            ADBMInterface.RunADBMMacro("ExportRemoteData");
            //PutChangedRemoteData();
            //PutChangedRemoteAttachments();
            //ClearRemoteChangedFlags();
        }

        protected void PrepareHostData()
        {
            //PutMessageToExportHostDataOnHost();
            //Sleep(5minutes);
        }

        protected void ImportHostData()
        {
            //GetChangedHostData();
            ADBMInterface.RunADBMMacro("ImportHostData");
            //GetChangedHostAttachments();
            //ClearRemoteChangedFlags();
        }


        
        protected void CheckForDataChange()
        {
            //try{if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["remoteFolder1"]))           //"Z" + ":\\"{UnmapDriveLetter(ConfigurationManager.AppSettings["remoteDriveLetter"]);                //"Z:"}System.Diagnostics.Process p = new System.Diagnostics.Process();p.StartInfo.UseShellExecute = false;p.StartInfo.CreateNoWindow = true;p.StartInfo.RedirectStandardError = true;p.StartInfo.RedirectStandardOutput = true;p.StartInfo.FileName = "net.exe";p.StartInfo.Arguments = @" use " + ConfigurationManager.AppSettings["remoteDriveLetter"]+ " " + ConfigurationManager.AppSettings["remoteShareName"]+ " " + ConfigurationManager.AppSettings["userPwd"]+ " /user:" + ConfigurationManager.AppSettings["userName"];//EventLog.WriteEntry("Conversion Service", "Remote drive mapped successfully", EventLogEntryType.Information);p.Start();p.WaitForExit();}catch (Exception ex6_5){WriteException(ex6_5, "Ex6.5");}
        }

        protected void ParseDataChange()
        {
        }

        public void ClearTransferFolders()
        {
            string[] filesToDelete = Directory.GetFiles(ConfigurationManager.AppSettings["pathRemoteTransfer"]);
            try { foreach (string file in filesToDelete) { File.Delete(file); } }
            catch (Exception ex6) { //DisplayServiceMessage(ex6, "Ex6"); 
            }
        }

        protected string ADBPath(string ADB)
        {
            string path = "";
            return path;
        }
        //private void WatchForUploadedFiles(){FileSystemWatcher fsWatcher = new FileSystemWatcher();try{MapDriveLetter();fsWatcher.Created += new FileSystemEventHandler(OnFileCreated);FolderCleanUp("Directory ready for operation.");                                      // Parameter used for debugging onlyfsWatcher.Path = ConfigurationManager.AppSettings["remoteFolder"];             // @"Z:\";fsWatcher.Filter = ConfigurationManager.AppSettings["sourceFileName"];         // "Default.pptx";fsWatcher.EnableRaisingEvents = true;}catch (Exception ex1){WriteException(ex1, "Ex1");}}
        
    }
}
