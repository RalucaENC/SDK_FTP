using System;
using System.Net;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace FTP_SDK
{
    // Custom action class for creating a directory on an FTP server
    public class CreateDirectory : CustomAction<CreateDirectoryConfig>
    {
        public static string responseCode = string.Empty;

        public override void Run(RunCustomActionParams args)
        {
            // Get the necessary configuration values
            string user = Configuration.credentials.User;
            string password = Configuration.credentials.Password;
            string path = Configuration.fileDescription.Path;
            string caleFisier = Configuration.fileDescription.CaleFolder;
            responseCode = string.Empty;
            try
            {
                // Call the method to create the FTP folder and get the response code
                responseCode = CreateFTPFolder(path + caleFisier, user, password);
                if (responseCode != string.Empty)
                {
                    // If there is a response code, set error flags and the corresponding error message
                    args.HasErrors = true;
                    args.Message = responseCode;
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs, set error flags and the exception message as the error message
                args.HasErrors = true;
                args.Message = ex.ToString();
            }
            // Optionally, set field values in the current document if needed
            //args.Context.CurrentDocument.SetFieldValue(Configuration.PriceFormFieldID, Configuration.Price);
        }
        // Method to create the FTP folder
        public static string CreateFTPFolder(string caleFisier, string UserId, string Password)
        {
            string code = string.Empty;

            bool IsCreated = false;
            bool existFile = checkFileExists(GetRequestDir(caleFisier, UserId, Password));


            if (existFile == false)
            {
                try
                {
                    IsCreated = true;
                    try
                    { 
                        // Create an FTP folder request
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(caleFisier);
                        request.EnableSsl = false;
                        request.UsePassive = true;
                        request.Method = WebRequestMethods.Ftp.MakeDirectory;
                        request.Credentials = new NetworkCredential(UserId, Password);
                        // Make the actual directory creation request
                        request.GetResponse().Close();

                    }
                    catch (Exception ex)
                    {
                        IsCreated = false;
                    }

                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        FtpWebResponse response = (FtpWebResponse)ex.Response;
                        if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            // Handle specific status code if needed
                        }
                        code = response.StatusDescription;

                    }

                }


            }
            else
            {
                code = "existing folder";
            }
            return code;
        }
        // Method to create an FtpWebRequest for listing directory details
        public static FtpWebRequest GetRequestDir(string uriString, string user, string password)
        {
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            try
            {
                request.Credentials = new NetworkCredential(user, password);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return request;
        }
        // Method to check if the file exists
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