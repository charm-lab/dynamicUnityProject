using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class GameLogic : MonoBehaviour
{
    #region
    GameObject indexSphere;
    GameObject thumbSphere;
    GameObject trakSTAROrigin;

    [Header("Index Variables")]
    public Vector3 indexPosition;
    public Vector3 indexOrientation;
    public float indexDistToCenter;
    public float indexToSpherePenetration;
    public float indexForce;
    public float indexScaleValue = 0.04f;  //m
    Vector3 indexScaling;

    public float indexPositionCommand;

    [Header("Thumb Variables")]
    public Vector3 thumbPosition;
    public Vector3 thumbOrientation;
    public float thumbDistToCenter;
    public float thumbToSpherePenetration;
    public float thumbForce;
    public float thumbScaleValue = 0.04f; //m
    Vector3 thumbScaling;

    public float thumbPositionCommand;

    /**** Create SPHERE *****/
    GameObject sphere; //Instatiate Sphere GameObject
    Rigidbody rigidSphere; //Declare rigid body on sphere
    SphereCollider sphereCollider; //This declares your SphereCollider
    MeshRenderer sphereMeshRenderer; //Declares Mesh Renderer
                                     //Sphere Scaling and Stiffness
    Vector3 sphereScaling;

    [Header("Sphere Variables")]
    public Vector3 spherePosition;
    public Vector3 sphereOrientation;
    public float sphereScaleValue = 0.05f; //m
    public float sphereStiffness = 50.0f; //in N/m

    /**** Create STARTINGAREA *****/
    GameObject startingArea; //Instatiate StartingArea GameObject
    Rigidbody rigidStartingArea; //Declare rigid body on sphere
    CapsuleCollider startingAreaCollider; //This declares your Collider
    MeshRenderer startingAreaMeshRenderer; //Declares Mesh Renderer

    [Header("Starting Area Variables")]
    public Vector3 startingAreaPosition;
    public float startingAreaRadius = 0.2f;
    public float startingAreaHeight;
    float startingX = 0.0f;
    float startingZ = 0.25f;
    float targetOffset = 0.50f;

    /**** Create TARGETAREA *****/
    GameObject targetArea; //Instatiate targetArea GameObject
    Rigidbody rigidTargetArea; //Declare rigid body on sphere
    CapsuleCollider targetAreaCollider; //This declares your Collider
    MeshRenderer targetAreaMeshRenderer; //Declares Mesh Renderer

    [Header("Target Area Variables")]
    public Vector3 targetAreaPosition;
    public float targetToSphereDist; //Distance between target and sphere
    public float targetAreaRadius = 0.2f;
    public float targetAreaHeight;
    public bool isSphereInTarget = false; //Sphere in target boolean


    /**** Create WAYPOINT *****/
    GameObject waypoint; //Instatiate waypoint GameObject
    Rigidbody rigidWaypoint; //Declare rigid body on waypoint
    SphereCollider waypointCollider; //This declares your Collider
    MeshRenderer waypointMeshRenderer; //Declares Mesh Renderer

    [Header("Waypoint Variables")]
    public float waypointCenterHeight = 0.15f; //Height of waypoint relative to floor
    public float waypointRadius = 0.2f;
    public bool passedWaypoint = false; //Waypoint Boolean
    
    /**** Trial Info *****/
    [Header("Contact Conditions")]
    //Contact booleans
    public bool indexContact;
    public bool thumbContact;
    public bool heldSphereBefore = false;
    
    /**** Trial Info *****/
    [Header("Trial Variables")]
    public int trialNumber = 1; //the actual trial being worked on
    public int numTrials = 3; // the total number of trials the user will participate in
    public int numElapsedTimes = 1; //num of attempts for each trial
    
    public int successCounter = 0; //number of successful moves within a trial
    public int failCounter = 0;

    /**** Timing *****/
    //use task success/fails
    [Header("Timing")]
    public float timeOfCurrentSuccess = 0.0f;
    public float timeSinceLastSuccess = 0.0f;
    
    public int timeIndex = 0;
    public float[] elapsedTimes;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        print("BUT DID YOU **TURN ON SAMPLE**???");

        indexSphere = GameObject.Find("trakSTAR/Index Sphere");
        thumbSphere = GameObject.Find("trakSTAR/Thumb Sphere");

        trakSTAROrigin = GameObject.Find("trakSTAR/trakSTAR Origin");
        trakSTAROrigin.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        indexSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        thumbSphere.GetComponent<MeshRenderer>().material.color = Color.cyan;
        trakSTAROrigin.GetComponent<MeshRenderer>().material.color = Color.magenta;

        indexScaling = new Vector3(indexScaleValue, indexScaleValue, indexScaleValue);
        thumbScaling = new Vector3(thumbScaleValue, thumbScaleValue, thumbScaleValue);
        sphereScaling = new Vector3(sphereScaleValue, sphereScaleValue, sphereScaleValue);

        //Set up timing saving
        elapsedTimes = new float[numElapsedTimes * numTrials];

        //Create the sphere Game Object
        createSphere(startingX, startingZ);

        //Create starting and target areas
        createStartingArea(startingX, startingZ);
        createTargetArea(startingX, startingZ);
        createWaypoint();
    }

    // Update is called once per frame
    // Purpose: check for detection of input, anything to be changed
    // or adjusted for non-physics objects, simple timers
    void Update()
    {
        //Debug.Log("GameLogic.cs");
        //Reset sphere if needed
        if (Input.GetKey("space"))
        {
            resetSphere();
        }

        //Wapyoint color
        if (passedWaypoint)
        {
            //Make opaque
            waypointMeshRenderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.0f, 1.0f));
            waypointMeshRenderer.material.SetFloat("_Mode", 3);
            waypointMeshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            waypointMeshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            waypointMeshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            waypointMeshRenderer.material.renderQueue = 3000;
        }
        else
        {
            //Keep translucent
            waypointMeshRenderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.0f, 0.5f));
            waypointMeshRenderer.material.SetFloat("_Mode", 3);
            waypointMeshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            waypointMeshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            waypointMeshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            waypointMeshRenderer.material.renderQueue = 3000;
        }
    }

    // FixedUpdate is called once every physics step
    // Purpose: Physics calcuations, adjusting physics/rigidbody objects
    void FixedUpdate()
    {
        //Check Progress of trial
        checkTrialProgress();

        //Sphere Pose
        spherePosition = sphere.transform.position;
        sphereOrientation = sphere.transform.eulerAngles;

        //Finger pose
        /*In Unity global units (may not be same as listed under trakSTAR Gameobject*/
        indexPosition = indexSphere.transform.position;
        indexOrientation = indexSphere.transform.eulerAngles;
        thumbPosition = thumbSphere.transform.position;
        thumbOrientation = thumbSphere.transform.eulerAngles;

        //Distance between centers of finger and sphere
        indexDistToCenter = Vector3.Magnitude(indexPosition - spherePosition);
        thumbDistToCenter = Vector3.Magnitude(thumbPosition - spherePosition);

        //Penetration of fingers into sphere
        indexToSpherePenetration = 0.5f * (indexScaleValue + sphereScaleValue) - indexDistToCenter;
        thumbToSpherePenetration = 0.5f * (thumbScaleValue + sphereScaleValue) - thumbDistToCenter;

        //Force Calculation
        indexForce = calculateForce(indexToSpherePenetration);
        thumbForce = calculateForce(thumbToSpherePenetration);

        //Derive Position Command from force calculation
        indexPositionCommand = getPositionCommand(indexForce);
        thumbPositionCommand = getPositionCommand(thumbForce);

        /*****************************************************************************************/

        //Check if sphere is inside of targetArea by calculating 
        //the distance between target area center and sphere center
        targetToSphereDist = Vector3.Magnitude(targetAreaPosition - spherePosition);

        //Check if the sphere passed through the waypoint
        checkWaypoint();

        //Check if sphere is in target
        checkIsSphereInTarget();

        //Determine successes/fails
       // trialLogic();

    }

    public void checkTrialProgress()
    {
        //Check progress on numMovesInTrial
        if (successCounter >= numElapsedTimes)
        {
            //move on to next trial
            trialNumber++;
            successCounter = 0;

            //end expriment
            if (trialNumber > numTrials)
            {
                //Now the user is done with these tasks
                trialNumber = 0;

                Debug.Log("EXPERIMENT OVER!!!");
            }
            //create next trial
            else
            {
                //Destroy old objects
                Destroy(startingArea);
                Destroy(targetArea);
                Destroy(waypoint);

                //create new start, target, and waypoint with increased spacing
                createStartingArea(startingX, startingZ + 0.5f * targetOffset);
                createTargetArea(startingX, startingZ - 0.5f * targetOffset);
                createWaypoint();
            }
        }
    }

    public void trialLogic()
    {
        // Look at after the sphere has been picked up and the fingers have moved sufficiently away
        // from the sphere
        //if (heldSphereBefore == true && palmToSphereDist > HOLD_THRESHOLD)
        //{
            // if we are also in the target AND passed the waypoint we succeed
            if (isSphereInTarget == true && passedWaypoint == true)
            {
                // SUCCESS!!!
                //Timing:
                timeOfCurrentSuccess = Time.time;
                elapsedTimes[timeIndex] = timeOfCurrentSuccess - timeSinceLastSuccess;
                timeIndex++;

                timeSinceLastSuccess = timeOfCurrentSuccess;

                sphereMeshRenderer.material.color = Color.green;
                successCounter++;
                Debug.Log("SUCCESS!!!");

                //Wait for 3 seconds
                //Thread.Sleep(3000);

                //Reset Sphere
                resetSphere();
            }
            // if one or both conditions fail, we missed/dropped
            else
            {
                sphereMeshRenderer.material.color = Color.red;
                failCounter++;
                Debug.Log("FAIL!!!");

                //Wait for 3 seconds
                //Thread.Sleep(3000);

                //Reset Sphere
                resetSphere();
            }
        //}
        // else {do nothing the sphere hasn't been picked up yet}
    }

    //Create Sphere GameObject:
    public void createSphere(float startingX, float startingZ)
    {
        //Sphere Object Definition
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //Name:
        sphere.name = "Sphere";

        //Set Size and Initial Position
        sphere.transform.localScale = new Vector3(sphereScaleValue, sphereScaleValue, sphereScaleValue);
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

    public void resetSphere()
    {
        //Reset device:
        indexForce = calculateForce(0.0f);
        thumbForce = calculateForce(0.0f);

        //Wait for 0.5 seconds
        Thread.Sleep(500);

        //Destory the old sphere game object
        Destroy(sphere);

        //reset hold condition since this is now a new object
        heldSphereBefore = false;

        //reset waypoint check
        passedWaypoint = false;

        //reset sphere-target check
        isSphereInTarget = false;

        //create the new object in starting positition
        createSphere(startingAreaPosition.x, startingAreaPosition.z);
    }

    public void createStartingArea(float startingX, float startingZ)
    {
        //Starting Area Object Definition
        startingArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        //Name:
        startingArea.name = "StartingArea";

        //Set Size and Initial Position
        startingAreaHeight = sphereCollider.radius * sphereScaleValue;
        startingArea.transform.localScale = new Vector3(startingAreaRadius, startingAreaHeight, startingAreaRadius);
        startingAreaPosition = new Vector3(startingX, startingAreaHeight, startingZ);
        startingArea.transform.position = startingAreaPosition;

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
        targetAreaHeight = sphereCollider.radius * sphereScaleValue;
        targetArea.transform.localScale = new Vector3(targetAreaRadius, targetAreaHeight, targetAreaRadius);
        targetAreaPosition = new Vector3(startingX, targetAreaHeight, startingZ - targetOffset);
        targetArea.transform.position = targetAreaPosition;

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

    public void createWaypoint()
    {
        waypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //Name:
        waypoint.name = "Waypoint";

        //Set Size and Initial Position
        waypoint.transform.localScale = new Vector3(waypointRadius, waypointRadius, waypointRadius);
        waypoint.transform.position = new Vector3(0.0f, waypointCenterHeight, 0.5f * (startingArea.transform.position.z + targetArea.transform.position.z));

        //This sets the Collider radius when the GameObject collides with a trigger Collider
        waypointCollider = waypoint.GetComponent<SphereCollider>();
        waypointCollider.enabled = false;

        //Set mesh properties
        waypointMeshRenderer = waypoint.GetComponent<MeshRenderer>();

        //Set Color and Transparency
        #region WaypointColor
        waypointMeshRenderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.0f, 0.5f));
        waypointMeshRenderer.material.SetFloat("_Mode", 3);
        waypointMeshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        waypointMeshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        waypointMeshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        waypointMeshRenderer.material.renderQueue = 3000;
        #endregion WaypointColor

        //Set Rigid Body dynamics
        rigidWaypoint = waypoint.AddComponent<Rigidbody>();
        rigidWaypoint.isKinematic = false;
        rigidWaypoint.useGravity = false;

        /***Lock location and orientation***/
        rigidWaypoint.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    public bool checkWaypoint()
    {
        //check if sphere is in waypoint
        if (Vector3.Magnitude(waypoint.transform.position - spherePosition) < 0.5f * waypointRadius - 0.5f * sphereScaleValue)
        {
            passedWaypoint = true;
        }
        else
        {
            //passedWaypoint = false;
        }

        return passedWaypoint;
    }

    public bool checkIsSphereInTarget()
    {
        if ((targetToSphereDist <= 0.5f * targetAreaRadius - 0.5f * sphereScaleValue) && (spherePosition.y <= 0.5f * sphereScaleValue))
        {
            isSphereInTarget = true;
        }
        else
        {
            isSphereInTarget = false;
        }
        return isSphereInTarget;
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
