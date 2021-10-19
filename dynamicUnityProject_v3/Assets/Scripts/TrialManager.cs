using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
    #region
    float height = 0.05f;

    /**** Create STARTINGAREA *****/
    GameObject startingArea; //Instatiate StartingArea GameObject
    MeshRenderer startingAreaMeshRenderer; //Declares Mesh Renderer

    [Header("Starting Area Variables")]
    public Vector3 startingAreaPosition;
    public float startingAreaRadius = 0.2f;
    public float startingAreaHeight;
    public float targetOffset = 0.40f;

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

    /**** Timing *****/
    //use task success/fails
    [Header("Timing")]
    public float timeOfCurrentSuccess = 0.0f;
    public float timeSinceLastSuccess = 0.0f;

    public int timeIndex = 0;
    public float[] elapsedTimes;

    /**** Trial Info *****/
    [Header("Trial Variables")]
    public int trialNumber = 1; //the actual trial being worked on
    public int trialState = 0; //the actual trial being worked on
    public int numTrials = 4; // the total number of trials the user will participate in
    public int numElapsedTimes = 3; //num of attempts for each trial

    public int successCounter = 0; //number of successful moves within a trial
    public int failCounter = 0;
    #endregion

    void Start()
    {
        //Set up timing saving
        elapsedTimes = new float[numElapsedTimes * numTrials];

        trialNumber = 1;
    }

    void Update()
    {
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
            GetComponent<ColorManager>().setObjectColor(waypointMeshRenderer, 0.5f, 0.5f, 0.0f, 1.0f);
        }
        else
        {
            //Keep translucent
            GetComponent<ColorManager>().setObjectColor(waypointMeshRenderer, 0.5f, 0.5f, 0.0f, 0.5f);
        }
    }

    public void createStartingArea(float startingX, float startingZ)
    {
        //Starting Area Object Definition
        startingArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(startingArea.GetComponent<CapsuleCollider>());

        //Name:
        startingArea.name = "StartingArea";

        //Set Size and Initial Position
        startingAreaHeight = 0.5f * height;
        startingArea.transform.localScale = new Vector3(startingAreaRadius, startingAreaHeight, startingAreaRadius);
        startingAreaPosition = new Vector3(startingX, startingAreaHeight, startingZ);
        startingArea.transform.position = startingAreaPosition;

        //Set mesh properties
        startingAreaMeshRenderer = startingArea.GetComponent<MeshRenderer>();

        //Set Color and Transparency
        GetComponent<ColorManager>().setObjectColor(startingAreaMeshRenderer, 0.5f, 0.0f, 1.0f, 0.5f);

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
        targetAreaHeight = 0.5f * height;
        targetArea.transform.localScale = new Vector3(targetAreaRadius, targetAreaHeight, targetAreaRadius);
        targetAreaPosition = new Vector3(startingX, targetAreaHeight, startingZ - targetOffset);
        targetArea.transform.position = targetAreaPosition;

        //Set mesh properties
        targetAreaMeshRenderer = targetArea.GetComponent<MeshRenderer>();

        //Set Color and Transparency
        GetComponent<ColorManager>().setObjectColor(targetAreaMeshRenderer, 0.4f, 0.6f, 0.1f, 0.5f);
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
        GetComponent<ColorManager>().setObjectColor(waypointMeshRenderer, 0.5f, 0.5f, 0.0f, 0.5f);
    }

    public void trialLogic(bool indexContact, bool thumbContact, Vector3 cubePosition, float indexDistToCenter)
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
                GetComponent<GameMaster>().resetCube();
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
                GetComponent<GameMaster>().resetCube();
            }
            //if cube lands in target before passing waypoint --> fail (not followed path)
            if (checkIsCubeInTarget(GetComponent<GameMaster>().cubePosition, GetComponent<GameMaster>().cubeLength) == true)
            {
                //Fail
                failCounter++;

                //Reset
                GetComponent<GameMaster>().resetCube();
            }
            //if passed waypoint
            if (checkWaypoint(GetComponent<GameMaster>().cubePosition, GetComponent<GameMaster>().cubeLength) == true)
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
                GetComponent<GameMaster>().resetCube();
            }
            //if landed in target after passing waypoint
            if (checkIsCubeInTarget(GetComponent<GameMaster>().cubePosition, GetComponent<GameMaster>().cubeLength) == true)
            {
                //Success
                successCounter++;
                GetComponent<GameMaster>().resetCube();
            }
        }
    }

    public bool checkWaypoint(Vector3 cubePosition, float cubeLength)
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

    public void checkTrialProgress(float startingX, float startingZ, float targetOffset)
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
                GetComponent<GameMaster>().resetCube();
            }
        }
    }

    public bool checkIsCubeInTarget(Vector3 cubePosition, float cubeLength)
    {
        //Check if cube is inside of targetArea by calculating 
        //the distance between target area center and cube center
        targetToCubeDist = Vector3.Magnitude(targetAreaPosition - cubePosition);

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
}
