extends VBoxContainer

@onready var patient_table_controller = $PanelContainer/PatientTableController
@onready var study_table_controller = $PanelContainer/StudyTableController

func _ready() -> void:
	patient_table_controller.visible = true
	study_table_controller.visible   = false
	
#func _on_button_back_to_patients_pressed() -> void:
#	print("Back to Patients")
#	study_table_controller.visible = false
#	patient_table_controller.visible = true

#func _on_patient_table_row_double_clicked(row: Variant) -> void:
#	print("Patient selected")
#	study_table_controller.visible = true
#	patient_table_controller.visible = false
