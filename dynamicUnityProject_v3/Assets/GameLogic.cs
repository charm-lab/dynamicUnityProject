using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class GameLogic : MonoBehaviour
{
    #region
    GameObject indexSphere;
    GameObject thumbSphere;
    GameObject trakSTAROrigin;

    [Header("Index Variables")]
    public Vector3 indexPosition;
    public Vector3 indexVelocity;
    public Vector3 indexOrientation;
    public Vector3 indexPenetration;
    public Vector3 indexPenetrationForce;
    public Vector3 indexShearForce;
    public Vector3 indexForce;
    public float indexDistToCenter;
    public float indexPenetrationMagSign;
    public float indexScaleValue = 0.02f;  //m
    Vector3 indexPositionPrev;
    Vector3 indexScaling;

    [Header("Thumb Variables")]
    public Vector3 thumbPosition;
    public Vector3 thumbVelocity;
    public Vector3 thumbOrientation;
    public Vector3 thumbPenetration;
    public Vector3 thumbPenetrationForce;
    public Vector3 thumbShearForce;
    public Vector3 thumbForce;
    public float thumbDistToCenter;
    public float thumbPenetrationMagSign;
    public float thumbScaleValue = 0.02f; //m
    Vector3 thumbPositionPrev;
    Vector3 thumbScaling;

    /**** Create SPHERE *****/
    GameObject sphere; //Instatiate Sphere GameObject
    MeshRenderer sphereMeshRenderer; //Declares Mesh Renderer
                                     //Sphere Scaling and Stiffness
    Vector3 sphereScaling;

    [Header("Sphere Variables")]
    public Vector3 spherePosition;
    public Vector3 sphereVelocity;
    public Vector3 sphereAcceleration;
    public Vector3 sphereForce;
    public Vector3 sphereOrientation;
    public float sphereScaleValue = 0.05f; //m
    public float sphereMass = 0.5f; //kg
    public float sphereStiffness = 500.0f; //in N/m
    public float sphereWeight; //N scalar
    [Range(1.0f, 500.0f)]
    public float sphereDamping = 10.0f; //in N/m/s
    [Range(1.0f, 500.0f)]
    public float fingerFrictionCoeff = 50.0f; //N scalar
    [Range(1.0f, 500.0f)]
    public float fingerDamping = 50.0f; //in N/m/s
    float stribeckVelocity = 0.1f; // in m/s
    float uS = 0.5f; // Coefficient of static friction sphere-finger [-]
    float uK = 0.2f; // Coefficient of kinetic friction sphere-finger [-]

    [Header("Position Commands to Arduino")]
    public float dorsalCommand;
    public float ventralCommand;

    [Header("Floor Variables")]
    public float floorPenetration;
    public float floorStiffness = 5000; // N/m
    public Vector3 floorNormalForce; //Strictly the normal from interacting with the floor

    Vector3[] forceValues;
    float[] positionCommands;

    /**** Create STARTINGAREA *****/
    GameObject startingArea; //Instatiate StartingArea GameObject
    MeshRenderer startingAreaMeshRenderer; //Declares Mesh Renderer

    [Header("Starting Area Variables")]
    public Vector3 startingAreaPosition;
    public float startingAreaRadius = 0.2f;
    public float startingAreaHeight;
    float startingX = 0.3f;
    float startingZ = 0.2f;
    float targetOffset = 0.40f;

    /**** Create TARGETAREA *****/
    GameObject targetArea; //Instatiate targetArea GameObject
    MeshRenderer targetAreaMeshRenderer; //Declares Mesh Renderer

    [Header("Target Area Variables")]
    public Vector3 targetAreaPosition;
    public float targetToSphereDist; //Distance between target and sphere
    public float targetAreaRadius = 0.2f;
    public float targetAreaHeight;
    public bool isSphereInTarget = false; //Sphere in target boolean

    /**** Create WAYPOINT *****/
    GameObject waypoint; //Instatiate waypoint GameObject
    MeshRenderer waypointMeshRenderer; //Declares Mesh Renderer

    [Header("Waypoint Variables")]
    public float waypointCenterHeight = 0.15f; //Height of waypoint relative to floor
    public float waypointRadius = 0.15f;
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
    public int numTrials = 4; // the total number of trials the user will participate in
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

    LineRenderer indexLineRenderer;
    LineRenderer thumbLineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        print("BUT DID YOU **TURN ON SAMPLE**???");

        indexSphere = GameObject.Find("trakSTAR/Index Sphere");
        thumbSphere = GameObject.Find("trakSTAR/Thumb Sphere");

        trakSTAROrigin = GameObject.Find("trakSTAR/trakSTAR Origin");
        trakSTAROrigin.transform.position = Vector3.zero;

        setObjectColor(indexSphere.GetComponent<MeshRenderer>(), 1.0f, 0.0f, 0.0f, 0.5f);
        setObjectColor(thumbSphere.GetComponent<MeshRenderer>(), 0.0f, 1.0f, 1.0f, 0.5f);

        trakSTAROrigin.GetComponent<MeshRenderer>().material.color = Color.magenta;

        indexScaling = new Vector3(indexScaleValue, indexScaleValue, indexScaleValue);
        indexSphere.transform.localScale = indexScaling;
        thumbScaling = new Vector3(thumbScaleValue, thumbScaleValue, thumbScaleValue);
        thumbSphere.transform.localScale = thumbScaling;

        //Set up timing saving
        elapsedTimes = new float[numElapsedTimes * numTrials];

        //Create the sphere Game Object
        createSphere(startingX, startingZ);

        //Create starting and target areas
        createStartingArea(startingX, startingZ);
        createTargetArea(startingX, startingZ);
        createWaypoint(startingX);

        //Variable initializations
        indexVelocity = Vector3.zero;
        thumbVelocity = Vector3.zero;
        indexPositionPrev = Vector3.zero;
        thumbPositionPrev = Vector3.zero;
        forceValues = new Vector3[] { Vector3.zero, Vector3.zero };
        positionCommands = new float[] { 0.0f, 0.0f };
        floorNormalForce = Vector3.zero;
        trialNumber = 1;

        //create Shear Velocity Vector lines
        indexLineRenderer = indexSphere.AddComponent<LineRenderer>();
        thumbLineRenderer = thumbSphere.AddComponent<LineRenderer>();
    }

    // Update is called once per frame
    // Purpose: check for detection of input, anything to be changed
    // or adjusted for non-physics objects, simple timers
    void Update()
    {
        //Debug.Log("GameLogic.cs");
        //Reset sphere if needed
        if (Input.GetKeyDown("space"))
        {
            resetSphere();
        }
        //Manually Change Trial Number
        if (Input.GetKeyDown("up"))
        {
            trialNumber++;
        }
        if (Input.GetKeyDown("down"))
        {
            trialNumber--;
        }

        //Wapyoint color
        if (passedWaypoint)
        {
            //Make opaque
            setObjectColor(waypointMeshRenderer, 0.5f, 0.5f, 0.0f, 1.0f);
        }
        else
        {
            //Keep translucent
            setObjectColor(waypointMeshRenderer, 0.5f, 0.5f, 0.0f, 0.5f);
        }
    }

    // FixedUpdate is called once every physics step
    // Purpose: Physics calcuations, adjusting physics/rigidbody objects
    void FixedUpdate()
    {
        //Check Progress of trial
        checkTrialProgress();

        //Finger pose
        /*In Unity global units (may not be same as listed under trakSTAR Gameobject*/
        indexPosition = indexSphere.transform.position;
        indexOrientation = indexSphere.transform.eulerAngles;
        thumbPosition = thumbSphere.transform.position;
        thumbOrientation = thumbSphere.transform.eulerAngles;

        //Finger Velocity
        indexVelocity = (indexPosition - indexPositionPrev) / Time.fixedDeltaTime;
        thumbVelocity = (thumbPosition - thumbPositionPrev) / Time.fixedDeltaTime;

        //Distance between centers of finger and sphere
        indexDistToCenter = Vector3.Magnitude(indexPosition - spherePosition);
        thumbDistToCenter = Vector3.Magnitude(thumbPosition - spherePosition);

        //Magnitude and sign of Penetration of fingers into sphere
        //Positive --> Inside sphere | Negative --> Outside sphere
        indexPenetrationMagSign = 0.5f * (indexScaleValue + sphereScaleValue)
                                    - indexDistToCenter;
        thumbPenetrationMagSign = 0.5f * (thumbScaleValue + sphereScaleValue)
                                    - thumbDistToCenter;

        //Vector of Penetration of fingers into sphere
        indexPenetration = indexPenetrationMagSign * (indexPosition - spherePosition) / indexDistToCenter;
        thumbPenetration = thumbPenetrationMagSign * (thumbPosition - spherePosition) / thumbDistToCenter;

        //Force Calculation
        forceValues = calculateFingerForceValues(indexPenetration, thumbPenetration,
            indexPenetrationMagSign, thumbPenetrationMagSign);
        indexForce = forceValues[0];
        thumbForce = forceValues[1];
        floorNormalForce = calculateFloorNormalForce();

        //Sphere Pose
        sphere.transform.position = getSpherePosition(indexForce, thumbForce, floorNormalForce);
        //sphereOrientation = sphere.transform.eulerAngles;

        //Derive Position Command from force calculation and assign 
        //positionCommands = getFingerPositionCommands(Vector3.Magnitude(indexPenetrationForce), Vector3.Magnitude(thumbPenetrationForce));
        positionCommands = getFingerPositionCommands(Vector3.Magnitude(indexForce), Vector3.Magnitude(thumbForce));
        dorsalCommand = positionCommands[0];
        ventralCommand = positionCommands[1];

        //set prev position
        indexPositionPrev = indexPosition;
        thumbPositionPrev = thumbPosition;

        /*****************************************************************************************/

        //Check if sphere is inside of targetArea by calculating 
        //the distance between target area center and sphere center
        targetToSphereDist = Vector3.Magnitude(targetAreaPosition - spherePosition);

        /**state machine for trials**/

        //Determine successes/fails
        //trialLogic();
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
                trialNumber = -1;

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
                createWaypoint(startingX);
                resetSphere();
            }
        }
    }

    public void trialLogic()
    {
        //Check if the sphere passed through the waypoint
        checkWaypoint();

        //Check if sphere is in target
        checkIsSphereInTarget();

    }

    //Create Sphere GameObject:
    public void createSphere(float startingX, float startingZ)
    {
        //Sphere Object Definition
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(sphere.GetComponent<SphereCollider>());

        //Name:
        sphere.name = "Sphere";

        //Set Size and Initial Position/Velocity
        sphereScaling = new Vector3(sphereScaleValue, sphereScaleValue, sphereScaleValue);
        sphere.transform.localScale = sphereScaling;
        sphere.transform.position = new Vector3(startingX, 0.5f * sphereScaleValue, startingZ);
        spherePosition = sphere.transform.position;
        sphereVelocity = Vector3.zero;
        sphereAcceleration = Vector3.zero;

        //Set Sphere mesh properties
        sphereMeshRenderer = sphere.GetComponent<MeshRenderer>();
        setObjectColor(sphereMeshRenderer, 0.0f, 0.0f, 1.0f, 0.5f);

        sphereWeight = sphereMass * Vector3.Magnitude(Physics.gravity);
    }

    public void resetSphere()
    {
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
        Destroy(startingArea.GetComponent<CapsuleCollider>());

        //Name:
        startingArea.name = "StartingArea";

        //Set Size and Initial Position
        startingAreaHeight = 0.5f * sphereScaleValue;
        startingArea.transform.localScale = new Vector3(startingAreaRadius, startingAreaHeight, startingAreaRadius);
        startingAreaPosition = new Vector3(startingX, startingAreaHeight, startingZ);
        startingArea.transform.position = startingAreaPosition;

        //Set mesh properties
        startingAreaMeshRenderer = startingArea.GetComponent<MeshRenderer>();

        //Set Color and Transparaency
        setObjectColor(startingAreaMeshRenderer, 0.5f, 0.0f, 1.0f, 0.5f);

        //Hide -- TEMPORARY
        startingAreaMeshRenderer.enabled = false;
    }

    public void createTargetArea(float startingX, float startingZ)
    {
        //Target area Object Definition
        targetArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(targetArea.GetComponent<CapsuleCollider>());

        //Name:
        targetArea.name = "TargetArea";

        //Set Size and Initial Position
        targetAreaHeight = 0.5f * sphereScaleValue;
        targetArea.transform.localScale = new Vector3(targetAreaRadius, targetAreaHeight, targetAreaRadius);
        targetAreaPosition = new Vector3(startingX, targetAreaHeight, startingZ - targetOffset);
        targetArea.transform.position = targetAreaPosition;

        //Set mesh properties
        targetAreaMeshRenderer = targetArea.GetComponent<MeshRenderer>();

        //Set Color and Transparency
        setObjectColor(targetAreaMeshRenderer, 0.4f, 0.6f, 0.1f, 0.5f);
    }

    public void createWaypoint(float startingX)
    {
        waypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(waypoint.GetComponent<SphereCollider>());

        //Name:
        waypoint.name = "Waypoint";

        //Set Size and Initial Position
        waypoint.transform.localScale = new Vector3(waypointRadius, waypointRadius, waypointRadius);
        waypoint.transform.position = new Vector3(startingX, waypointCenterHeight, 0.5f * (startingArea.transform.position.z + targetArea.transform.position.z));

        //Set mesh properties
        waypointMeshRenderer = waypoint.GetComponent<MeshRenderer>();

        //Set Color and Transparency
        setObjectColor(waypointMeshRenderer, 0.5f, 0.5f, 0.0f, 0.5f);
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

    public Vector3[] calculateFingerForceValues(Vector3 indexPenetration, Vector3 thumbPenetration,
        float indexPenetrationMagSign, float thumbPenetrationMagSign)
    { 
        Vector3 indexForceVal;
        Vector3 thumbForceVal;

        Vector3 indexRelVelocity = indexVelocity - sphereVelocity; //sphereVelocity - indexVelocity;
        Vector3 thumbRelVelocity = thumbVelocity - sphereVelocity; //sphereVelocity - thumbVelocity;

        Vector3 indexShearVelocity = getShearVelocity(indexRelVelocity, indexPenetration);
        Vector3 thumbShearVelocity = getShearVelocity(thumbRelVelocity, thumbPenetration);

        if (indexPenetrationMagSign <= 0.0f)
        {
            indexPenetrationForce = Vector3.zero;
            indexShearForce = Vector3.zero;
            indexForceVal = Vector3.zero;
        }
        else 
        {
            //Use Hooke's Law to find penetration force of sphere on finger
            indexPenetrationForce = sphereStiffness * indexPenetration;

            //Add shear forces of sphere on finger due to friction
            indexShearForce = fingerDamping * indexShearVelocity/**/;
            //getFrictionForce(indexRelVelocity, stribeckVelocity, uK * indexPenetrationForce, uS * indexPenetrationForce);
            //fingerDamping * indexRelVelocity + fingerFrictionCoeff * Vector3.Magnitude(indexPenetrationForce) * sign(indexRelVelocity)/**/;

            drawShearVelocityVector(indexShearVelocity, indexPosition - spherePosition, 
                0.5f * sphereScaleValue, 0.5f * indexScaleValue, indexLineRenderer);

            //Sum of forces of sphere on finger
            //indexForceVal = indexPenetrationForce + indexShearForce;
            indexForceVal = indexPenetrationForce;
        }

        if (thumbPenetrationMagSign <= 0.0f)
        {
            thumbPenetrationForce = Vector3.zero;
            thumbShearForce = Vector3.zero;
            thumbForceVal = Vector3.zero;
        }
        else
        {
            //Use Hooke's Law to find force of sphere on finger
            thumbPenetrationForce = sphereStiffness * thumbPenetration;

            //Add shear forces of sphere on finger due to friction
            thumbShearForce = fingerDamping * thumbShearVelocity/**/;
            //getFrictionForce(thumbRelVelocity, stribeckVelocity, uK * thumbPenetrationForce, uS * thumbPenetrationForce);
            //fingerDamping * thumbRelVelocity + fingerFrictionCoeff * Vector3.Magnitude(thumbPenetrationForce) * sign(thumbRelVelocity)/**/;
            
            drawShearVelocityVector(thumbShearVelocity, thumbPosition - spherePosition,
                0.5f * sphereScaleValue, 0.5f * thumbScaleValue, thumbLineRenderer);

            //Sum of forces of sphere on finger
            //thumbForceVal = thumbPenetrationForce + thumbShearForce;
            thumbForceVal = thumbPenetrationForce;
        }

        //Finger Force on sphere
        forceValues[0] = -indexForceVal;
        forceValues[1] = -thumbForceVal;
        return forceValues;
    }

    public Vector3 calculateFloorNormalForce()
    {
        if (spherePosition.y < 0.5f * sphereScaleValue)
        {
            floorPenetration = 0.5f * sphereScaleValue - spherePosition.y;
        }
        else
        {
            floorPenetration = 0.0f;
        }
        return new Vector3(0.0f, sphereStiffness * floorPenetration, 0.0f);

        #region
        /*
        //No contact with floor
        if (spherePosition.y > 0.5f * sphereScaleValue)
        {
            floorPenetration = 0.0f;
            return Vector3.zero;
        }
        //Touching ground but no penetration into floor
        else if (spherePosition.y == 0.5f * sphereScaleValue)
        {
            floorPenetration = 0.0f;
            return new Vector3(0.0f, (floorStiffness * floorPenetration) + sphereWeight, 0.0f);
        }
        //Sphere penetrates floor but some portion is above floor
        else if (spherePosition.y <= 0.5f * sphereScaleValue && spherePosition.y >= -0.5f * sphereScaleValue)
        {
            floorPenetration = 0.5f * sphereScaleValue - spherePosition.y;
            return new Vector3(0.0f, (floorStiffness * floorPenetration) + sphereWeight, 0.0f);
        }
        //sphere has completely broken through floor and normal should be practically infinite
        else
        {
            floorPenetration = -spherePosition.y + 0.5f * sphereScaleValue;
            return new Vector3(0.0f, (floorStiffness * floorPenetration) + sphereWeight, 0.0f);
        }
        */
        #endregion
    }

    public Vector3 getSpherePosition(Vector3 indexForce, Vector3 thumbForce, Vector3 floorNormalForce)
    {
        Vector3 positionPrev = spherePosition;
        Vector3 velocityPrev = sphereVelocity;
        Vector3 accelerationPrev = sphereAcceleration;

        sphereAcceleration = Physics.gravity + (indexForce + thumbForce + floorNormalForce - sphereDamping * velocityPrev) / sphereMass;
        sphereForce = sphereMass * sphereAcceleration;

        sphereVelocity = velocityPrev + sphereAcceleration * Time.fixedDeltaTime;
        spherePosition = positionPrev + sphereVelocity * Time.fixedDeltaTime;

        return spherePosition;
    }

    public float[] getFingerPositionCommands(float indexForceMag, float thumbForceMag)
    {
        float dorsalVal;
        float ventralVal;

        /*Condition 0: No Haptic Feebdack*/
        if (trialNumber == 0)
        {
            dorsalVal = 0.0f;
            ventralVal = 0.0f;
        }

        /*Condition 1: Index --> Dorsal | Thumb --> Ventral*/
        else if (trialNumber == 1)
        {
            dorsalVal = 1000 * (indexForceMag / sphereStiffness);
            ventralVal = 1000 * (thumbForceMag / sphereStiffness);
        }

        /*Condition 2: Index --> Ventral | Thumb --> Dorsal*/
        else if (trialNumber == 2)
        {
            dorsalVal = 1000 * (thumbForceMag / sphereStiffness);
            ventralVal = 1000 * (indexForceMag / sphereStiffness);
        }

        /*Condition 3: Single Tactor Feedback to Index Finger on Dorsal Side*/
        else if (trialNumber == 3)
        {
            dorsalVal = 1000 * (indexForceMag / sphereStiffness);
            ventralVal = 0.0f;
        }

        /*Condition 4: Average of both finger forces to single tactor on dorsal side*/
        else if (trialNumber == 4)
        {
            dorsalVal = 500.0f * (indexForceMag + thumbForceMag) / sphereStiffness;
            ventralVal = 0.0f;
        }

        /*Experiment over*/
        else
        {
            dorsalVal = 0.0f;
            ventralVal = 0.0f;
        }

        /*penetration values in mm*/
        positionCommands[0] = dorsalVal;
        positionCommands[1] = ventralVal;
        return positionCommands;
    }

    public Vector3 getFrictionForce(Vector3 fingerRelVelocity, float stribeckVelocity, Vector3 coulombFrictionForce, Vector3 staticFrictionForce)
    {
        Vector3 sgn_vT = sign(fingerRelVelocity);
        float vT = Vector3.Magnitude(fingerRelVelocity);
        float vS = stribeckVelocity;
        float Fc = Vector3.Magnitude(coulombFrictionForce);
        float Fs = Vector3.Magnitude(staticFrictionForce);

        float Ff_Mag = Fc * (float)Math.Tanh((double)(4 * vT / vS))
            + (Fs - Fc) * (vT / vS) / (Mathf.Pow(0.25f * Mathf.Pow((vT / vS), 2.0f) + 0.75f, 2.0f));

        Vector3 Ff = Ff_Mag * sgn_vT;
        return Ff;
    }

    public Vector3 getShearVelocity(Vector3 vRel, Vector3 penetration)
    {
        //vector normal to the plane of the instersceting spheres
        Vector3 normal = penetration;
        float normalMag = Vector3.Magnitude(normal);

        Vector3 shearVelocity = -Vector3.Cross(normal, Vector3.Cross(vRel, normal)) / Mathf.Pow(normalMag, 2.0f);

        return shearVelocity;
    }

    public void drawShearVelocityVector(Vector3 shearVelocity, Vector3 distanceVec, float sphereRadius, float fingerRadius, LineRenderer fingerLine)
    {
        //vector distance between centers instersceting spheres
        float distanceMag = Vector3.Magnitude(distanceVec);
        Vector3 distanceVec_hat = distanceVec / Vector3.Magnitude(distanceVec);

        float sphereCenterToIntersectionPoint = (Mathf.Pow(sphereRadius, 2.0f) - Mathf.Pow(fingerRadius, 2.0f) + Mathf.Pow(distanceMag, 2.0f))
                                                                        / (2.0f * distanceMag);
        Vector3 intersectionPoint = spherePosition + sphereCenterToIntersectionPoint * distanceVec_hat;
        
        fingerLine.useWorldSpace = true;
        fingerLine.SetPosition(0, intersectionPoint);
        fingerLine.SetPosition(1, intersectionPoint + shearVelocity);
        fingerLine.SetWidth(0.01f, 0.01f);

    }

    public Vector3 sign(Vector3 x)
    {
        if (Vector3.Magnitude(x) != 0.0f)
        {
            return x / Vector3.Magnitude(x);
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void setObjectColor(MeshRenderer mesh, float r, float g, float b, float alpha)
    {
        mesh.material.SetColor("_Color", new Color(r, g, b, alpha));
        mesh.material.SetFloat("_Mode", 3);
        mesh.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mesh.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mesh.material.EnableKeyword("_ALPHABLEND_ON");
        mesh.material.renderQueue = 3000;
    }
}
