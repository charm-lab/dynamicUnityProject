using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollision : MonoBehaviour
{

    public GameObject BLE;
    CubeBLE cubeBLE;

    public GameObject hapticVisualization;
    public GameObject fingerOne;
    public GameObject fingerOneSide;
    public GameObject fingerTwo;

    private int BLEInt = 0;
    public int intStep = 10;
    public int sends = 0;

    private int handByte = 177;
    private int ribByte = 50;
    private string fingerState = "none";

    private bool hitRib = false;

    public void Start()
    {
        cubeBLE = BLE.GetComponent<CubeBLE>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFinger(bool fingerBool, int fingerNumber)
    {
        if (fingerState == "none")
        {
            if (fingerNumber == 1 && fingerBool == true)
            {
                fingerState = "left";
            }
            else if (fingerNumber == 2 && fingerBool == true)
            {
                fingerState = "right";
            }
        }
        else if (fingerState == "both")
        {
            if (fingerNumber == 1 && fingerBool == false)
            {
                fingerState = "right";
            }
            else if (fingerNumber == 2 && fingerBool == false)
            {
                fingerState = "left";
            }
        }
        else if (fingerState == "left")
        {
            if (fingerNumber == 1 && fingerBool == false)
            {
                fingerState = "none";
            }
            else if (fingerNumber == 2 && fingerBool == true)
            {
                fingerState = "both";
            }
        }
        else if (fingerState == " right")
        {
            if (fingerNumber == 1 && fingerBool == true)
            {
                fingerState = "both";
            }
            else if (fingerNumber == 2 && fingerBool == false)
            {
                fingerState = "none";
            }
        }
    }

    public void SumHapticsV2()
    {
        int finger = 177;

        if (fingerState == "both")
        {
            finger = 177;
        }
        else if (fingerState == "none")
        {
            finger = 0;
        }
        else if (fingerState == "left")
        {
            finger = 0;
        }
        else if (fingerState == "right")
        {
            finger = 255;
        }

        int pressure = 0;// Mathf.Max(fingerOne.GetComponent<FingerCollision>().pressureByte, fingerOneSide.GetComponent<FingerCollision>().pressureByte);
        
        if (fingerOne.GetComponent<FingerCollision>().hitRib == true || fingerOneSide.GetComponent<FingerCollision>().hitRib == true)
        {
            Debug.Log("On Rib");
            pressure = 255;
        }
        else
        {
            Debug.Log("Off Rib");
            pressure = 0;
        }

        Debug.Log(fingerState);
        

        MotorWriteHapticMapping(finger, pressure);
    }

    public void SumHaptics()
    {
        int finger = 0;

        if (fingerOne.GetComponent<FingerCollision>().hitRib == true && fingerOne.GetComponent<FingerCollision>().hitRib == true)
        {
            finger = (fingerOne.GetComponent<FingerCollision>().fingerByte + fingerTwo.GetComponent<FingerCollision>().fingerByte) / 2;
        }
        else if (fingerOne.GetComponent<FingerCollision>().hitRib == true)
        {
            finger = fingerOne.GetComponent<FingerCollision>().fingerByte;
        }
        else if (fingerTwo.GetComponent<FingerCollision>().hitRib == true)
        {
            finger = fingerTwo.GetComponent<FingerCollision>().fingerByte;
        }
        else
        {
            finger = (fingerOne.GetComponent<FingerCollision>().fingerByte + fingerTwo.GetComponent<FingerCollision>().fingerByte) / 2;
        }

        int pressure = Mathf.Max(fingerOne.GetComponent<FingerCollision>().pressureByte, fingerTwo.GetComponent<FingerCollision>().pressureByte);

        MotorWriteHapticMapping(finger, pressure);
    }


    public void MotorWriteHapticMapping(int finger, int pressure)
    {
        byte xByte = (byte)finger;
        byte yByte = (byte)pressure;

        Debug.Log("Write " + (int)xByte + " to X wristworn and " + (int)yByte + " to Y wristworn");
        cubeBLE.WriteMotor(xByte, yByte);
        hapticVisualization.GetComponent<HapticVisualization>().X = xByte;
        hapticVisualization.GetComponent<HapticVisualization>().Y = yByte;
        hapticVisualization.GetComponent<HapticVisualization>().UpdateHapticVisualizer();
    }
}
