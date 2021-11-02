using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleCollisionv2 : MonoBehaviour
{
    public GameObject BLE;
    CubeBLE cubeBLE;
    
    public void Start()
    {
        cubeBLE= BLE.GetComponent<CubeBLE>();
    }
    
    void OnTriggerEnter(Collider collider)
    {
//        Debug.Log(collider.name);
        if (collider.name == "Ribs")
        {
            float stiff = collider.GetComponent<Stiffness>().getStiffness();
            int y = Mathf.RoundToInt(127*(1-stiff));
            byte yByte = (byte)y;
            cubeBLE.WriteMotor(0,yByte);
            this.GetComponent<Renderer>().material.color = Color.red;

        }
        
        else if (collider.name == "Body")
        {
//            cubeBLE.WriteMotor(127,127);
            float stiff = collider.GetComponent<Stiffness>().getStiffness();
            int y = Mathf.RoundToInt(127*(1-stiff));
            byte yByte = (byte)y;
            cubeBLE.WriteMotor(0,yByte);
        }
        
        else if (collider.name == "AngleInsertionRibDown")
        {
//            cubeBLE.WriteMotor(127,127);

            int x = Mathf.RoundToInt(255*collider.GetComponent<Stiffness>().getStiffness());
            byte xByte = (byte)x;
            cubeBLE.WriteMotor(0,xByte);
        }
    }

    public void SetWristWornX()
    {
        Debug.Log("Set wristworn X to ");
    }

    public void SetWristWornY()
    {
        Debug.Log("Set wristworn Y to ");
    }
    
    void OnTriggerExit()
    {
    
//        GameObject parent = this.transform.parent.gameObject;
        cubeBLE.WriteMotor(127,127);
        this.GetComponent<Renderer>().material.color = Color.blue;
    }
    
}