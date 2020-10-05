using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePopup : MonoBehaviour
{
    public AuthorityItem auth;
    public Authority authority;
    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void DeleteAccount()
    {
        authority.accounts.Remove(auth);
        Destroy(auth.gameObject);
        
        gameObject.SetActive(false);
    }
}
