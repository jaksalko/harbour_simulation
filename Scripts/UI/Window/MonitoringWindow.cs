using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonitoringWindow : MonoBehaviour
{
    public GameObject[] listpanels;//job , alarm , equip , otr , vessel

    public void SlideList(int num)
    {
        for(int i = 0; i < listpanels.Length; i++)
        {
            if (listpanels[i].activeSelf && i != num)
            {
                listpanels[i].SetActive(false);
                break;
            }

            
                
        }

        listpanels[num].SetActive(true);
    }


    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
