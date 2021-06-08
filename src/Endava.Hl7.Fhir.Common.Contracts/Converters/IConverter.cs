namespace Endava.Hl7.Fhir.Common.Contracts.Converters
{
    /// <summary>
    /// Generic converter interface contract
    /// </summary>
    public interface IConverter<in TFrom, out TTo>
    {
        TTo Convert(TFrom source);
    }
}
