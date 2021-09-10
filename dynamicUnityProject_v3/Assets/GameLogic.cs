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
    GameObject middleSphere;
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
    public float indexDiameter = 0.02f;  //m
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

    [Header("Middle Variables")]
    public Vector3 middlePosition;
    public Vector3 middleVelocity;
    public Vector3 middleOrientation;
    public Vector3 middlePenetration;
    public Vector3 middlePenetrationForce;
    public Vector3 middleShearForce;
    public Vector3 middleForce;
    public float middleDistToCenter;
    public float middlePenetrationMagSign;
    public float middleScaleValue = 0.02f; //m
    Vector3 middlePositionPrev;
    Vector3 middleScaling;

    /**** Create SPHERE *****/
    GameObject cube; //Instatiate Cube GameObject
    MeshRenderer cubeMeshRenderer; //Declares Mesh Renderer
                                   //Cube Scaling and Stiffness
    Vector3 cubeScaling;

    [Header("Cube Variables")]
    public Vector3 cubePosition;
    public Vector3 cubeVelocity;
    public Vector3 cubeAcceleration;
    public Vector3 cubeForce;
    public Vector3 cubeOrientation;
    public float cubeLength = 0.05f; //m
    public float cubeMass = 0.3f; //kg
    public float cubeStiffness = 500.0f; //in N/m
    public float cubeWeight; //N scalar
    [Range(1.0f, 500.0f)]
    public float cubeDamping = 10.0f; //in N/m/s
    [Range(0.0f, 100.0f)]
    public float fingerDamping = 10.0f; //in N/m/s
    [Range(0.0f, 100.0f)]
    public float uK = 5.0f; // Coefficient of kinetic friction cube-finger [-]

    [Header("Position Commands to Arduino")]
    public float dorsalCommand;
    public float ventralCommand;

    [Header("Floor Variables")]
    public float floorPenetration;
    public float floorStiffness = 5000; // N/m
    public Vector3 floorNormalForce; //Strictly the normal from interacting with the floor

    public Vector3[] cubeStatus;
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
    public float targetToCubeDist; //Distance between target and cube
    public float targetAreaRadius = 0.2f;
    public float targetAreaHeight;
    public bool isCubeInTarget = false; //Cube in target boolean

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
    public bool middleContact;
    public bool heldCubeBefore = false;

    /**** Trial Info *****/
    [Header("Trial Variables")]
    public int trialNumber = 1; //the actual trial being worked on
    public int trialState = 0; //the actual trial being worked on
    public int numTrials = 4; // the total number of trials the user will participate in
    public int numElapsedTimes = 3; //num of attempts for each trial

    public int successCounter = 0; //number of successful moves within a trial
    public int failCounter = 0;

    /**** Timing *****/
    //use task success/fails
    [Header("Timing")]
    public float timeOfCurrentSuccess = 0.0f;
    public float timeSinceLastSuccess = 0.0f;

    public int timeIndex = 0;
    public float[] elapsedTimes;

    LineRenderer indexLineRenderer;
    LineRenderer thumbLineRenderer;
    LineRenderer middleLineRenderer;

    #endregion

    LineRenderer cubeLineRenderer;
    LineRenderer cubeRadialLineRenderer;
    LineRenderer penetrationLineRenderer;

    public Vector3 indexCubeVec;
    public Vector3 cubeAxisX;
    public Vector3 cubeAxisY;
    public Vector3 cubeAxisZ;

    //Cube Wall IDs:
    public int entryWallIndex;
    public int none = 0;
    public int cubeRight = 1;
    public int cubeLeft = 2;
    public int cubeUp = 3;
    public int cubeDown = 4;
    public int cubeFront = 5;
    public int cubeBack = 6;
    bool isPenetrating;


    // Start is called before the first frame update
    void Start()
    {
        print("BUT DID YOU **TURN ON SAMPLE**???");

        indexSphere = GameObject.Find("trakSTAR/Index Sphere");
        thumbSphere = GameObject.Find("trakSTAR/Thumb Sphere");
        middleSphere = GameObject.Find("trakSTAR/Middle Sphere");

        trakSTAROrigin = GameObject.Find("trakSTAR/trakSTAR Origin");
        trakSTAROrigin.transform.position = Vector3.zero;

        setObjectColor(indexSphere.GetComponent<MeshRenderer>(), 1.0f, 0.0f, 0.0f, 0.5f);
        setObjectColor(thumbSphere.GetComponent<MeshRenderer>(), 0.0f, 1.0f, 1.0f, 1.0f);
        setObjectColor(middleSphere.GetComponent<MeshRenderer>(), 1.0f, 0.647f, 0.0f, 1.0f);

        trakSTAROrigin.GetComponent<MeshRenderer>().material.color = Color.magenta;

        indexScaling = new Vector3(indexDiameter, indexDiameter, indexDiameter);
        indexSphere.transform.localScale = indexScaling;
        thumbScaling = new Vector3(thumbScaleValue, thumbScaleValue, thumbScaleValue);
        thumbSphere.transform.localScale = thumbScaling;
        middleScaling = new Vector3(middleScaleValue, middleScaleValue, middleScaleValue);
        middleSphere.transform.localScale = middleScaling;

        //Set up timing saving
        elapsedTimes = new float[numElapsedTimes * numTrials];

        //Create the cube Game Object
        createCube(startingX, startingZ);

        //Create starting and target areas
        createStartingArea(startingX, startingZ);
        createTargetArea(startingX, startingZ);
        createWaypoint(startingX);

        //Variable initializations
        indexVelocity = Vector3.zero; indexCubeVec = Vector3.zero;
        thumbVelocity = Vector3.zero;
        middleVelocity = Vector3.zero;

        indexPositionPrev = Vector3.zero;
        thumbPositionPrev = Vector3.zero;
        middlePositionPrev = Vector3.zero;
        cubePositionPrev = Vector3.zero;

        cubeStatus = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        forceValues = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };

        positionCommands = new float[] { 0.0f, 0.0f };
        floorNormalForce = Vector3.zero;
        trialNumber = 1;

        //create Shear Velocity Vector lines
        indexLineRenderer = indexSphere.AddComponent<LineRenderer>();
        thumbLineRenderer = thumbSphere.AddComponent<LineRenderer>();
        middleLineRenderer = middleSphere.AddComponent<LineRenderer>();

        isPenetrating = false;
        entryWallIndex = none;
    }

    // Update is called once per frame
    // Purpose: check for detection of input, anything to be changed
    // or adjusted for non-physics objects, simple timers
    void Update()
    {
        //Debug.Log("GameLogic.cs");
        //Reset cube if needed
        if (Input.GetKeyDown("space"))
        {
            resetCube();
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
        #region
        //Check Progress of trial
        checkTrialProgress();

        //Finger pose
        /*In Unity global units (may not be same as listed under trakSTAR Gameobject*/
        indexPosition = indexSphere.transform.position;
        indexOrientation = indexSphere.transform.eulerAngles;
        thumbPosition = thumbSphere.transform.position;
        thumbOrientation = thumbSphere.transform.eulerAngles;
        middlePosition = middleSphere.transform.position;
        middleOrientation = middleSphere.transform.eulerAngles;

        //Finger Velocity
        indexVelocity = (indexPosition - indexPositionPrev) / Time.fixedDeltaTime;
        thumbVelocity = (thumbPosition - thumbPositionPrev) / Time.fixedDeltaTime;
        middleVelocity = (middlePosition - middlePositionPrev) / Time.fixedDeltaTime;
        #endregion

        //Distance Vector between centers of finger and cube
        indexCubeVec = indexPosition - cubePosition;  // Index-->Cube 

        //Distance Magnitude between centers of finger and cube
        indexDistToCenter = Vector3.Magnitude(indexCubeVec);
        thumbDistToCenter = Vector3.Magnitude(thumbPosition - cubePosition);
        middleDistToCenter = Vector3.Magnitude(middlePosition - cubePosition);

        //Magnitude and sign of Penetration of fingers into cube
        //Positive --> Inside cube | Negative --> Outside cube
        indexPenetrationMagSign = 0.5f * (indexDiameter + cubeLength)
                                    - indexDistToCenter;
        thumbPenetrationMagSign = 0.5f * (thumbScaleValue + cubeLength)
                                    - thumbDistToCenter;
        middlePenetrationMagSign = 0.5f * (middleScaleValue + cubeLength)
                                    - middleDistToCenter;
        
        entryWall = getEntryWall(indexDistToCenter, 0.5f * indexDiameter, indexVelocity);

        //Vector of Penetration of fingers into cube
        indexPenetration = ;
        thumbPenetration = thumbPenetrationMagSign * (thumbPosition - cubePosition) / thumbDistToCenter;
        middlePenetration = middlePenetrationMagSign * (middlePosition - cubePosition) / middleDistToCenter;

        //indexPenetrationMagSign = Vector3.Magnitude(indexPenetration) * sign(indexPenetration)
        //drawPenetrationVector( penetrationLineRenderer);

        #region
        //Force Calculation
        forceValues = calculateFingerForceValues(cubeVelocity, indexVelocity, thumbVelocity, middleVelocity,
            cubePosition, indexPosition, thumbPosition, middlePosition, indexPenetration, thumbPenetration, middlePenetration,
            indexPenetrationMagSign, thumbPenetrationMagSign, middlePenetrationMagSign);
        indexForce = forceValues[0];
        thumbForce = forceValues[1];
        middleForce = forceValues[2];
        floorNormalForce = calculateFloorNormalForce();

        //Cube Pose
        cubeStatus = getCubeStatus(cubePosition, cubeVelocity, cubeAcceleration,
                       indexForce, thumbForce, middleForce, floorNormalForce);
        cubePosition = cubeStatus[0];
        cube.transform.position = cubePosition;
        //cubeOrientation = cube.transform.eulerAngles;
        cubeVelocity = cubeStatus[1];
        cubeAcceleration = cubeStatus[2];

        //Derive Position Command from force calculation and assign 
        positionCommands = getFingerPositionCommands(Vector3.Magnitude(indexForce), Vector3.Magnitude(thumbForce));
        dorsalCommand = positionCommands[0];
        ventralCommand = positionCommands[1];

        //set prev position
        indexPositionPrev = indexPosition;
        thumbPositionPrev = thumbPosition;
        middlePositionPrev = middlePosition;
        cubePositionPrev = cubePosition;
        /*****************************************************************************************/

        //Check if cube is inside of targetArea by calculating 
        //the distance between target area center and cube center
        targetToCubeDist = Vector3.Magnitude(targetAreaPosition - cubePosition);

        #endregion

        //Determine successes/fails
        trialLogic();
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
                createStartingArea(startingX, startingZ + 0.1f * targetOffset);
                createTargetArea(startingX, startingZ - 0.1f * targetOffset);
                createWaypoint(startingX);
                resetCube();
            }
        }
    }

    public void trialLogic()
    {

        /*STATE 0: Before Cube Pickup*/
        if (trialState == 0)
        {
            //If cube has been picked up == contacted with all fingers & lifted
            if (indexContact == true && thumbContact == true && middleContact == true && (cubePosition.y > 0.03))
            {
                //move to next state
                trialState = 1;
            }
            //if too far from finger --> fail
            if (indexDistToCenter > 2.0f)
            {
                //Fail
                failCounter++;

                //Reset
                resetCube();
            }
        }

        /*STATE 1: Cube Pickup but before Passed Waypoint*/
        if (trialState == 1)
        {
            //if too far from finger --> fail (by drop)
            if (indexDistToCenter > 0.05f)
            {
                //Fail
                failCounter++;

                //Reset
                resetCube();
            }
            //if cube lands in target before passing waypoint --> fail (not followed path)
            if (checkIsCubeInTarget() == true)
            {
                //Fail
                failCounter++;

                //Reset
                resetCube();
            }
            //if passed waypoint
            if (checkWaypoint() == true)
            {
                //move to next state
                trialState = 2;
            }
        }

        /*STATE 2: After Passed Waypoint but before reaching target*/
        if (trialState == 2)
        {
            //if too far from finger --> fail (by drop)
            if (indexDistToCenter > 0.05f)
            {
                //Fail
                failCounter++;

                //Reset
                resetCube();
            }
            //if landed in target after passing waypoint
            if (checkIsCubeInTarget() == true)
            {
                //Success
                successCounter++;
                resetCube();
            }
        }


    }

    public void createCube(float startingX, float startingZ)
    {
        //Cube Object Definition
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(cube.GetComponent<BoxCollider>());

        //Name:
        cube.name = "Cube";

        //Set Size and Initial Position/Velocity
        cubeScaling = new Vector3(cubeLength, cubeLength, cubeLength);
        cube.transform.localScale = cubeScaling;
        cube.transform.position = new Vector3(startingX, 0.5f * cubeLength, startingZ);
        cubePosition = cube.transform.position;
        cubeVelocity = Vector3.zero;
        cubeAcceleration = Vector3.zero;

        //Define local axes
        cubeAxisX = cube.transform.right;
        cubeAxisY = cube.transform.up;
        cubeAxisZ = cube.transform.forward;

        //Set Cube mesh properties
        cubeMeshRenderer = cube.GetComponent<MeshRenderer>();
        setObjectColor(cubeMeshRenderer, 0.0f, 0.0f, 1.0f, 0.5f);

        cubeWeight = cubeMass * Vector3.Magnitude(Physics.gravity);

        // cubeLineRenderer = cube.AddComponent<LineRenderer>();
        penetrationLineRenderer = cube.AddComponent<LineRenderer>();
    }

    public void resetCube()
    {
        //Destory the old cube game object
        Destroy(cube);

        //reset hold condition since this is now a new object
        heldCubeBefore = false;

        //reset waypoint check
        passedWaypoint = false;

        //reset cube-target check
        isCubeInTarget = false;

        //create the new object in starting positition
        createCube(startingAreaPosition.x, startingAreaPosition.z);

        //Reset trial state
        trialState = 0;
    }

    public void createStartingArea(float startingX, float startingZ)
    {
        //Starting Area Object Definition
        startingArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(startingArea.GetComponent<CapsuleCollider>());

        //Name:
        startingArea.name = "StartingArea";

        //Set Size and Initial Position
        startingAreaHeight = 0.5f * cubeLength;
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
        targetAreaHeight = 0.5f * cubeLength;
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
        //check if cube is in waypoint
        if (Vector3.Magnitude(waypoint.transform.position - cubePosition) < 0.5f * waypointRadius - 0.5f * cubeLength)
        {
            passedWaypoint = true;
        }
        else
        {
            //passedWaypoint = false;
        }

        return passedWaypoint;
    }

    public bool checkIsCubeInTarget()
    {
        if ((targetToCubeDist <= 0.5f * targetAreaRadius - 0.5f * cubeLength) && (cubePosition.y <= 0.5f * cubeLength))
        {
            isCubeInTarget = true;
        }
        else
        {
            isCubeInTarget = false;
        }
        return isCubeInTarget;
    }

    public float getEntryWall(float distToCenter, float radius, float fingerVelocity)
    {
        if ( distToCenter >= (cubeLength + radius))
        {
            isPentrating = false;
            return none;
        }
        else
        {
            isPenetrating = true;

            //Find direction of entry
            float[] dotProducts = { Vector3.Dot(fingerVelocity, cubeAxisX),
                Vector3.Dot(fingerVelocity, cubeAxisY),
                Vector3.Dot(fingerVelocity, cubeAxisZ) };

            //Max component in +/-x:
            if ( Array.IndexOf(dotProducts, dotProducts.Select(Math.Abs).Max()) = 0 )
            {
                if ( dotProducts[0] > 0 )
                {
                    return cubeFront;
                }
                else
                {
                    return cubeBack;
                }
            }
            //Max component in +/-y:
            if (Array.IndexOf(dotProducts, dotProducts.Select(Math.Abs).Max()) = 1)
            {
                if (dotProducts[1] > 0)
                {
                    return cubeUp;
                }
                else
                {
                    return cubeDown;
                }
            }
            //Max component in +/-z:
            if (Array.IndexOf(dotProducts, dotProducts.Select(Math.Abs).Max()) = 2)
            {
                if (dotProducts[2] > 0)
                {
                    return cubeRight;
                }
                else
                {
                    return cubeLeft;
                }
            }

        }
    }

    public Vector3[] calculateFingerForceValues(Vector3 cubeVelocity, Vector3 indexVelocity, Vector3 thumbVelocity, Vector3 middleVelocity,
        Vector3 cubePosition, Vector3 indexPosition, Vector3 thumbPosition, Vector3 middlePosition,
        Vector3 indexPenetration, Vector3 thumbPenetration, Vector3 middlePenetration,
        float indexPenetrationMagSign, float thumbPenetrationMagSign, float middlePenetrationMagSign)
    {
        Vector3 indexForceVal;
        Vector3 thumbForceVal;
        Vector3 middleForceVal;

        Vector3 indexRelVelocity = indexVelocity - cubeVelocity; //cubeVelocity - indexVelocity;
        Vector3 thumbRelVelocity = thumbVelocity - cubeVelocity; //cubeVelocity - thumbVelocity;
        Vector3 middleRelVelocity = middleVelocity - cubeVelocity; //cubeVelocity - middleVelocity;

        Vector3 indexShearVelocity = getShearVelocity(indexRelVelocity, indexPosition - cubePosition);
        Vector3 thumbShearVelocity = getShearVelocity(thumbRelVelocity, thumbPosition - cubePosition);
        Vector3 middleShearVelocity = getShearVelocity(middleRelVelocity, middlePosition - cubePosition);

        if (isPenetrating == false)
        {
            indexContact = false;
            indexPenetrationForce = Vector3.zero;
            indexShearForce = Vector3.zero;
            indexForceVal = Vector3.zero;
        }
        else
        {
            indexContact = true;

            //Use Hooke's Law to find penetration force of cube on finger
            indexPenetrationForce = cubeStiffness * indexPenetration;

            //Add shear forces of cube on finger due to friction
            indexShearForce = -fingerDamping * indexShearVelocity - uK * Vector3.Magnitude(indexPenetrationForce) * sign(indexShearVelocity)
                + (1.0f / 3.0f) * cubeMass * Physics.gravity;

            /*For debugging*/
            //drawShearVelocityVector(indexShearVelocity, indexPosition - cubePosition,
            //   0.5f * cubeLength, 0.5f * indexDiameter, indexLineRenderer);

            //drawPenetrationVector( penetrationLineRenderer);

            //Sum of forces of cube on finger
            indexForceVal = indexPenetrationForce + indexShearForce;
        }

        if (thumbPenetrationMagSign <= 0.0f)
        {
            thumbContact = false;
            thumbPenetrationForce = Vector3.zero;
            thumbShearForce = Vector3.zero;
            thumbForceVal = Vector3.zero;
        }
        else
        {
            thumbContact = true;

            //Use Hooke's Law to find force of cube on finger
            thumbPenetrationForce = cubeStiffness * thumbPenetration;

            //Add shear forces of cube on finger due to friction
            thumbShearForce = -fingerDamping * thumbShearVelocity - uK * Vector3.Magnitude(thumbPenetrationForce) * sign(thumbShearVelocity);

            /*For debugging*/
            //drawShearVelocityVector(thumbShearVelocity, thumbPosition - cubePosition,
            //    0.5f * cubeLength, 0.5f * thumbScaleValue, thumbLineRenderer);

            //Sum of forces of cube on finger
            thumbForceVal = thumbPenetrationForce + thumbShearForce;
        }


        if (middlePenetrationMagSign <= 0.0f)
        {
            middleContact = false;
            middlePenetrationForce = Vector3.zero;
            middleShearForce = Vector3.zero;
            middleForceVal = Vector3.zero;
        }
        else
        {
            middleContact = true;

            //Use Hooke's Law to find force of cube on finger
            middlePenetrationForce = cubeStiffness * middlePenetration;

            //Add shear forces of cube on finger due to friction
            middleShearForce = -fingerDamping * middleShearVelocity - uK * Vector3.Magnitude(middlePenetrationForce) * sign(middleShearVelocity)
                + (1.0f / 3.0f) * cubeMass * Physics.gravity;

            /*For debugging*/
            drawShearVelocityVector(middleShearVelocity, middlePosition - cubePosition,
                0.5f * cubeLength, 0.5f * middleScaleValue, middleLineRenderer);

            //Sum of forces of cube on finger
            middleForceVal = middlePenetrationForce + middleShearForce;
        }

        //Finger Force on cube
        forceValues[0] = -indexForceVal;
        forceValues[1] = -thumbForceVal;
        forceValues[2] = -middleForceVal;
        return forceValues;
    }

    public Vector3 calculateFloorNormalForce()
    {
        if (cubePosition.y < 0.5f * cubeLength)
        {
            floorPenetration = 0.5f * cubeLength - cubePosition.y;
        }
        else
        {
            floorPenetration = 0.0f;
        }

        return new Vector3(0.0f, cubeStiffness * floorPenetration, 0.0f);
    }

    public Vector3[] getCubeStatus(Vector3 cubePosition, Vector3 cubeVelocity, Vector3 cubeAcceleration,
        Vector3 indexForce, Vector3 thumbForce, Vector3 middleForce, Vector3 floorNormalForce)
    {
        Vector3 positionPrev = cubePosition;
        Vector3 velocityPrev = cubeVelocity;
        Vector3 accelerationPrev = cubeAcceleration;

        cubeAcceleration = Physics.gravity + (indexForce + thumbForce + middleForce + floorNormalForce - cubeDamping * velocityPrev) / cubeMass;
        cubeForce = cubeMass * cubeAcceleration;

        cubeVelocity = velocityPrev + cubeAcceleration * Time.fixedDeltaTime;
        cubePosition = positionPrev + cubeVelocity * Time.fixedDeltaTime;

        /*For Debugging*/
        //drawCubeForceVector(cubeForce, cubePosition, cubeLineRenderer);

        cubeStatus[0] = cubePosition;
        cubeStatus[1] = cubeVelocity;
        cubeStatus[2] = cubeAcceleration;

        return cubeStatus;
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
            dorsalVal = 1000 * (indexForceMag / cubeStiffness);
            ventralVal = 1000 * (thumbForceMag / cubeStiffness);
        }

        /*Condition 2: Index --> Ventral | Thumb --> Dorsal*/
        else if (trialNumber == 2)
        {
            dorsalVal = 1000 * (thumbForceMag / cubeStiffness);
            ventralVal = 1000 * (indexForceMag / cubeStiffness);
        }

        /*Condition 3: Single Tactor Feedback to Index Finger on Dorsal Side*/
        else if (trialNumber == 3)
        {
            dorsalVal = 1000 * (indexForceMag / cubeStiffness);
            ventralVal = 0.0f;
        }

        /*Condition 4: Average of both finger forces to single tactor on dorsal side*/
        else if (trialNumber == 4)
        {
            dorsalVal = 500.0f * (indexForceMag + thumbForceMag) / cubeStiffness;
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

    public Vector3 getShearVelocity(Vector3 vRel, Vector3 distanceVec)
    {
        //vector normal to the plane of the instersceting cubes
        Vector3 normal = distanceVec;
        float normalMag = Vector3.Magnitude(normal);

        //Shear Velocity
        return Vector3.Cross(normal, Vector3.Cross(vRel, normal)) / Mathf.Pow(normalMag, 2.0f);
    }

    public void drawShearVelocityVector(Vector3 shearVelocity, Vector3 distanceVec, float cubeRadius, float fingerRadius, LineRenderer fingerLine)
    {
        //vector distance between centers instersceting cubes
        float distanceMag = Vector3.Magnitude(distanceVec);
        Vector3 distanceVec_hat = distanceVec / Vector3.Magnitude(distanceVec);

        float cubeCenterToIntersectionPoint = (Mathf.Pow(cubeRadius, 2.0f) - Mathf.Pow(fingerRadius, 2.0f) + Mathf.Pow(distanceMag, 2.0f))
                                                                        / (2.0f * distanceMag);
        Vector3 intersectionPoint = cubePosition + cubeCenterToIntersectionPoint * distanceVec_hat;

        fingerLine.useWorldSpace = true;
        fingerLine.SetPosition(0, intersectionPoint);
        fingerLine.SetPosition(1, intersectionPoint + shearVelocity);
        fingerLine.SetWidth(0.01f, 0.01f);
    }

    public void drawCubeForceVector(Vector3 cubeForce, Vector3 cubePosition, LineRenderer cubeLine)
    {
        cubeLine.useWorldSpace = true;
        cubeLine.SetPosition(0, cubePosition);
        cubeLine.SetPosition(1, cubePosition + cubeForce);
        cubeLine.SetWidth(0.01f, 0.01f);
        cubeLine.material.color = Color.blue;
    }

    public void drawCubeRadialVector(Vector3 cubePosition, LineRenderer cubeRadialLine)
    {
        cubeRadialLine.useWorldSpace = true;
        cubeRadialLine.SetPosition(0, cubePosition);
        cubeRadialLine.SetPosition(1, cubePosition + cubeForce);
        cubeRadialLine.SetWidth(0.01f, 0.01f);
        cubeRadialLine.material.color = Color.yellow;
    }

    public void drawPenetrationVector(Vector3 penetration, Vector3 distanceVec, Vector3 cubeRadialVec, float fingerRadius, LineRenderer penetrationLine)
    {
        Vector3 cubeCenterToIntersectionPoint = distanceVec - cubeRadialVec;
        Vector3 intersectionPoint = cubePosition + cubeCenterToIntersectionPoint;

        penetrationLine.useWorldSpace = true;
        penetrationLine.SetPosition(0, indexSpherePosition);
        penetrationLine.SetPosition(1, indexSpherePosition + penetration);
        penetrationLine.SetWidth(0.005f, 0.005f);
        penetrationLine.material.color = Color.black;
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
