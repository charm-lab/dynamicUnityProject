using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;

public class SerialComms : MonoBehaviour
{
    GameObject player;

    //Set the port and the baud rate to 9600
    public string portName = "COM3";
    public int baudRate = 9600;
    SerialPort stream;

    private float lastTime = 0.0f;
    private float currentTime = 0.0f;

    public static string[] ardninoDataVals;
    public static float[] unityDataVals;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Serial Comms");

        //Start the hand behavior/game logic as disabled until serial comms is up
        player = GameObject.Find("Player");

        //Define and open serial port       
        stream = new SerialPort(portName, 9600);
        stream.Open();

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
        if (stream.IsOpen)
        {
            currentTime = Time.time;

            if (currentTime - lastTime > 0.005)
            {
                float num1 = player.GetComponent<GameLogic>().indexPositionCommand;
                float num2 = player.GetComponent<GameLogic>().thumbPositionCommand;
                float num3 = currentTime;
                int num4 = 1;//gameLogic.trialNumber; // stop or trial number

                string message = num1.ToString() + "A" + num2.ToString() + "B";// + num3.ToString() + "C" + num4.ToString() + "D";
                                                                               //Debug.Log(message);

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
                    num4
                    };

                //Write to Arudino via serial
                writeSerial(message);

                #region
                /*
                message_returns = false;
                while message_returns is false:
                    message_returns = readSerial();
                Thread.sleep(1);
                */
                #endregion

                //Read the serial data that cam from arduino
                readSerial();

                //Debug.Log("Back from Arduino");
                lastTime = currentTime;

                #region
                /*
                if (Input.GetKey("0") || (player.trialNumber == 0))
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
                ardninoDataVals = arduinoMessage.Split(',');

                //Debug.Log(arduinoMessage);
                //Debug.Log(ardninoDataVals.Length);

                
                if (ardninoDataVals.Length != 3)
                {
                    //Data is invalid - do not save or denote the bad data
                }
                
                else 
                {
                    //Debug.Log(arduinoMessage);
                    //send to be saved
                    CSVManager.appendToReport(ardninoDataVals, unityDataVals);
                }

            }
            catch (System.TimeoutException)
            {
                //time out exception
                //Do Nothing
            }
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("GOODBYE");
        //Close Serial Stream
        stream.Close();
    }
}
