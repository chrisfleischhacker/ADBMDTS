using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using System.Configuration;

namespace ADBMDTS
{
    class SFTPInterface
    {

        public static int SynchronizeAttachmentFolder(string remotepath, string hostpath)
        {
            // Setup session options
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = ConfigurationManager.AppSettings["ftpserver"].ToString(),
                UserName = ConfigurationManager.AppSettings["ftplogin"].ToString(),
                Password = ConfigurationManager.AppSettings["ftppassword"].ToString(),
                SshHostKeyFingerprint = ConfigurationManager.AppSettings["ftpfingerprint"].ToString()
            };

            using (Session session = new Session())
            {
                try
                {

                    TransferOptions xto = new TransferOptions();
                    xto.PreserveTimestamp = false;
                    //xto.TransferMode = TransferMode.Automatic;
                    //FilePermissions xxx = new FilePermissions();
                    //xxx.Octal = "0777";
                    xto.FilePermissions = null;
                    xto.FileMask = "*.pdf";


                    // Will continuously report progress of synchronization
                    session.FileTransferred += FileTransferred;

                    // Connect
                    session.Open(sessionOptions);

                    SynchronizationResult synchronizationResult;
                    synchronizationResult = session.SynchronizeDirectories(SynchronizationMode.Remote, remotepath, hostpath, false, false, SynchronizationCriteria.Size, xto);  //, false, SynchronizationCriteria.Time, xto
                    SynchronizationResult rsynchronizationResult;
                    rsynchronizationResult = session.SynchronizeDirectories(SynchronizationMode.Local, remotepath, hostpath, false);  //, false, SynchronizationCriteria.Time, xto

                    // Throw on any error
                    synchronizationResult.Check();
                    rsynchronizationResult.Check();

                    return 0;
                }
                catch (Exception e)
                {
                    string error = e.InnerException.ToString();
                    return 1;
                }
                finally
                {
                    session.Dispose();
                }
            }
        }

        //protected void PutRemoteAttachment(string remotepath, string hostpath)
        //{
        //    try
        //    {
        //        // Setup session options
        //        SessionOptions sessionOptions = new SessionOptions
        //        {
        //            Protocol = Protocol.Sftp,
        //            HostName = "example.com",
        //            UserName = "user",
        //            Password = "mypassword",
        //            SshHostKeyFingerprint = "ssh-rsa 1024 xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx"
        //        };

        //        using (Session session = new Session())
        //        {
        //            // Connect
        //            session.Open(sessionOptions);

        //            // Upload files
        //            TransferOptions transferOptions = new TransferOptions();
        //            transferOptions.TransferMode = TransferMode.Binary;

        //            TransferOperationResult transferResult;
        //            transferResult = session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);

        //            // Throw on any error
        //            transferResult.Check();

        //            // Print results
        //            foreach (TransferEventArgs transfer in transferResult.Transfers)
        //            {
        //                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
        //            }
        //        }

        //        return 0;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: {0}", e);
        //        return 1;
        //    }

        //}

        //protected void GetHostAttachment(string remotepath, string hostpath)
        //{
        //    try
        //    {
        //        // Setup session options
        //        SessionOptions sessionOptions = new SessionOptions
        //        {
        //            Protocol = Protocol.Sftp,
        //            HostName = "example.com",
        //            UserName = "user",
        //            Password = "mypassword",
        //            SshHostKeyFingerprint = "ssh-rsa 1024 xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx"
        //        };

        //        using (Session session = new Session())
        //        {
        //            // Connect
        //            session.Open(sessionOptions);

        //            string stamp = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        //            string fileName = "export_" + stamp + ".txt";
        //            string remotePath = "/home/user/sysbatch/" + fileName;
        //            string localPath = "d:\\backup\\" + fileName;

        //            // Manual "remote to local" synchronization.

        //            // You can achieve the same using:
        //            // session.SynchronizeDirectories(
        //            //     SynchronizationMode.Local, localPath, remotePath, false, false, SynchronizationCriteria.Time, 
        //            //     new TransferOptions { IncludeMask = fileName }).Check();
        //            if (session.FileExists(remotePath))
        //            {
        //                bool download;
        //                if (!File.Exists(localPath))
        //                {
        //                    Console.WriteLine("File {0} exists, local backup {1} does not", remotePath, localPath);
        //                    download = true;
        //                }
        //                else
        //                {
        //                    DateTime remoteWriteTime = session.GetFileInfo(remotePath).LastWriteTime;
        //                    DateTime localWriteTime = File.GetLastWriteTime(localPath);

        //                    if (remoteWriteTime > localWriteTime)
        //                    {
        //                        Console.WriteLine(
        //                            "File {0} as well as local backup {1} exist, " +
        //                            "but remote file is newer ({2}) than local backup ({3})",
        //                            remotePath, localPath, remoteWriteTime, localWriteTime);
        //                        download = true;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine(
        //                            "File {0} as well as local backup {1} exist, " +
        //                            "but remote file is not newer ({2}) than local backup ({3})",
        //                            remotePath, localPath, remoteWriteTime, localWriteTime);
        //                        download = false;
        //                    }
        //                }

        //                if (download)
        //                {
        //                    // Download the file and throw on any error
        //                    session.GetFiles(remotePath, localPath).Check();

        //                    Console.WriteLine("Download to backup done.");
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("File {0} does not exist yet", remotePath);
        //            }
        //        }

        //        return 0;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: {0}", e);
        //        return 1;
        //    }

        //}

        //protected void PutRemoteData(string remotepath, string hostpath)
        //{
        //    try
        //    {
        //        // Setup session options
        //        SessionOptions sessionOptions = new SessionOptions
        //        {
        //            Protocol = Protocol.Sftp,
        //            HostName = "example.com",
        //            UserName = "user",
        //            Password = "mypassword",
        //            SshHostKeyFingerprint = "ssh-rsa 1024 xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx"
        //        };

        //        using (Session session = new Session())
        //        {
        //            // Connect
        //            session.Open(sessionOptions);

        //            // Upload files
        //            TransferOptions transferOptions = new TransferOptions();
        //            transferOptions.TransferMode = TransferMode.Binary;

        //            TransferOperationResult transferResult;
        //            transferResult = session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);

        //            // Throw on any error
        //            transferResult.Check();

        //            // Print results
        //            foreach (TransferEventArgs transfer in transferResult.Transfers)
        //            {
        //                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
        //            }
        //        }

        //        return 0;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: {0}", e);
        //        return 1;
        //    }

        //}

        //protected void GetHostData(string remotepath, string hostpath)
        //{
        //    try
        //    {
        //        // Setup session options
        //        SessionOptions sessionOptions = new SessionOptions
        //        {
        //            Protocol = Protocol.Sftp,
        //            HostName = "example.com",
        //            UserName = "user",
        //            Password = "mypassword",
        //            SshHostKeyFingerprint = "ssh-rsa 1024 xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx"
        //        };

        //        using (Session session = new Session())
        //        {
        //            // Connect
        //            session.Open(sessionOptions);

        //            string stamp = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        //            string fileName = "export_" + stamp + ".txt";
        //            string remotePath = "/home/user/sysbatch/" + fileName;
        //            string localPath = "d:\\backup\\" + fileName;

        //            // Manual "remote to local" synchronization.

        //            // You can achieve the same using:
        //            // session.SynchronizeDirectories(
        //            //     SynchronizationMode.Local, localPath, remotePath, false, false, SynchronizationCriteria.Time, 
        //            //     new TransferOptions { IncludeMask = fileName }).Check();
        //            if (session.FileExists(remotePath))
        //            {
        //                bool download;
        //                if (!File.Exists(localPath))
        //                {
        //                    Console.WriteLine("File {0} exists, local backup {1} does not", remotePath, localPath);
        //                    download = true;
        //                }
        //                else
        //                {
        //                    DateTime remoteWriteTime = session.GetFileInfo(remotePath).LastWriteTime;
        //                    DateTime localWriteTime = File.GetLastWriteTime(localPath);

        //                    if (remoteWriteTime > localWriteTime)
        //                    {
        //                        Console.WriteLine(
        //                            "File {0} as well as local backup {1} exist, " +
        //                            "but remote file is newer ({2}) than local backup ({3})",
        //                            remotePath, localPath, remoteWriteTime, localWriteTime);
        //                        download = true;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine(
        //                            "File {0} as well as local backup {1} exist, " +
        //                            "but remote file is not newer ({2}) than local backup ({3})",
        //                            remotePath, localPath, remoteWriteTime, localWriteTime);
        //                        download = false;
        //                    }
        //                }

        //                if (download)
        //                {
        //                    // Download the file and throw on any error
        //                    session.GetFiles(remotePath, localPath).Check();

        //                    Console.WriteLine("Download to backup done.");
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("File {0} does not exist yet", remotePath);
        //            }
        //        }

        //        return 0;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: {0}", e);
        //        return 1;
        //    }

        //}

        private static void FileTransferred(object sender, TransferEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Upload of {0} succeeded", e.FileName);
            }
            else
            {
                Console.WriteLine("Upload of {0} failed: {1}", e.FileName, e.Error);
            }

            if (e.Chmod != null)
            {
                if (e.Chmod.Error == null)
                {
                    Console.WriteLine("Permisions of {0} set to {1}", e.Chmod.FileName, e.Chmod.FilePermissions);
                }
                else
                {
                    Console.WriteLine("Setting permissions of {0} failed: {1}", e.Chmod.FileName, e.Chmod.Error);
                }
            }
            else
            {
                Console.WriteLine("Permissions of {0} kept with their defaults", e.Destination);
            }

            if (e.Touch != null)
            {
                if (e.Touch.Error == null)
                {
                    Console.WriteLine("Timestamp of {0} set to {1}", e.Touch.FileName, e.Touch.LastWriteTime);
                }
                else
                {
                    Console.WriteLine("Setting timestamp of {0} failed: {1}", e.Touch.FileName, e.Touch.Error);
                }
            }
            else
            {
                // This should never happen with Session.SynchronizeDirectories
                Console.WriteLine("Timestamp of {0} kept with its default (current time)", e.Destination);
            }
        }
    }
}
