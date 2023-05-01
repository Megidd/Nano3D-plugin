using Rhino;
using Rhino.PlugIns;
using System;

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

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            HttpHelper.port = HttpHelper.FindAvailablePort().ToString();
            service = System.Diagnostics.Process.Start("nano3d-service.exe", HttpHelper.port);
            RhinoApp.WriteLine("Nano3D service is started on port {0}.", HttpHelper.port);
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
    }
}