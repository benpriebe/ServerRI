#region Using directives

using System.Reflection;
using System.Runtime.InteropServices;
using log4net.Config;

#endregion


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Data.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Data.Tests")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("decef7d4-55f8-4255-95ee-75acc581261b")]
[assembly: XmlConfigurator(ConfigFile = "Log4Net.config", Watch = true)]