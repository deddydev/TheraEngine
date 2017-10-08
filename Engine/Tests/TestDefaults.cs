using System;

namespace TheraEngine.Tests
{
    public static class TestDefaults
    {
        public static string DesktopPath { get; private set; }
        public static string GoogleDrivePath { get; private set; }
        static TestDefaults()
        {
            switch (Environment.MachineName)
            {
                case "DAVID-DESKTOP":
                    DesktopPath = "X:\\Desktop\\";
                    GoogleDrivePath = "X:\\Cloud Storage\\Google Drive\\TheraDev\\";
                    break;
                case "DAVID-LAPTOP":
                    DesktopPath = "C:\\Users\\David\\Desktop\\";
                    GoogleDrivePath = "C:\\Users\\David\\Google Drive\\TheraDev\\";
                    break;
            }
        }
    }
}
