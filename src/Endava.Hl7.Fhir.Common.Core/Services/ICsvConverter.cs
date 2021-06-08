using CsvHelper.Configuration;
using System.Collections.Generic;

namespace Endava.Hl7.Fhir.Common.Core.Services
{
    public interface ICsvConverter
    {
        List<T> ConvertTo<T, TMap>(string file)
            where T : class
            where TMap : ClassMap<T>;
    }
}