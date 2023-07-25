using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace FTP_SDK
{
    public class CreateDirectoryConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Credentials", Order = 1)]

        public Credentials_CreateDir credentials { get; set; }


        [ConfigGroupBox(DisplayName = "FTP", Order = 2)]

        public FileDescription_CreateDir fileDescription { get; set; }


        [ConfigGroupBox(DisplayName = "Get Status", Order = 3)]
        public Code_CreateDir code { get; set; }
    }
    public class Credentials_CreateDir
    {
        [ConfigEditableText(DisplayName = "User")]
        public string User { get; set; }

        [ConfigEditableText(DisplayName = "Password")]
        public string Password { get; set; }
    }
    public class FileDescription_CreateDir
    {
        [ConfigEditableText(DisplayName = "Path", DescriptionAsHTML = true, Description = "ex: ftp://51.140.72.53  ")]
        public string Path { get; set; }

        [ConfigEditableText(DisplayName = "Path to create a new folder.", DescriptionAsHTML = true, Description = "ex: /folderExisting/newFolder </br> or <br> /newFolder if it is in the parent")]
        public string CaleFolder { get; set; }
    }
    public class Code_CreateDir
    {
        [ConfigEditableText(DisplayName = "Source Id field Status Code", Description = "Db Name Field")]
        public string IdStatusCode { get; set; }
    }
}