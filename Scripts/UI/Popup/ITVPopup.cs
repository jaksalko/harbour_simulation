using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITVPopup : MonoBehaviour
{
    public GameObject[] view;
   
    public GameObject buttonArea2;
    
    public RectTransform viewCategory; // m450 j270 c230 a230 i330

    public DrivingView Div;

    private Transform Itv;

    public static event System.Action Close;



    public void MachineButtonClicked()
    {
        for(int i = 0; i < view.Length; i++)
        {
            if(view[i].activeSelf)
            {
                view[i].SetActive(false);
                break;
            }
        }
        viewCategory.sizeDelta = new Vector2(400,450);
        buttonArea2.SetActive(true);
        view[0].SetActive(true);

    }
    public void JobButtonClicked()
    {
        for (int i = 0; i < view.Length; i++)
        {
            if (view[i].activeSelf)
            {
                view[i].SetActive(false);
                break;
            }
        }
        viewCategory.sizeDelta = new Vector2(400, 270);
        buttonArea2.SetActive(true);
        view[1].SetActive(true);
    }
    public void ContainerButtonClicked()
    {
        for (int i = 0; i < view.Length; i++)
        {
            if (view[i].activeSelf)
            {
                view[i].SetActive(false);
                break;
            }
        }
        viewCategory.sizeDelta = new Vector2(400, 230);
        buttonArea2.SetActive(true);
        view[2].SetActive(true);
    }
    public void AlarmButtonClicked()
    {
        for (int i = 0; i < view.Length; i++)
        {
            if (view[i].activeSelf)
            {
                view[i].SetActive(false);
                break;
            }
        }
        viewCategory.sizeDelta = new Vector2(400, 230);
        buttonArea2.SetActive(true);
        view[3].SetActive(true);
    }
    public void IMClicked()
    {
        for (int i = 0; i < view.Length; i++)
        {
            if (view[i].activeSelf)
            {
                view[i].SetActive(false);
                break;
            }
        }
        viewCategory.sizeDelta = new Vector2(400, 330);
        buttonArea2.SetActive(false);
        view[4].SetActive(true);
    }
    public void GetITV(Transform target)
    {
        Itv = target;
    }
    public void DriverViewClicked()
    {
        if(!Div.gameObject.activeSelf)
        {
            Div.SetCam(Itv);
            Div.gameObject.SetActive(true);
        }
        else
        {
            Div.gameObject.SetActive(false);
        }
        

    }
    public void Exit()
    {
        Close?.Invoke();
        this.gameObject.SetActive(false);
    }
    public void SendIM()
    {
        this.gameObject.SetActive(false);

    }
}
