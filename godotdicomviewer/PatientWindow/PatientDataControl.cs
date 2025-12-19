using Godot;
using System;
using Serilog;

public partial class PatientDataControl : Control
{
	private static readonly ILogger _log = Log.ForContext<PatientDataControl>();
	
	[Export]
	public Godot.Node PatientData {get; set;}
	
	private void _on_patient_table_row_double_clicked(int row)
	{
		_log.Information($"_on_patient_table_row_double_clicked [Row={row}]");
	}
}
