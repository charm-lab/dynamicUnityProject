using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroHaptics : MonoBehaviour
{
    public GameObject BLE;
    CubeBLE cubeBLE;
    // Start is called before the first frame update
    void Start()
    {
        cubeBLE= BLE.GetComponent<CubeBLE>();

        //First corner -- zeros out X 
        //cubeBLE.WriteMotor(255,255);
        
        //Second Corner -- puts in the proper place to realign
        //cubeBLE.WriteMotor(0,255);
        
        //Third Corner -- zeros out Y
        //cubeBLE.WriteMotor(0,0);
        
        //Center
        //cubeBLE.WriteMotor(127, 127);
        
    }
    
    public void Zero()
    {
        
        Debug.Log("Zero");
        StartCoroutine(ZeroCoroutine());
        

        //First corner -- zeros out X 
    }

    
    IEnumerator ZeroCoroutine()
    {
        
        
        cubeBLE.WriteMotor(255,255);
        yield return new WaitForSeconds(2);

        //Second Corner -- puts in the proper place to realign
        cubeBLE.WriteMotor(0,255);
        yield return new WaitForSeconds(2);

        //Third Corner -- zeros out Y
        cubeBLE.WriteMotor(0,0);
        yield return new WaitForSeconds(2);

        //Center
        cubeBLE.WriteMotor(127, 127);
        yield return new WaitForSeconds(2);
        
        
        
    }
}
