#region Using directives

using System.Reflection;
using System.Runtime.InteropServices;
using log4net.Config;

#endregion


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("WebApi.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("WebApi.Tests")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("2074d008-b7b0-4105-8fa4-08c7e6b35054")]
[assembly: XmlConfigurator(ConfigFile = "Log4Net.config", Watch = true)]