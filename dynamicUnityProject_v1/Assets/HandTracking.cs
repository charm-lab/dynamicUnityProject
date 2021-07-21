using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

//#include "ATC3DG.h"

using tagDOUBLE_POSITION = System.Int32;
//using tagDOUBLE_ANGLES = System.Int32;

public class HandTracking : MonoBehaviour
{
    
    [DllImport("ATC3DG64", EntryPoint = "InitializeBIRDSystem")]
    public static extern int InitializeBIRDSystem();

    

    // Start is called before the first frame update
    void Start()
    {
        //print("YAY! trakSTAR .dll is working in HandTracking.cs :D");

        // First initialize the system
        int errorCode = InitializeBIRDSystem();
        if (errorCode != 0)
        {
            Debug.Log("**INITIALIZATION FAILED** Error: " + errorCode.ToString());
        }
        else
        {
            Debug.Log("**INITIALIZATION SUCCESS, continue**");
        }

        print(tagDOUBLE_POSITION.x);


        /*
        // Turn on the transmitter.
        // We turn on the transmitter by selecting the
        // transmitter using its ID
        USHORT id = 0;
        errorCode = SetSystemParameter(SELECT_TRANSMITTER, &id, sizeof(id));
        if (errorCode != BIRD_ERROR_SUCCESS)
        {
            errorHandler(errorCode);
        }
        //////////////////////////////////////////////////////////////////
        //
        // Get a record from sensor #0.
        // The default record type is DOUBLE_POSITION_ANGLES
        //
        USHORT sensorID = 0;
        DOUBLE_POSITION_ANGLES_RECORD record;
        errorCode = GetAsynchronousRecord(sensorID, &record, sizeof(record));
        if (errorCode != BIRD_ERROR_SUCCESS)
        {
            errorHandler(errorCode);
        }



        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
