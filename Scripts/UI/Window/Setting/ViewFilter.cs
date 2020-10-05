using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewFilter : MonoBehaviour
{
    public Toggle[] equipmentFilter;
    public Toggle gangFilter;
    public Toggle vesselFilter;

    public ControlTower ct;

    bool[] isRender;

    private void Start()
    {
        equipmentFilter[0].onValueChanged.AddListener(
            isOn =>{
                if (isOn)
                {
                    for (int i = 1; i < equipmentFilter.Length; i++)
                    {
                        equipmentFilter[i].isOn = true;
                    }
                }
                else
                {
                    for (int i = 1; i < equipmentFilter.Length; i++)
                    {
                        equipmentFilter[i].isOn = false;
                    }
                }
            });

    }

    public void ApplyButtonClicked()
    {
        for(int i = 1; i < equipmentFilter.Length; i++)
        {

            RenderDisable(ct.objectFilter[i - 1].transform , equipmentFilter[i].isOn);

        }
    }

    public void DefaultButtonClicked()
    {      
        equipmentFilter[0].isOn = true;
        gangFilter.isOn = true;
        vesselFilter.isOn = true;
    }


    private void RenderDisable(Transform t, bool check)
    {
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                RenderDisable(child, check);
            }
        }
        if (t.gameObject.GetComponent<Renderer>())
        {
            t.gameObject.GetComponent<Renderer>().enabled = check;
        }
       
    }
}
