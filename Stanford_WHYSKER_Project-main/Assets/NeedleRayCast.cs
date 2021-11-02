using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleRayCast : MonoBehaviour
{
    public GameObject BLE;
    CubeBLE cubeBLE;

    public GameObject hapticVisualization;
    private GameObject trainer;
    public LineRenderer needlePointer;
    public float needleWidth = .01f;
    public float needleLength = .05f;
    public bool needleVisible = false;
    public bool needleInserted = false;
    public float needleInsertionDistance = 0;


    public void Start()
    {
        cubeBLE = BLE.GetComponent<CubeBLE>();

        Vector3[] startNeedlePosition = new Vector3[2] { Vector3.zero, Vector3.zero };
        needlePointer.SetPositions(startNeedlePosition);
        needlePointer.enabled = true;

    }

    public void FixedUpdate()
    {
        //setup raycast
        //RaycastHit hit;
        RaycastHit[] hits;
        Ray needleRay = new Ray(transform.position, transform.forward);

        //visible needle
        Vector3 needleTipPosition = transform.position + (needleLength * transform.eulerAngles);
        needlePointer.SetPosition(0, transform.position);
        needlePointer.SetPosition(1, needleTipPosition);

        hits = Physics.RaycastAll(needleRay, needleLength);

        float normalForce = 0;
        float AOI = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            float insertionDistance = needleLength - hit.distance;
            float deltaInsertionDistance = needleInsertionDistance - insertionDistance;
            needleInsertionDistance = insertionDistance;

            trainer = hit.collider.gameObject;

            if (trainer.name == "Body")
            {
                normalForce = CalculateForces(trainer, 0, deltaInsertionDistance);
            }
            else if (trainer.name == "Ribs")
            {
                normalForce = CalculateForces(trainer, 1);
            }
            else if (trainer.name == "AngleInsertionRibDown")
            {
                AOI = CalculateForces(trainer, 2);
            }
        }
        MotorWriteHapticMapping(normalForce, AOI);
    }

    public float CalculateForces(GameObject trainer, int type, float deltaInsertionDistance = 0)
    {
        float stiff = trainer.GetComponent<Stiffness>().getStiffness();
        float force = 0;

        if (type == 1)
        {
            if (deltaInsertionDistance >= 0)
            {
                force = (1f - stiff);
                //force = Mathf.RoundToInt(127 * (1 - stiff));
            }
            else if (deltaInsertionDistance < 0)
            {
                force = -(1f - stiff);
                //force = -Mathf.RoundToInt(127 * (1 - stiff));
            }

        }
        else if (type == 2)
        {
            force = Mathf.RoundToInt(255 * stiff);
        }
        return force;
    }

    private void MotorWriteHapticMapping(float normalForce, float AOI)
    {
        int xByte = (byte)0;
        int yByte = (byte)0;

        float X = 0f;
        float Y = 0f;

        if (TrainerController.instance.XHapticsType == 1)
        {
            xByte = Mathf.RoundToInt(normalForce * 127f);
            xByte = Mathf.RoundToInt((127.5f * X) + 127.5f);
            X = normalForce;

        }
        else if (TrainerController.instance.XHapticsType == 2)
        {
            xByte = Mathf.RoundToInt(AOI * 127f);
            X = AOI;
        }

        if (TrainerController.instance.YHapticsType == 1)
        {
            yByte = Mathf.RoundToInt(normalForce * 127f);
            yByte = Mathf.RoundToInt((127.5f * Y) + 127.5f);
            Y = normalForce;
        }
        else if (TrainerController.instance.YHapticsType == 2)
        {
            yByte = Mathf.RoundToInt(AOI * 127f);
            Y = AOI;
        }

        Debug.Log("Write " + (int)xByte + " to X wristworn and " + (int)yByte + " to Y wristworn");
        hapticVisualization.GetComponent<HapticVisualization>().X = X;
        hapticVisualization.GetComponent<HapticVisualization>().Y = Y;
        hapticVisualization.GetComponent<HapticVisualization>().UpdateHapticVisualizer();
        cubeBLE.WriteMotor((byte)xByte, (byte)yByte);
    }
}
