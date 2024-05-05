using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.Models
{
    public class DocumentModel
    {
        public string DocumentHeaderID { get; set; }
        public string DocumentDetailID { get; set; }
        public string DocumentID { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string DocumentFileName { get; set; }
        public byte[] DocumentFile { get; set; }
        public byte[] SignatureFile { get; set; }
        public string LastCommentedUser { get; set; }
        public DateTime? LastCommentedDateTime { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public List<CommentDetails> CommentDetails { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsComment { get; set; }
        public bool? IsSignaturePresent { get; set; }
        public string SignatureFileBase64 { get; set; }
        public bool? SignatureRequired { get; set; }
        public string UserID { get; set; }
        public bool IsNotRecycleBin { get; set; }
        public int LastCommentedUserID { get; set; }
        public string SenderId { get; set; }
        public string DocumentFileBase64 { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string RoomNo { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public string GuestName { get; set; }
    }

    public class CommentDetails
    {
        public int DocumentDetailID { get; set; }
        public int? CommentsParentID { get; set; }
        public string Comments { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UserName { get; set; }
    }

    public class DocumentTypeMaster
    {
        public string DocumentID { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public bool? IsActive { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string TotalRecords { get; set; }
    }

    public class DocumentInfoModel
    {
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string UpdatedDocumentType { get; set; }
        public string UpdatedDocumentName { get; set; }
        public string DocumentHeaderID { get; set; }
        public string DocumentDetailID { get; set; }
        public string Base64Signature { get; set; }
        public string DocumentFileName { get; set; }
        public string UserID { get; set; }
        public string ApproverUser { get; set; }
        public string Comment { get; set; }
        public bool IsApproval { get; set; }
        public int LastCommentedUserID { get; set; }
        public string SenderId { get; set; }
    }

    public class ApproverUser
    {
        public string ApprUserID { get; set; }
        public string ApprUserRealName { get; set; }
    }
    public class ApproverDetails
    {
        public string RealNames { get; set; }
        public string Comment { get; set; }
        //public byte[] SignatureFile { get; set; }
        public DateTime CreatedDateTime { get; set; }
        //public string SignatureFileBase64 { get; set; }
        public string ApprovalStatus { get; set; }

    }
    public class DocumentRequestModel
    {
        public string DocumentBase64 { get; set; }

        public string Username { get; set; }

        public string DocumentName { get; set; }

        public string DocumentType { get; set; }
        public string RoomNo { get; set; }
    }
    public class DocumentResponseModel
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public string DocID { get; set; }
        public string DocumentDetailID { get; set; }
    }

    public class SearchRequestModel
    {
        public string SearchQuery { get; set; }
    }

    public class ReservationData
    {
        public string ReservationNumber { get;set; }
    }
}
