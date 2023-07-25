using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace FTP_SDK
{
    public class UploadFileConfig : PluginConfiguration
    {//UF- acronime : UploadFile
        [ConfigGroupBox(DisplayName = "Credentials", Order = 1)]
        public Credentials_UF credentials { get; set; }


        [ConfigGroupBox(DisplayName = "Details about the file loaded", Order = 2)]
        public FileDescription_UF fileDescription { get; set; }

        [ConfigGroupBox(DisplayName = "Details about attachaments", Order = 2)]
        public DetailsAttachment_UF detailsAttachment { get; set; }
    }
    public class Credentials_UF
    {
        [ConfigEditableText(DisplayName = "User FTP")]
        public string User { get; set; }

        [ConfigEditableText(DisplayName = "Password FTP")]
        public string Password { get; set; }
    }
    public class FileDescription_UF
    {

        [ConfigEditableText(DisplayName = "Path FTP", Description = "ex ftp://dev.encorsa.ro/folderUndeSalvam")]
        public string caleFTP { get; set; }
    }

    public class DetailsAttachment_UF
    {
        [ConfigEditableText(DisplayName = "Id attachaments", Description = @" The ID from the database is retrieved through a query.")]
        public string IdAttachaments { get; set; }

    }
}