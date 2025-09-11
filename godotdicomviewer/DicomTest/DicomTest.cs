using Godot;

using FellowOakDicom;

public partial class DicomTest : Node
{
	public override void _Ready()
	{
		string filepath = ProjectSettings.GlobalizePath("res://DicomTest//DicomTest.dcm");
		var dicomFile = DicomFile.Open(filepath);
		var patientName = dicomFile.Dataset.GetString(DicomTag.PatientName);
		GD.Print($"Patient Name: {patientName}");
	}
}
