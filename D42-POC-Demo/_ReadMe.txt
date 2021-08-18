Project Structure:
	
	D42_API.cs
		- Device42 API logic. Static API class. All API methods should be implemented here. 

	D42_Helper.cs : Wizard_Helper.cs
		- Main application logic. 

	Wizard_Helper.cs : Wizard_Helper_Interface.cs
		- For methods that are not unique to this application and whose logic may potentially be reused in future applications.
		- Parent class that D42_Helper extends. If a method in D42_Helper can be genericized and abstracted, it should be implemented here and the method should be documented in the interface. 
	
	Wizard_Helper_Interface.cs
		- For methods that are not unique to this application and whose logic may potentially be reused in future applications. 

	Column_Struct.cs
		- Struct to hold the columns for storing the data dictionary.

	MainWindow.xaml.cs
		- Main WPF window. Instantiates a object of D42_Helper and uses it throughout the lifespan of the application.

	ConfigWindow.xaml.cs
		- Configuration window. Takes a D42_Helper object and calls update config with the present form values.