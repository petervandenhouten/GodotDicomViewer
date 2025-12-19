using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotDicomViewer.Code.Utils
{ 
    public static class DateTimeExtensions
    {
        public const string DicomDateFormat = "yyyyMMdd";
        public const string DicomDateTimeFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// Converts a DateTime to a DICOM-compliant date string format (yyyyMMdd).
        /// </summary>
        /// <param name="dateTime">The input DateTime.</param>
        /// <returns>The DICOM-compliant date string.</returns>
        public static string ToDicomString(this DateTime dateTime)
        {
            return dateTime.ToString(DicomDateFormat);
        }

        /// <summary>
        /// Converts a DateTime to a DICOM-compliant date and time string format (yyyyMMddHHmmss).
        /// </summary>
        /// <param name="dateTime">The input DateTime.</param>
        /// <returns>The DICOM-compliant date and time string.</returns>
        public static string ToDicomDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString(DicomDateTimeFormat);
        }

    }

}
