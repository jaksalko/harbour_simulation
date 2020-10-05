using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VesselListMonitoring : MonoBehaviour
{
    public Dropdown dropdown;
    public InputField input;

    public RectTransform content;

    [Header("Prefab")]
    public GameObject vesselListItemPrefab;

    public void Search()
    {
        print("search");
        //Clear list
        ClearResult();

        //Get search type and input text
        //Get Filtering toggle list

        //Set Result
        SetResult();
    }
    public void ClearResult()
    {
        int resultsize = content.childCount;

        for (int i = 0; i < resultsize; i++)
        {
            GameObject listitem = content.GetChild(i).gameObject;


            Destroy(listitem);
        }
    }
    public void SetResult()
    {
        int testitem = Random.Range(0, 15);
        for (int i = 0; i < testitem; i++)
        {
            GameObject item = Instantiate(vesselListItemPrefab, default(Vector3), Quaternion.identity);
            item.transform.SetParent(content);
        }
    }
    public void Default()
    {
        print("default");

    }
}