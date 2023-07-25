using System;
using System.Net;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace FTP_SDK
{
    public class DeleteFile : CustomAction<DeleteFileConfig>
    {
        public override void Run(RunCustomActionParams args)
        {
            try
            {
                string code = string.Empty;
                code = DeleteFTPFile(Configuration.fileDetails.Path, Configuration.credentialsFTP.user, Configuration.credentialsFTP.Password);
                if (code != string.Empty)
                {
                    args.HasErrors = true;
                    args.Message = code;
                }
            }
            catch (Exception ex)
            {
                args.HasErrors = true;
                args.Message = ex.ToString();
            }
        }
        public static string DeleteFTPFile(string Folderpath, string UserId, string Password)
        {

            string code = string.Empty;
            try
            {
                while (checkFileExists(GetRequest(Folderpath, UserId, Password)))
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Folderpath);
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    request.Credentials = new System.Net.NetworkCredential(UserId, Password);
                    request.EnableSsl = false;
                    request.UsePassive = true;
                    FtpWebResponse response1 = (FtpWebResponse)request.GetResponse();
                    // response = response1.StatusDescription;
                    response1.Close();

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
    }
}