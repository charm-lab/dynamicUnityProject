using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ATC3DGCppLibraryIntegration : MonoBehaviour
{
    //Import and expose native c++ functions

    [DllImport("ATC3DG64", EntryPoint = "InitializeBIRDSystem")]
    public static extern int InitializeBIRDSystem();

    // Start is called before the first frame update
    void Start()
    {
        print(InitializeBIRDSystem());
        print("YAY! trakSTAR .dll is working :D");

    }

    // Update is called once per frame
    void Update()
    {

    }
}
