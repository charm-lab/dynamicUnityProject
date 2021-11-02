using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshsize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       Vector3 objectSize = Vector3.Scale(transform.localScale, GetComponent<MeshFilter>().mesh.bounds.size);
       Debug.Log(objectSize);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
