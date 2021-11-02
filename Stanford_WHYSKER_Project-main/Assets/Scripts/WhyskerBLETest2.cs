using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;


/*
File Name: WhyskerBLETest2.CS
Author: Triton Systems -- modified from Adabru 
Purpose: Creates and maintans connection with WHYSKER.  Uses WhyskerBle2. NAME SHOULD BE CHANGED
Last Modification: 6/29/2021 -- Added Documentation Header
*/



// This Script is for testing UWP + BLE On PC comms
public class WhyskerBLETest2 : MonoBehaviour
{
    public bool isScanningDevices = false;

    public TMP_Text deviceScanButtonText;
    public TMP_Text deviceScanStatusText;
    public Dropdown deviceScanResultProto;
    

    public Interactable writeLEDOn;
    public Interactable writeLEDOff;
    public Interactable writeMotorPos;
    public TMP_InputField xInput;
    public TMP_InputField yInput;
//    public Text errorText;

    public Interactable startCon;

    Transform scanResultRoot;
    public string selectedDeviceId;
    public string selectedServiceId;
    Dictionary<string, string> characteristicNames = new Dictionary<string, string>();
    public string selectedCharacteristicId;
    Dictionary<string, Dictionary<string, string>> devices = new Dictionary<string, Dictionary<string, string>>();
    string lastError;
    List<string> discoveredIDs = new List<string>();
    WhyskerBLE2 whyskerBle = new WhyskerBLE2();

    // Start is called before the first frame update
    void Start()
    {
//        scanResultRoot = deviceScanResultProto.transform.parent;
//        deviceScanResultProto.transform.SetParent(null);
        #if ENABLE_WINMD_SUPPORT
            Debug.Log("Windows Runtime Support enabled");
            // Put calls to your custom .winmd API here
          #endif
    }

    // Update is called once per frame
    void Update()
    {
        BleApi.ScanStatus status;
        if (isScanningDevices)
        {
            BleApi.DeviceUpdate res = new BleApi.DeviceUpdate();
            do
            {
                status = BleApi.PollDevice(ref res, false);
                if (status == BleApi.ScanStatus.AVAILABLE)
                {
                    if (!devices.ContainsKey(res.id))
                        devices[res.id] = new Dictionary<string, string>() {
                            { "name", "" },
                            { "isConnectable", "False" }
                        };
                    if (res.nameUpdated)
                        devices[res.id]["name"] = res.name;
                    if (res.isConnectableUpdated)
                        devices[res.id]["isConnectable"] = res.isConnectable.ToString();
                    // consider only devices which have a name and which are connectable
                    if (devices[res.id]["name"] != "" && devices[res.id]["isConnectable"] == "True" && (devices[res.id]["name"].Contains("WHYSKER") | devices[res.id]["name"].Contains("BMD")))
                    {
                        // add new device to list
//                        Debug.Log(devices[res.id]["name"]);
                        deviceScanResultProto.options.Add(new Dropdown.OptionData() {text = devices[res.id]["name"]});
                        deviceScanResultProto.value = 1;
                        deviceScanResultProto.value = 0;
                        discoveredIDs.Add(res.id);
                    }
                }
                else if (status == BleApi.ScanStatus.FINISHED)
                {
                    isScanningDevices = false;
                    deviceScanButtonText.text = "Scan devices";
                    deviceScanStatusText.text = "finished";
                }
            } while (status == BleApi.ScanStatus.AVAILABLE);
        }
        
//            do
//            {
//                status = BleApi.PollService(out res, false);
//                if (status == BleApi.ScanStatus.AVAILABLE)
//                {
//                    serviceDropdown.AddOptions(new List<string> { res.uuid });
//                    // first option gets selected
//                    if (serviceDropdown.options.Count == 1)
//                        SelectService(serviceDropdown.gameObject);
//                }
//                else if (status == BleApi.ScanStatus.FINISHED)
//                {
//                    isScanningServices = false;
//                    serviceScanButton.interactable = true;
//                    serviceScanStatusText.text = "finished";
//                }
//            } while (status == BleApi.ScanStatus.AVAILABLE);
//        }
//        if (isScanningCharacteristics)
//        {
//            BleApi.Characteristic res = new BleApi.Characteristic();
//            do
//            {
//                status = BleApi.PollCharacteristic(out res, false);
//                if (status == BleApi.ScanStatus.AVAILABLE)
//                {
//                    string name = res.userDescription != "no description available" ? res.userDescription : res.uuid;
//                    characteristicNames[name] = res.uuid;
//                    characteristicDropdown.AddOptions(new List<string> { name });
//                    // first option gets selected
//                    if (characteristicDropdown.options.Count == 1)
//                        SelectCharacteristic(characteristicDropdown.gameObject);
//                }
//                else if (status == BleApi.ScanStatus.FINISHED)
//                {
//                    isScanningCharacteristics = false;
//                    characteristicScanButton.interactable = true;
//                    characteristicScanStatusText.text = "finished";
//                }
//            } while (status == BleApi.ScanStatus.AVAILABLE);
//        }
//        if (isSubscribed)
//        {
//            BleApi.BLEData res = new BleApi.BLEData();
//            while (BleApi.PollData(out res, false))
//            {
//                subcribeText.text = BitConverter.ToString(res.buf, 0, res.size);
//                // subcribeText.text = Encoding.ASCII.GetString(res.buf, 0, res.size);
//            }
//        }
        {
            // log potential errors
            BleApi.ErrorMessage res = new BleApi.ErrorMessage();
            BleApi.GetError(out res);
            if (lastError != res.msg)
            {
                Debug.LogError(res.msg);
//                errorText.text = res.msg;
                lastError = res.msg;
            }
        }
    }

    private void OnApplicationQuit()
    {
        BleApi.Quit();
    }

    public void StartStopDeviceScan()
    {
        if (!isScanningDevices)
        {
            // start new scan
            deviceScanResultProto.options.Clear();
            BleApi.StartDeviceScan();
            isScanningDevices = true;
            deviceScanButtonText.text = "Stop scan";
            deviceScanStatusText.text = "scanning";
        }
        else
        {
            // stop scan
            isScanningDevices = false;
            BleApi.StopDeviceScan();
            deviceScanButtonText.text = "Start scan";
            deviceScanStatusText.text = "stopped";
        }
    }

    public void StartConnection()
    {
        // PLACE DROPDOWN CODE
        Debug.Log("StartCon");
        int index = deviceScanResultProto.value;
        selectedDeviceId = discoveredIDs[index];
        WhyskerBLE2.deviceId = selectedDeviceId;
        Debug.Log(selectedDeviceId);
        Debug.Log(devices[selectedDeviceId]["name"]);
        StartServiceScan();
//        serviceScanButton.interactable = true;
    }

    public void StartServiceScan()
    {
            // start new scan
//            serviceDropdown.ClearOptions();
        BleApi.ScanServices(selectedDeviceId);
        BleApi.Service res = new BleApi.Service();
        Debug.Log(res.uuid);
//            writeLEDOn.interactable = true;
//            writeLEDOff.interactable = true;
//            writeMotorPos.interactable = true;

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
        data.deviceId = selectedDeviceId;
        data.serviceUuid = WhyskerBLE2.ledServiceUuid;
        data.characteristicUuid = WhyskerBLE2.ledCharUuid;
        
        Debug.Log("Led Message Created");
        Debug.Log(data.size);


        data.buf[0] = 1;
        
//        #if NETFX_CORE
        Debug.Log("UWP Supported");
//        BleApi.sendMessage(data.characteristicUuid);
        bool res = BleApi.SendData(in data, false);
//        #endif 
        Debug.Log("Message Written");
        }
        
        catch (Exception e){
            Debug.Log("Failed");
            Debug.Log(e);
        }

    }
    public void WriteLEDOff()
    {
        Debug.Log("Write LED Off Start");
        byte[] payload = Encoding.ASCII.GetBytes("1");

        BleApi.BLEData data = new BleApi.BLEData();
        data.buf = new byte[512];
        data.size = (short)payload.Length;
        data.deviceId = selectedDeviceId;
        data.serviceUuid = WhyskerBLE2.ledServiceUuid;
        data.characteristicUuid = WhyskerBLE2.ledCharUuid;
        Debug.Log("Led Message Created");

        data.buf[0] = 0;
        bool res = BleApi.SendData(in data, false);

    }
    
    public void WriteMotor(){
        // get X and Y positions to write 
        
        Debug.Log("Write Motor Started");
        Byte xByte;
        Byte.TryParse(xInput.text, out xByte);
        Byte yByte;
        Byte.TryParse(yInput.text, out yByte);
        
        Debug.Log("X and Y parsed");
        Debug.Log(xByte);
        Debug.Log(yByte);
        

        byte[] payload = Encoding.ASCII.GetBytes("16");

        BleApi.BLEData data = new BleApi.BLEData();
        data.buf = new byte[512];
        data.size = (short)payload.Length;
        data.deviceId = selectedDeviceId;
        data.serviceUuid = WhyskerBLE2.motorServiceUuid;
        data.characteristicUuid = WhyskerBLE2.motorCharUuid;
        Debug.Log("Motor Message Created");
        data.buf[0] = xByte;
        data.buf[1] = yByte;
        
        bool res = BleApi.SendData(in data, false);
        
        Debug.Log("Written");
//    
    }
     
}
