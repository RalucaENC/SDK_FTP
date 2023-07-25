using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace FTP_SDK
{
    public class DownloadFieConfig : PluginConfiguration
    {//acronime _DF from class name = Download File
        [ConfigGroupBox(DisplayName = "Credentials", Order = 1)]
        public CredentialsFTP_DF credentialsFTP { get; set; }

        // Configuration group for details about the download file
        [ConfigGroupBox(DisplayName = "Details about the download file", Order = 2)]
        public FileDetails_DF fileDetails { get; set; }

        // Configuration group for the download type
        [ConfigGroupBox(DisplayName = "Type download", Order = 3)]
        public DownloadType_DF downloadType { get; set; }
    }
    public class CredentialsFTP_DF
    {
        [ConfigEditableText(DisplayName = "User FTP")]
        public string user { get; set; }

        [ConfigEditableText(DisplayName = "Password FTP")]
        public string Password { get; set; }

    }

    // Class for file details
    public class FileDetails_DF
    {
        [ConfigEditableText(DisplayName = "Path download file", Description = "ex:ftp://51.140.72.53", DescriptionAsHTML = true)]
        public string Path { get; set; }

        [ConfigEditableText(DisplayName = "Folder downlaod file", Description = "ex: /folder/folder2")]
        public string FolderDownload { get; set; }

        [ConfigEditableText(DisplayName = "File name download", Description = "ex: File.txt")]
        public string FileDownload { get; set; }

    }


    // Class for download type
    public class DownloadType_DF
    {
        [ConfigEditableText(DisplayName = "Download To Specific Location ?", Description = "Choices will be: False/true depending on location </br> download In Attachments=False, </br> download In a Specified Location=True", DescriptionAsHTML = true)]
        public bool downloadToSpecificLocation { get; set; }
        [ConfigEditableText(DisplayName = "Download Type", Description = "The choices will be as follows: download a single file by specifying the name and path = true </br> Download all files from the specified folder = false", DescriptionAsHTML = true)]
        public bool downloadType { get; set; }

        [ConfigEditableText(DisplayName = "File path", Description = " Specify the complete file path and name where you want to save the attachments. We will complete only if the variable \"Download To Specific Location ?\" is true.")]
        public string FileSpecificLocation { get; set; }


    }
}