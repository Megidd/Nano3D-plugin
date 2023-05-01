﻿using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nano3D
{
    internal class Utilities
    {
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

        public static uint GetUint32FromUser(string prompt)
        {
            double doubleResult = 0;
            uint result = 0;
            while (true)
            {
                var getNumberResult = RhinoGet.GetNumber(prompt, false, ref doubleResult);
                if (getNumberResult == Result.Cancel)
                    RhinoApp.WriteLine("Canceled by user.");
                else if (getNumberResult == Result.Success && doubleResult >= 0)
                {
                    result = (uint)doubleResult;
                    return result;
                }
                else
                    RhinoApp.WriteLine("Invalid input.");
            }
        }
    }
}
