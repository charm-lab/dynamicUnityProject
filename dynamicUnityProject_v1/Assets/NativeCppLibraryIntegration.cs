using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class NativeCppLibraryIntegration : MonoBehaviour
{
    //Import and expose native c++ functions

    [DllImport("NATIVECPPLIBRARY", EntryPoint = "displayNumber")]
    public static extern int displayNumber();

    [DllImport("NATIVECPPLIBRARY", EntryPoint = "getRandom")]
    public static extern int getRandom();


    // Start is called before the first frame update
    void Start()
    {
        print(displayNumber());
        print(getRandom());
        print("YAY! test .dll is working :D");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
