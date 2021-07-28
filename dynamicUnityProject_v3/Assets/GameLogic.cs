using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    GameObject indexSphere;
    GameObject thumbSphere;
    GameObject trakSTAROrigin;

    [Header("Index Variables")]
    public Vector3 indexSpherePosition;
    public float indexDistToCenter;
    public float indexToSpherePenetration;
    public float indexForce;
    Vector3 indexScaling;

    public float indexPositionCommand;

    [Header("Thumb Variables")]
    public Vector3 thumbSpherePosition;
    public float thumbDistToCenter;
    public float thumbToSpherePenetration;
    public float thumbForce;
    Vector3 thumbScaling;

    public float thumbPositionCommand;

    /**** Create SPHERE *****/
    GameObject sphere; //Instatiate Sphere GameObject
    Rigidbody rigidSphere; //Declare rigid body on sphere
    SphereCollider sphereCollider; //This declares your SphereCollider
    MeshRenderer sphereMeshRenderer; //Declares Mesh Renderer
                                     //Sphere Scaling and Stiffness

    Vector3 sphereScalingV;

    [Header("Sphere Variables")]
    public Vector3 spherePosition;
    float sphereScaling = 0.05f; //m
    float sphereStiffness = 50.0f; //in N/m

    #region sphere-SA-TA
    /**** Create STARTINGAREA *****/
    GameObject startingArea; //Instatiate StartingArea GameObject
    Rigidbody rigidStartingArea; //Declare rigid body on sphere
    CapsuleCollider startingAreaCollider; //This declares your Collider
    MeshRenderer startingAreaMeshRenderer; //Declares Mesh Renderer
    float startingAreaRadius = 0.2f;
    float startingAreaHeight;
    float startingX = 0.0f;
    float startingZ = 0.25f;
    float targetOffset = 0.50f;

    /**** Create TARGETAREA *****/
    GameObject targetArea; //Instatiate targetArea GameObject
    Rigidbody rigidTargetArea; //Declare rigid body on sphere
    CapsuleCollider targetAreaCollider; //This declares your Collider
    MeshRenderer targetAreaMeshRenderer; //Declares Mesh Renderer
    float targetToSphereDist; //Distance between target and sphere
    float targetAreaHeight;
    float targetAreaRadius = 0.2f;
    #endregion sphere-SA-TA

    // Start is called before the first frame update
    void Start()
    {
        print("BUT DID YOU **TURN ON SAMPLE**???");

        indexSphere = GameObject.Find("trakSTAR/Index Sphere");
        thumbSphere = GameObject.Find("trakSTAR/Thumb Sphere");

        trakSTAROrigin = GameObject.Find("trakSTAR/trakSTAR Origin");
        trakSTAROrigin.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        indexSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        thumbSphere.GetComponent<MeshRenderer>().material.color = Color.blue;
        trakSTAROrigin.GetComponent<MeshRenderer>().material.color = Color.green;

        indexScaling = indexSphere.transform.localScale;
        thumbScaling = thumbSphere.transform.localScale;
        sphereScalingV = new Vector3(0.05f, 0.05f, 0.05f);

        //Create the sphere Game Object
        createSphere(startingX, startingZ);

        //Create starting and target areas
        createStartingArea(startingX, startingZ);
        createTargetArea(startingX, startingZ);

    }

    // Update is called once per frame
    // Purpose: check for detection of input, anything to be changed
    // or adjusted for non-physics objects, simple timers
    void Update()
    {
        spherePosition = sphere.transform.position;

        /*In Unity global units (may not be same as listed under trakSTAR Gameobject*/
        indexSpherePosition = indexSphere.transform.position;
        thumbSpherePosition = thumbSphere.transform.position;
    }

    // FixedUpdate is called once every physics step
    // Purpose: Physics calcuations, adjusting physics/rigidbody objects
    void FixedUpdate()
    {
        indexDistToCenter = Vector3.Magnitude(indexSpherePosition - spherePosition);
        thumbDistToCenter = Vector3.Magnitude(thumbSpherePosition - spherePosition);

        indexToSpherePenetration = 0.5f * (indexScaling.x + sphereScaling) - indexDistToCenter;
        thumbToSpherePenetration = 0.5f * (thumbScaling.x + sphereScaling) - thumbDistToCenter;

        indexForce = calculateForce(indexToSpherePenetration);
        thumbForce = calculateForce(thumbToSpherePenetration);

        indexPositionCommand = getPositionCommand(indexForce);
        thumbPositionCommand = getPositionCommand(thumbForce);

    }

    //Create Sphere GameObject:
    public void createSphere(float startingX, float startingZ)
    {
        //Sphere Object Definition
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //Name:
        sphere.name = "Sphere";

        //Set Size and Initial Position
        sphere.transform.localScale = new Vector3(sphereScaling, sphereScaling, sphereScaling);
        sphere.transform.position = new Vector3(0.3f, 0.025f, startingZ);
        //sphere.transform.position = new Vector3(startingX, 1.0f, startingZ);

        //This sets the Collider radius when the GameObject collides with a trigger Collider
        sphereCollider = sphere.GetComponent<SphereCollider>();
        sphereCollider.radius = 0.5f;

        PhysicMaterial sphereFriction = new PhysicMaterial();
        sphereFriction.dynamicFriction = 10.0f;
        sphereFriction.staticFriction = 10.0f;

        sphereCollider.material = sphereFriction;

        //Set Sphere mesh properties
        sphereMeshRenderer = sphere.GetComponent<MeshRenderer>();
        sphereMeshRenderer.material.color = Color.blue;

        //Set Sphere Rigid Body dynamics
        rigidSphere = sphere.AddComponent<Rigidbody>();
        rigidSphere.isKinematic = false;
        rigidSphere.useGravity = true;
        rigidSphere.mass = 1.0f;
        rigidSphere.drag = 1.0f;
        rigidSphere.angularDrag = 1.0f;

        /***Lock sphere location and orientation - TEMPORARY***/
        rigidSphere.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    public void createStartingArea(float startingX, float startingZ)
    {
        //Starting Area Object Definition
        startingArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        //Name:
        startingArea.name = "StartingArea";

        //Set Size and Initial Position
        startingAreaHeight = sphereCollider.radius * sphereScaling;
        startingArea.transform.localScale = new Vector3(startingAreaRadius, startingAreaHeight, startingAreaRadius);
        startingArea.transform.position = new Vector3(startingX, startingAreaHeight, startingZ);

        //This sets the Collider radius when the GameObject collides with a trigger Collider
        startingAreaCollider = startingArea.GetComponent<CapsuleCollider>();
        startingAreaCollider.enabled = false;

        //Set mesh properties
        startingAreaMeshRenderer = startingArea.GetComponent<MeshRenderer>();

        //Set Color and Transparaency
        #region StartColor
        startingAreaMeshRenderer.material.SetColor("_Color", new Color(0.5f, 0f, 1.0f, 0.5f));
        startingAreaMeshRenderer.material.SetFloat("_Mode", 3);
        startingAreaMeshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        startingAreaMeshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        startingAreaMeshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        startingAreaMeshRenderer.material.renderQueue = 3000;
        #endregion StartColor

        //Set Rigid Body dynamics
        rigidStartingArea = startingArea.AddComponent<Rigidbody>();
        rigidStartingArea.isKinematic = false;
        rigidStartingArea.useGravity = false;

        /***Lock location and orientation***/
        rigidStartingArea.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    public void createTargetArea(float startingX, float startingZ)
    {
        //Target area Object Definition
        targetArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        //Name:
        targetArea.name = "TargetArea";

        //Set Size and Initial Position
        targetAreaHeight = sphereCollider.radius * sphereScaling;
        targetArea.transform.localScale = new Vector3(targetAreaRadius, targetAreaHeight, targetAreaRadius);
        targetArea.transform.position = new Vector3(startingX, targetAreaHeight, startingZ - targetOffset);

        //This sets the Collider radius when the GameObject collides with a trigger Collider
        targetAreaCollider = targetArea.GetComponent<CapsuleCollider>();
        targetAreaCollider.enabled = false;

        //Set mesh properties
        targetAreaMeshRenderer = targetArea.GetComponent<MeshRenderer>();

        //Set Color and Transparaency
        #region TargetColor
        targetAreaMeshRenderer.material.SetColor("_Color", new Color(0.4f, 0.6f, 0.1f, 0.5f));
        targetAreaMeshRenderer.material.SetFloat("_Mode", 3);
        targetAreaMeshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        targetAreaMeshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        targetAreaMeshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        targetAreaMeshRenderer.material.renderQueue = 3000;
        #endregion TargetColor

        //Set Rigid Body dynamics
        rigidTargetArea = targetArea.AddComponent<Rigidbody>();
        rigidTargetArea.isKinematic = false;
        rigidTargetArea.useGravity = false;

        /***Lock location and orientation***/
        rigidTargetArea.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    public float calculateForce(float penetration)
    {
        //there should be no force in a direction
        //if the penetration is negative or 0
        //(signifying no contact from that direction)
        if (penetration <= 0.0f)
        {
            return 0.0f;
        }
        else
        {
            //Don't change value
        }
        //Use Hooke's Law to find force
        return sphereStiffness * penetration;
    }

    public float getIndexForce()
    {
        return indexForce;
    }   
    public float getThumbForce()
    {
        return thumbForce;
    }
    
    public float getPositionCommand(float force)
    {
        /*penetration value in mm*/
        return 1000 * (force / sphereStiffness);
    }


}
