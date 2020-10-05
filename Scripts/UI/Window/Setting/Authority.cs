using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Authority : MonoBehaviour
{
    public AuthorityItem newitem;
    public GameObject buttonArea;
    public Transform content;

    public List<AuthorityItem> accounts;

   
    public GameObject editGroupPopup;

    

    

    public void EditGroup()
    {
        editGroupPopup.SetActive(true);
    }

    public void Edit()
    {
        var newAuth = Instantiate(newitem, transform.position, Quaternion.identity);
        accounts.Add(newAuth);
        newAuth.transform.SetParent(content);
        
        for (int i = 0; i < accounts.Count; i++)
        {
            accounts[i].EditMode();
        }
        buttonArea.SetActive(true);

    }

    public void Delete()
    {
        for(int i = 0;  i < accounts.Count; i++)
        {
            accounts[i].DeleteMode();
        }
    }

    public void Apply()
    {
        for (int i = 0; i < accounts.Count; i++)
        {
            accounts[i].SaveEdit();
        }
        buttonArea.SetActive(false);
    }

    public void Cancel()
    {
        buttonArea.SetActive(false);
    }
}
