using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AuthorityItem : MonoBehaviour
{
    public Button delete;
    public InputField id;
    public InputField pwd;
    public InputField profile;
    public Dropdown group;

    public static event System.Action<AuthorityItem> deleteAccount;
    

    public void EditMode()
    {
        
        id.interactable = true;
        pwd.interactable = true;
        profile.interactable = true;
        group.interactable = true;
    }
    public void SaveEdit()
    {
        id.interactable = false;
        pwd.interactable = false;
        profile.interactable = false;
        group.interactable = false;
    }
    public void DeleteMode()
    {
        delete.interactable = true;
    }
    public void DestroyAccount()//click delete button event...
    {
        deleteAccount?.Invoke(this);
    }

}
