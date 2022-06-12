using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class FileUpload
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// True: File is stored in database in FileData field
        /// False: File is stored in FilePath relative to System_UploadPath.
        /// </summary>
        public bool IsDatabaseFile { get; set; }
        /// <summary>
        /// Binary data if IsDatabaseFile = true
        /// </summary>
        public byte[]? FileData { get; set; }
        /// <summary>
        /// Relative file path if IsDatabaseFile = false
        /// </summary>
        public string? FilePath { get; set; }
        /// <summary>
        /// IdentityUser Id of user who uploaded this file.
        /// </summary>
        public string? UploadedBy { get; set; }
        /// <summary>
        /// Date time for when this file was uploaded
        /// </summary>
        public DateTime WhenUploaded { get; set; }
        /// <summary>
        /// File name supplied by the user.
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty!;
        /// <summary>
        /// File type detected from the file.
        /// </summary>
        public string DetectedFileType { get; set; } = string.Empty!;
        /// <summary>
        /// Length in bytes of file.
        /// </summary>
        public int FileLength { get; set; }
        /// <summary>
        /// True: Indicates that this file is temporary (such as a CSV for a user import process).
        /// </summary>
        public bool IsTempFile { get; set; }
        public DocumentType? DocumentType { get; set; }
    }
}
