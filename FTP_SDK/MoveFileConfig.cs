using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace FTP_SDK
{
    public class MoveFileConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Credentials", Order = 1)]

        public Credentials_MF credentialsFTP { get; set; }

        [ConfigGroupBox(DisplayName = "Details about the move file", Order = 2)]
        public FileDetails_MF fileDetails { get; set; }
    }
    public class Credentials_MF
    {
        [ConfigEditableText(DisplayName = "User FTP")]
        public string user { get; set; }

        [ConfigEditableText(DisplayName = "Password FTP")]
        public string Password { get; set; }

    }
    public class FileDetails_MF
    {
        [ConfigEditableText(DisplayName = "Path move file", Description = "ex:ftp://51.140.72.53 cand fisierul este in parinte </br> sau  ftp://51.140.72.53/folder/", DescriptionAsHTML = true)]
        public string Path { get; set; }

        [ConfigEditableText(DisplayName = "Name file move", Description = "ex: FisierDeMutat.txt")]
        public string FileMove { get; set; }

        [ConfigEditableText(DisplayName = "Folder name move file", Description = "ex: Folder 1 sau Folder1/Folder2")]
        public string FolderMove { get; set; }

        [ConfigEditableText(DisplayName = "Copy single file/ all files from filder", Description = "false- single file; true-all file")]
        public bool Download { get; set; }
    }
}