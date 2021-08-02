using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System.Threading;

public class SerialComms : MonoBehaviour
{
    GameObject player;

    //Set the port and the baud rate to 9600
    public string portName = "COM3";
    public int baudRate = 9600;
    SerialPort stream;

    private float lastTime = 0.0f;
    private float currentTime = 0.0f;

    public static string[] arduinoDataVals;
    public static float[] unityDataVals;

    public int expectedUnityEntries;

    private List<string[]> arduinoDataList;
    private List<float[]> unityDataList;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start Serial Comms");
        //initialize lists
        arduinoDataList = new List<string[]>();
        unityDataList = new List<float[]>();

        //Start the hand behavior/game logic as disabled until serial comms is up
        player = GameObject.Find("Player");

        //Define and open serial port       
        stream = new SerialPort(portName, baudRate);
        stream.Open();
        Debug.Log("Serial Communication Established");

        //Serial Port Read and Write Timeouts
        stream.ReadTimeout = 5;
        stream.WriteTimeout = 10;

        //Enable Game Logic
        player.GetComponent<GameLogic>().enabled = true;
        Debug.Log("Game Logic Enabled");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("SerialComms.cs");
        if (stream.IsOpen)
        {
            currentTime = Time.time;

            if (currentTime - lastTime > 0.005)
            {
                float num1 = player.GetComponent<GameLogic>().indexPositionCommand;
                float num2 = player.GetComponent<GameLogic>().thumbPositionCommand;
                float num3 = currentTime;
                int num4 = 1;//player.GetComponent<GameLogic>().trialNumber; // stop or trial number

                string message = num1.ToString("0.00") + "A" + num2.ToString("0.00") + "B";

                //Prep Unity variables for saving

                unityDataVals = new float[] {
                    //Unity runtime
                    currentTime,
                    //Index Info
                    num1,
                    player.GetComponent<GameLogic>().indexPosition.x,
                    player.GetComponent<GameLogic>().indexPosition.y,
                    player.GetComponent<GameLogic>().indexPosition.z,
                    player.GetComponent<GameLogic>().indexOrientation.x,
                    player.GetComponent<GameLogic>().indexOrientation.y,
                    player.GetComponent<GameLogic>().indexOrientation.z,
                    player.GetComponent<GameLogic>().indexScaleValue,
                    //Thumb Info
                    num2,
                    player.GetComponent<GameLogic>().thumbPosition.x,
                    player.GetComponent<GameLogic>().thumbPosition.y,
                    player.GetComponent<GameLogic>().thumbPosition.z,
                    player.GetComponent<GameLogic>().thumbOrientation.x,
                    player.GetComponent<GameLogic>().thumbOrientation.y,
                    player.GetComponent<GameLogic>().thumbOrientation.z,
                    player.GetComponent<GameLogic>().thumbScaleValue,
                    //Sphere Info
                    player.GetComponent<GameLogic>().spherePosition.x,
                    player.GetComponent<GameLogic>().spherePosition.y,
                    player.GetComponent<GameLogic>().spherePosition.z,
                    player.GetComponent<GameLogic>().sphereOrientation.x,
                    player.GetComponent<GameLogic>().sphereOrientation.y,
                    player.GetComponent<GameLogic>().sphereOrientation.z,
                    player.GetComponent<GameLogic>().sphereScaleValue,
                    player.GetComponent<GameLogic>().sphereStiffness,
                    //Start Area Info
                    player.GetComponent<GameLogic>().startingAreaPosition.x,
                    player.GetComponent<GameLogic>().startingAreaPosition.y,
                    player.GetComponent<GameLogic>().startingAreaPosition.z,
                    player.GetComponent<GameLogic>().startingAreaRadius,
                    player.GetComponent<GameLogic>().startingAreaHeight,
                    //Target Area Info
                    player.GetComponent<GameLogic>().targetAreaPosition.x,
                    player.GetComponent<GameLogic>().targetAreaPosition.y,
                    player.GetComponent<GameLogic>().targetAreaPosition.z,
                    player.GetComponent<GameLogic>().targetAreaRadius,
                    player.GetComponent<GameLogic>().targetAreaHeight,
                    //Trial Info
                    player.GetComponent<GameLogic>().successCounter,
                    player.GetComponent<GameLogic>().failCounter,
                    player.GetComponent<GameLogic>().timeOfCurrentSuccess,
                    player.GetComponent<GameLogic>().timeSinceLastSuccess,
                    num4
                    };

                expectedUnityEntries = unityDataVals.Length;

                //Write to Arudino via serial
                writeSerial(message);

                //Read the serial data that came from arduino
                readSerial();

                //Add data to the lists
                arduinoDataList.Add(arduinoDataVals);
                unityDataList.Add(unityDataVals);

                //Debug.Log("Back from Arduino");
                lastTime = currentTime;

                #region
                /*
                if (Input.GetKey("0") || (player.trialNumber == -1))
                {
                    //string endMessage = 0 + "A" + 0 + "B" + 0 + "C" + 0 + "D";
                    string endMessage = 1 + "A" + 2 + "B" + 34 + "C" + 0 + "D"; //0 trial number to quit
                    writeSerial(endMessage);
                    Debug.Log("Sent endMessage: " + endMessage);

                    //quit the editor
                    UnityEditor.EditorApplication.isPlaying = false; 
                    //Application.Quit(); //Quits game when not in editor (after built project)
                }
                */
                #endregion
            }
        }
    }

    public void writeSerial(string message)
    {
        try
        {
            //read stuff
            stream.Write(message);
        }
        catch (IOException e)
        {
            //time out exception
        }
    }

    public void readSerial()
    {
        if (stream.IsOpen)
        {
            try
            {
                //read stuff
                string arduinoMessage = stream.ReadLine();
                arduinoDataVals = arduinoMessage.Split(',');
            }
            catch (System.TimeoutException)
            {
                //time out exception
                //Do Nothing
            }
        }
    }

    //Check every line of data that has been collected from all trials
    //to make sure it contains legitimate data: no NaNs or special characters
    private List<string[]> validateArduinoData(List<string[]> arduinoDataList)
    {
        //Assume data is valid
        bool dataIsValid = true;
       
        #region
        ///** TEST THE DATA VALIDITY **/
        ///*
        //if (arduinoDataVals == null || unityDataVals == null)
        //{
        //    //Data is invalid - do not save or denote the bad data                    
        //    dataIsValid = false;

        //}
        //*/
        ////Check that there are even enough values in the to be saved
        //if (arduinoDataVals.Length != 3 || unityDataVals.Length != expectedUnityEntries)
        //{
        //    //Data is invalid - do not save or denote the bad data                    
        //    dataIsValid = false;
        //}

        ///*
        ////Check that all unity values are real numbers
        //for (int i = 0; i < unityDataVals.Length; i++)
        //{

        //    if (float.IsNaN(unityDataVals[i]))
        //    {
        //        dataIsValid = false;
        //        break;
        //    }
        //}
        //*/
        ////Check if ? are in data 
        //for (int i = 0; i < aruninoDataVals.Length; i++)
        //{
        //    if (arduinoDataVals[i].Contains("?"))
        //    {
        //        dataIsValid = false;
        //        break;
        //    }

        //    /*
        //    float tempFloat = float.Parse(arduinoDataVals[i]);
        //    if (float.IsNaN(tempFloat))
        //    {
        //        dataIsValid = false;
        //    }
        //    */
        //}
        #endregion

        Debug.Log("Arduino Data Validated");
        return arduinoDataList;
    }

    private List<float[]> validateUnityData(List<float[]> unityDataList)
    {
        //Assume data is valid
        bool dataIsValid = true;

        
        #region
        ///** TEST THE DATA VALIDITY **/
        ///*
        //if (ardninoDataVals == null || unityDataVals == null)
        //{
        //    //Data is invalid - do not save or denote the bad data                    
        //    dataIsValid = false;

        //}
        //*/
        ////Check that there are even enough values in the to be saved
        //if (ardninoDataVals.Length != 3 || unityDataVals.Length != expectedUnityEntries)
        //{
        //    //Data is invalid - do not save or denote the bad data                    
        //    dataIsValid = false;
        //}

        ///*
        ////Check that all unity values are real numbers
        //for (int i = 0; i < unityDataVals.Length; i++)
        //{

        //    if (float.IsNaN(unityDataVals[i]))
        //    {
        //        dataIsValid = false;
        //        break;
        //    }
        //}
        //*/
        ////Check if ? are in data 
        //for (int i = 0; i < ardninoDataVals.Length; i++)
        //{
        //    if (ardninoDataVals[i].Contains("?"))
        //    {
        //        dataIsValid = false;
        //        break;
        //    }

        //    /*
        //    float tempFloat = float.Parse(ardninoDataVals[i]);
        //    if (float.IsNaN(tempFloat))
        //    {
        //        dataIsValid = false;
        //    }
        //    */
        //}
        #endregion


        Debug.Log("Unity Data Validated");
        return unityDataList;
    }

    //Send all valid data to CSV Manager to be saved and formatted to .csv
    private void saveDataToCSV(List<string[]> arduinoValidDataList, List<float[]> unityValidDataList)
    {
        //Get row of data and save it to .csv file
        for (int i = 0; i < arduinoValidDataList.Count; i++)
        {
            string[] arduinoDataRow = arduinoValidDataList[i];
            float[] unityDataRow = unityDataList[i];

            //send to be saved
            CSVManager.appendToReport(arduinoDataRow, unityDataRow);
        }

        Debug.Log("Data Saved");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Validating Environment Data");
        List<string[]> arduinoValidDataList = validateArduinoData(arduinoDataList);
        List<float[]> unityValidDataList = validateUnityData(unityDataList);

        Debug.Log("Saving Valid Environment Data to .csv");
        saveDataToCSV(arduinoValidDataList, unityValidDataList);

        //Close Serial Stream
        Debug.Log("GOODBYE");
        stream.Close();
    }
}
