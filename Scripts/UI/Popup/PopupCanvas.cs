using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCanvas : MonoBehaviour
{
    [Header("Popup")]
    public GameObject exitPopup;
    public GameObject ipSettingPopup;
    public GameObject instanceMEssage;
    public DeletePopup deletePopup;
    public GameObject authoritySettingPopup;
    public GameObject kpiPopup;

    private void Awake()
    {
        AuthorityItem.deleteAccount += ActivatePopup;
    }
    public void ActivatePopup(AuthorityItem auth)
    {
        deletePopup.auth = auth;
        deletePopup.gameObject.SetActive(true);
        
    }
    public void ExitClicked()
    {
        if (!exitPopup.activeSelf)
            exitPopup.SetActive(true);
    }

    public void IPSettingClicked()
    {
        if (!ipSettingPopup.activeSelf)
            ipSettingPopup.SetActive(true);
    }

    public void InstanceMessageClicked()
    {
        if (!instanceMEssage.activeSelf)
            instanceMEssage.SetActive(true);
    }
    public void KPIClicked()
    {
        if(!kpiPopup.activeSelf)
            kpiPopup.SetActive(true);
    }
}
