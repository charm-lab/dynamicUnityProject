using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;


#if WINDOWS_UWP 
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#endif


public class BLE3
{
    
    
    // State Variables
    
    public enum ScanStatus { PROCESSING, AVAILABLE, FINISHED };
        
        
    //Initialize Public variables for all functions that are Windows UWP
    #if WINDOWS_UWP
    DeviceWatcher deviceWatcher;
    List<string> DeviceIds = new List<string>();
    Dictionary<string, DeviceInformation> KnownDevices = new Dictionary<string, DeviceInformation>();
    Queue<DeviceUpdate> deviceQueue = new Queue<DeviceUpdate>();
    bool deviceScanFinished;
        
    #endif
    
    // Lets you control the physical layout of the data fields of a class or structure in memory.
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)] 
    public struct DeviceUpdate
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string id;
        [MarshalAs(UnmanagedType.I1)]
        public bool isConnectable;
        [MarshalAs(UnmanagedType.I1)]
        public bool isConnectableUpdated;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string name;
        [MarshalAs(UnmanagedType.I1)]
        public bool nameUpdated;
    }
    

   
    public void StartDeviceScan(){
        // Scans Through Devices 
        
        #if WINDOWS_UWP
        string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

        
        string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
        
        deviceWatcher = DeviceInformation.CreateWatcher(
            aqsAllBluetoothLEDevices,
            requestedProperties,
            DeviceInformationKind.AssociationEndpoint);
        
        System.Diagnostics.Debug.WriteLine("Device Watcher Called");
        
        // see https://docs.microsoft.com/en-us/windows/uwp/cpp-and-winrt-apis/handle-events#revoke-a-registered-delegate
//        deviceWatcher.Added(auto_revoke, &DeviceWatcher_Added);
        deviceWatcher.Added += DeviceWatcher_Added;
        deviceWatcher.Updated += DeviceWatcher_Updated;
//        deviceWatcher.Removed += DeviceWatcher_Removed;
        deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        
        deviceScanFinished = false;
        deviceWatcher.Start();
        
//        System.Diagnostics.Debug.WriteLine(deviceWatcher);
        
        #endif
    }
    
    
    #if WINDOWS_UWP
    private void StopDeviceScan(){
        
        System.Diagnostics.Debug.WriteLine("Stop Device Scan");
        System.Diagnostics.Debug.WriteLine(deviceWatcher);
        
        try{
            //stops device watcher properties
            System.Diagnostics.Debug.WriteLine("In Try");
            deviceWatcher.Stop();
//            System.Diagnostics.Debug.WriteLine("Stopped");
            deviceWatcher.Added-=DeviceWatcher_Added;
            System.Diagnostics.Debug.WriteLine("Adder Removed");
            deviceWatcher.Updated-=DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
            deviceWatcher = null;
        }
        
        catch(Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Stopping Failed");
            System.Diagnostics.Debug.WriteLine(e);
        }
//        
        deviceScanFinished = true;
        
        
    }
    
    
    
    public void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
    {
        // We must update the collection on the UI thread because the collection is databound to a UI element.

        DeviceUpdate dev = new DeviceUpdate();
        
//        System.Diagnostics.Debug.WriteLine("Add Watcher -- Device Name", deviceInfo.Name);
//        System.Diagnostics.Debug.WriteLine(deviceWatcher);
        
        if (!DeviceIds.Contains(deviceInfo.Id))
        {
            
//            System.Diagnostics.Debug.WriteLine("Device Ids doesnt contain ID");
            try
            {
                KnownDevices.Add(deviceInfo.Id,deviceInfo);
//                System.Diagnostics.Debug.WriteLine("DevidInfo Added to Known Devices");
                DeviceIds.Add(deviceInfo.Id);
//                System.Diagnostics.Debug.WriteLine("DeviceIds added");
                dev.id = deviceInfo.Id;
                dev.name = deviceInfo.Name;
                dev.nameUpdated = true;
                
                bool prop = deviceInfo.Properties.ContainsKey("System.Devices.Aep.Bluetooth.Le.IsConnectable");
                
//                System.Diagnostics.Debug.WriteLine("Has Property: ");
//                System.Diagnostics.Debug.Write(prop);
                
                if (prop)
                {
                   dev.isConnectable = prop;
                }
//                
//                dev.isConnectable = true;
                deviceQueue.Enqueue(dev);
//                System.Diagnostics.Debug.WriteLine("Device Added to Device Queue", deviceQueue.Count);
//                System.Diagnostics.Debug.WriteLine("Device Name:");
                System.Diagnostics.Debug.Write(deviceInfo.Name);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed to open service.");
            }
        }

    }

    public void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
    {
        // We must update the collection on the UI thread because the collection is databound to a UI element.
//            await CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
//            {
//                lock (this)
//                {
        
//        System.Diagnostics.Debug.WriteLine(String.Format("Updated {0}{1}", deviceInfoUpdate.Id, ""));
//        System.Diagnostics.Debug.WriteLine(deviceWatcher);
        
        DeviceUpdate dev = new DeviceUpdate();
        // Protect against race condition if the task runs after the app stopped the deviceWatcher.
        if (sender == deviceWatcher)
        {
//                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
            if (DeviceIds.Contains(deviceInfoUpdate.Id))
            {
                // Device is already being displayed - update UX.
                KnownDevices[deviceInfoUpdate.Id].Update(deviceInfoUpdate);
                DeviceIds.Add(deviceInfoUpdate.Id);
                DeviceInformation knownDevice = KnownDevices[deviceInfoUpdate.Id];
                dev.id = deviceInfoUpdate.Id;
                dev.name = knownDevice.Name;
                dev.isConnectable = true;
                deviceQueue.Enqueue(dev);
                return;
            }


        }
    }

    
    
       

    public async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
    {
        StopDeviceScan();
    }

    
    #endif
        
    
    // Poll Device--polling == the sampling of status of the target device by a client 
    #if WINDOWS_UWP
    public unsafe void PollDevice(out DeviceUpdate deviceUpdate, out ScanStatus res, bool block)
    {
        
        //
        deviceUpdate = new DeviceUpdate();
        res = new ScanStatus(); 
        
//        System.Diagnostics.Debug.WriteLine("Poll Device Dev Count:");
//        System.Diagnostics.Debug.WriteLine(deviceQueue.Count);
        
        
        if (block && deviceQueue.Count == 0 && !deviceScanFinished)
        {
//            System.Diagnostics.Debug.WriteLine("Dev Count is 0  + Blocked");
            res = ScanStatus.FINISHED;
            deviceScanFinished = true;
            return;
        }
        
        
        if(deviceQueue.Count !=0)
        {
//            System.Diagnostics.Debug.WriteLine("Dev Queue Dequeued");
            deviceUpdate = deviceQueue.Peek();
            deviceQueue.Dequeue();
//            eviceIds.Add(deviceUpdate.id); // TEST 
            res = ScanStatus.AVAILABLE;
        }
        else if (deviceScanFinished)
        {
            
            res = ScanStatus.FINISHED;
            deviceScanFinished = true;
//            System.Diagnostics.Debug.WriteLine("ScanStatus set to 0 - Stop Dev Scan called ");
        }
        else
        {
            res = ScanStatus.PROCESSING;
//            System.Diagnostics.Debug.WriteLine("Dev Count is 0 but Stop Dev Scan not called");
        }
        
//        System.Diagnostics.Debug.WriteLine(deviceQueue.Count);
        
        
    }
    
    #endif
        
    // BLEScan
    
    public static Thread scanThread;
//    public static BLEScan currentScan = new BLEScan();
    public BLEScan currentScan = new BLEScan();
    public bool isConnected = false;

    public class BLEScan
    {
        public delegate void FoundDel(string deviceId, string deviceName);
        public delegate void FinishedDel();
        public FoundDel Found;
        public FinishedDel Finished;
        internal bool cancelled = false;

        public void Cancel()
        {
            cancelled = true;
        }
    }
    
    public BLEScan ScanDevices()
    {
        
        #if WINDOWS_UWP
        BLE3 ble = new BLE3();
        System.Diagnostics.Debug.WriteLine("BLE.ScanDevices() started");
        
        
        
        if (scanThread == Thread.CurrentThread){
            System.Diagnostics.Debug.WriteLine("a new scan can not be started from a callback of the previous scan");
            throw new InvalidOperationException("a new scan can not be started from a callback of the previous scan");}
            
        
        
        else if (scanThread != null)
//            throw new InvalidOperationException("the old scan is still running");
            System.Diagnostics.Debug.WriteLine("old scan is still running");
        
        
        currentScan.Found = null;
        currentScan.Finished = null;
//        ble.StartDeviceScan();
        System.Diagnostics.Debug.WriteLine("about to call thread ");
        
        scanThread = new Thread(() =>
        {
            ble.StartDeviceScan();
            System.Diagnostics.Debug.WriteLine("StartDeviceScan called");
            DeviceUpdate res = new DeviceUpdate();
//            List<string> deviceIds = new List<string>();
            Dictionary<string, string> deviceNames = new Dictionary<string, string>();
            ScanStatus status;            
            
            ble.PollDevice(out res, out status, false);
            while(status != ScanStatus.FINISHED)
            {
                
                if (res.nameUpdated)
                {
//                    System.Diagnostics.Debug.WriteLine("Res Name Updated ");
                    try{
                        DeviceIds.Add(res.id);
                        deviceNames.Add(res.id, res.name);
                        
//                        System.Diagnostics.Debug.WriteLine("Device Name Added in Scan ");
                        
                    }
                    
                    catch (ArgumentException e){}
                    
                }
                // connectable device
                
                
                ble.PollDevice(out res, out status, false );
                
//                System.Diagnostics.Debug.WriteLine("ID");
//                System.Diagnostics.Debug.WriteLine(res.id);
                System.Diagnostics.Debug.WriteLine("Scan Thread Device Name");
                System.Diagnostics.Debug.WriteLine(res.name);
                System.Diagnostics.Debug.WriteLine("Contains ID");
                System.Diagnostics.Debug.WriteLine(DeviceIds.Contains(res.id));
                System.Diagnostics.Debug.WriteLine("IsConnectable");
                System.Diagnostics.Debug.WriteLine(res.isConnectable);
                
//                
//                System.Diagnostics.Debug.WriteLine("Current Scan Status: ");
//                System.Diagnostics.Debug.WriteLine(currentScan.Found);
                
                
                if (DeviceIds.Contains(res.id) && res.isConnectable)
                {
                    System.Diagnostics.Debug.WriteLine("Scan Found");
                    currentScan.Found?.Invoke(res.id, deviceNames[res.id]);
                }
//                // check if scan was cancelled in callback
//                if (currentScan.cancelled)
//                    System.Diagnostics.Debug.WriteLine("Cancelled");
//                    break;
                
      
            }
            currentScan.Finished?.Invoke();
            scanThread = null;
        });
        scanThread.Start();
        #endif
        return currentScan;
    }
    
    
    public async void Connect(string deviceId, string serviceUuid, string[] characteristicUuids)
    {
//        result = false;
        
        System.Diagnostics.Debug.Write("BLE.Connect");
//        if (isConnected)
//            return;
//        
////        Debug.Log("retrieving ble profile...");
//        
//        #if WINDOWS_UWP
//        
//        try
//        {
//            // BT_Code: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
//            bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceId);
//
//            if (bluetoothLeDevice == null)
//            {
////                rootPage.NotifyUser("Failed to connect to device.", NotifyType.ErrorMessage);
//                System.Diagnostics.Debug.Write("Failed to Connect to Device");
//                isConnected = false;
////                result = false;
//                return;
//            }
//            
////            result = true;
//            isConnected = true;
//        }
//        catch (Exception ex)
//        {
////            rootPage.NotifyUser("Bluetooth radio is not on.", NotifyType.ErrorMessage);
//            
//            System.Diagnostics.Debug.Write("Bluetooth Radio is Not ON ");
//            System.Diagnostics.Debug.WriteLine(ex);
//            isConnected = false;
//        }
//
//        #endif
            
        
    }
    public void Close()
    {
//        Impl.Quit();
        isConnected = false;
    }
    ~BLE3()
    {
        Close();
    }
    
    
}