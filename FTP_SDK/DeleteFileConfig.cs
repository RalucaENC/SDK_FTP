using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace FTP_SDK
{
    public class DeleteFileConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Credentials", Order = 1)]

        public Credentials_DelF credentialsFTP { get; set; }

        [ConfigGroupBox(DisplayName = "Details about the deleted file", Order = 2)]
        public FileDetails_DelF fileDetails { get; set; }
    }
    public class Credentials_DelF
    {
        [ConfigEditableText(DisplayName = "User FTP")]
        public string user { get; set; }

        [ConfigEditableText(DisplayName = "Password FTP")]
        public string Password { get; set; }

    }
    public class FileDetails_DelF
    {
        [ConfigEditableText(DisplayName = "Path deleted file", Description = "ex: ftp://51.140.72.53/bravo/fisierDeSters.txt")]
        public string Path { get; set; }
    }

}