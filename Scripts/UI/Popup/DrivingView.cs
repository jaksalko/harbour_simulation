using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrivingView : MonoBehaviour
{
    private Transform TargetITV;
    public Camera Cam;
    public RawImage ri;
    private RenderTexture rt;
    

    private void Awake()
    {
       
    }

    public void SetCam(Transform target)
    {
        Cam.transform.SetParent(target);
        Cam.transform.localPosition = new Vector3(0, 7.5f, -6.5f);
        Cam.transform.localRotation = Quaternion.Euler(new Vector3(26f, 0, 0));
        if (!transform.gameObject.activeSelf)
            transform.gameObject.SetActive(true);

        rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        rt.Create();
        Cam.targetTexture = rt;
        ri.texture = rt;

    }
}
