namespace Endava.Hl7.Fhir.Common.Contracts.Models
{
    public class PatientCsv : PatientBase
    {
        // Header:
        // Nr,Prefix,Identifier,FirstName,LastName,BirthDate,BirthPlace,CitizenshipCode,Gender,AddressStreetName,AddressStreetNo,AddressAppartmentNo,AddressPostalCode,AddressCity,AddressCountry,AddressType
        public string Nr { get; set; }

        public string AddressStreetName { get; set; }
        
        public string AddressStreetNo { get; set; }
        
        public string AddressAppartmentNo { get; set; }
        
        public string AddressPostalCode { get; set; }
        
        public string AddressCity { get; set; }
        
        public string AddressCountry { get; set; }
        
        public string AddressType { get; set; }
    }
}
