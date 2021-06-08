namespace Endava.Hl7.Fhir.Common.Contracts.Dto.Enum
{
    //
    // Summary:
    //     The type of an address (physical / postal). (url: http://hl7.org/fhir/ValueSet/address-type)
    //     (system: http://hl7.org/fhir/address-type)
    public enum AddressTypeEnum
    {
        //
        // Summary:
        //     Mailing addresses - PO Boxes and care-of addresses. (system: http://hl7.org/fhir/address-type)
        Postal = 0,
        //
        // Summary:
        //     A physical address that can be visited. (system: http://hl7.org/fhir/address-type)
        Physical = 1,
        //
        // Summary:
        //     An address that is both physical and postal. (system: http://hl7.org/fhir/address-type)
        Both = 2
    }
}
