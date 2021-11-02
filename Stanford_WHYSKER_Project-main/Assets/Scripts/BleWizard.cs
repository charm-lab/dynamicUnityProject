//Version 3: MRTK 
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;


#if NETFX_CORE
using System;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth;
#endif


public class BleWizard: MonoBehaviour
{
    // Change this to match your device.
    string targetDeviceName = "WHYSKER 3";
    
    string[] whyskerList = {"WHYSKER 1", "WHYSKER 2", "WHYSKER 3", "WHYSKER 4"};
    
    Dictionary<string, string> discoveredDevices = new Dictionary<string, string>();
//    List<string> discoveredEntries = new List<string>();
    int devicesCount = 0;

    // BLE Threads 
    Thread scanningThread, connectionThread, readingThread, writeThread;

    // GUI elements
    public Dropdown DropdownDiscoveredDevices;
    public TMP_Text TextIsScanning, TextTargetDeviceConnection, TextTargetDeviceData;
    public Interactable ButtonEstablishConnection, ButtonStartScan;
    int remoteAngle, lastRemoteAngle;
    
    bool isScanning = false;
    
    //Position BUtons
//    public Button Button00, Button01, Button02, Button10, Button11, Button12, Button20, 
//    Button21, Button22;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
//        if (isScanning)
//        {
//            if (ButtonStartScan.enabled)
//                ButtonStartScan.enabled = false;
//
//            if (discoveredDevices.Count > devicesCount)
//            {
//                UpdateGuiText("scan");
//                devicesCount = discoveredDevices.Count;
//            }                
//        } else
//        {
//            /* Restart scan in same play session not supported yet.
//            if (!ButtonStartScan.enabled)
//                ButtonStartScan.enabled = true;
//            */
//            if (TextIsScanning.text != "Not scanning.")
//            {
//                TextIsScanning.color = Color.white;
//                TextIsScanning.text = "Not scanning.";
//            }
//        }

    }

    private void OnDestroy()
    {
        CleanUp();
    }

    private void OnApplicationQuit()
    {
        CleanUp();
    }

    // Prevent threading issues and free BLE stack.
    // Can cause Unity to freeze and lead
    // to errors when omitted.
    private void CleanUp()
    {
            
    }

    public void StartScanHandler()
    {
        devicesCount = 0;
        isScanning = true;
        discoveredDevices.Clear();
        TextIsScanning.text = "Scanning...";
        DropdownDiscoveredDevices.options.Clear();
                
        //wait 2 seconds
        Invoke("ScanBleDevices",2);
        

    }
    
    
    public void ResetHandler()
    {
        TextTargetDeviceData.text = "";
        TextTargetDeviceConnection.text = targetDeviceName + " not found.";
        // Reset previous discovered devices
        discoveredDevices.Clear();
        DropdownDiscoveredDevices.options.Clear();
        CleanUp();
    }



    void UpdateGuiText(string action)
    {
        switch(action) {
            case "scan":
                
//                foreach (string entry in discoveredEntries)
//                {
//            
//                    
//                    DropdownDiscoveredDevices.options.Add (new Dropdown.OptionData() {text=entry});  
//                    
//                    Debug.Log("Added device: " + entry);
//
//                    
//                    
//                }
//                
                DropdownDiscoveredDevices.value = 1;
                DropdownDiscoveredDevices.value = 0;
                break;
                
            case "connected":
                ButtonEstablishConnection.enabled = false;
                TextTargetDeviceConnection.text = "Connected to target device:\n" + targetDeviceName;
                
                break;
            case "writeData":
                
                break;
        }
    }

    void SetText(string text)
    {
        TextIsScanning.text = text;
    
    }
    void ScanBleDevices()
    {
        
            
        
//            Debug.Log("found device with name: " + deviceName);
//            System.Diagnostics.Debug.Write("found device with name: " + deviceName);
//           
            for (int i = 0; i < whyskerList.Length; i++){
            
                string deviceName = whyskerList[i];
                
                string devId = i.ToString();
                discoveredDevices.Add(devId, deviceName);
    //                discoveredEntries.Add(deviceName);
                DropdownDiscoveredDevices.options.Add(new Dropdown.OptionData() {text=deviceName}); 


            }

            
            
        
        TextIsScanning.color = Color.white;
        TextIsScanning.text = "Not Scanning";
    }

    // Start establish BLE connection with
    // target device in dedicated thread.
    public void StartConHandler()
    {
        TextTargetDeviceConnection.text = "Connecting...";
        Invoke("ConnectBleDevice", 2);
        
    }

    void ConnectBleDevice()
    {
        TextTargetDeviceConnection.text = "Connected";
        TextTargetDeviceConnection.color = Color.white;
    }
    
    
    
}
