using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleMouseController
{
    /// <summary>
    /// The Program class
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args"></param>
        static public void Main(String[] args)
        {
            World world = new World();

            // Continuously update the world
            while (true)
            {
                world.Update();
            }
        }
    }
}