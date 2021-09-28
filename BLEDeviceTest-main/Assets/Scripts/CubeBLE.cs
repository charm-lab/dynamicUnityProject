using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
//using Microsoft.MixedReality.Toolkit.UI;


public class CubeBLE : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.X))
        //{
            //WriteLEDOn();
        //}
        

        
        
    }
    
    
    public void WriteLEDOn()
    {
        Debug.Log("Write LED On Start");
        
        try{
//        int writeInt = int.Parse(writeInput.text);//for integer 
//        Debug.Log(writeInt.GetType());
//        Debug.Log(writeInt);
        byte[] payload = Encoding.ASCII.GetBytes("1");
//        byte[] payload = BitConverter.GetBytes(writeInt);
//        Debug.Log(payload);
//        Array.Reverse(payload);
        BleApi.BLEData data = new BleApi.BLEData();
        data.buf = new byte[512];
        data.size = (short)payload.Length;
        data.deviceId = WhyskerBLE2.deviceId;
        data.serviceUuid = WhyskerBLE2.ledServiceUuid;
        data.characteristicUuid = WhyskerBLE2.ledCharUuid;
        
      
        data.buf[0] = 1;
        
//        #if NETFX_CORE
//        BleApi.sendMessage(data.characteristicUuid);
        bool res = BleApi.SendData(in data, false);
//        #endif 
        }
        
        catch (Exception e){
            Debug.Log("Failed");
            Debug.Log(e);
        }

    }
    
    public void WriteLEDOff()
    {
        
        try{
//        int writeInt = int.Parse(writeInput.text);//for integer 
//        Debug.Log(writeInt.GetType());
//        Debug.Log(writeInt);
        byte[] payload = Encoding.ASCII.GetBytes("1");
//        byte[] payload = BitConverter.GetBytes(writeInt);
//        Debug.Log(payload);
//        Array.Reverse(payload);
        BleApi.BLEData data = new BleApi.BLEData();
        data.buf = new byte[512];
        data.size = (short)payload.Length;
        data.deviceId = WhyskerBLE2.deviceId;
        data.serviceUuid = WhyskerBLE2.ledServiceUuid;
        data.characteristicUuid = WhyskerBLE2.ledCharUuid;
        
        Debug.Log(data.size);


        data.buf[0] = 0;
        
        bool res = BleApi.SendData(in data, false);
//        #endif 
        }
        
        catch (Exception e){
            Debug.Log("Failed");
            Debug.Log(e);
        }

    }
    
    public void WriteMotor(byte x, byte y){
        // get X and Y positions to write 
        
        Byte xByte = x;
        Byte yByte = y;
        

        byte[] payload = Encoding.ASCII.GetBytes("16");

        BleApi.BLEData data = new BleApi.BLEData();
        data.buf = new byte[512];
        data.size = (short)payload.Length;
        data.deviceId = WhyskerBLE2.deviceId;
        data.serviceUuid = WhyskerBLE2.motorServiceUuid;
        data.characteristicUuid = WhyskerBLE2.motorCharUuid;
        data.buf[0] = xByte;
        data.buf[1] = yByte;
        
        bool res = BleApi.SendData(in data, false);

        //Debug.Log(res);
        
        //Debug.Log(xByte);
//    
    }
}
