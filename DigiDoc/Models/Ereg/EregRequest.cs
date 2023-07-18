using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models.Ereg
{
    public class EregRequest
    {
        public string ReservationNameId { get; set; }
    }
    public class EregResponseModel
    {

        public object responseData { get; set; }
        public bool result { get; set; }
        public string responseMessage { get; set; }
        public int statusCode { get; set; }


    }

    public class EregTempResponseModel
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public int TempID { get; set; }


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
        public byte[] RegTemplate { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public int? TempReservationID { get; set; }
        public string ClientConnection { get; set; }
         public string MembershipNo { get; set; }
        public bool IsBreakFastAvailable { get; set; }
        public string Base64Signature { get; set; }
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
        public string StateName { get; set; }
        public string CountryName { get; set; }
        }
    public class TempReservationRequests
    {
        public int? TempReservationID { get; set; }
        public string Base64Signature { get; set; }
        public string DocumentType { get; set; }
        public string EmailAddress { get; set; }
        public bool Enableemail { get; set; } = true;
        public string Phone { get; set; }
    }
    public class TempReservationRequest2
    {
        public int? TempReservationID { get; set; }
        public string Base64Signature { get; set; }
        public string DocumentType { get; set; }

        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string PostalCode { get; set; }
        public string CountryMasterID { get; set; }
        public string StateMasterID { get; set; }
        public string City { get; set; }
    }
    public class TempReservationRequest
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
        public byte[] RegTemplate { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public int? TempReservationID { get; set; }
        public string Base64Signature { get; set; }
        public string ClientConnection { get; set; }
        public string MembershipNo { get; set; }

        public string ProfileId { get; set; }

        public bool? IsBreakFastAvailable { get; set; }
        public string RoomNo { get; set; }
        public string GuestName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
    }
    public class DocumentRequestModel
    {
        public string DocumentBase64 { get; set; }

        public string Username { get; set; }

        public string DocumentName { get; set; }

        public string DocumentType { get; set; }

        public string Base64Signature { get; set; }
        public String ReservationNumber { get; set; }
    }
    public class DocumentResponseModel
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public string DocID { get; set; }
        public string DocumentDetailID { get; set; }
        public string DocumentHeaderID { get; set; }
    }
    public  class CountryMaster
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

    public class NotificationStatus
    {
        public string StatusCode { get; set; }  
        public string Message { get; set; }
    }
    public class ProcessDocumentRequestModel
    {
        public string UserName { get; set; }
        public byte[] DocumentFile { get; set; }
        public string ReservationNumber { get; set; }
        public string DocumentType{ get; set; }
        public byte[]  SignatureFile{ get; set; }
        public string DocumentFileName { get; set; }        
        public int ParentDocumentHeaderID { get; set; }
        public string Phonenumber{ get; set; }
        public string EmailAddress{ get; set; }
        public string RoomNo{ get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string GuestName{ get; set; }
    }
}