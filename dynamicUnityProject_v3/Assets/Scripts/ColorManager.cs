using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public void setObjectColor(MeshRenderer mesh, float r, float g, float b, float alpha)
    {
        mesh.material.SetColor("_Color", new Color(r, g, b, alpha));
        mesh.material.SetFloat("_Mode", 3);
        mesh.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mesh.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mesh.material.EnableKeyword("_ALPHABLEND_ON");
        mesh.material.renderQueue = 3000;
    }
}
