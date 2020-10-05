using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPSettingPopup : MonoBehaviour
{

    // Start is called before the first frame update
    public void SaveIP_Port()
    {
        print("save ip & port");
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Default_IP_Port()
    {
        print("set default ip & port");
    }
}
