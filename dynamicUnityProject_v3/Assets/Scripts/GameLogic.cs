using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

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
    public float thumbDiameter = 0.02f; //m
    Vector3 thumbPositionPrev;
    Vector3 thumbScaling;

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
    public float cubeMass = 1.0f; //kg
    public float cubeStiffness = 500.0f; //in N/m
    public float cubeWeight; //N scalar
    [Range(0.0f, 100.0f)]
    public float cubeDamping = 40.0f; //in N/m/s
    [Range(-100.0f, 100.0f)]
    public float fingerDamping = 67.0f; //in N/m/s
    [Range(0.0f, 1.5f)]
    public float uK = 0.7f; // Coefficient of kinetic friction cube-finger [-]

    [Header("Position Commands to Arduino")]
    public float dorsalCommand;
    public float ventralCommand;

    [Header("Floor Variables")]
    public float floorPenetration;
    public float floorStiffness = 5000; // N/m
    public Vector3 floorNormalForce; //Strictly the normal from interacting with the floor

    public Vector3[] cubeStatus;
    Vector3[] indexForceValues;
    Vector3[] thumbForceValues;
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

    #endregion

    LineRenderer cubeLineRenderer;

    public Vector3 indexCubeVec;
    public Vector3 thumbCubeVec;
    public Vector3 cubeAxisX;
    public Vector3 cubeAxisY;
    public Vector3 cubeAxisZ;

    //Cube Wall IDs:
    Vector3 cubePositionPrev;
    public int indexEntryWall;
    public int thumbEntryWall;
    int none = 0;
    int cubeRight = 1;
    int cubeLeft = 2;
    int cubeUp = 3;
    int cubeDown = 4;
    int cubeFront = 5;
    int cubeBack = 6;
    public Vector3 indexPentrationDirection;
    public Vector3 thumbPentrationDirection;


    // Start is called before the first frame update
    void Start()
    {
        print("BUT DID YOU **TURN ON SAMPLE**???");

        indexSphere = GameObject.Find("trakSTAR/Index Sphere"); /*GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //fixed value for testing**
        indexSphere.transform.position = new Vector3(0.32f, 0.025f, 0.2f);*/
        thumbSphere = GameObject.Find("trakSTAR/Thumb Sphere");

        trakSTAROrigin = GameObject.Find("trakSTAR/trakSTAR Origin");
        trakSTAROrigin.transform.position = Vector3.zero;

        setObjectColor(indexSphere.GetComponent<MeshRenderer>(), 1.0f, 0.0f, 0.0f, 1.0f);
        setObjectColor(thumbSphere.GetComponent<MeshRenderer>(), 0.0f, 1.0f, 1.0f, 1.0f);

        trakSTAROrigin.GetComponent<MeshRenderer>().material.color = Color.magenta;

        indexScaling = new Vector3(indexDiameter, indexDiameter, indexDiameter);
        indexSphere.transform.localScale = indexScaling;
        thumbScaling = new Vector3(thumbDiameter, thumbDiameter, thumbDiameter);
        thumbSphere.transform.localScale = thumbScaling;

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
        thumbVelocity = Vector3.zero; thumbCubeVec = Vector3.zero;

        indexPositionPrev = Vector3.zero;
        thumbPositionPrev = Vector3.zero;
        cubePositionPrev = Vector3.zero;

        cubeStatus = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        indexForceValues = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        thumbForceValues = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };

        indexContact = false;
        thumbContact = false;

        positionCommands = new float[] { 0.0f, 0.0f };
        floorNormalForce = Vector3.zero;
        trialNumber = 1;

        //create Shear Velocity Vector lines
        indexLineRenderer = indexSphere.AddComponent<LineRenderer>();
        indexLineRenderer.SetPosition(0, Vector3.zero);
        indexLineRenderer.SetPosition(1, new Vector3(0.001f, 0.001f, 0.001f));
        thumbLineRenderer = thumbSphere.AddComponent<LineRenderer>();
        thumbLineRenderer.SetPosition(0, Vector3.zero);
        thumbLineRenderer.SetPosition(1, new Vector3(0.001f, 0.001f, 0.001f));

        indexEntryWall = none;
        thumbEntryWall = none;

        indexPentrationDirection = Vector3.zero;
        thumbPentrationDirection = Vector3.zero;
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


        //Distance Vector between centers of finger and cube
        indexCubeVec = indexPosition - cubePosition;  // Index-->Cube 
        thumbCubeVec = thumbPosition - cubePosition;  // Thumb-->Cube 

        //Distance Magnitude between centers of finger and cube
        indexDistToCenter = Vector3.Magnitude(indexCubeVec);
        thumbDistToCenter = Vector3.Magnitude(thumbCubeVec);

        //Find wall of penetration
        indexEntryWall = getEntryWall(indexDistToCenter, 0.5f * indexDiameter, indexCubeVec);
        thumbEntryWall = getEntryWall(thumbDistToCenter, 0.5f * thumbDiameter, thumbCubeVec);

        //Vector of Penetration of fingers into cube
        indexPenetration = getPenetration(indexEntryWall, indexCubeVec, cubePosition, indexPosition, 0.5f * indexDiameter);
        thumbPenetration = getPenetration(thumbEntryWall, thumbCubeVec, cubePosition, thumbPosition, 0.5f * thumbDiameter);

        //Set Contact booleans
        indexContact = setContactBoolean(indexPenetration);
        thumbContact = setContactBoolean(thumbPenetration);

        indexPentrationDirection = sign(indexPenetration);
        thumbPentrationDirection = sign(thumbPenetration);

        //Force Calculation
        indexForceValues = calculateFingerForce(indexCubeVec, cubeVelocity, indexVelocity, indexPenetration,
                   indexPentrationDirection, indexContact, indexLineRenderer, Color.red);
        thumbForceValues = calculateFingerForce(thumbCubeVec, cubeVelocity, thumbVelocity, thumbPenetration,
            thumbPentrationDirection, thumbContact, thumbLineRenderer, Color.cyan);

        //Penetration Force
        indexPenetrationForce = indexForceValues[0];
        thumbPenetrationForce = thumbForceValues[0];
        //Shear Force
        indexShearForce = indexForceValues[1];
        thumbShearForce = thumbForceValues[1];
        //Total force
        indexForce = indexForceValues[2];
        thumbForce = thumbForceValues[2];
        floorNormalForce = calculateFloorNormalForce();

        //Cube Pose
        cubeStatus = getCubeStatus(cubePosition, cubeVelocity, cubeAcceleration,
                       indexForce, thumbForce, floorNormalForce);

        cubePosition = cubeStatus[0];
        cube.transform.position = cubePosition;
        /**/
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
        cubePositionPrev = cubePosition;
        /*****************************************************************************************/

        //Check if cube is inside of targetArea by calculating 
        //the distance between target area center and cube center
        targetToCubeDist = Vector3.Magnitude(targetAreaPosition - cubePosition);

        //Determine successes/fails
        /*
        trialLogic();*/
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
            if (indexContact == true && thumbContact == true && (cubePosition.y > 0.03))
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
        setObjectColor(cubeMeshRenderer, 0.0f, 0.0f, 1.0f, 0.8f);

        cubeWeight = cubeMass * Vector3.Magnitude(Physics.gravity);

        // cubeLineRenderer = cube.AddComponent<LineRenderer>();
        cubeLineRenderer = cube.AddComponent<LineRenderer>();
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

    public int getEntryWall(float distToCenter, float radius, Vector3 fingerToCubeVec)
    {
        if (distToCenter >= (0.5f * cubeLength + radius))
        {
            return none;
        }
        else
        {
            //Find direction of entry
            float[] dotProducts = { Vector3.Dot(fingerToCubeVec, cubeAxisX),
                Vector3.Dot(fingerToCubeVec, cubeAxisY),
                Vector3.Dot(fingerToCubeVec, cubeAxisZ) };

            float[] dotProductsAbs = { Mathf.Abs(dotProducts[0]),
                Mathf.Abs(dotProducts[1]),
                Mathf.Abs(dotProducts[2]) };

            //Max component in +/-x:
            if (Array.IndexOf(dotProductsAbs, dotProductsAbs.Max()) == 0)
            {
                if (dotProducts[0] >= 0)
                {
                    return cubeFront;
                }
                else
                {
                    return cubeBack;
                }
            }
            //Max component in +/-y:
            if (Array.IndexOf(dotProductsAbs, dotProductsAbs.Max()) == 1)
            {
                if (dotProducts[1] >= 0)
                {
                    return cubeUp;
                }
                else
                {
                    return cubeDown;
                }
            }
            //Max component in +/-z:
            if (Array.IndexOf(dotProductsAbs, dotProductsAbs.Max()) == 2)
            {
                if (dotProducts[2] >= 0)
                {
                    return cubeRight;
                }
                else
                {
                    return cubeLeft;
                }
            }
            //***EDIT LATER*** cases for multiple wall entered 
            else
            {
                return none;
            }
        }
    }

    public Vector3 getPenetration(int entryWall, Vector3 fingerToCubeVec, Vector3 cubePosition, Vector3 fingerPosition, float radius)
    {
        Vector3 penetrationVector = Vector3.zero;

        //Pentration in cube +z
        if (entryWall == cubeRight)
        {
            penetrationVector.z = -(radius - fingerToCubeVec.z + 0.5f * cubeLength);
        }
        //Pentration in cube -z
        if (entryWall == cubeLeft)
        {
            penetrationVector.z = radius + fingerToCubeVec.z + 0.5f * cubeLength;
        }
        //Pentration in cube +y
        if (entryWall == cubeUp)
        {
            penetrationVector.y = -(radius - fingerToCubeVec.y + 0.5f * cubeLength);
        }
        //Pentration in cube -y
        if (entryWall == cubeDown)
        {
            penetrationVector.y = radius + fingerToCubeVec.y + 0.5f * cubeLength;
        }
        //Pentration in cube +x
        if (entryWall == cubeFront)
        {
            penetrationVector.x = -(radius - fingerToCubeVec.x + 0.5f * cubeLength);
        }
        //Pentration in cube -x
        if (entryWall == cubeBack)
        {
            penetrationVector.x = radius + fingerToCubeVec.x + 0.5f * cubeLength;
        }
        //entryWall == 0 --> no pentration
        else
        {
            return penetrationVector;
        }

        return penetrationVector;
    }

    public bool setContactBoolean(Vector3 penetration)
    {
        //No penetration
        if (Vector3.Magnitude(penetration) == 0.0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Vector3[] calculateFingerForce(Vector3 fingerCubeVec, Vector3 cubeVelocity, Vector3 fingerVelocity,
           Vector3 penetration, Vector3 penetrationDirection, bool contactBoolean, LineRenderer lineRenderer, Color lineColor)
    {
        Vector3[] forceValues = { Vector3.zero, Vector3.zero, Vector3.zero };

        Vector3 penetrationForce;
        Vector3 shearForce;
        Vector3 totalForceVal;
        Vector3 relVelocity = fingerVelocity - cubeVelocity;
        Vector3 shearVelocity = getShearVelocity(fingerVelocity, fingerCubeVec, penetrationDirection);

        Vector3 staticFriction = Vector3.zero;

        if (contactBoolean == false)
        {
            penetrationForce = Vector3.zero;
            shearForce = Vector3.zero;
            totalForceVal = Vector3.zero;
        }
        else
        {
            //Use Hooke's Law to find penetration force of finger on cube
            penetrationForce = cubeStiffness * Vector3.Magnitude(penetration) * penetrationDirection;

            //Add shear forces of on cube due to friction
            Vector3 dampingFriction = fingerDamping * shearVelocity;
            Vector3 coulombFriction = uK * Vector3.Magnitude(penetrationForce) * sign(shearVelocity);

            //Apply static friction when slip occurs
            if (Vector3.Magnitude(shearVelocity) <= 0.1f)
            {
                staticFriction = uK * Vector3.Magnitude(penetrationForce) * sign(-Physics.gravity);
            }        
            
            shearForce = /**/coulombFriction + dampingFriction + staticFriction;

            Debug.Log("Coulomb Friction: " + coulombFriction.ToString() + "  Damping Friction: " + dampingFriction.ToString());
            /*For debugging*/
            drawShearVelocityVector(shearVelocity, fingerCubeVec, 0.5f * cubeLength, 0.01f, lineRenderer, lineColor);

            //Sum of forces of on cube 
            totalForceVal = penetrationForce + shearForce;
        }

        //Finger Forces on cube
        forceValues[0] = penetrationForce;
        forceValues[1] = shearForce;
        forceValues[2] = totalForceVal;

        return forceValues;
    }

    public Vector3 calculateFloorNormalForce()
    {
        if (cubePosition.y <= 0.5f * cubeLength)
        {
            floorPenetration = 0.5f * cubeLength - cubePosition.y;
            return new Vector3(0.0f, cubeWeight + cubeStiffness * floorPenetration, 0.0f);
        }
        else
        {
            floorPenetration = 0.0f;
            return Vector3.zero;
        }      
    }

    public Vector3[] getCubeStatus(Vector3 cubePosition, Vector3 cubeVelocity, Vector3 cubeAcceleration,
        Vector3 indexForce, Vector3 thumbForce, Vector3 floorNormalForce)
    {
        Vector3 positionPrev = cubePosition;
        Vector3 velocityPrev = cubeVelocity;
        Vector3 accelerationPrev = cubeAcceleration;

        cubeAcceleration = Physics.gravity + (-cubeDamping * velocityPrev + indexForce + thumbForce + floorNormalForce) / cubeMass;
        cubeForce = cubeMass * cubeAcceleration;

        //**Eliminate accumulation of velocity??
        cubeVelocity = /*velocityPrev +*/ cubeAcceleration * Time.fixedDeltaTime;
        cubePosition = /**/positionPrev + cubeVelocity * Time.fixedDeltaTime;

        /*For Debugging*/
        drawCubeForceVector(cubeForce, cubePosition, cubeLineRenderer);

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

    public Vector3 getShearVelocity(Vector3 vRel, Vector3 distanceVec, Vector3 penetrationDirection)
    {
        //vector normal to the plane of the instersceting cubes
        Vector3 normal = Vector3.Dot(distanceVec, penetrationDirection) * penetrationDirection;
        float normalMag = Vector3.Magnitude(normal);

        //Shear Velocity
        return Vector3.Cross(normal, Vector3.Cross(vRel, normal)) / Mathf.Pow(normalMag, 2.0f);
    }

    public void drawShearVelocityVector(Vector3 shearVelocity, Vector3 distanceVec, float cubeRadius, float fingerRadius, LineRenderer fingerLine,
        Color lineColor)
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
        fingerLine.SetWidth(0.005f, 0.005f);
        fingerLine.material.color = lineColor;
        lineColor.a = 0.5f;
    }

    public void drawCubeForceVector(Vector3 cubeForce, Vector3 cubePosition, LineRenderer cubeLine)
    {
        cubeLine.useWorldSpace = true;
        cubeLine.SetPosition(0, cubePosition);
        cubeLine.SetPosition(1, cubePosition + cubeForce);
        cubeLine.SetWidth(0.005f, 0.005f);
        cubeLine.material.color = Color.blue;
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
