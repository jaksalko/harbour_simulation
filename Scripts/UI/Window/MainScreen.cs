using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainScreen : MonoBehaviour
{

    public RectTransform menuList;
    public GameObject toolbar;
    public GameObject menutbtn;
    Vector3 defaultposition;


    public UnityTemplateProjects.SimpleCameraController mainCamera;
    public GameObject treeview;
    public FocusCameraController focusView;
    public GameObject summaryViewPopup;
    private void Awake()
    {
        defaultposition = toolbar.GetComponent<RectTransform>().position;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            toolbar.SetActive(!toolbar.activeSelf);
            menutbtn.SetActive(!menutbtn.activeSelf);
        }
    }
    public void MenuClicked()
    {
        GameObject[] list = menuList.GetComponent<MenuList>().menulists;
        

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i].activeSelf)
            {
                list[i].SetActive(false);
                break;
            }
        }
        menuList.gameObject.SetActive(!menuList.gameObject.activeSelf);
        if(menuList.gameObject.activeSelf)
        {
            toolbar.GetComponent<RectTransform>().position = toolbar.GetComponent<RectTransform>().position - Vector3.up*menuList.rect.height;
        }
        else
        {
            toolbar.GetComponent<RectTransform>().position = toolbar.GetComponent<RectTransform>().position + Vector3.up * menuList.rect.height;
        }

    }

    public void _3DView()
    {
        focusView.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        mainCamera.lockRotation = false;
        
    }
    public void _2DView()
    {
        focusView.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        mainCamera.TopView();
        
        Debug.Log(mainCamera.transform.rotation.eulerAngles);
        mainCamera.lockRotation = true;
    }
    public void TreeView()
    {
        //focusView.gameObject.SetActive(false);
        treeview.SetActive(!treeview.activeSelf);
    }
    public void SummaryView()
    {
        summaryViewPopup.SetActive(true);
    }
}
