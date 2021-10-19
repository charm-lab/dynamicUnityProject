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
        //stream.DiscardInBuffer();
        //stream.DiscardOutBuffer();

        Debug.Log("<color=green>Serial Communication Established</color>");

        //Serial Port Read and Write Timeouts
        stream.ReadTimeout = 5;
        stream.WriteTimeout = 10;

        //Enable Game Logic
        player.GetComponent<GameLogic>().enabled = true;
        Debug.Log("<color=blue>Game Logic Enabled</color>");

        writeSerial("0.00A0.00B");
        readSerial();
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
                float num1 = player.GetComponent<GameLogic>().dorsalCommand;
                float num2 = player.GetComponent<GameLogic>().ventralCommand;
                float num3 = currentTime;
                int num4 = player.GetComponent<GameLogic>().trialNumber; // stop or trial number

                string message = num1.ToString("0.00") + "A" + num2.ToString("0.00") + "B";

                float arudinoStartTime = Time.time;

                stream.DiscardInBuffer();
                stream.DiscardOutBuffer();
                //Write to Arudino via serial
                writeSerial(message);

                //Read the serial data that came from arduino
                readSerial();

                //Debug.Log("Back from Arduino");

            }
        }
    }

    public void writeSerial(string message)
    {
        try
        {
            //read stuff
            //Debug.Log("MESSAGE: " + message);
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
                //Debug.Log("arduinoMessage: " + arduinoMessage);
                arduinoDataVals = arduinoMessage.Split(',');

                #region
                /*
                for (int i = 0; i < arduinoDataVals.Length; i++)
                {

                    Debug.Log("arduinoDataVals[" + i.ToString() + "]: " + arduinoDataVals[i]);// + " Length: " + arduinoDataVals[i].Length);
                }
                */
                #endregion
                //lineRead = true;
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
        //Close Serial Stream
        Debug.Log("<color=blue>GOODBYE</color>");
        stream.Close();

        /*Shut down the application*/
        //UnityEditor.EditorApplication.isPlaying = false;

        //Ignored in editor, used in build
        Application.Quit();
    }
}
