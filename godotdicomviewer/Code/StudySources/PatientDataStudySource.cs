using Godot;
using GodotDicomViewer.Code.Interfaces;
using GodotDicomViewer.Code.Data;
using GodotDicomViewer.StudySources;
using System;
using System.Collections.Generic;
using Serilog;

namespace GodotDicomViewer.StudySources
{
	// A top level study source should implement a GetPatient and update it with every study that is retrieved
	public partial class PatientDataStudySource : BaseStudySource, IStudySource
	{
		private static readonly ILogger _log = Log.ForContext<PatientDataStudySource>();

		// Configured child study sources
		[Export]
		public Godot.Collections.Array<Node> StudySources = new();

		#region private data
		// Cache of files belonging to a study
		private List<IStudySource> m_study_sources_interfaces = new List<IStudySource>();

		// Counter to keep track of study sources that have responded a request
		private int m_sources_reponded;
		#endregion

		public PatientDataStudySource()
		{
			Name = "PatientDataStudySource";
		}

		public override void _Ready()
		{
			connect_to_child_study_source_interfaces();
			set_active(false);
		}

		private void connect_to_child_study_source_interfaces()
		{
			if (StudySources.Count == m_study_sources_interfaces.Count) return;

			m_study_sources_interfaces = new List<IStudySource>();

			foreach (var node in StudySources)
			{
				//var node = GetNode<IStudySource>("")
				if (node is IStudySource study_source)
				{
					study_source.PatientDataAvailable += handle_patient_data_available;
					m_study_sources_interfaces.Add(study_source);
					_log.Information($"PatientData Connected to StudySource node {node.Name}");
				}
			}

			if (m_study_sources_interfaces.Count == 0)
			{
				_log.Warning("No StudySource node configured in Patient Data");
			}
		}

		private void handle_connection_status(ConnectionStatus status)
		{
			ConnectionStatusChanged?.Invoke(ConnectionStatus);
		}

		public new ConnectionStatus ConnectionStatus
		{
			get
			{
				bool all_connected = true;
				bool some_connected = false;
				foreach (var study_interface in m_study_sources_interfaces)
				{
					if (study_interface.ConnectionStatus != ConnectionStatus.NotApplicable)
					{
						if (study_interface.ConnectionStatus != ConnectionStatus.Connected) all_connected = false;
						if (study_interface.ConnectionStatus == ConnectionStatus.Connected) some_connected = true;
					}
				}
				if (all_connected) return ConnectionStatus.Connected;
				if (some_connected) return ConnectionStatus.PartiallyConnected;
				return ConnectionStatus.Disconnected;
			}
		}

		//public Action<string, PatientData> PatientDataAvailable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		private void handle_patient_data_available(string sender, List<PatientData> patient_data)
		{
			m_sources_reponded++; // todo check for duplicates,use image set
			_log.Information($"Patient Data Available [Nr.Items={patient_data.Count}, Nr.Responses={m_sources_reponded}]");
			this.PatientDataAvailable?.Invoke(sender, patient_data);
		}

		//private void handle_all_images_downloaded(string sender, string series_uid, List<ImageData> images)
		//{
		//    // todo check for duplicates, use image set
		//    m_sources_reponded++;
		//    UberDebug.LogChannel("StudySource", $"Image All available [Nr.Reponses={m_sources_reponded}");
		//    this.AllImagesAvailable?.Invoke(this.Name, series_uid, images);
		//    set_active(false);
		//}

		//private void handle_one_image_downloaded(string sender, string sop_uid, ImageData image)
		//{
		//    // todo check for duplicates,use image set
		//    m_sources_reponded++;
		//    UberDebug.LogChannel("StudySource", $"Image One available [Nr.Reponses={m_sources_reponded}");
		//    this.OneImageAvailable?.Invoke(this.Name, sop_uid, image);
		//    set_active(false);
		//}

		//private void handle_one_series_available(string sender, string study_uid, SeriesData series)
		//{
		//    UberDebug.LogChannel("StudySource", $"Received One series from {sender}.");
		//    this.OneSeriesAvailable?.Invoke(this.Name, study_uid, series);
		//    set_active(true);
		//}

		//private void handle_all_series_available(string sender, string study_uid, List<SeriesData> series)
		//{
		//    UberDebug.LogChannel("StudySource", $"Received {series.Count} series from {sender}.");
		//    this.AllSeriesAvailable?.Invoke(this.Name, study_uid, series);
		//    set_active(false);
		//}

		//private void handle_all_studies_available(string sender, List<StudyData> studies)
		//{
		//    UberDebug.LogChannel("StudySource", $"Received {studies.Count} studies from {sender}.");
		//    this.AllStudiesAvailable?.Invoke(this.Name, studies);
		//    set_active(false);
		//}

		//private void handle_one_studies_available(string sender, StudyData study)
		//{
		//    UberDebug.LogChannel("StudySource", $"Received One studies from {sender}.");
		//    this.OneStudyAvailable?.Invoke(this.Name, study);
		//    set_active(true);
		//}

		//// Request for all availAble data without filtering
		//public bool GetAllStudies(bool force_refresh)
		//{
		//    set_active(true);

		//    bool found_in_cache = false;

		//    if (!found_in_cache)
		//    {
		//        connect_to_child_study_source_interfaces();
		//        UberDebug.LogChannel("StudySource", $"Call GetAllStudies for all sources.");
		//        foreach (var study_interface in m_study_sources_interfaces)
		//        {
		//            study_interface.GetAllStudies();
		//        }
		//    }
		//    return true;
		//}

		//public bool GetImages(string study_uid, string series_uid, bool force_refesh = false)
		//{
		//    set_active(true);

		//    // todo cache (maybe on disk to save memory)
		//    bool found_in_cache = false;

		//    if (!found_in_cache)
		//    {
		//        connect_to_child_study_source_interfaces();
		//        UberDebug.LogChannel("StudySource", $"Call GetImages for all sources.");
		//        foreach (var study_interface in m_study_sources_interfaces)
		//        {
		//            study_interface.GetImages(study_uid, series_uid, force_refesh);
		//        }
		//    }
		//    return true;
		//}
		//public bool GetImage(string study_uid, string series_uid, bool force_refresh = false)
		//{
		//    set_active(true);

		//    // todo cache (maybe on disk to save memory)
		//    bool found_in_cache = false;

		//    if (!found_in_cache)
		//    {
		//        connect_to_child_study_source_interfaces();
		//        UberDebug.LogChannel("StudySource", $"Call GetImage for all sources.");
		//        foreach (var study_interface in m_study_sources_interfaces)
		//        {
		//            study_interface.GetImage(study_uid, series_uid, force_refresh);
		//        }
		//    }
		//    return true;
		//}
		////        private IEnumerator RetrieveImages(string study_uid, string series_uid)
		////        {
		////            // Maybe we should accept only 1 image source??
		////            m_combined_images = new List<ImageData>();
		////            m_sources_reponded = 0;

		////            foreach (var study_interface in m_study_sources_interfaces)
		////            {
		//////                study_interface.GetImages(study_uid, series_uid);
		////            }

		////            while (m_sources_reponded < m_study_sources_interfaces.Count)
		////            {
		////                yield return null;
		////            }

		////            ImagesDownloaded?.Invoke(m_combined_images);
		////        }

		////public PatientSet GetPatients()
		////{
		////    return m_patient_data;
		////}

		//public bool GetStudies(StudySourceSearchParameters search, bool force_refresh)
		//{
		//    set_active(true);

		//    bool found_in_cache = false;

		//    if (!found_in_cache)
		//    {
		//        connect_to_child_study_source_interfaces();
		//        UberDebug.LogChannel("StudySource", $"Call GetStudies for all sources. [PatientName={search.PatientName}]");
		//        foreach (var study_interface in m_study_sources_interfaces)
		//        {
		//            study_interface.GetStudies(search, force_refresh);
		//        }
		//    }
		//    return true;
		//}

		//public bool GetSeries(string study_uid, bool force_refresh)
		//{
		//    set_active(true);

		//    bool found_in_cache = false;

		//    if (!found_in_cache)
		//    {
		//        connect_to_child_study_source_interfaces();
		//        UberDebug.LogChannel("StudySource", $"Call GetSeries for all sources. [StudyUID={study_uid}]");
		//        foreach (var study_interface in m_study_sources_interfaces)
		//        {
		//            study_interface.GetSeries(study_uid, force_refresh);
		//        }
		//    }
		//    return true;
		//}

		private void set_active(bool active)
		{
			ActiveChanged?.Invoke(active);
		}

		#region IStudySource API
		public int GetPatients(StudySourceSearchParameters search, bool force_refresh = false)
		{
			_log.Information("Main GetPatients");
			int request_count = -1;
			foreach (var study_interface in m_study_sources_interfaces)
			{
				_log.Information($"Main GetPatients calling {study_interface.SourceName}");
				request_count = study_interface.GetPatients(search, force_refresh);
			}
			return request_count;
		}
		#endregion

		//public bool GetPatients(StudySourceSearchParameters search, bool force_refresh = false)
		//{
		//    throw new NotImplementedException();
		//}
	}
}
