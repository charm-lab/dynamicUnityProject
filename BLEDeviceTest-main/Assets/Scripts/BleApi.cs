using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

/*
File Name: WhyskerBLE2.CS
Author: Adabru 
Purpose: Connecter for the Bluetooth Dll
Last Modification: 6/29/2021 -- Added Documentation Header
*/

public class BleApi
{
    // dll calls
    public enum ScanStatus { PROCESSING, AVAILABLE, FINISHED };

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

    [DllImport("BLEWinrtDll.dll", EntryPoint = "StartDeviceScan")]
    public static extern void StartDeviceScan();

    [DllImport("BLEWinrtDll.dll", EntryPoint = "PollDevice")]
    public static extern ScanStatus PollDevice(ref DeviceUpdate device, bool block);

    [DllImport("BLEWinrtDll.dll", EntryPoint = "StopDeviceScan")]
    public static extern void StopDeviceScan();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Service
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string uuid;
    };

    [DllImport("BLEWinrtDll.dll", EntryPoint = "ScanServices", CharSet = CharSet.Unicode)]
    public static extern void ScanServices(string deviceId);

    [DllImport("BLEWinrtDll.dll", EntryPoint = "PollService")]
    public static extern ScanStatus PollService(out Service service, bool block);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Characteristic
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string uuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string userDescription;
    };

    [DllImport("BLEWinrtDll.dll", EntryPoint = "ScanCharacteristics", CharSet = CharSet.Unicode)]
    public static extern void ScanCharacteristics(string deviceId, string serviceId);

    [DllImport("BLEWinrtDll.dll", EntryPoint = "PollCharacteristic")]
    public static extern ScanStatus PollCharacteristic(out Characteristic characteristic, bool block);

    [DllImport("BLEWinrtDll.dll", EntryPoint = "SubscribeCharacteristic", CharSet = CharSet.Unicode)]
    public static extern bool SubscribeCharacteristic(string deviceId, string serviceId, string characteristicId, bool block);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct BLEData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] buf;
        [MarshalAs(UnmanagedType.I2)]
        public short size;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string deviceId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string serviceUuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string characteristicUuid;
    };

    [DllImport("BLEWinrtDll.dll", EntryPoint = "PollData")]
    public static extern bool PollData(out BLEData data, bool block);

    [DllImport("BLEWinrtDll.dll", EntryPoint = "SendData")]
    public static extern bool SendData(in BLEData data, bool block);

    [DllImport("BLEWinrtDll.dll", EntryPoint = "Quit")]
    public static extern void Quit();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ErrorMessage
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string msg;
    };

    [DllImport("BLEWinrtDll.dll", EntryPoint = "GetError")]
    public static extern void GetError(out ErrorMessage buf);



    //    public string sendMessage (string deviceId, string serviceId, string selectedCharacteristic){
    //        var writer = new DataWriter();
    //        // WriteByte used for simplicity. Other common functions - WriteInt16 and WriteSingle
    //        writer.Write[DllImport("k8055d.dll")] Byte(0x01);
    //        
    //        GattDeviceService service = await GattDeviceService.FromIdAsync(devidId);
    //        var gattCharacteristic = service.GetCharacteristics(new Guid(selectedCharacteristic)).First();
    //
    //
    //        GattCommunicationStatus result = await selectedCharacteristic.WriteValueAsync(writer.DetachBuffer());
    //        if (result == GattCommunicationStatus.Success)
    //        {
    //            // Successfully wrote to device
    //            Debug.Log("Write Successful");
    //        }
    //        
    //        return "send data";
    //    }
}