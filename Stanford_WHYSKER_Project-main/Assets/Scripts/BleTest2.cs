//Version 4: MRTK 
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BleTest2 : MonoBehaviour
{
    // Change this to match your device.
    string targetDeviceName = "WHYSKER 3";
    string serviceUuid = "{00005000-0000-1000-8000-00805f9b34fb}";
    string[] characteristicUuids = {
         "{00005001-0000-1000-8000-00805f9b34fb}",      // CUUID 1
         "{00005002-0000-1000-8000-00805f9b34fb}",       // CUUID 2
         "{00005003-0000-1000-8000-00805f9b34fb}",
         "{00005004-0000-1000-8000-00805f9b34fb}"
    };

    BLE ble;
    BLE.BLEScan scan;
    bool isScanning = false, isConnected = false;
    string deviceId = null;  
    Dictionary<string, string> discoveredDevices = new Dictionary<string, string>();
    int devicesCount = 0;

    // BLE Threads 
    Thread scanningThread, connectionThread, readingThread, writeThread;

    // GUI elements
    public Dropdown DropdownDiscoveredDevices;
    public Text TextIsScanning, TextTargetDeviceConnection, TextTargetDeviceData;
    public Button ButtonEstablishConnection, ButtonStartScan, ButtonSendAck;
    int remoteAngle, lastRemoteAngle;
    
    //Position BUtons
//    public Button Button00, Button01, Button02, Button10, Button11, Button12, Button20, 
//    Button21, Button22;
    

    // Start is called before the first frame update
    void Start()
    {
        ble = new BLE();
        ButtonEstablishConnection.enabled = false;
        TextTargetDeviceConnection.text = targetDeviceName + " not found.";
        readingThread = new Thread(ReadBleData);
    }

    // Update is called once per frame
    void Update()
    {  
        if (isScanning)
        {
            if (ButtonStartScan.enabled)
                ButtonStartScan.enabled = false;

            if (discoveredDevices.Count > devicesCount)
            {
                UpdateGuiText("scan");
                devicesCount = discoveredDevices.Count;
            }                
        } else
        {
            /* Restart scan in same play session not supported yet.
            if (!ButtonStartScan.enabled)
                ButtonStartScan.enabled = true;
            */
            if (TextIsScanning.text != "Not scanning.")
            {
                TextIsScanning.color = Color.white;
                TextIsScanning.text = "Not scanning.";
            }
        }

        // The target device was found.
        if (deviceId != null && deviceId != "-1")
        {
            // Target device is connected and GUI knows.
            if (ble.isConnected && isConnected)
            {
                UpdateGuiText("writeData");
            }
            // Target device is connected, but GUI hasn't updated yet.
            else if (ble.isConnected && !isConnected)
            {
                UpdateGuiText("connected");
                isConnected = true;
            // Device was found, but not connected yet. 
            } else if (!ButtonEstablishConnection.enabled && !isConnected)
            {
                ButtonEstablishConnection.enabled = true;
                TextTargetDeviceConnection.text = "Found target device:\n" + targetDeviceName;
            } 
        } 
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
        try
        {
            scan.Cancel();
            ble.Close();
            scanningThread.Abort();
            connectionThread.Abort();
            writeThread.Abort();
        } catch(NullReferenceException e)
        {
            Debug.Log("Thread or object never initialized.\n" + e);
        }        
    }

    public void StartScanHandler()
    {
        devicesCount = 0;
        isScanning = true;
        discoveredDevices.Clear();
        scanningThread = new Thread(ScanBleDevices);
        scanningThread.Start();
        TextIsScanning.color = new Color(244, 180, 26);
        TextIsScanning.text = "Scanning...";
        DropdownDiscoveredDevices.options.Clear();
    }
    
    public void SendAckHandler(){
        if (ble.isConnected){
        
        // Send Ack UUID
            Debug.Log("SendAckHandler Called");
            writeThread = new Thread(SendAck);
            writeThread.Start();
         
        }
    
    
    }
    
    public void SendPosHandler(byte pos){
        if (ble.isConnected){
        
            Debug.Log("SendPosHandler Called");
            writeThread = new Thread(()=> SendPos(pos));
            writeThread.Start();
        }
    
    }
    public void SendAck()
    {
        string ackUuid = characteristicUuids[0];
        byte[] data = {1};
        
        bool x = BLE.WritePackage(deviceId, serviceUuid, ackUuid, data);
        Debug.Log("This is where we send the package: ");
        Debug.Log(x);
    }

    public void SendPos(byte pos)
    {
        //TODO: Change this to the proper characteristics 
        string xUUID = characteristicUuids[1];
        string yUUID = characteristicUuids[2];
        
//        byte[] posData = BitConverter.GetBytes(pos)
        byte[] posData = {pos};
//        Array.Reverse(posData);
//        byte[] yData = BitConverter.GetBytes(y);
//        Array.Reverse(yData);
        
        bool err = BLE.WritePackage(deviceId, serviceUuid, xUUID, posData);
        
        Debug.Log("Sending the POS ");
        Debug.Log(err);
    
    }
    public void ResetHandler()
    {
        TextTargetDeviceData.text = "";
        TextTargetDeviceConnection.text = targetDeviceName + " not found.";
        // Reset previous discovered devices
        discoveredDevices.Clear();
        DropdownDiscoveredDevices.options.Clear();
        deviceId = null;
        CleanUp();
    }

    private void ReadBleData(object obj)
    {
        byte[] packageReceived = BLE.ReadBytes();
        // Convert little Endian.
        // In this example we're interested about an angle
        // value on the first field of our package.
        remoteAngle = packageReceived[0];
        Debug.Log("ReadBleData");
        Debug.Log("Angle: " + remoteAngle);
        //Thread.Sleep(100);
    }

    void UpdateGuiText(string action)
    {
        switch(action) {
            case "scan":
                
                foreach (KeyValuePair<string, string> entry in discoveredDevices)
                {
            
                    
                    DropdownDiscoveredDevices.options.Add (new Dropdown.OptionData() {text=entry.Value});  
                    
                    Debug.Log("Added device: " + entry.Key);

                    
                    
                }
                
                DropdownDiscoveredDevices.value = 1;
                DropdownDiscoveredDevices.value = 0;
                break;
                
            case "connected":
                ButtonEstablishConnection.enabled = false;
                TextTargetDeviceConnection.text = "Connected to target device:\n" + targetDeviceName;
                
                break;
            case "writeData":
                if (!readingThread.IsAlive)
                {
                    readingThread = new Thread(ReadBleData);
                    readingThread.Start();
                }
                if (remoteAngle != lastRemoteAngle)
                {
                    TextTargetDeviceData.text = "Remote angle: " + remoteAngle;
                    lastRemoteAngle = remoteAngle;
                }
                break;
        }
    }

    void ScanBleDevices()
    {
        scan = BLE.ScanDevices();
        Debug.Log("BLE.ScanDevices() started.");
        scan.Found = (_deviceId, deviceName) =>
        {
        
            Debug.Log("found device with name: " + deviceName);
        
            if (deviceName.Contains("WHYSKER"))
            {
                if (deviceId == null && deviceName == targetDeviceName)
                    deviceId = _deviceId;
                    
                
                discoveredDevices.Add(_deviceId, deviceName);
                
                
            }
            
            
            
        };

        scan.Finished = () =>
        {
            isScanning = false;
            Debug.Log("scan finished");
            if (deviceId == null)
                deviceId = "-1";
        };
        while (deviceId == null)
            Thread.Sleep(500);
        scan.Cancel();
        scanningThread = null;
        isScanning = false;

        if (deviceId == "-1")
        {
            Debug.Log("no device found!");
            return;
        }
    }

    // Start establish BLE connection with
    // target device in dedicated thread.
    public void StartConHandler()
    {
        connectionThread = new Thread(ConnectBleDevice);
        connectionThread.Start();
    }

    void ConnectBleDevice()
    {
        if (deviceId != null)
        {
            try
            {
                ble.Connect(deviceId,
                serviceUuid,
                characteristicUuids);
            } catch(Exception e)
            {
                Debug.Log("Could not establish connection to device with ID " + deviceId + "\n" + e);
            }
        }
        if (ble.isConnected)
            Debug.Log("Connected to: " + targetDeviceName);
             if (!readingThread.IsAlive)
                {
                    Debug.Log("start reading thread");
                    readingThread = new Thread(ReadBleData);
                    readingThread.Start();
                }
                if (remoteAngle != lastRemoteAngle)
                {
                    TextTargetDeviceData.text = "Remote angle: " + remoteAngle;
                    lastRemoteAngle = remoteAngle;
                }
    }

    ulong ConvertLittleEndian(byte[] array)
    {
        int pos = 0;
        ulong result = 0;
        foreach (byte by in array)
        {
            result |= ((ulong)by) << pos;
            pos += 8;
        }
        return result;
    }
}
