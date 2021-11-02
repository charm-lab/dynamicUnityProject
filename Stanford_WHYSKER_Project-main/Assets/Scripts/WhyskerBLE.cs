using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;


/*
File Name: WhyskerBLE.CS
Author: Triton Systems 
Purpose: Stores known constants and returns them. No Longer in Use
Last Modification: 6/29/2021 -- Added Documentation Header
*/

public class WhyskerBLE{
    
    
    
    string targetDeviceName = "WHYSKER";
    
    //Service uuids
    string ledServiceUuid = "{e54b0001-67f5-479e-8711-b3b99198ce6c}";
    string motorServiceUuid = "{e64b0003-67f5-479e-8711-b3b99198ce6c}";
    
    //characteristic uuids
    string ledCharUuid = "{e54b0002-67f5-479e-8711-b3b99198ce6c}";
    string motorCharUuid = "{e64b0004-67f5-479e-8711-b3b99198ce6c}";
    
    
    
    public string getName(){
        // access function for the target device name
        return targetDeviceName;
    }
    
    public string getService(string name){
        // access function for the ServiceUuid
        string lowername = name.ToLower();
        Debug.Log("Get Service");
        if (lowername == "led"){
            return ledServiceUuid;
        }
        
        else{
            return motorServiceUuid;
        }
    }
    
    public string getChar(string name){
        Debug.Log("Get Char");
        // access function for the ServiceUuid
        string lowername = name.ToLower();
        
        if (lowername == "led"){
            return ledCharUuid;
        }
        
        else{
            return motorCharUuid;
        }
        
    }
}