using System.Collections.Generic;
using Godot;
using GodotDicomViewer.Code.Data;
using GodotDicomViewer.Code.Interfaces;
using Serilog;

public partial class Search : PanelContainer
{
	private static readonly ILogger _log = Log.ForContext<Search>();
	private IStudySource? m_study_source = null;
	
	public override void _Ready()
	{
		m_study_source = GetStudySource();

		if ( m_study_source != null)
		{
			_log.Information("Connected to Patient Data.");
			m_study_source.PatientDataAvailable += handle_received_patient_data;
			m_study_source.GetPatients( new StudySourceSearchParameters() );
		}
	}

	private void handle_received_patient_data(string source, List<PatientData> data)
	{
		_log.Information("handle_received_patient_data from");
		
		// MVC:
		// update patient data 
		// trigger update GUI
		
	}

	private IStudySource? GetStudySource()
	{
		var patient_data_node = GetNode("/root/Main/PatientData");
		if (patient_data_node is null)
		{
			_log.Warning("No Patient Data node found.");
		}
		else
		{ 
			if (patient_data_node is IStudySource study_source)
			{
				return study_source;
			}
			else
			{
				_log.Warning("Patient Data node does not have IStudySource interface.");
			}
		}
		return null;
	}

}
