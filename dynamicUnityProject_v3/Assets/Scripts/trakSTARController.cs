using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trakSTARController : MonoBehaviour
{
    public static trakSTARController instance;

    public GameObject trakSTARPlaceholder;

    public float[] trakSTARBasePose = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    public Vector3 trakSTARBasePosition;
    public Vector3 trakSTARBaseRotation;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        trakSTARBasePosition = new Vector3(0, 0, 0);
        trakSTARBaseRotation = new Vector3(0, 0, 0);
    }

    public void trakSTARPoseSet()
    {
        // Get Placeholder position and rotation
        trakSTARBasePosition = trakSTARPlaceholder.transform.position;
        trakSTARBaseRotation = trakSTARPlaceholder.transform.eulerAngles;
        Debug.Log("Zero trakSTARPosition");
    }
}
