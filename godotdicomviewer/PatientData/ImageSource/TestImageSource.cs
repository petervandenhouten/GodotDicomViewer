using Godot;
using System;

public partial class TestImageSource : Node, IImageSource
{
	[Export]
	public string Filename {get; set;}
	
	public override void _Ready()
	{
		Filename = "res://PatientData/ImageSource/background.jpg";
	}
}
