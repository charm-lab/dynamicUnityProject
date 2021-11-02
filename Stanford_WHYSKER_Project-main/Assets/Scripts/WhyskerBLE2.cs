using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;

/*
File Name: WhyskerBLE2.CS
Author: Triton Systems 
Purpose: Stores known constants across scenes
Last Modification: 6/29/2021 -- Added Documentation Header
*/


public class WhyskerBLE2{
    
    
    public static string targetDeviceName = "BMD340";
    
    public static string deviceId;
    //Service uuids
    public static string ledServiceUuid =  "{e54b0001-67f5-479e-8711-b3b99198ce6c}";
    public static string motorServiceUuid = "{e64b0003-67f5-479e-8711-b3b99198ce6c}";
    
    //characteristic uuids
    public static string ledCharUuid = "{e54b0002-67f5-479e-8711-b3b99198ce6c}";
    public static string motorCharUuid = "{e64b0004-67f5-479e-8711-b3b99198ce6c}";
    
}