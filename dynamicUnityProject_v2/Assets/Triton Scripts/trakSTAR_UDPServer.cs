using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class trakSTAR_UDPServer : MonoBehaviour
{
    public static trakSTAR_UDPServer instance;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public string message = "nothing";

    private const int bufSize = 8 * 1024;
    private Socket _socket;
    private State state;
    private EndPoint epFrom;
    private AsyncCallback recv = null;

    public float[] poseZero = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    public float[] poseOne = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };

    private Vector3 NeedlePosition;
    private Vector3 NeedleRotation;

    public class State
    {
        public byte[] buffer = new byte[bufSize];
    }

    public void Server(string address, int port)
    {
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
        _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
        Receive();
    }

    private void Receive()
    {
        _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
        {
            State so = (State)ar.AsyncState;
            int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
            _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
            message = Encoding.ASCII.GetString(so.buffer, 0, bytes);
            ExtractPositions(message);
        }, state);
    }

    private void ExtractPositions(string message)
    {
        //Split single string apart and remove empty strings
        char[] delimiters = { '[', ']', ' ', ':' };
        string[] sStrings = message.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        //Debug.Log(sStrings[0]);

        //Parse strings into floats
        float sensor = float.Parse(sStrings[0]);
        float x = float.Parse(sStrings[1]);
        float y = float.Parse(sStrings[2]);
        float z = float.Parse(sStrings[3]);
        float a = float.Parse(sStrings[4]);
        float e = float.Parse(sStrings[5]);
        float r = float.Parse(sStrings[6]);

        //Make Pose Array
        float[] localPose = { sensor, x, y, z, a, e, r };
        float[] localPosition = {x, y, z };
        float[] localRotation = {a, e, r};

        //Set pose to sensor 0 or 1
        if (localPose[0] == 0)
        {
            poseZero = localPose;
            //NeedleRotation = localRotation;
            //Vector3 positionZero = Vector3.Set(x, y, z);
            //Quaternion rotationZero = Quaternion.Euler(a, e, r);
            //Pose poseZero = Pose.Set(positionZero, rotationZero);
        }
        else if (localPose[0] == 1)
        {
            poseOne = localPose;
        }
        else
        {
            Debug.Log("Not sensor 0 or 1?");
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        //Setup UDP Server
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        state = new State();
        epFrom = new IPEndPoint(IPAddress.Any, 0);
        Server(ip, port);

        //Setup Connection to Needle Asset
        //NeedlePosition = new Vector3(poseZero[1], poseZero[2], poseZero[3]);
        //NeedleRotation = new Vector3(poseZero[4], poseZero[5], poseZero[6]);
    }

    // Update is called once per frame
    void Update()
    {
        //Receive Data from trakSTAR
        Receive();
    }
}
