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
//using Windows.Devices.Bluetooth;
//using Windows.Devices.Bluetooth.GenericAttributionProfile;


// This Script is for testing UWP + BLE On PC comms
public class WhyskerBLEDemo : MonoBehaviour
{
    public bool isScanningDevices = false;

    public TMP_Text deviceScanButtonText;
    public TMP_Text deviceScanStatusText;
    public Dropdown deviceScanResultProto;
    

    
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



}
