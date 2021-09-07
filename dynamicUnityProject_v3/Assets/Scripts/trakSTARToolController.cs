using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trakSTARToolController : MonoBehaviour
{
    private Vector3 objectPosition;
    private Vector3 objectRotation;

    public float[] toolPose = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    public int toolNumber = 0;
    //Convert default position values from in to m
    private float poseScale = 0.0254f;

    void Update()
    {
        transformGameObject();
    }

    private void transformGameObject()
    {
        setObjectPose();
        //Map Raw stream to Unity coordinate system and convert in to m
        objectPosition = new Vector3(toolPose[1] , -toolPose[3] , -toolPose[2] );
        objectPosition *= poseScale;

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
        else if (toolNumber == 2)
        {
            toolPose = TrainerController.instance.poseTwo;
        }
    }
}
