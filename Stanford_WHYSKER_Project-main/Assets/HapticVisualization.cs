using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticVisualization : MonoBehaviour
{
    public float X;
    private float scaleX;
    public float Y;
    private float scaleY;
    private Vector3 visualizerPostion;

    // Update is called once per frame
    public void UpdateHapticVisualizer()
    {

        float Z = this.transform.position.z;
        scaleX = .16f * ((X-127.5f)/255f);
        scaleY = .16f * ((Y - 127.5f) / 255f);
        visualizerPostion = new Vector3(scaleX, scaleY, Z);
        this.transform.position = visualizerPostion;
    }
}
