using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.WebAPI.Models.DigiDocMobile
{
    public class AppDetails
    {
        public int AppId { get; set; }
        public string AppName { get; set; }
        public string connectionId { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }
    public class TempReservation
    {
        public string ReservationNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CountryMasterID { get; set; }
        public string StateMasterID { get; set; }
        public string City { get; set; }
        public string RoomType { get; set; }
        public int ChildCount { get; set; }
        public int AdultCount { get; set; }
        public string FlightNo { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string PostalCode { get; set; }
        public string AppId { get; set; }
        public string DocumentType { get; set; }
        public byte[] FolioTemplate { get; set; }
        public int? TempId { get; set; }
        public string DocumentBase64 { get; set; }
        public string Base64Signature { get; set; }
        public string ClientConnection { get; set; }
        public string Base64Folio { get; set; }
        public string Phone { get; set; }
        public string ProfileId { get; set; }
        public string AddressLine1 { get; set; }
        public string MembershipNo { get; set; }
        public bool? IsBreakFastAvailable { get; set; }
        public byte[] SignatureFile { get; set; }
        public string RoomNo { get; set; }
        public string ReservationNameId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public decimal? AverageRoomRate { get; set; }
        public string Country_2Char_code { get; set; }
        public string Country_3Char_code { get; set; }
        public bool IsPDf { get; set; }
        public bool? IsPrintRate { get; set; }
        
    }
    public class EregTempResponseModel
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public int TempID { get; set; }


    }
    public class CountryMaster
    {
        public int CountryMasterID { get; set; }
        public string Country_Full_name { get; set; }
        public string Country_3Char_code { get; set; }
        public string Country_2Char_code { get; set; }
    }
    public class StateMaster
    {
        public int StateMasterID { get; set; }
        public string Statename { get; set; }
        public int? CountryMasterID { get; set; }
    }
    public class Document
    {
        public int DocumentId { get; set; }
        public int TempId { get; set; }
        public string Base64Folio { get; set; }


    }
    public class DocumentReq
    {

        public int Id { get; set; }

    }
    public class Request
    {
        public int Id { get; set; }

    }
    public class DocumentModel
    {

        public byte[] DocumentFile { get; set; }

    }
    public class NotificationStatus
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class DeviceDetails
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public int AppId { get; set; }
        public string SystemIP { get; set; }
    }


}