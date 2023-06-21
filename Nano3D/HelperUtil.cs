using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nano3D
{
    internal class HelperUtil
    {
        public const bool debugMode = true;

        public const string logfile = "Nano3D-log.txt";
        public const string logfileMeshChecksHollowing = "Nano3D-log--mesh-checks--hollowing.txt";

        public static void WriteToLogFile(string text)
        {
            var now = DateTime.Now;
            var timestamp = now.ToString("MMM/dd/yyyy h:mm:ss tt");
            var message = $"{timestamp}: {text}";
            using (var file = new StreamWriter(logfile, true))
            {
                file.WriteLine(message);
            }
        }

        public static float GetFloatFromUser(double defaultValue, double lowerLimit, double upperLimit, string message)
        {
            // Create a GetNumber object
            GetNumber numberGetter = new GetNumber();
            numberGetter.SetLowerLimit(lowerLimit, false);
            numberGetter.SetUpperLimit(upperLimit, false);
            numberGetter.SetDefaultNumber(defaultValue);
            numberGetter.SetCommandPrompt(message);

            // Prompt the user to enter a number
            GetResult result = numberGetter.Get();

            // Check if the user entered a number
            switch (result){
                case GetResult.Number:
                    break;
                default:
                    return Convert.ToSingle(defaultValue);
            }

            // Get the number entered by the user
            double number = numberGetter.Number();

            return Convert.ToSingle(number);
        }

        public static uint GetUint32FromUser(string prompt, uint defaultValue, uint lowerLimit, uint upperLimit)
        {
            double doubleResult = defaultValue;
            uint result = defaultValue;
            while (true)
            {
                var getNumberResult = RhinoGet.GetNumber(prompt, false, ref doubleResult, lowerLimit, upperLimit);
                if (getNumberResult == Result.Cancel)
                {
                    RhinoApp.WriteLine("Canceled by user.");
                    return defaultValue;
                }
                else if (getNumberResult == Result.Success)
                {
                    result = (uint)doubleResult;
                    if (result < lowerLimit || result > upperLimit)
                        RhinoApp.WriteLine("Input out of range.");
                    else
                        return result;
                }
                else
                    RhinoApp.WriteLine("Invalid input.");
            }
        }

        public static bool GetYesNoFromUser(string prompt)
        {
            bool boolResult = false;
            while (true)
            {
                var getBoolResult = RhinoGet.GetBool(prompt, true, "No", "Yes", ref boolResult);
                if (getBoolResult == Result.Cancel)
                    RhinoApp.WriteLine("Canceled by user.");
                else if (getBoolResult == Result.Success)
                    return boolResult;
                else
                    RhinoApp.WriteLine("Invalid input.");
            }
        }

        public static void PrintWaitMessage(string commandName)
        {
            RhinoApp.WriteLine("The {0} command is running...", commandName);
            RhinoApp.WriteLine("Please wait...The output will be available when {0} command finishes...", commandName);
        }
    }
}
