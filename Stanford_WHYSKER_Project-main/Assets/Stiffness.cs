using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stiffness : MonoBehaviour
{
    public float stiffness = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public float getStiffness()
    {
        return stiffness;
    }
    
    public void setStiffness(float stiff)
    {
        stiffness = stiff;
    }
}
