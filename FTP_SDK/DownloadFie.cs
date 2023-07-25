using System;
using System.IO;
using System.Net;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace FTP_SDK
{
    public class DownloadFie : CustomAction<DownloadFieConfig>
    {
        // Enumeration for target value types - Location
        public enum TargetValueTypeLocatie
        {  //downlaod in current instance
            downloadToCurrentInstance = 0,
            //download in specific location
            downlaodToLocation = 1,

        }

        // Enumeration for target value types
        public enum TargetValueType
        {
            //download single file
            downloadSingleFile = 0,
            //download all files
            downloadAllAttachments = 1
        }
        public override void Run(RunCustomActionParams args)
        {
            // Extract the value of tipDescarcare.Locatie from the configuration
            bool locatieBool = Configuration.downloadType.downloadToSpecificLocation;

            // Extract the value of tipDescarcare.Type from the configuration
            bool typeBool = Configuration.downloadType.downloadType;

            // Convert boolean values to integers
            int type = Convert.ToInt32(typeBool);
            int locatie = Convert.ToInt32(locatieBool);

            // Convert integers to enumerations
            var typeLocatie = (TargetValueTypeLocatie)locatie;
            var typeDownload = (TargetValueType)type;

            byte[] file = null;
            string[] nameFile = null;

            // Check the download type and perform corresponding actions
            switch (typeDownload)
            {
                //Type download only file
                case TargetValueType.downloadSingleFile:

                    try
                    {    // Download a single file using the DownloadFileFTP method
                        file = DownloadFileFTP(Configuration.fileDetails.Path, Configuration.fileDetails.FolderDownload, Configuration.fileDetails.FileDownload, Configuration.credentialsFTP.user, Configuration.credentialsFTP.Password);

                    }
                    catch (WebException)
                    { // In case of an error, set error flags and the corresponding error message
                        args.HasErrors = true;
                        args.Message = "err download single file";
                    }

                    // Check the location type and add the file accordingly
                    switch (typeLocatie)
                    {
                        //add fine in current instance
                        case TargetValueTypeLocatie.downloadToCurrentInstance:
                            try
                            {
                                args.Context.CurrentDocument.Attachments.AddNew(Configuration.fileDetails.FileDownload, file);
                            }
                            catch (WebException)
                            {
                                args.HasErrors = true;
                                args.Message = "err incarca fisier in atasament";
                            }
                            break;
                        case TargetValueTypeLocatie.downlaodToLocation:
                            try
                            {
                                
                                // Specify the complete file path and name where you want to save the byte array
                                string filePath = Configuration.downloadType.FileSpecificLocation;

                                // Use FileStream to create or overwrite the specified file
                                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    // Write the byte array to the file
                                    fileStream.Write(file, 0, file.Length);
                                }

                                Console.WriteLine("The file has been successfully saved to the specified path.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An error occurred while saving the file: {ex.Message}");
                            }
                            break;

                    }
                    break;

                case TargetValueType.downloadAllAttachments:

                    try
                    {

                        // Get the list of files from the specified folder
                        nameFile = GetFileList(Configuration.fileDetails.Path + Configuration.fileDetails.FolderDownload, Configuration.credentialsFTP.user, Configuration.credentialsFTP.Password);

                    }
                    catch (WebException)
                    {
                        args.HasErrors = true;
                        args.Message = "err extrage lista fisiere";
                    }
                    // Download each file from the list and add it to the specified location
                    for (int i = 0; i < nameFile.Length; i++)
                    {
                        if (Path.GetExtension(nameFile[i]) != "")
                        {
                            file = DownloadFileFTP(Configuration.fileDetails.Path, Configuration.fileDetails.FolderDownload, nameFile[i], Configuration.credentialsFTP.user, Configuration.credentialsFTP.Password);
                            switch (typeLocatie)
                            {
                                case TargetValueTypeLocatie.downloadToCurrentInstance:
                                    try
                                    {
                                        args.Context.CurrentDocument.Attachments.AddNew(nameFile[i], file);
                                    }
                                    catch (WebException)
                                    {
                                        args.HasErrors = true;
                                        args.Message = "err descarca in atasamente lista de fisiere";
                                    }
                                    break;
                                case TargetValueTypeLocatie.downlaodToLocation:
                                    try
                                    {
                                      

                                        // Specify the complete file path and name where you want to save the byte array
                                        string filePath = Configuration.downloadType.FileSpecificLocation;

                                        // Use FileStream to create or overwrite the specified file
                                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            // Write the byte array to the file
                                            fileStream.Write(file, 0, file.Length);
                                        }

                                        Console.WriteLine("The file has been successfully saved to the specified path.");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"An error occurred while saving the file: {ex.Message}");
                                    }
                                    break;

                            }
                        }

                    }
                    break;

            }

        }

        // Method for downloading a file using FTP protocol
        public static byte[] DownloadFileFTP(string url, string caleFolder, string fisier, string user, string pass)
        {
            byte[] file = null;
            try
            {
                // Check if the file exists on the FTP server
                if (checkFileExists(GetRequest(url + caleFolder + fisier, user, pass)))
                {
                    // Create a download request using FTP protocol
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url + caleFolder + fisier);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    request.Credentials = new NetworkCredential(user, pass);


                    // Get the response and data stream
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse(); // Error here
                                                                                     // Read data from the stream and convert it to a byte array
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    file = ToByteArray(responseStream);
                    reader.Close();
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                ex.ToString();
            }


            return file;
        }
        // Method for creating an FtpWebRequest
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

        // Method for checking if the file exists
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

        // Method for converting a Stream to a byte array
        public static Byte[] ToByteArray(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] chunk = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                ms.Write(chunk, 0, bytesRead);
            }

            return ms.ToArray();
        }

        // Method for getting the list of files from the FTP server
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