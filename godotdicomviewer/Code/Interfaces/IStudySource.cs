using System;
using System.Collections.Generic;
using GodotDicomViewer.Code.Data;

namespace GodotDicomViewer.Code.Interfaces
{
    public enum ConnectionStatus { NotApplicable, Connected, PartiallyConnected, Disconnected };

    public interface IStudySource 
    {
        // Should interface be RestFull and return a JSON object?
        // Should interface also include more traditional functions call?

        // Implementation rules
        // Every Get<data> function MUST return at least an Action "All Data Available" with the final results
        // A responde of "one data available" is optional
        // If the Get<data> function return false, is should be interpreted as "no data available"

        #region Status
        public string SourceName { get; set; }
        public Action<bool> ActiveChanged { get; set; }
        public Action<ConnectionStatus> ConnectionStatusChanged { get; set; }
        public ConnectionStatus ConnectionStatus { get; }
        #endregion

        #region Patients
        // maybe a bool to indicate the last?
        public Action<string /* source name */, List<PatientData>> PatientDataAvailable { get; set; }
        public int GetPatients(StudySourceSearchParameters search, bool force_refresh = false);
        #endregion

        //#region Studies
        //public Action<string /* source name */, StudyData> OneStudyAvailable { get; set; }
        //public Action<string /* source name */, List<StudyData>> AllStudiesAvailable { get; set; }
        //public bool GetAllStudies(bool force_refresh = false);
        //public bool GetStudies(StudySourceSearchParameters search, bool force_refresh = false);
        //#endregion

        //#region Series
        //public Action<string /* source name */, string /* study_uid */, SeriesData> OneSeriesAvailable { get; set; }
        //public Action<string /* source name */, string /* study_uid */, List<SeriesData>> AllSeriesAvailable { get; set; }
        //public bool GetSeries(string study_uid, bool force_refresh = false);
        //#endregion

        //#region Images
        //public Action<string, string, ImageData> OneImageAvailable { get; set; }
        //public Action<string, string, List<ImageData>> AllImagesAvailable { get; set; }
        //public bool GetImages(string study_uid, string series_uid, bool force_refresh = false);
        //public bool GetImage(string study_uid, string series_uid, bool force_refresh = false);
        //#endregion
    }
}
