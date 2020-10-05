using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class FocusCameraController : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    public Transform cameraPosition;
    public float distance;
    Vector3 distanceVector;
    public float sidewalk;
    public float height;
    public float sensitivity;
    // Start is called before the first frame update


    public GameObject isFocus;
    private void Awake()
    {
        this.ObserveEveryValueChanged(_ => _.gameObject.activeSelf)
            .Subscribe(_ => isFocus.SetActive(_));
    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float horiziontal = Input.GetAxis("Mouse X");
            float vertical = Input.GetAxis("Mouse Y");
            sidewalk += horiziontal * sensitivity;
            height += vertical * sensitivity*5;
        }
        
        //target.position = Vector3.MoveTowards(target.position, target.position + dir, speed * Time.deltaTime);
    }
    void CameraWalk()
    {
        //runningTime += Time.deltaTime * speed;
        float z = distance * Mathf.Cos(sidewalk);
        float x = distance * Mathf.Sin(sidewalk);

        distanceVector = new Vector3(x, height, z);
        //Debug.Log(distanceVector);
    }
    private void LateUpdate()
    {
        //distanceVector = Vector3.one * distance;
        CameraWalk();
        cam.transform.position = cameraPosition.position + distanceVector;
        cam.transform.LookAt(target);
    }

}
