using System;
using System.IO;
using System.Net;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace FTP_SDK
{
    public class UploadFile : CustomAction<UploadFileConfig>
    {
        public static string user = string.Empty;
        public static string password = string.Empty;

        // Method that executes the custom action logic
        public override void Run(RunCustomActionParams args)
        {
            // Extract FTP credentials from the configuration
            user = Configuration.credentials.User;
            password = Configuration.credentials.Password;
            string code = string.Empty;
            try
            {
                // Split the attachment IDs from the configuration
                string[] splitIdAtasament = Configuration.detailsAttachment.IdAttachaments.Split(';');

                // Loop through each attachment ID
                for (int i = 0; i < splitIdAtasament.Length; i++)
                {
                    // Get the attachment based on the ID
                    var attachment = args.Context.CurrentDocument.Attachments.GetByID(Convert.ToInt32(splitIdAtasament[i]));
                    var attachmentExtension = args.Context.CurrentDocument.Attachments.GetByID(Convert.ToInt32(splitIdAtasament[i])).FileExtension;
                    var nameAttachament = args.Context.CurrentDocument.Attachments.GetByID(Convert.ToInt32(splitIdAtasament[i])).FileName;
                    string extention = Path.GetExtension(nameAttachament);
                    string fileName = nameAttachament.Remove(nameAttachament.Length - extention.Length);

                    // Call the UploadFile method to upload the attachment to the FTP server
                    code = UploadFiles(Configuration.fileDescription.caleFTP + "/" + fileName + attachmentExtension, user, password, Configuration.fileDescription.caleFTP, attachment, fileName + attachmentExtension);

                    // Check for errors during the upload process
                    if (code != string.Empty)
                    {
                        args.HasErrors = true;
                        args.Message = code;

                    }
                }
            }
            catch (Exception ex)
            {
                args.HasErrors = true;
                args.Message = ex.ToString();
                args.LogMessage = ex.ToString();
            }
        }
        // Method for uploading a file to the FTP server
        public static string UploadFiles(string To, string UserId, string Password, string validUrl, WebCon.WorkFlow.SDK.Documents.Model.Attachments.AttachmentData attachment, string numeFis)
        {
            string code = string.Empty;
            try
            {
                // Check if the FTP directory exists
                if (checkFileExists(GetRequestDir(validUrl, UserId, Password)))
                {
                    // Check if the file does not already exist on the FTP server
                    if (!checkFileExists(GetRequestDir(validUrl + "/" + numeFis, UserId, Password)))
                    {
                        // Convert the attachment content to a string
                        string fileConvert = Encoding.Default.GetString(attachment.Content);

                        // Create the FTP upload request
                        FtpWebRequest myFtpWebRequest;
                        FtpWebResponse myFtpWebResponse;
                        StreamWriter myStreamWriter;

                        myFtpWebRequest = (FtpWebRequest)WebRequest.Create(To);
                        myFtpWebRequest.Credentials = new NetworkCredential(UserId, Password);
                        myFtpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

                        myFtpWebRequest.UseBinary = true;
                        myFtpWebRequest.UsePassive = true;
                        myFtpWebRequest.KeepAlive = true;

                        // Write the attachment content to the FTP request stream
                        myStreamWriter = new StreamWriter(myFtpWebRequest.GetRequestStream());
                        myStreamWriter.Write(fileConvert);
                        myStreamWriter.Close();

                        // Get the FTP response and complete the upload
                        myFtpWebResponse = (FtpWebResponse)myFtpWebRequest.GetResponse();
                        myFtpWebResponse.Close();
                    }
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

        // Method for checking if a file or directory exists on the FTP server
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

        // Method for creating an FTP request to check if a directory exists
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
        // Method for creating an FTP request to check if a file exists
        public static FtpWebRequest GetRequest(string uriString, string user, string pass)
        {
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Credentials = new NetworkCredential(user, pass);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            return request;
        }
    }
}