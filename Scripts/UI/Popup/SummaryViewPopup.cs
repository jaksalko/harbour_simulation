using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryViewPopup : MonoBehaviour
{
    public Toggle all;
    public Toggle[] summaryList;

    public SummaryView summary;

    private void Awake()
    {
        all.onValueChanged.AddListener(
           isOn => {
               if (isOn)
               {
                   for (int i = 0; i < summaryList.Length; i++)
                   {
                       summaryList[i].isOn = true;
                   }
               }
               else
               {
                   for (int i = 0; i < summaryList.Length; i++)
                   {
                       summaryList[i].isOn = false;
                   }
               }
           }
           );


    }

    public void Exit()
    {

        gameObject.SetActive(false);      
    }
    public void Apply()
    {

        for (int i = 0; i < summaryList.Length; i++)
        {      
            summary.viewList[i].SetActive(summaryList[i].isOn);

        }
    }
    
}
