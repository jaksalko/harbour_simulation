using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingWindow : MonoBehaviour
{
    public GameObject[] listpanels;//view filter , hotkey , camera , authority

    public Button[] settingButton;
    public List<Toggle> radio;
   
    public Color pressedColor;
    public Color normalColor;
    private void Awake()
    {
        
        foreach(Toggle tog in radio)
        {
            

            tog.onValueChanged.AddListener(
            isOn => {
                if (isOn)
                {
                    tog.GetComponent<Image>().color = pressedColor;
                    for(int i = 0; i < 4; i++)
                    {
                        if (tog == radio[i])
                            SlideList(i);
                    }
                }
                else
                {
                    tog.GetComponent<Image>().color = normalColor;

                }
            });
        }
        
    }
  

    public void SlideList(int num)
    {
        //radio[num].isOn = true;
        for (int i = 0; i < listpanels.Length; i++)
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
