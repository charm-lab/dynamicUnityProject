using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trakSTARToolController : MonoBehaviour
{
    private Vector3 NeedlePosition;
    private Vector3 NeedleRotation;

    public float[] toolPose = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    public int toolNumber = 0;
    public float poseScale = .024f;// .0254f; //real life inches to meters is .0254 but in hololens .024 is closer
    public float yPoseScale = .024f;

    void Update()
    {
        transformGameObject();
    }

    private void transformGameObject()
    {
        setTool();
        NeedlePosition = new Vector3(-toolPose[2] * poseScale, -toolPose[3] * yPoseScale, -toolPose[1] * poseScale);
        gameObject.transform.position = NeedlePosition + trakSTARController.instance.trakSTARBasePosition;
        NeedleRotation = new Vector3(toolPose[5], toolPose[4], toolPose[6]);
        gameObject.transform.eulerAngles = NeedleRotation + trakSTARController.instance.trakSTARBaseRotation;
    }

    private void setTool()
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
