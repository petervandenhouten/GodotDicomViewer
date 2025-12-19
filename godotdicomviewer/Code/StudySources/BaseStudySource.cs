using System;
using System.Collections.Generic;
using System.Linq;
using FellowOakDicom;
using Godot;
using GodotDicomViewer.Code.Data;
using GodotDicomViewer.Code.Interfaces;

namespace GodotDicomViewer.StudySources
{
	public partial class BaseStudySource : Node
	{
		// Name of the study souce (hides Node.Name!!)
		public string SourceName { get; set; }

        private int m_request_count = 1;

		#region output actions
		public Action<bool> ActiveChanged { get; set; }
		public Action<ConnectionStatus> ConnectionStatusChanged { get; set; }

		public Action<string, List<PatientData>> PatientDataAvailable { get; set; }
		//public Action<string, string, ImageData> OneImageAvailable { get; set; }
		//public Action<string, string, List<ImageData>> AllImagesAvailable { get; set; }
		//public Action<string, List<StudyData>> AllStudiesAvailable { get; set; }
		//public Action<string, StudyData> OneStudyAvailable { get; set; }
		//public Action<string, string, SeriesData> OneSeriesAvailable { get; set; }
		//public Action<string, string, List<SeriesData>> AllSeriesAvailable { get; set; }
		#endregion

		#region output properties
		public ConnectionStatus ConnectionStatus => ConnectionStatus.NotApplicable;
        #endregion

        protected int new_request_count()
        {
            return m_request_count++;
        }
    //    protected List<StudyData> FindStudies(StudySourceSearchParameters search, Dictionary<string,StudyData>.ValueCollection studies)
    //    {
    //        if (search.Empty()) return studies.ToList();

        //        var query = from s in studies
        //                    where (string.IsNullOrEmpty(search.PatientName) || s.PatientName.Contains(search.PatientName, StringComparison.InvariantCultureIgnoreCase))
        //                    && (string.IsNullOrEmpty(search.PatientID) || s.PatientID.Contains(search.PatientID, StringComparison.InvariantCultureIgnoreCase))
        //                    && (search.PatientBirthDate is not null || s.PatientBirthDate == search.PatientBirthDate )
        //                    && (string.IsNullOrEmpty(search.StudyID) || s.StudyID.Contains(search.StudyID, StringComparison.InvariantCultureIgnoreCase))
        //                    && (search.StudyDate is not null || s.StudyDate == search.StudyDate)
        //                    && (string.IsNullOrEmpty(search.StudyAccessionNumber) || s.AccessionNumber.Contains(search.StudyAccessionNumber, StringComparison.InvariantCultureIgnoreCase))
        //                    && (string.IsNullOrEmpty(search.StudyDescription) || s.StudyDescription.Contains(search.StudyDescription, StringComparison.InvariantCultureIgnoreCase))
        //                    select s;
        //        return new List<StudyData>(query);
        //    }
        //    protected List<StudyData> FindStudies(StudySourceSearchParameters search, List<StudyData> studies)
        //    {
        //        if (search.Empty()) return studies.ToList();

        //        var query = from s in studies
        //                    where string.IsNullOrEmpty(search.PatientName)          || s.PatientName.Contains(search.PatientName, StringComparison.InvariantCultureIgnoreCase)
        //                    where string.IsNullOrEmpty(search.PatientID)            || s.PatientID.Contains(search.PatientID, StringComparison.InvariantCultureIgnoreCase)
        //                    where search.PatientBirthDate is null                   || s.PatientBirthDate == search.PatientBirthDate
        //                    where string.IsNullOrEmpty(search.StudyID)              || s.StudyID.Contains(search.StudyID, StringComparison.InvariantCultureIgnoreCase)
        //                    where search.StudyDate is null                          || s.StudyDate == search.StudyDate
        //                    where string.IsNullOrEmpty(search.StudyAccessionNumber) || s.AccessionNumber.Contains(search.StudyAccessionNumber, StringComparison.InvariantCultureIgnoreCase)
        //                    where string.IsNullOrEmpty(search.StudyDescription)     || s.StudyDescription.Contains(search.StudyDescription, StringComparison.InvariantCultureIgnoreCase)
        //                    select s;
        //        return new List<StudyData>(query);
        //    }
        //    protected List<SeriesData> FindSeries(string study_uid, List<SeriesData> series)
        //    {
        //        if (string.IsNullOrEmpty(study_uid)) return new List<SeriesData>();

        //        var dicom_uid = DicomUID.Parse(study_uid);
        //        var series_query = from x in series
        //                           where x.StudyInstanceUID == dicom_uid
        //                           select x;
        //        return new List<SeriesData>(series_query);
        //    }

        //    protected List<SeriesData> FindSeries(string study_uid, Dictionary<string, SeriesData>.ValueCollection series)
        //    {
        //        if (string.IsNullOrEmpty(study_uid)) return new List<SeriesData>();
        //        var dicom_uid = DicomUID.Parse(study_uid);
        //        var series_query = from x in series
        //                           where x.StudyInstanceUID == dicom_uid
        //                           select x;
        //        return new List<SeriesData>(series_query);
        //    }

        //    protected void InvokeOneAndAll(List<StudyData> studies)
        //    {
        //        foreach (var study in studies)
        //        {
        //            OneStudyAvailable?.Invoke(this.Name, study);
        //        }
        //        AllStudiesAvailable?.Invoke(this.Name, studies);
        //    }

        //    protected void InvokeOneAndAll(string study_uid, List<SeriesData> series)
        //    {
        //        foreach (var serie in series)
        //        {
        //            OneSeriesAvailable?.Invoke(this.Name, study_uid, serie);
        //        }
        //        AllSeriesAvailable?.Invoke(this.Name, study_uid, series);
        //    }
    }
}
