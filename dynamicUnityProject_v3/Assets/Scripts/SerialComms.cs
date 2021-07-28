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
                int num4 = 0;//gameLogic.trialNumber; // stop or trial number

                string message = num1.ToString() + "A" + num2.ToString() + "B" + num3.ToString() + "C" + num4.ToString() + "D";
                //Debug.Log(message);

                writeSerial(message);

                /*
                message_returns = false;
                while message_returns is false:
                    message_returns = readSerial();
                Thread.sleep(1);
                */

                readSerial();

                //Debug.Log("Back from Arduino");
                lastTime = currentTime;

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

                if (ardninoDataVals.Length != 7)
                {
                    //Data is invalid - do not save or denote the bad data
                }
                else 
                {
                    //Debug.Log(arduinoMessage);
                    //send to be saved
                    CSVManager.appendToReport(ardninoDataVals);
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
