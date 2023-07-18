using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class DocumentPdfModel
	{
        public string DocumentName { get; set; }
	    public string Signature { get; set; }
		public string DataIdentifier { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string DocumentType { get; set; }
		public string imgheight { get; set; }
		public string imgwidth { get; set; }
		public string imgxaxis{ get; set; }
	    public string imgyaxis { get; set; }
		public string EmailAddress { get; set; }	
		public string PhoneNumber { get; set; }
		public string Address { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string City { get; set; }
		public string PinCode { get; set; }
		public string EmailAddressxaxis { get; set; }
		public string EmailAddressyaxis { get; set; }
		public string PhoneNumberxaxis { get; set; }
		public string PhoneNumberyaxis { get; set; }
		public string Addressxaxis { get; set; }
		public string Addressyaxis { get; set; }
		public string Countryxaxis { get; set; }
		public string Countryyaxis { get; set; }
		public string Statexaxis { get; set; }
		public string Stateyaxis { get; set; }
		public string Cityxaxis { get; set; }
		public string Cityyaxis { get; set; }
		public string PinCodexaxis { get; set; }
		public string PinCodeyaxis { get; set; }
		
	}
}