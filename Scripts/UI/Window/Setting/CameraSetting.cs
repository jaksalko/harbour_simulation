using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CameraSetting : MonoBehaviour
{
    public CameraRAY cam;


    public Dropdown view;
    public InputField[] inputPosition;
    public InputField[] inputRotation;

    int selectedView = 0;

    public void ViewNumberClick(int num)
    {
        selectedView = num;
        SetInputField(cam.viewPoints[selectedView].cameraPosition, cam.viewPoints[selectedView].cameraRotation);
        if (cam.viewPoints[selectedView].lockrotation)
        {
            view.value = 1;//2d
        }
        else
        {
            view.value = 0;
           
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        view.onValueChanged.AddListener(
            isSelected => { IsLocked(isSelected); }
            );

        SetInputField(cam.viewPoints[selectedView].cameraPosition, cam.viewPoints[selectedView].cameraRotation);

        if (cam.viewPoints[selectedView].lockrotation)
        {
            view.value = 1;//2d
        }
        else
        {
            view.value = 0;

        }

    }

    void IsLocked(int num)
    {
        if(num == 0)//3d
        {
            for (int i = 0; i < 3; i++)
            {
                inputRotation[i].interactable = true;
            }
        }
        else
        {

            inputRotation[0].text = "90";
            inputRotation[1].text = "0";
            inputRotation[2].text = "0";
            for (int i = 0; i < 3; i++)
            {
                inputRotation[i].interactable = false;
            }
        }
    }


    public void Apply()
    {
        if (view.value == 0)
            cam.viewPoints[selectedView].lockrotation = false;
        else
            cam.viewPoints[selectedView].lockrotation = true;

        cam.viewPoints[selectedView].cameraPosition
            = new Vector3(float.Parse(inputPosition[0].text), float.Parse(inputPosition[1].text), float.Parse(inputPosition[2].text));
        cam.viewPoints[selectedView].cameraRotation 
            = new Vector3(float.Parse(inputRotation[0].text), float.Parse(inputRotation[1].text), float.Parse(inputRotation[2].text));


    }

    public void Default()
    {

    }
 

    public void GetCurrentCamera()
    {
        Transform cameraTransform = cam.main.transform;

        SetInputField(cameraTransform.position, cameraTransform.rotation.eulerAngles);


        if(cam.main.lockRotation)
            view.value = 1;//2d
        else       
            view.value = 0;//3d
        

        
    }



    void SetInputField(Vector3 pos, Vector3 rot)
    {
        inputPosition[0].text = pos.x.ToString();
        inputPosition[1].text = pos.y.ToString();
        inputPosition[2].text = pos.z.ToString();

        inputRotation[0].text = rot.x.ToString();
        inputRotation[1].text = rot.y.ToString();
        inputRotation[2].text = rot.z.ToString();
    }
}
