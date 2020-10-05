using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField id;

    public InputField pwd;
    public Text warningMsg;

    public string[] id_arr;
    public string[] pwd_arr;

    public GameObject fade;

    public void Awake()
    {
        Screen.SetResolution(800, 450, false);
    }



    public void AccessLogin()
    {
        for(int i = 0; i < id_arr.Length; i++)
        {
            if(id.text == id_arr[i] && pwd.text == pwd_arr[i])
            {
                SceneManager.LoadScene(1);
                fade.SetActive(true);
                Screen.SetResolution(1920, 1080, true);
                return;
            }
          
                
            
        }
        warningMsg.text = "계정 ID 혹은 PWD가 틀렸습니다. 다시 확인해주세요";


    }

    public void Exit()
    {
        //application quit
    }
}
