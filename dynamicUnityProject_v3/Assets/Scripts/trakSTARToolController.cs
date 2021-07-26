using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trakSTARToolController : MonoBehaviour
{
    private Vector3 objectPosition;
    private Vector3 objectRotation;

    public float[] toolPose = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    public int toolNumber = 0;
    private float poseScale = .0254f;

    GameObject indexSphere;
    GameObject thumbSphere;
    GameObject trakSTAROrigin;

    void Start()
    {
        Debug.Log("DID YOU TURN ON SAMPLE???");

        indexSphere = GameObject.Find("trakSTAR/Index Sphere");
        thumbSphere = GameObject.Find("trakSTAR/Thumb Sphere");
        
        trakSTAROrigin = GameObject.Find("trakSTAR/trakSTAR Origin");
        trakSTAROrigin.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        indexSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        thumbSphere.GetComponent<MeshRenderer>().material.color = Color.blue;
        trakSTAROrigin.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    void Update()
    {
        transformGameObject();
    }

    private void transformGameObject()
    {
        setObjectPose();
        objectPosition = new Vector3(-toolPose[3] * poseScale, -toolPose[1] * poseScale, -toolPose[2] * poseScale);
        gameObject.transform.position = objectPosition + trakSTARController.instance.trakSTARBasePosition;
        objectRotation = new Vector3(toolPose[4], toolPose[6], toolPose[5]);
        gameObject.transform.eulerAngles = objectRotation + trakSTARController.instance.trakSTARBaseRotation;
    }

    private void setObjectPose()
    {
        if (toolNumber == 0)
        {
            toolPose = TrainerController.instance.poseZero;
        }
        else if (toolNumber == 1)
        {
            toolPose = TrainerController.instance.poseOne;
        }
    }
}
