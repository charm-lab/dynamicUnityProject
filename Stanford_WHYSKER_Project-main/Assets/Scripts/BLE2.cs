//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;
//using UnityEngine;
//
//
//#if WINDOWS_UWP 
//using System;
//using Windows.Devices.Bluetooth.Advertisement;
//using Windows.Devices.Bluetooth;
//using Windows.Devices.Enumeration;
//#endif
//
//
//public class BLE2
//{
//    // dll calls
//    
//
//        public enum ScanStatus { PROCESSING, AVAILABLE, FINISHED };
//        
//        
//        //Initialize Public variables for all functions that are Windows UWP
//        #if WINDOWS_UWP
//            DeviceWatcher deviceWatcher;
//            DeviceWatcher.Added_revoker deviceWatcherAddedRevoker;
//            DeviceWatcher.Updated_revoker deviceWatcherUpdatedRevoker;
//            DeviceWatcher.EnumerationCompleted_revoker deviceWatcherCompletedRevoker;
//        #endif
//    
//    private static async void TestFunction()
//    {
//        #if WINDOWS_UWP 
//        System.Diagnostics.Debug.Write("=Received Advertisement=");
//        BluetoothLEAdvertisementWatcher _watcher = new BluetoothLEAdvertisementWatcher();
//        System.Diagnostics.Debug.Write("Watcher Initialized");
////        _watcher.Received += WatcherOnReceived;
////        _watcher.Start();
//        #endif
//    }
//   
//    
//    private static async void StartDeviceScan(){
//        
//        
//    }
//
//    public static Thread scanThread;
//    public static BLEScan currentScan = new BLEScan();
//    public bool isConnected = false;
//
//    public class BLEScan
//    {
//        public delegate void FoundDel(string deviceId, string deviceName);
//        public delegate void FinishedDel();
//        public FoundDel Found;
//        public FinishedDel Finished;
//        internal bool cancelled = false;
//
//        public void Cancel()
//        {
//            cancelled = true;
////            Impl.StopDeviceScan();
//        }
//    }
//
//    // don't block the thread in the Found or Finished callback; it would disturb cancelling the scan
//    public static BLEScan ScanDevices()
//    {
//        System.Diagnostics.Debug.Write("BLE.ScanDevices() started");
//        if (scanThread == Thread.CurrentThread)
//            throw new InvalidOperationException("a new scan can not be started from a callback of the previous scan");
//            
//        else if (scanThread != null)
////            throw new InvalidOperationException("the old scan is still running");
//            System.Diagnostics.Debug.Write("old scan is still running");
//
//        currentScan.Found = null;
//        currentScan.Finished = null;
//        scanThread = new Thread(() =>
//        {
//            System.Diagnostics.Debug.Write("using dll now");
//            System.Diagnostics.Debug.Write("About to call test function");
//            #if WINDOWS_UWP
//            System.Diagnostics.Debug.Write("This is NETFX CORE");
//            TestFunction();
//            
//            StartDeviceScan();
//            #endif
//            System.Diagnostics.Debug.Write("Test Function Called");
////            Impl.StartDeviceScan();
//            System.Diagnostics.Debug.Write("StartDeviceScan called");
////            Impl.DeviceUpdate res = new Impl.DeviceUpdate();
//            System.Diagnostics.Debug.Write("Device Update called");
//            List<string> deviceIds = new List<string>();
//            Dictionary<string, string> deviceNames = new Dictionary<string, string>();
//            //Impl.ScanStatus status;
//////            while (Impl.PollDevice(out res, true) != Impl.ScanStatus.FINISHED)
//////            {
//////                if (res.nameUpdated)
//////                {
//////                    try{
//////                        
//////                        deviceIds.Add(res.id);
//////                        deviceNames.Add(res.id, res.name);
//////                        
//////                    }
//////                    
//////                    catch (ArgumentException e){
//////                        
//////                        
//////                    }
//////                    
//////                }
////                // connectable device
////                if (deviceIds.Contains(res.id) && res.isConnectable)
////                    currentScan.Found?.Invoke(res.id, deviceNames[res.id]);
////                // check if scan was cancelled in callback
////                if (currentScan.cancelled)
////                    break;
//            }
//            currentScan.Finished?.Invoke();
//            scanThread = null;
//        });
//        scanThread.Start();
//        return currentScan;
//    }
//
//    public static void RetrieveProfile(string deviceId, string serviceUuid)
//    {
//        Impl.ScanServices(deviceId);
//        Impl.Service service = new Impl.Service();
//        while (Impl.PollService(out service, true) != Impl.ScanStatus.FINISHED)
//            Debug.Log("service found: " + service.uuid);
//        // wait some delay to prevent error
//        Thread.Sleep(200);
//        Impl.ScanCharacteristics(deviceId, serviceUuid);
//        Impl.Characteristic c = new Impl.Characteristic();
//        while (Impl.PollCharacteristic(out c, true) != Impl.ScanStatus.FINISHED)
//            Debug.Log("characteristic found: " + c.uuid + ", user description: " + c.userDescription);
//    }
//
//    public static bool Subscribe(string deviceId, string serviceUuids, string[] characteristicUuids)
//    {
//        foreach (string characteristicUuid in characteristicUuids)
//        {
//            Debug.Log("Subscribing to: "+ characteristicUuid);
//            try{
//                bool res = Impl.SubscribeCharacteristic(deviceId, serviceUuids, characteristicUuid);
//                Debug.Log(res + characteristicUuid);
//
//            }
//            
//            catch(Exception e){
//                Debug.Log(e);
//                return false;
//                
//            }
////            if (!res)
////                Debug.Log("Failed to Subscribe" + characteristicUuid);
////                return false;
//        }
//        return true;
//    }
//
//    public bool Connect(string deviceId, string serviceUuid, string[] characteristicUuids)
//    {
//        if (isConnected)
//            return false;
//        Debug.Log("retrieving ble profile...");
//        RetrieveProfile(deviceId, serviceUuid);
//        if (GetError() != "Ok")
//            throw new Exception("Connection failed: " + GetError());
//        Debug.Log("subscribing to characteristics...");
//        bool result = Subscribe(deviceId, serviceUuid, characteristicUuids);
//        if (GetError() != "Ok" || !result)
//            throw new Exception("Connection failed: " + GetError());
//        isConnected = true;
//        return true;
//    }
//
//    public static bool WritePackage(string deviceId, string serviceUuid, string characteristicUuid, byte[] data)
//    {
//        
//        
////        Impl.BLEData packageSend;
//        packageSend.buf = new byte[512];
//        packageSend.size = (short)data.Length;
//        packageSend.deviceId = deviceId;
//        packageSend.serviceUuid = serviceUuid;
//        packageSend.characteristicUuid = characteristicUuid;
//        for (int i = 0; i < data.Length; i++)
//            packageSend.buf[i] = data[i];
//        Debug.Log("Write Package: "+ packageSend.buf[0]);
//        return Impl.SendData(packageSend);
//    }
//
//    public static void ReadPackage()
//    {
//        Impl.BLEData packageReceived;
//        bool result = Impl.PollData(out packageReceived, true);
//        if (result)
//        {
//            if (packageReceived.size > 512)
//                throw new ArgumentOutOfRangeException("Please keep your ble package at a size of maximum 512, cf. spec!\n"
//                    + "This is to prevent package splitting and minimize latency.");
//            Debug.Log("received package from characteristic: " + packageReceived.characteristicUuid
//                + " and size " + packageReceived.size + " use packageReceived.buf to access the data.");
//        }
//    }
//
//    public static byte[] ReadBytes()
//    {
//        Impl.BLEData packageReceived;
//        bool result = Impl.PollData(out packageReceived, true);
//
//        if (result)
//        {
//            Debug.Log("Size: " + packageReceived.size);
//            Debug.Log("From: " + packageReceived.deviceId);
//
//            if (packageReceived.size > 512)
//                throw new ArgumentOutOfRangeException("Package too large.");
//
//            return packageReceived.buf;
//        } else
//        {
//            return new byte[] { 0x0 };
//        }
//    }
//
//    public void Close()
//    {
//        Impl.Quit();
//        isConnected = false;
//    }
//
//    public static string GetError()
//    {
//        Impl.ErrorMessage buf;
//        Impl.GetError(out buf);
//        return buf.msg;
//    }
//
//    ~BLE2()
//    {
//        Close();
//    }
//
//}