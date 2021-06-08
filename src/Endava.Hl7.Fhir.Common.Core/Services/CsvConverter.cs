using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Endava.Hl7.Fhir.Common.Core.Services
{
    public class CsvConverter : ICsvConverter
    {
        public List<T> ConvertTo<T, TMap>(string file)
            where T : class
            where TMap : ClassMap<T>
        {
            using var reader = new StreamReader(file, Encoding.Default);
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim
            };
            using var csvReader = new CsvReader(reader, csvConfig);
            csvReader.Context.RegisterClassMap<TMap>();
            var records = csvReader.GetRecords<T>().ToList();
            return records;
        }
    }
}
