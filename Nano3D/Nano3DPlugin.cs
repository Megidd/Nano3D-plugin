using Rhino;
using Rhino.PlugIns;
using System;
using System.IO;
using System.Text;

namespace Nano3D
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class Nano3DPlugin : Rhino.PlugIns.PlugIn
    {
        public Nano3DPlugin()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the Nano3DPlugin plug-in.</summary>
        public static Nano3DPlugin Instance { get; private set; }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.

        private System.Diagnostics.Process service;

        /// <summary>
        /// Called when the plugin is being loaded.
        /// </summary>
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            // Pick a free port and run service listening on it.
            HttpHelper.port = HttpHelper.FindAvailablePort().ToString();
            service = System.Diagnostics.Process.Start("nano3d-service.exe", HttpHelper.port);
            RhinoApp.WriteLine("Nano3D service is started on port {0}.", HttpHelper.port);

            restageRUI();

            return LoadReturnCode.Success;
        }

        protected override void OnShutdown()
        {
            try
            {
                bool closed = service.CloseMainWindow();
                if (!closed)
                {
                    RhinoApp.WriteLine("Nano3D service window couldn't be closed.");
                }
                service.Close();
                RhinoApp.WriteLine("Nano3D service is closed.");
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("An error occurred while shutting down the Nano3D service: {0}", ex.Message);

                // Rhino3D would be closed, so the prompt cannot be read.
                // Write the error to the log file to be able to read it after Rhino3D is closed.
                Utilities.WriteToLogFile("Exception while closing the plugin: "+ex.Message);
            }
        }

        private void restageRUI() {
            // Finally, if you update your plugin,
            // Rhino will not re-stage the RUI file because it already exists.
            // You can get Rhino to re-stage the RUI file by deleting it in %APPDATA% and
            // restarting which will cause Rhino to copy the file again since it no longer exists.
            // This can be done programmatically by adding the following code to
            // your plugin object’s OnLoad override.
            // https://developer.rhino3d.com/guides/rhinocommon/create-deploy-plugin-toolbar/

            // Get the version number of our plugin, that was last used, from our settings file.
            var plugin_version = Settings.GetString("PlugInVersion", null);
            RhinoApp.WriteLine("Nano3D plugin version last used: {0}", plugin_version);
            RhinoApp.WriteLine("Nano3D plugin version currently: {0}", Version);

            if (!string.IsNullOrEmpty(plugin_version))
            {
                // If the version number of the plugin that was last used does not match the
                // version number of this plugin, proceed.
                if (0 != string.Compare(Version, plugin_version, StringComparison.OrdinalIgnoreCase))
                {
                    // Build a path to the user's staged RUI file.
                    var sb = new StringBuilder();
                    sb.Append(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    sb.Append(@"\McNeel\Rhinoceros\7.0\UI\Plug-ins\");
                    sb.AppendFormat("{0}.rui", Assembly.GetName().Name);

                    // Verify the RUI file exists.
                    var path = sb.ToString();
                    if (File.Exists(path))
                    {
                        try
                        {
                            // Delete the RUI file.
                            File.Delete(path);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    // Save the version number of this plugin to our settings file.
                    Settings.SetString("PlugInVersion", Version);
                }
            }

            // After successfully loading the plugin, if Rhino detects a plugin RUI
            // file, it will automatically stage it, if it doesn't already exist.
        }
    }
}