using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

public class GameMaster : MonoBehaviour
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

    /**** Create CUBE *****/
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
    public float cubeMass = 0.1f; //kg
    public float cubeStiffness = 500.0f; //in N/m
    public float cubeWeight; //N scalar
    [Range(0.0f, 100.0f)]
    public float cubeDamping = 1.2f; //in N/m/s
    [Range(0.0f, 100.0f)]
    public float fingerDamping = 67.0f; //in N/m/s
    [Range(0.0f, 1.5f)]
    public float uK = 0.7f; // Coefficient of kinetic friction cube-finger [-]
    [Range(0.0f, 1.5f)]
    public float uS = 0.9f; // Coefficient of static friction cube-finger [-]

    public float startingX = 0.3f;
    public float startingZ = 0.2f;

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

    [Header("Contact Conditions")]
    //Contact booleans
    public bool indexContact;
    public bool thumbContact;
    public bool heldCubeBefore = false;

    LineRenderer indexLineRenderer;
    LineRenderer thumbLineRenderer;
    LineRenderer cubeLineRenderer;
    #endregion

    Vector3[] shearForces;

    [Header("Other Cube Variables")]
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
    public Vector3 indexPenetrationDirection;
    public Vector3 thumbPenetrationDirection;

    //Friction variables
    [Range(0.0f, 0.5f)]
    public float deltaV = 0.1f; //m/s

    Vector3 prevThumbSF;

    public Vector3 indexShearVelocity;
    public Vector3 thumbShearVelocity;

    // Start is called before the first frame update
    void Start()
    {
        print("<size=14><color=red>BUT DID YOU **TURN ON SAMPLE**???</color></size>");

        indexSphere = GameObject.Find("trakSTAR/Index Sphere"); /*GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //fixed value for testing**
        indexSphere.transform.position = new Vector3(0.32f, 0.025f, 0.2f);*/
        thumbSphere = GameObject.Find("trakSTAR/Thumb Sphere");

        trakSTAROrigin = GameObject.Find("trakSTAR/trakSTAR Origin");
        trakSTAROrigin.transform.position = Vector3.zero;

        GetComponent<ColorManager>().setObjectColor(indexSphere.GetComponent<MeshRenderer>(), 1.0f, 0.0f, 0.0f, 1.0f);
        GetComponent<ColorManager>().setObjectColor(thumbSphere.GetComponent<MeshRenderer>(), 0.0f, 1.0f, 1.0f, 1.0f);

        trakSTAROrigin.GetComponent<MeshRenderer>().material.color = Color.magenta;

        indexScaling = new Vector3(indexDiameter, indexDiameter, indexDiameter);
        indexSphere.transform.localScale = indexScaling;
        thumbScaling = new Vector3(thumbDiameter, thumbDiameter, thumbDiameter);
        thumbSphere.transform.localScale = thumbScaling;

        //Create the cube Game Object
        createCube(startingX, startingZ);

        //Create starting and target areas
        GetComponent<TrialManager>().createStartingArea(startingX, startingZ);
        GetComponent<TrialManager>().createTargetArea(startingX, startingZ);
        GetComponent<TrialManager>().createWaypoint(startingX);

        //Variable initializations
        indexVelocity = Vector3.zero; indexCubeVec = Vector3.zero;
        thumbVelocity = Vector3.zero; thumbCubeVec = Vector3.zero;

        indexPositionPrev = Vector3.zero;
        thumbPositionPrev = Vector3.zero;
        cubePositionPrev = Vector3.zero;

        cubeStatus = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        indexForceValues = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        thumbForceValues = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        shearForces = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };

        indexContact = false;
        thumbContact = false;

        positionCommands = new float[] { 0.0f, 0.0f };
        floorNormalForce = Vector3.zero;

        //create Shear Velocity Vector lines
        indexLineRenderer = indexSphere.AddComponent<LineRenderer>();
        indexLineRenderer.SetPosition(0, Vector3.zero);
        indexLineRenderer.SetPosition(1, new Vector3(0.001f, 0.001f, 0.001f));
        thumbLineRenderer = thumbSphere.AddComponent<LineRenderer>();
        thumbLineRenderer.SetPosition(0, Vector3.zero);
        thumbLineRenderer.SetPosition(1, new Vector3(0.001f, 0.001f, 0.001f));

        indexEntryWall = none;
        thumbEntryWall = none;

        indexPenetrationDirection = Vector3.zero;
        thumbPenetrationDirection = Vector3.zero;

        prevThumbSF = Vector3.zero;
        indexShearVelocity = Vector3.zero;
        thumbShearVelocity = Vector3.zero;
    }

    // Update is called once per frame
    // Purpose: check for detection of input, anything to be changed
    // or adjusted for non-physics objects, simple timers
    void Update()
    {
        //Reset cube if needed
        if (Input.GetKeyDown("space"))
        {
            resetCube();
        }
    }

    // FixedUpdate is called once every physics step
    // Purpose: Physics calcuations, adjusting physics/rigidbody objects
    void FixedUpdate()
    {
        //Check Progress of trial
        GetComponent<TrialManager>().checkTrialProgress(startingX, startingZ, GetComponent<TrialManager>().targetOffset);

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

        indexPenetrationDirection = sign(indexPenetration);
        thumbPenetrationDirection = sign(thumbPenetration);

        //Force Calculation      
        //Penetration Force
        indexPenetrationForce = calculatePenetrationForce(indexPenetration, indexPenetrationDirection, indexContact);
        thumbPenetrationForce = calculatePenetrationForce(thumbPenetration, thumbPenetrationDirection, thumbContact);

        //Floor normal force
        floorNormalForce = calculateFloorNormalForce();
        //Shear Force Calculations:
        //Calculate shear velocities using relative velocities
        //Find index shear force 1st and use that to find the thumb shear force
        if (indexContact == true)
        {
            indexShearVelocity = getVectorProjection(indexVelocity - cubeVelocity, indexCubeVec, indexPenetrationDirection);
            indexShearForce = calculateFingerShearForce(indexCubeVec, indexShearVelocity, cubeVelocity, indexPenetrationForce, indexPenetrationDirection,
                           thumbPenetrationForce, prevThumbSF, floorNormalForce, indexLineRenderer, Color.red);
        }
        else
        {
            indexShearVelocity = Vector3.zero;
            indexShearForce = Vector3.zero;
        }
        if (thumbContact == true)
        {
            thumbShearVelocity = getVectorProjection(thumbVelocity - cubeVelocity, thumbCubeVec, thumbPenetrationDirection);
            thumbShearForce = calculateFingerShearForce(thumbCubeVec, thumbShearVelocity, cubeVelocity, thumbPenetrationForce, thumbPenetrationDirection,
                indexPenetrationForce, indexShearForce, floorNormalForce, thumbLineRenderer, Color.cyan);
        }
        else
        {
            thumbShearVelocity = Vector3.zero;
            thumbShearForce = Vector3.zero;
        }
        Debug.Log("indexPenetrationDirection: " + indexPenetrationDirection.ToString());
        Debug.Log("indexShearVelocity: " + indexShearVelocity.ToString("F4"));
        Debug.Log("indexShearForce: " + indexShearForce.ToString("F4"));

        //Total force
        indexForce = indexPenetrationForce + indexShearForce;
        thumbForce = thumbPenetrationForce + thumbShearForce;

        //Cube Pose
        cubeStatus = getCubeStatus(cubePosition, cubeVelocity, cubeAcceleration,
                       indexForce, thumbForce, floorNormalForce);
        /*
                cubePosition = cubeStatus[0];
                cube.transform.position = cubePosition;
                */
        //cubeOrientation = cube.transform.eulerAngles;
        cubeVelocity = cubeStatus[1];
        cubeAcceleration = cubeStatus[2];

        //Derive Position Command from force calculation and assign 
        positionCommands = getFingerPositionCommands(Vector3.Magnitude(indexForce), Vector3.Magnitude(thumbForce),
            GetComponent<TrialManager>().trialNumber);
        dorsalCommand = positionCommands[0];
        ventralCommand = positionCommands[1];

        //set prev position
        indexPositionPrev = indexPosition;
        thumbPositionPrev = thumbPosition;
        cubePositionPrev = cubePosition;
        prevThumbSF = thumbShearForce;
        /*****************************************************************************************/

        //Determine successes/fails
        //GetComponent<TrialManager>().trialLogic(indexContact, thumbContact, cubePosition, indexDistToCenter);/**/
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
        GetComponent<ColorManager>().setObjectColor(cubeMeshRenderer, 0.0f, 0.0f, 1.0f, 0.8f);

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
        GetComponent<TrialManager>().passedWaypoint = false;

        //reset cube-target check
        GetComponent<TrialManager>().isCubeInTarget = false;

        //create the new object in starting positition
        createCube(GetComponent<TrialManager>().startingAreaPosition.x, GetComponent<TrialManager>().startingAreaPosition.z);

        //Reset trial state
        GetComponent<TrialManager>().trialState = 0;
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

    public Vector3 calculatePenetrationForce(Vector3 penetration, Vector3 penetrationDirection, bool contactBoolean)
    {
        if (contactBoolean == false)
        {
            return Vector3.zero;
        }
        else
        {
            //Use Hooke's Law to find penetration force of finger on cube
            return cubeStiffness * Vector3.Magnitude(penetration) * penetrationDirection;
        }
    }

    public Vector3 calculateFingerShearForce(Vector3 fingerToCubeVec, Vector3 shearVel, Vector3 cubeVel, Vector3 penetrationForce,
            Vector3 penetrationDirection, Vector3 otherPenetrationForce, Vector3 otherShearForce, Vector3 floorNormalForce,
            LineRenderer fingerLine, Color lineColor)
    {
        Vector3 shearForce = Vector3.zero;
        Vector3 otherFingerForces = otherPenetrationForce + otherShearForce;
        //Debug.Log("otherFingerForces: " + otherFingerForces.ToString("F4"));
        Vector3 D = Vector3.zero;

        //Sum of non-frictional forces applied to the system:
        Vector3 Fa = cubeMass * Physics.gravity + (-cubeDamping * cubeVel + otherFingerForces + floorNormalForce);
        // Debug.Log("Fa: " + Fa.ToString("F4"));

        //Static Friction:
        if (Fa == Vector3.zero)
        {
            D = Vector3.zero;
        }
        else
        {
            D = new Vector3(-2.0f, 0.0f, -2.0f);// uS * Vector3.Magnitude(penetrationForce) * sign(-getVectorProjection(Fa, fingerToCubeVec, penetrationDirection));
        }
        //Debug.Log("D: " + D.ToString("F4"));

        //Iterate throuhg each element of the velocity vector to determine if it is in dynamic of static regime:
        float[] vS = { shearVel.x, shearVel.y, shearVel.z };
        float[] fPen = { penetrationForce.x, penetrationForce.y, penetrationForce.z };
        float[] dynamicFriction = { 0.0f, 0.0f, 0.0f };
        float[] staticFriction = { 0.0f, 0.0f, 0.0f };
        float[] _D = { D.x, D.y, D.z };
        float[] _Fa = { Fa.x, Fa.y, Fa.z };

        for (int i = 0; i < vS.Length; i++)
        {
            //Dynamic Regime:
            if (vS[i] < -deltaV || vS[i] > deltaV)
            {
                //Dynamic friction
                dynamicFriction[i] = uK * fPen[i] + fingerDamping * vS[i];
            }
            //Static Regime:
            if (-deltaV <= vS[i] && vS[i] <= 0.0f)
            {
                staticFriction[i] = Mathf.Max(_D[i], _Fa[i]);
            }
            if (0.0f <= vS[i] && vS[i] <= deltaV)
            {
                staticFriction[i] = Mathf.Min(_D[i], _Fa[i]);
            }
        }

        Vector3 SF = new Vector3(staticFriction[0], staticFriction[1], staticFriction[2]);
        Vector3 DF = new Vector3(dynamicFriction[0], dynamicFriction[1], dynamicFriction[2]);
        //Debug.Log("dynamicFriction: " + DF.ToString("F4"));
        //Debug.Log("staticFriction: " + SF.ToString("F4"));

        shearForce = new Vector3(dynamicFriction[0] + staticFriction[0],
                                 dynamicFriction[1] + staticFriction[1],
                                 dynamicFriction[2] + staticFriction[2]);

        #region old
        /*
        Vector3 staticFriction = Vector3.zero;
        Vector3 dynamicFriction = Vector3.zero;
        //Add shear forces of on cube due to friction
        if (-Vector3.Magnitude(shearVel) < -deltaV || Vector3.Magnitude(shearVel) > deltaV)
        {
            //Dynamic friction
            Vector3 dampingFriction = fingerDamping * shearVel;
            Vector3 coulombFriction = uK * Vector3.Magnitude(penetrationForce) * sign(shearVel);
            dynamicFriction = coulombFriction + dampingFriction;
        }
        /* else 
        {
            //Sum of non-frictional forces applied to the system:
            Vector3 Fa = cubeMass * Physics.gravity + (-cubeDamping * cubeVelocity + indexPenetrationForce
                + thumbPenetrationForce + floorNormalForce);

            Vector3 D = uK * Vector3.Magnitude(indexPenetrationForce + thumbPenetrationForce) * sign(Fa);

            staticFriction = getStaticFriction(indexShearVel, Fa, D) + getStaticFriction(thumbShearVel, Fa, D);
            Debug.Log("**AppliedForces: " + Fa.ToString("F4") + " | fDynamic: " + dynamicFriction.ToString("F4") + " | fStatic: " + staticFriction.ToString("F4"));

        }*/
        //Debug.Log("Dn: " + Dn.ToString("F4") + " | Dp: " + Dp.ToString("F4") + " | Fa: " + Fa.ToString("F4"));
        //Debug.Log("\nShearV: " + shearVelocity.ToString("F4") + "\nfStatic:  " + staticFriction.ToString("F4") + "\nFa:         " + Fa.ToString("F4"));

        //shearForce = dynamicFriction /*+ staticFriction*/;
        #endregion old

        //DEBUGGING
        drawShearVector(shearForce, fingerToCubeVec, 0.5f * cubeLength, 0.5f * indexDiameter, fingerLine, lineColor);

        return shearForce;
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
        cubeVelocity = /*velocityPrev + */cubeAcceleration * Time.fixedDeltaTime;
        cubePosition = /**/positionPrev + cubeVelocity * Time.fixedDeltaTime;

        /*For Debugging*/
        drawCubeForceVector(cubeForce, cubePosition, cubeLineRenderer);

        cubeStatus[0] = cubePosition;
        cubeStatus[1] = cubeVelocity;
        cubeStatus[2] = cubeAcceleration;

        return cubeStatus;
    }

    public float[] getFingerPositionCommands(float indexForceMag, float thumbForceMag, int trialNumber)
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

    public Vector3 getVectorProjection(Vector3 vectorToProject, Vector3 distanceVec, Vector3 penetrationDirection)
    {
        //vector normal to the plane of instersection
        Vector3 normal = penetrationDirection;//Vector3.Dot(distanceVec, penetrationDirection) * penetrationDirection;
        float normalMag = Vector3.Magnitude(normal);

        //Shear
        return Vector3.Cross(normal, Vector3.Cross(vectorToProject, normal)) / Mathf.Pow(normalMag, 2.0f);
    }

    public void drawShearVector(Vector3 shearVector, Vector3 distanceVec, float cubeRadius,
        float fingerRadius, LineRenderer fingerLine, Color lineColor)
    {
        //vector distance between centers instersceting cubes
        float distanceMag = Vector3.Magnitude(distanceVec);
        Vector3 distanceVec_hat = distanceVec / Vector3.Magnitude(distanceVec);

        float cubeCenterToIntersectionPoint = (Mathf.Pow(cubeRadius, 2.0f) - Mathf.Pow(fingerRadius, 2.0f) + Mathf.Pow(distanceMag, 2.0f))
                                                                        / (2.0f * distanceMag);
        Vector3 intersectionPoint = cubePosition + cubeCenterToIntersectionPoint * distanceVec_hat;

        fingerLine.useWorldSpace = true;
        fingerLine.SetPosition(0, intersectionPoint);
        fingerLine.SetPosition(1, intersectionPoint + shearVector);
        fingerLine.SetWidth(0.005f, 0.005f);
        fingerLine.material.color = lineColor;
        lineColor.a = 0.5f;
    }

    /*
    public void drawFingerForceVector(Vector3 fingerForce, Vector3 distanceVec, float cubeRadius,
        float fingerRadius, LineRenderer fingerLine, Color lineColor)
    {
        //vector distance between centers instersceting cubes
        float distanceMag = Vector3.Magnitude(distanceVec);
        Vector3 distanceVec_hat = distanceVec / Vector3.Magnitude(distanceVec);

        float cubeCenterToIntersectionPoint = (Mathf.Pow(cubeRadius, 2.0f) - Mathf.Pow(fingerRadius, 2.0f) + Mathf.Pow(distanceMag, 2.0f))
                                                                        / (2.0f * distanceMag);
        Vector3 intersectionPoint = cubePosition + cubeCenterToIntersectionPoint * distanceVec_hat;

        fingerLine.useWorldSpace = true;
        fingerLine.SetPosition(0, intersectionPoint);
        fingerLine.SetPosition(1, intersectionPoint + fingerForce);
        fingerLine.SetWidth(0.005f, 0.005f);
        fingerLine.material.color = lineColor;
        lineColor.a = 0.5f;
    }*/

    public void drawCubeForceVector(Vector3 cubeForce, Vector3 cubePosition, LineRenderer cubeLine)
    {
        cubeLine.useWorldSpace = true;
        cubeLine.SetPosition(0, cubePosition);
        cubeLine.SetPosition(1, cubePosition + cubeForce);
        cubeLine.SetWidth(0.005f, 0.005f);
        cubeLine.material.color = Color.blue;
    }

    private Vector3 sign(Vector3 x)
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
}
