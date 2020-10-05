using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCanvas : MonoBehaviour
{
    [Header("Window")]
    public MonitoringWindow monitoringWindow;

    public SettingWindow settingWindow;
    //public GameObject treeWindow;

    public void MonitoringListClicked(int num)
    {
        if (!monitoringWindow.gameObject.activeSelf)
        {
            monitoringWindow.gameObject.SetActive(true);
            monitoringWindow.SlideList(num);
        }

    }

    public void SettingListClicked(int num)
    {
        if (!settingWindow.gameObject.activeSelf)
        {
            settingWindow.gameObject.SetActive(true);
            settingWindow.radio[num].isOn = true;
        }
    }
}
