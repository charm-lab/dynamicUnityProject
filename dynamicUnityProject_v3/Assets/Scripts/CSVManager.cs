using System.IO;
using UnityEngine;

public static class CSVManager
{
    //get timestamp
    private static string startTrialTimeStamp = getIRLTimeStamp();

    //csv directory/folder name
    private static string reportDirectoryName = "Report_Tests"; //Comment for real experiment
    //private static string reportDirectoryName = "Report"; //Use for real experiment**
    //csv file name
    private static string reportFileName = "report_" + startTrialTimeStamp + ".csv";
    //private static string reportFileName = "report.csv";
    //Define the delimter
    private static string reportSeparator = ",";
    //Deifne data headers for csv file
    private static string[] reportHeaders = new string[]
    {
        //Unity Info:
        "CurrentTime [sec]",
        "indexPositionCommand (1-DoF) [mm]",/*
        "indexPosition X [mm]",
        "indexPosition Y [mm]",
        "indexPosition Z [mm]",
        "indexOrientation X [deg]",
        "indexOrientation Y [deg]",
        "indexOrientation Z [deg]",
        "indexDiameter [m]",*/
        "thumbPositionCommand (1-DoF) [mm]",/*
        "thumbPosition X [mm]",
        "thumbPosition Y [mm]",
        "thumbPosition Z [mm]",
        "thumbOrientation X [deg]",
        "thumbOrientation Y [deg]",
        "thumbOrientation Z [deg]",
        "thumbScaleValue [m]",
        "cubePosition X [mm]",
        "cubePosition Y [mm]",
        "cubePosition Z [mm]",
        "cubeOrientation X [deg]",
        "cubeOrientation Y [deg]",
        "cubeOrientation Z [deg]",
        "cubeLength [m]",
        "cubeStiffness [N/m]",
        "startingAreaPosition X [mm]",
        "startingAreaPosition Y [mm]",
        "startingAreaPosition Z [mm]",
        "startingAreaRadius [m]",
        "startingAreaHeight [m]",
        "targetAreaPosition X [mm]",
        "targetAreaPosition Y [mm]",
        "targetAreaPosition Z [mm]",
        "targetAreaRadius [m]",
        "targetAreaHeight [m]",
        "Successes",
        "Failures",
        "timeOfCurrentSuccess [sec]",
        "timeSinceLastSuccess [sec]",
        "Trial Number",
        "Index Shear ForceY",
        "Thumb Shear ForceY",
        "CubeWeight",
        "CubeAccelerationX",
        "CubeAccelerationY",
        "CubeAccelerationZ",
        "Lift [>0 for pickup]",*/
        //Arduino Info:
        "Arduino finalPos1 [mm]",
        "Arduino finalPos2 [mm]",
        "Arduino Elapsed Time [ms]"
    };

    //Header for timestamp of real-world time
    //private static string irlTimeStampHeader = "IRL Timestamp";

    #region FileInteractions
    //Add line to the report
    //takes in string array of data
    public static void appendToReport(string[] arduinoVals, float[] unityVals)
    {
        //Check if the directory/folder is there, if not create it
        verifyDirectory();
        //Check if the file is there, if not create it
        verifyFile();

        //Write the new line to be appended to the file
        using (StreamWriter sw = File.AppendText(getFilePath()))
        {
            string finalString = "";
            //Loop through each element of the data
            for (int i = 0; i < unityVals.Length; i++)
            {
                //if finalString is not already written, it need the separator/delimtier added
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                //add data
                finalString += unityVals[i].ToString();
            }
            for (int j = 0; j < arduinoVals.Length; j++)
            {
                //if finalString is not already written, it need the separator/delimtier added
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                //add data and ensure arduino data goes after unity data
                finalString += arduinoVals[j];
            }

            //add the IRL timestamp
            //finalString += reportSeparator + getIRLTimeStamp();

            //Write data element to file
            sw.WriteLine(finalString);
        }

    }

    //Create a file to place data in
    public static void createReport()
    {
        //Verifiy the directory/folder exists first
        verifyDirectory();

        //Writing the reoprt - same concept as in appendToReport
        using (StreamWriter sw = File.CreateText(getFilePath()))
        {
            string finalString = "";
            //loop through all headers:
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                //if finalString is not already written, it need the separator/delimtier added
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                //add header
                finalString += reportHeaders[i];
            }
            //add the IRL timestamp header
            //finalString += reportSeparator + irlTimeStampHeader;

            //Write header line to file
            sw.WriteLine(finalString);

        }

    }

    #endregion FileInteractions

    #region FileOperations
    //Make sure there is a directory/folder to save data file to
    static void verifyDirectory()
    {
        //make string representing directory/folder path 
        string dir = getDirectoryPath();

        if (!Directory.Exists(dir))
        {
            //create Directory if one doesn't already exist
            Directory.CreateDirectory(dir);
        }
    }

    //Make sure there is a file to save data to
    static void verifyFile()
    {
        //make string representing file path 
        string file = getFilePath();

        if (!File.Exists(file))
        {
            //create file for data if one doesn't already exist
            createReport();
        }
    }
    #endregion FileOperations

    #region Queries
    //Returns name of folder/directory the csv is located in
    static string getDirectoryPath()
    {
        return Application.dataPath + "/" + reportDirectoryName;
    }

    //Returns name of the file path to csv
    static string getFilePath()
    {
        return getDirectoryPath() + "/" + reportFileName;
    }

    //Get the local time stamp of the real world
    static string getIRLTimeStamp()
    {
        return System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss-ff");
    }
    #endregion Queries

}
