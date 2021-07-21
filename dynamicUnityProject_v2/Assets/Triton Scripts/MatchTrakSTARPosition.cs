using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTrakSTARPosition : MonoBehaviour
{

    private Vector3 NeedlePosition;
    private Vector3 NeedleRotation;

    public float[] poseZero = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    //public float[] poseOne = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };

    private float poseScale = .0254f;

    GameObject tool;

    // Start is called before the first frame update
    /**
    void Start()
    {

        /*tool = GameObject.Find("Main Scene/Tool").transform.gameObject;
        tool.name = "Tool2";
        tool = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tool.name = "Tool";

    }
*/
    // Update is called once per frame
    void Update()
    {
        transformGameObject();
    }

    private void transformGameObject()
    {
        poseZero = trakSTAR_UDPServer.instance.poseZero;

        NeedlePosition = new Vector3(poseZero[1]*poseScale, poseZero[2]*poseScale, poseZero[3]*poseScale);
        //tool.transform.position = NeedlePosition;
        gameObject.transform.position = NeedlePosition;

        NeedleRotation = new Vector3(poseZero[4], poseZero[6], poseZero[5]);
        //tool.transform.eulerAngles = NeedleRotation;
        gameObject.transform.eulerAngles = NeedleRotation;
    }





}
