using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTrakSTARPosition : MonoBehaviour
{
    private Vector3 NeedlePosition;
    private Vector3 NeedleRotation;

    public float[] toolPose = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    public int toolNumber = 0;
    private float poseScale = .0254f;

    void Update()
    {
        transformGameObject();
    }

    private void transformGameObject()
    {
        setTool();
        NeedlePosition = new Vector3(toolPose[1]*poseScale, toolPose[2]*poseScale, toolPose[3]*poseScale);
        gameObject.transform.position = NeedlePosition;// + trakSTARController.instance.trakSTARBasePosition;
        NeedleRotation = new Vector3(toolPose[4], toolPose[6], toolPose[5]);
        gameObject.transform.eulerAngles = NeedleRotation;// + trakSTARController.instance.trakSTARBaseRotation;
        
    }

    private void setTool()
    {
        if (toolNumber == 0)
        {
            toolPose = trakSTAR_UDPServer.instance.poseZero;
        }
        else if (toolNumber == 1)
        {
            toolPose = trakSTAR_UDPServer.instance.poseOne;
        }
    }
}
