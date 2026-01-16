using Godot;
using Godot.Collections;
using Serilog;

public partial class SearchResultsClass : Node
{
	private Godot.Collections.Array m_patient_data;
	private static readonly ILogger _log = Log.ForContext<SearchResultsClass>();

	public int Score = 100;

	public Godot.Collections.Array GetData()
	{
		return m_patient_data;
	}

	public override void _Ready()
	{
		_log.Information("SearchResultsClass ready");
		
		m_patient_data = new Godot.Collections.Array
		{
			new Array { 1, "Michael", "Smith", 34, "Engineer", "London", "10/12/2005", 0.5, 0, 0 },
			new Array { 2, "Louis", "Johnson", 28, "Doctor", "New York", "05/11/2023", 0 },
			new Array{3, "Ann", "Williams", 42, "Lawyer", "Tokyo", "18/03/2025", 0, 0},
			new Array{4, "John", "Brown", 31, "Teacher", "Sydney", "02/07/2024", 0, 0},
			new Array{5, "Frances", "Jones", 25, "Designer", "Paris", "29/09/2023", 0, 0},
			new Array{6, "Robert", "", 39, "Architect", "Berlin", "14/01/2026", 0, 0},
			new Array{7, "Lucy", "Davis", 36, "Accountant", "Madrid", "07/04/2024", 0, 0},
			new Array{8, "Mark", "Miller", 44, "Entrepreneur", "Toronto", "21/08/2025", 0, 0},
			new Array{9, "Paula", "Wilson", 29, "Journalist", "Rio de Janeiro", "10/12/2023", 0, 0},
			new Array{10, "Stephen", "Moore", 33, "Programmer", "Dubai", "30/11/2024", 0, 0},
			new Array{11, "Mark", "Jefferson", 31, "Dentist", "Lisbona", "10/02/2018", 0.47, 1},
			new Array{12, "James", "Taylor", 28, "Doctor", "Chicago", "03/06/2026", 0, 0},
			new Array{13, "Carmen", "Anderson", 42, "Lawyer", "Hong Kong", "25/02/2024", 0, 0},
			new Array{14, "John", "Thomas", 39, "Architect", "Amsterdam", "17/10/2025", 0, 0},
			new Array{15, "Paul", "Jackson", 44, "Entrepreneur", "Singapore", "09/05/2023", 0, 0},
			new Array{16, "Jennifer", "White", 29, "Journalist", "Cape Town", "01/03/2023", 0, 0},
			new Array{17, "Luke", "Harris", 33, "Programmer", "Seoul", "28/04/2023", 0, 0},
			new Array{18, "Peter", "Martin", 25, "Designer", "Mexico City", "11/08/2024", 0, 0},
			new Array{19, "Matthew", "Thompson", 39, "Architect", "Moscow", "13/09/2024", 0, 0},
			new Array{20, "Louise", "Garcia", 36, "Accountant", "Istanbul", "04/12/2025", 0, 0},
			new Array{21, "Matthew", "Martinez", 44, "Entrepreneur", "Buenos Aires", "06/01/2025", 0, 0},
			new Array{22, "Stephanie", "Robinson", 29, "Journalist", "Cairo", "22/07/2023", 0, 0},
			new Array{23, "Christopher", "Clark", 51, "Architect", "Tokyo", "12/05/2021", 0, 0},
			new Array{24, "Amanda", "Rodriguez", 33, "Graphic Designer", "Sydney", "11/03/2020", 0, 0},
			new Array{25, "Daniel", "Lewis", 47, "Software Engineer", "Berlin", "03/04/2023", 0, 0},
			new Array{26, "Victoria", "Lee", 28, "Marketing Specialist", "Toronto", "04/05/2021", 0, 0},
			new Array{27, "Joseph", "Walker", 55, "Professor", "London", "12/05/2021", 0, 0},
			new Array{28, "Ashley", "Young", 39, "Chef", "Paris", "22/05/2024", 0, 0},
			new Array{29, "Kevin", "Allen", 42, "Financial Analyst", "Mexico City", "08/02/2025", 0, 0},
			new Array{30, "Elizabeth", "King", 31, "Photographer", "Rome", "11/09/2020", 0, 0}
		};
		
	}
}
