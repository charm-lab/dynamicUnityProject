using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trakSTARController : MonoBehaviour
{
    public static trakSTARController instance;

    public GameObject trakSTARPlaceholder;
    public GameObject trainer;

    public float[] trakSTARBasePose = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
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

        Vector3 trainerPosition = new Vector3( -1.2f, .2f, .4f);
        Vector3 trainerRotation = new Vector3(-90, 90, 0);
        trainer.transform.position = trakSTARBasePosition + trainerPosition;
        trainer.transform.eulerAngles = trakSTARBaseRotation + trainerRotation;
        Debug.Log("Locate Trainer");
    }
}
