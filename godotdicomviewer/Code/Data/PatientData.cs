using FellowOakDicom;
using GodotDicomViewer.Code.Utils;
using System;

namespace GodotDicomViewer.Code.Data
{
	[Serializable]
	public class PatientData : BaseData
	{
		// public StudySet Studies { get; set; }

		public PatientData()
		{ 
		}

		public PatientData(DicomDataset data)
		{
			data.CopyTo(this);
		   // Studies = new StudySet();
		}

		public PatientData(string patient_name, string patient_birthdate, string patient_id)
		{
			Add(DicomTag.PatientName,       patient_name);
			Add(DicomTag.PatientBirthDate,  patient_birthdate);
			Add(DicomTag.PatientID,         patient_id);
		   // Studies = new StudySet();
		}

		#region Data API
		public string PatientName { get { return GetSingleValueOrDefault(DicomTag.PatientName, ""); } }
		public string PatientBirthDate { get { return GetSingleValueOrDefault(DicomTag.PatientBirthDate, ""); } }
		public string PatientID { get { return GetSingleValueOrDefault(DicomTag.PatientID, ""); } }
		public string PatientSex { get { return GetSingleValueOrDefault(DicomTag.PatientSex, ""); } }

		// This UID is our own definition of a unique(?) identifier for a patient.
		// It can be used to create keys to store patient data, and to compare studies and patients.
		public override string UID => PatientName + "_" + PatientBirthDate + "_" + PatientID;
		#endregion

		public static string CreatePatientUID(string patient_name, DateTime patient_brithdate, string patient_id)
		{
			// Note: must be the same as the UID
			return patient_name + "_" + patient_brithdate.ToDicomString() + "_" + patient_id;
		}
		#region Utility functions
		//public List<int> GetYearsWithOneOrMoreStudies()
		//{
		//    var years = new List<int>();
		//    foreach(var study in Studies.Values)
		//    {
		//        if (!years.Contains(study.StudyDate.Year)) years.Add(study.StudyDate.Year);
		//    }
		//    return years;
		//}
		#endregion

		//public void extract_patient_information_from_study(StudyData study)
		//{
		//    // these info we should already have
		//    // Add(DicomTag.PatientName        , study.PatientName);
		//    // Add(DicomTag.PatientBirthDate   , study.PatientBirthDate);
		//    // Add(DicomTag.PatientID          , study.PatientID);

		//    var tagsToCopy = new DicomTag[] { DicomTag.PatientSex, DicomTag.OtherPatientIDsSequence, DicomTag.IssuerOfPatientID };
		//    copy_specific_tags(study, this, tagsToCopy);
		//}
	}
}
