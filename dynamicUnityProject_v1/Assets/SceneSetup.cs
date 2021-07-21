using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    //Floor
    GameObject floor;

    // Start is called before the first frame update
    void Start()
    {
        //Setup the floor object
        floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(10f,1f,10f);
        floor.transform.position = new Vector3(0f,0f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
