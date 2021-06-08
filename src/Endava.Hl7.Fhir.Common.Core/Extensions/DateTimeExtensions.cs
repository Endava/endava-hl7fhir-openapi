using System;

namespace Endava.Hl7.Fhir.Common.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFhirDate(this DateTime me)
        {
            return me.ToString("yyyy-MM-dd");
        }

        public static string ToFhirDate(this DateTime? me)
        {
            if (me.HasValue)
            {
                return me.Value.ToString("yyyy-MM-dd");
            }
            return null;
        }

        public static string ToFhirDateTime(this DateTime me)
        {
            return me.ToString("yyyy-MM-ddTHH:mm:ss.sss");
        }

        public static string ToFhirDateTime(this DateTime? me)
        {
            if (me.HasValue)
            {
                return me.Value.ToString("yyyy-MM-ddTHH:mm:ss.sss");
            }
            return null;
        }
    }
}
