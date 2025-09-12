extends Node

var view_window = preload("res://ViewerWindow/viewer_window.tscn")

func _ready() -> void:
	var main = get_parent()
	if ( main == null ):
		print("No main parent control found")
		return
		
	var config = main.get_node("Configuration");
	if ( config == null ):
		print("No configuration node found.")
		return
		
	if ( view_window == null ):
		print("No viewing window resource found.")
		return
	
	var config_monitors 	= config.NumberOfMonitors
	var system_monitors		= DisplayServer.get_screen_count()
	var addicional_viewers	= min(config_monitors-1, system_monitors-1)
	print("Monitor in config={0}, connected={1}, additional={2}".format([config_monitors, system_monitors, addicional_viewers]))

	get_viewport().set_embedding_subwindows(false)

	# set patient window to size of default monitor
	var patient_window = main.get_node("PatientControl");
	if ( patient_window == null ):
		print("No patient window node found.")
	if ( patient_window != null ):
		patient_window.visible  = true
		patient_window.position = DisplayServer.screen_get_position(-1)
		patient_window.size  	= DisplayServer.screen_get_size(-1)
	
	if ( addicional_viewers > 0 ):
		for additional in range(addicional_viewers):
			var viewer = view_window.instantiate()
			if ( viewer == null ):
				print("Cannot instantiate a viewer window")
				return
			if ( viewer.get_class() != "Window"):
				print("Viewer window is not of type Window")
				return
			if ( viewer != null ):
				# add the viewer as a child of this node (note that this is not the main control)
				add_child(viewer)
				var monitor_id = additional + 1
				print("Create additional viewer on monitor {0}".format([monitor_id]))
				viewer.visible 	= true;
				viewer.position = DisplayServer.screen_get_position(monitor_id)
				viewer.title 	= "Viewer " + str(monitor_id)
				viewer.size  	= DisplayServer.screen_get_size(monitor_id)
		
		
