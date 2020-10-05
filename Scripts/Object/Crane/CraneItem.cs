using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneItem : MonoBehaviour
{
    public Crane crane;
    public Text craneName;
    public CameraRAY ray;

    public void CraneItemClicked()
    {
        //clicked item event
    }

    public void Initialize(Crane crane)
    {
        this.crane = crane;
        craneName.text = crane.name;
    }

    public void CraneTargeting()
    {

        if (crane.name.Contains("TC"))
        {
            //tcw = FindObjectOfType<TCWindow>();
            ray.callTCWindow(crane.transform);
        }
            
        else
        {
            //gcw = FindObjectOfType<GCWindow>();
            ray.callGCWindow(crane.transform);
        }
            



    }
}
