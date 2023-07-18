using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.DataAccess.Models
{
    public class DocumentDataTableModel
    {
        public List<DataTableColumn> Columns { get; set; }
        public int draw { get; set; }
        public int Length { get; set; }
        public List<DataOrder> Order { get; set; }
        public Search search { get; set; }
        public int Start { get; set; }
    }

    public class DocumentFileSummaryModel
    {
        public int DocumentDetailID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string UserName { get; set; }
        public string ProfileName { get; set; }
        public string LastCommentedUser { get; set; }
        public DateTime CommentedDatetime { get; set; }
        public int TotalRecords { get; set; }
        public string ApprovalStatus { get; set; }
        public string Approver { get; set; }
        public DateTime ApprovalCreatedDateTime { get; set; }
        public DateTime? ApprovallastUpdatedDateTime { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public int DocumentHeaderID { get; set; }  
        public string Createdtime { get; set; }
        public string ApprovalCreatedTime { get; set; }
        public string ApprovallastUpdatedTime { get; set; }
        public string Commentedtime { get; set; }

        public string CreatedDate { get; set; }

        public string ApprovalCreatedDate { get; set; }
        public string ApprovallastUpdatedDate { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string RoomNo { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public string GuestName { get; set; }
    }


    public class Search
    {
        public bool Regex { get; set; }
        public string Value { get; set; }
    }

    public class DataTableColumn
    {
        public int Data { get; set; }
        public string Name { get; set; }
        public bool Orderable { get; set; }
        public bool Searchable { get; set; }
        public Search Search { get; set; }

    }

    public class DapperResult
    {
        public string Dapper { get; set; }
    }
    public class DataOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; }


    }

}