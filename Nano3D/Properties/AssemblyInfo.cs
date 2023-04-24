using Rhino.PlugIns;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Plug-in Description Attributes - all of these are optional.
// These will show in Rhino's option dialog, in the tab Plug-ins.
[assembly: PlugInDescription(DescriptionType.Address, "10164 Yonge St. Unit 3 Richmond Hill ON")]
[assembly: PlugInDescription(DescriptionType.Country, "Canada")]
[assembly: PlugInDescription(DescriptionType.Email, "contact@nanodlp.com")]
[assembly: PlugInDescription(DescriptionType.Phone, "+1 289 809 4399")]
[assembly: PlugInDescription(DescriptionType.Fax, "")]
[assembly: PlugInDescription(DescriptionType.Organization, "Nano3D")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "")]
[assembly: PlugInDescription(DescriptionType.WebSite, "https://nano3dtech.com/")]

// Icons should be Windows .ico files and contain 32-bit images in the following sizes: 16, 24, 32, 48, and 256.
[assembly: PlugInDescription(DescriptionType.Icon, "Nano3D.EmbeddedResources.plugin-utility.ico")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
// This will also be the Guid of the Rhino plug-in
[assembly: Guid("aaf80a5c-08f2-43f3-8d0c-3154578fcd8f")]
