using System;

namespace GodotDicomViewer.Code.Interfaces
{
    public struct StudySourceSearchParameters
    {
        public string PatientName;
        public string PatientID;
        public DateTime? PatientBirthDate;
        public string StudyID;
        public string StudyAccessionNumber;
        public DateTime? StudyDate;
        public string StudyDescription;
        public string SeriesModality;

        public bool Empty()
        {
            return PatientName is null
                && PatientID is null
                && PatientBirthDate is null
                && StudyID is null
                && StudyAccessionNumber is null
                && StudyDate is null
                && StudyDescription is null
                && SeriesModality is null;
        }
    }
}
