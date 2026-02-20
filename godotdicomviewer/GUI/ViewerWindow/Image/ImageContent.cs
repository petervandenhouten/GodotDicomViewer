using Godot;
using System;
using Serilog;

public partial class ImageContent : GridContainer
{
	private static readonly ILogger _log = Log.ForContext<ImageContent>();
	private Sprite2D image_holder;
	private IImageSource? image_source;
	
 	public override void _Ready()
 	{
		image_holder = GetNode<Sprite2D>("ImageHolder");
		if ( image_holder is null ) 
		{
			_log.Error("No image holder for image content");
			return;
		}
				
		foreach (var child in GetChildren())
		{
			if (child is Node node && node.IsInGroup("DataSource"))
			{
				if ( image_source == null )
				{
					image_source = child as IImageSource;
					_log.Information("Direct child in group: {name}", node.Name);
				}
			}
		}
		
		if ( image_source != null )
		{
			var texture = ResourceLoader.Load<Texture2D>(image_source.Filename);
			if ( texture != null )
			{
				image_holder.Texture = texture;
			}
			else
			{
				_log.Error("No image file found: {f}", image_source.Filename);	
			}
		}
		else
		{
			_log.Error("No image source available");
		}
	}
}
