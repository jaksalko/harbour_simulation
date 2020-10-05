using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCPopup : MonoBehaviour
{
    public GameObject[] view;

    public RectTransform viewCategory; // m450 j270 c230 a230 i330

    private Transform GC;

    public static event System.Action Close;


    public void MachineButtonClicked()
    {
        for (int i = 0; i < view.Length; i++)
        {
            if (view[i].activeSelf)
            {
                view[i].SetActive(false);
                break;
            }
        }
        viewCategory.sizeDelta = new Vector2(400, 350);
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
        view[1].SetActive(true);
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
        view[2].SetActive(true);
    }
  
    public void GetGC(Transform target)
    {
        GC = target;
    }

    public void Exit()
    {
        Close?.Invoke();
        this.gameObject.SetActive(false);
    }
 
}
