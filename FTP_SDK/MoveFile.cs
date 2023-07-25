using System;
using System.IO;
using System.Net;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace FTP_SDK
{
    // This method is called when the custom action is executed.
    public class MoveFile : CustomAction<MoveFileConfig>
    {
        public override void Run(RunCustomActionParams args)
        {
            // Retrieve configuration values from CustomAction configuration.
            string user = Configuration.credentialsFTP.user;
            string pass = Configuration.credentialsFTP.Password;
            string path = Configuration.fileDetails.Path;
            string fileMove = Configuration.fileDetails.FileMove;
            string folderMove = Configuration.fileDetails.FolderMove;
            string[] fileNameCopy = null;
            bool download = Configuration.fileDetails.Download;
            string code = string.Empty;
            // If 'download' is set to 'false', move a single file specified by 'fileMove' to 'folderMove'.
            if (download == false)
            {
                code = MoveFiles(path, user, pass, folderMove, fileMove);
                if (code != string.Empty)
                {
                    args.HasErrors = true;
                    args.Message = code;
                }
            }
            else
            {// If 'download' is set to 'true', move multiple files in 'path' to 'folderMove'.
                if (checkFileExists(GetRequestDir(Configuration.fileDetails.Path, Configuration.credentialsFTP.user, Configuration.credentialsFTP.Password)))
                {
                    fileNameCopy = GetFileList(Configuration.fileDetails.Path, Configuration.credentialsFTP.user, Configuration.credentialsFTP.Password);

                    for (int i = 0; i < fileNameCopy.Length; i++)
                    {
                        if (Path.GetExtension(fileNameCopy[i]) != "")
                        {
                            i++;
                            try
                            {
                                code = MoveFiles(path, user, pass, folderMove, fileNameCopy[i]);
                                if (code != string.Empty)
                                {
                                    args.HasErrors = true;
                                    args.Message = code;
                                }
                            }
                            catch (WebException ex)
                            {

                                args.HasErrors = true;
                                args.Message = ex.ToString();
                            }
                        }
                    }
                }
            }
            //DeleteFTPFile(path+"/"+fileMove,user,pass);
        }
        // Method to move a single file from 'ftp_uri_from' to 'ftptopath'.
        public static string MoveFiles(string ftp_uri_from, string username, string password, string ftptopath, string filenameMove)
        {
            string code = string.Empty;
            try
            {
                // Loop until the file no longer exists on the FTP server.
                while (checkFileExists(GetRequest(ftp_uri_from + "/" + filenameMove, username, password)))
                {
                    FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftp_uri_from + "/" + filenameMove);
                    ftp.Method = WebRequestMethods.Ftp.Rename;
                    ftp.Credentials = new NetworkCredential(username, password);
                    ftp.UsePassive = true;
                    ftp.RenameTo = ftptopath + "/" + filenameMove;
                    FtpWebResponse ftpresponse = (FtpWebResponse)ftp.GetResponse();
                    ftpresponse.Close();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {

                    }
                    code = response.StatusDescription;
                }
            }
            return code;

        }
        
        // Method to delete an FTP file given its 'Folderpath', 'UserId', and 'Password'.
        public static string DeleteFTPFile(string Folderpath, string UserId, string Password)
        {

            string response = "";
            while (checkFileExists(GetRequest(Folderpath, UserId, Password)))
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Folderpath);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new System.Net.NetworkCredential(UserId, Password);
                request.EnableSsl = false;
                request.UsePassive = true;
                FtpWebResponse response1 = (FtpWebResponse)request.GetResponse();
                response = response1.StatusDescription;
                response1.Close();

            }
            return response;
        }
        // Method to create an FTP web request given the 'uriString', 'user', and 'pass'.
        public static FtpWebRequest GetRequest(string uriString, string user, string pass)
        {
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            try
            {
                request.Credentials = new NetworkCredential(user, pass);
                request.Method = WebRequestMethods.Ftp.GetFileSize;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return request;
        }
        // Method to create an FTP web request for listing directory details.
        public static FtpWebRequest GetRequestDir(string uriString, string user, string pass)
        {
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            try
            {
                request.Credentials = new NetworkCredential(user, pass);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return request;
        }
        // Method to check if an FTP file exists.
        public static bool checkFileExists(WebRequest request)
        {
            try
            {
                request.GetResponse().Close();
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }
        // Method to get the list of files from the specified 'url' using FTP.
        public static string[] GetFileList(string url, string ftpUserName, string ftpPassWord)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassWord);
                request.KeepAlive = false;
                request.UsePassive = true;
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);

                request.GetResponse().Close();
                response.Close();
                reader.Close();

                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                downloadFiles = null;
                return downloadFiles;
            }
        }
    }
}