using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerCollision : MonoBehaviour
{
    public int finger = 0;

    //accessed by hand collision
    public int fingerByte = 177;
    public int pressureByte = 50;
    public bool hitRib = false;

    //internal variables
    private int fingerValue = 177;
    private int fingerZero = 177;
    private int pressureZero = 50;

    void Start()
    {

        //set finger haptic value based on finger number
        if (finger == 0)
        {
            Debug.Log("set finger number");
        }
        else if (finger == 1)
        {
            fingerValue = 255;
        }
        else if (finger == 2)
        {
            fingerValue = 0;
        }
        else
        {
            Debug.Log("add more fingers");
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        
        //Debug.Log(collider.name);
        //if a rib is hit, set hit rib to be true (overpower other fingers not hitting ribs)
        if (collider.name == "Rib")
        {
            
            hitRib = true;
            GetComponentInParent<HandCollision>().UpdateFinger(true, finger);
        }

        //there is something colliding with this finger!
        fingerByte = fingerValue;

        //calculate haptic pressure on finger
        float stiff = collider.GetComponent<Stiffness>().getStiffness();
        int pressueValue = Mathf.RoundToInt((127 * stiff) + pressureZero);

        Debug.Log(pressueValue);
        pressureByte = pressueValue;

        
        GetComponentInParent<HandCollision>().SumHapticsV2();
    }

    void OnTriggerExit(Collider collider)
    {
        float stiff = collider.GetComponent<Stiffness>().getStiffness();
        int pressueValue = Mathf.RoundToInt((127 * stiff) + pressureZero);

        pressureByte = pressureByte - pressueValue;

        fingerByte = fingerZero;

        Debug.Log(collider.name);
        if (collider.name == "Rib")
        {
            
            hitRib = false;
            GetComponentInParent<HandCollision>().UpdateFinger(false, finger);
        }

        
        GetComponentInParent<HandCollision>().SumHapticsV2();
    }
}
