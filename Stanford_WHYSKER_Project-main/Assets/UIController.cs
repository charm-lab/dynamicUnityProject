using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject setupMenu;
    public GameObject needleMenu;
    public GameObject palpatingMenu;
    public GameObject BLEMenu;

    public GameObject ribAsset;
    public GameObject bodyAsset;
    public GameObject trakSTARAsset;
    public GameObject toolAsset;
    public GameObject handColliderAsset;
    public GameObject hapticVisualizer;

    //public GameObject handColliders;
    //public GameObject needleColliders;

    void Start()
    {
        BLEMenu.SetActive(true);
        setupMenu.SetActive(false);
        needleMenu.SetActive(false);
        palpatingMenu.SetActive(false);

        ribAsset.SetActive(false);
        bodyAsset.SetActive(false);
        trakSTARAsset.SetActive(true);
        toolAsset.SetActive(false);
        handColliderAsset.SetActive(false);
        hapticVisualizer.SetActive(false);
    }

    //SETUPMODE
    public void EnterSetup()
    {
        setupMenu.SetActive(true);
        needleMenu.SetActive(false);
        palpatingMenu.SetActive(false);

        ribAsset.SetActive(false);
        bodyAsset.SetActive(false);
        trakSTARAsset.SetActive(true);
        toolAsset.SetActive(false);
        handColliderAsset.SetActive(false);
        hapticVisualizer.SetActive(false);

        //needleColliders.GetComponent<NeedleCollision>().enabled = false;
        //handColliders.GetComponent<HandCollision>().enabled = false;
    }

    public void EnterBLE()
    {
        BLEMenu.SetActive(true);

        setupMenu.SetActive(false);
        needleMenu.SetActive(false);
        palpatingMenu.SetActive(false);

        ribAsset.SetActive(false);
        bodyAsset.SetActive(false);
        trakSTARAsset.SetActive(false);
        toolAsset.SetActive(false);
        handColliderAsset.SetActive(false);
        hapticVisualizer.SetActive(false);
    }

    public void trakSTARZeroing()
    {
        trakSTARController.instance.trakSTARPoseSet();
    }

    public void EnterNeedleMode()
    {
        setupMenu.SetActive(false);
        needleMenu.SetActive(true);
        palpatingMenu.SetActive(false);

        ribAsset.SetActive(true);
        bodyAsset.SetActive(true);
        trakSTARAsset.SetActive(false);
        toolAsset.SetActive(true);
        handColliderAsset.SetActive(false);
        hapticVisualizer.SetActive(true);

        //needleColliders.GetComponent<NeedleCollision>().enabled = true;
        //handColliders.GetComponent<HandCollision>().enabled = false;
    }

    public void EnterPalpatingMode()
    {
        setupMenu.SetActive(false);
        needleMenu.SetActive(false);
        palpatingMenu.SetActive(true);

        ribAsset.SetActive(true);
        bodyAsset.SetActive(true);
        trakSTARAsset.SetActive(false);
        toolAsset.SetActive(false);
        handColliderAsset.SetActive(true);
        hapticVisualizer.SetActive(true);

        //needleColliders.GetComponent<NeedleCollision>().enabled = false;
        //handColliders.GetComponent<HandCollision>().enabled = true;
    }


    //NEEDLE MODE
    public void SetWristWornXHaptic(int typeNumber)
    {
        TrainerController.instance.XHapticsType = typeNumber;
        Debug.Log("Set wristworn X to ");
    }

    public void SetWristWornYHaptic(int typeNumber)
    {
        TrainerController.instance.YHapticsType = typeNumber;
        Debug.Log("Set wristworn Y to ");
    }

    //PALPATING MODE
    

    //BODY CONTROLS
    public void SetBodyAssetMaterial(Material material)
    {
        bodyAsset.GetComponentInChildren<MeshRenderer>().material = material;
    }
}
