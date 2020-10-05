using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthorityGroupSetting : MonoBehaviour
{

    public class Group
    {
        public string groupName;
        public List<int> authorityList;

        public Group(string name , int[] authority)
        {
            groupName = name;
            authorityList = new List<int>();
            authorityList.AddRange(authority);
        }

        public Group(string name)
        {
            groupName = name;
            authorityList = new List<int>();
        }
    }
    public int selectedGroup;
    int selected;
   
    public List<Group> groups = new List<Group>();
    public InputField newGroupName;
    public Dropdown groupDropdown;

    public Toggle[] authorityList;

    int[] adminList = {  1, 2, 3, 4, 5, 6 ,7 };
    int[] guestList = {  1, 2, 3, 4 , 5};
    // Start is called before the first frame update
    void Awake()
    {
        /*{
            "OTR List Viewer" , "Rewinder" , "Screen Capture" , "SnapShot" , "Instance Message"
           ,"View Filter" , "Setup Configuration"};*/

        //virtual group admin && guest
       
        Group admin = new Group("admin", adminList);
        Group guest = new Group("guest", guestList);
        selectedGroup = 0;
        selected = 0;
        groups.Add(admin);
        groups.Add(guest);

        groupDropdown.onValueChanged.AddListener(
            isSelected =>
            {
                selected = isSelected;
                for(int i = 0; i < authorityList.Length; i++)
                {
                    authorityList[i].isOn = false;
                }

                for(int i = 0; i < groups.Count; i++ )
                {
                    if(groups[i].groupName == groupDropdown.options[isSelected].text)
                    {
                        selectedGroup = i;
                        for(int j = 0; j < groups[i].authorityList.Count ; j++)
                        {
                            int auth = groups[i].authorityList[j];
                            authorityList[auth].isOn = true;

                        }
                    }
                }
            }
            );

    }

    public void AddGroupButtonClick()
    {
        Group newGroup = new Group(newGroupName.text);
        groups.Add(newGroup);
        Dropdown.OptionData option = new Dropdown.OptionData();
        option.text = newGroup.groupName;


        groupDropdown.options.Add(option);
    }

    public void DefaultButtonClick()
    {

    }

    public void DeleteGroupClick()
    {
        groups.RemoveAt(selectedGroup);
        groupDropdown.options.RemoveAt(selected);
        groupDropdown.value = selected - 1;
    }

    public void ApplyButtonClick()
    {
     
        groups[selectedGroup].authorityList.Clear();
        for(int i = 0; i < authorityList.Length; i++)
        {            
            if(authorityList[i].isOn)
            {
                groups[selectedGroup].authorityList.Add(i);
            }
        }


        gameObject.SetActive(false);
    }

    public void CancelButtonClick()
    {
        gameObject.SetActive(false);
    }
}
