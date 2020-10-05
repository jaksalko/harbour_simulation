using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuList : MonoBehaviour
{
    public GameObject[] menulists;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenulistClicked(int listnum)
    {
        for(int i = 0; i < menulists.Length; i++)
        {
            menulists[i].SetActive(false);
        }
        menulists[listnum].SetActive(true);

       
    }

    public void MenuListItemClicked(GameObject gameObject)
    {
        GameObject itemParent = gameObject.transform.parent.gameObject;
        itemParent.SetActive(false);
    }
}
