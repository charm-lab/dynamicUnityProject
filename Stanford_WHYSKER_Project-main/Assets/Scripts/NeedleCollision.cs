using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleCollision : MonoBehaviour
{
    public GameObject BLE;
    CubeBLE cubeBLE;

    public GameObject hapticVisualization;

    private int BLEInt = 0;
    public int intStep = 10;
    public int sends = 0;

    private bool hitRib = false;

    public void Start()
    {
        cubeBLE= BLE.GetComponent<CubeBLE>();
    }

    void Update()
    {
        //BLEInt++;

        //while(sends <= 10)
        //{
        //    BLEInt++;
            
        //    if (BLEInt == 1 * intStep)
        //    {
        //        cubeBLE.WriteMotor(0, 0);
        //        Debug.Log(Time.time);
        //        sends++;
        //    }
        //    else if (BLEInt == 2 * intStep)
        //    {
        //        cubeBLE.WriteMotor(255, 255);
        //        Debug.Log(Time.time);
        //        sends++;
        //    }
        //    else if (BLEInt == 3 * intStep)
        //    {
        //        cubeBLE.WriteMotor(255, 0);
        //        Debug.Log(Time.time);
        //        sends++;
        //    }
        //    else if (BLEInt == 4 * intStep)
        //    {
        //        cubeBLE.WriteMotor(0, 255);
        //        BLEInt = 0;
        //        Debug.Log(Time.time);
        //        sends++;
        //    }
            
        //}

        
    }

    void OnTriggerEnter(Collider collider)
    {
        int force = 0;
        int AOI = 0;

        Debug.Log(collider.name);

        if (!hitRib)
        {
            if (collider.name == "Rib")
            {
                float stiff = collider.GetComponent<Stiffness>().getStiffness();
                int y = Mathf.RoundToInt(127 * (stiff));
                force = y;
                byte yByte = (byte)y;
                cubeBLE.WriteMotor(0,yByte);
                this.GetComponent<Renderer>().material.color = Color.red;
                hitRib = true;
            }

            else if (collider.name == "Body")
            {
                //            cubeBLE.WriteMotor(127,127);
                float stiff = collider.GetComponent<Stiffness>().getStiffness();
                int y = Mathf.RoundToInt(127 * (stiff));
                force = y;
                byte yByte = (byte)y;
                //cubeBLE.WriteMotor(0,yByte);
            }

            else if (collider.name == "AngleInsertionRibDown")
            {
                //            cubeBLE.WriteMotor(127,127);
                float stiff = collider.GetComponent<Stiffness>().getStiffness();
                int x = Mathf.RoundToInt(255 * stiff);
                AOI = x;
                byte xByte = (byte)x;
                this.GetComponent<Renderer>().material.color = Color.yellow;
                //cubeBLE.WriteMotor(0,xByte);
            }

            MotorWriteHapticMapping(force, AOI);
        }
        
    }

    private void MotorWriteHapticMapping(int normalForce, int AOI)
    {
    
        Debug.Log("MotorWriteHaptic");
        byte xByte = (byte)0;
        byte yByte = (byte)0;


        if (TrainerController.instance.XHapticsType == 1)
        {
            xByte = (byte)normalForce;
        }
        else if (TrainerController.instance.XHapticsType == 2)
        {
            xByte = (byte)AOI;
        }

        if (TrainerController.instance.YHapticsType == 1)
        {
            yByte = (byte)normalForce;
        }
        else if (TrainerController.instance.YHapticsType == 2)
        {
            yByte = (byte)AOI;
        }

        Debug.Log("Write " + (int)xByte + " to X wristworn and " + (int)yByte + " to Y wristworn");
        cubeBLE.WriteMotor(xByte, yByte);
        hapticVisualization.GetComponent<HapticVisualization>().X = xByte;
        hapticVisualization.GetComponent<HapticVisualization>().Y = yByte;
        hapticVisualization.GetComponent<HapticVisualization>().UpdateHapticVisualizer();
    }
    
    void OnTriggerExit(Collider collider)
    {
        int force = 0;
        int AOI = 0;

        //Debug.Log(collider.name);
        if (collider.name == "Rib")
        {
            float stiff = .6f;
            int y = Mathf.RoundToInt(127 * (1 - stiff));
            force = y;
            byte yByte = (byte)y;
            this.GetComponent<Renderer>().material.color = Color.blue;
            hitRib = false;
        }

        else if (collider.name == "Body")
        {
            force = 0;
            byte yByte = (byte)force;
        }

        else if (collider.name == "AngleInsertionRibDown")
        {
            int x = 0;
            AOI = x;
            byte xByte = (byte)x;
            this.GetComponent<Renderer>().material.color = Color.blue;
        }

        MotorWriteHapticMapping(force, AOI);
    }

    
    //public void Start()
    //{
    //    cubeBLE= BLE.GetComponent<CubeBLE>();
    //}
    
    //void OnTriggerEnter(Collider other)
    //{
    
       
    //    Debug.Log(other.name + "  " + this.name);
    //    GameObject parent = this.transform.parent.gameObject;
    //    parent.GetComponent<Renderer>().material.color = Color.green;
        
    //    cubeBLE.WriteMotor(125,125);
    //}
    
    //void OnTriggerExit()
    //{
    
    //    GameObject parent = this.transform.parent.gameObject;
    //    parent.GetComponent<Renderer>().material.color = Color.red;
    //    cubeBLE.WriteMotor(0,0);
    
    //}
    
}