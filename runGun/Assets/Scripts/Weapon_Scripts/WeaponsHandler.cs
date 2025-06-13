using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponsHandler : MonoBehaviour
{

    private int curr_weapon_index = 0;
    GameObject[] WeaponsArray;
    // make private
    public GameObject gun_1;
    public GameObject gun_2;


    void Start()
    {
        WeaponsArray = new GameObject[2] { gun_1, gun_2 };
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(1);
        }
        // scroll up and down to change weapons
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            EquipWeapon((curr_weapon_index + 1) % WeaponsArray.Length);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            EquipWeapon((curr_weapon_index - 1 + WeaponsArray.Length) % WeaponsArray.Length);
        }
    }

    // Update is called once per frame
    public void EquipWeapon(int index)
    {
        print("Equipping weapon " + index + " from " + curr_weapon_index);
        if (index != curr_weapon_index && index >= 0 && index < WeaponsArray.Length)
        {
            WeaponsArray[curr_weapon_index].SetActive(false);
            curr_weapon_index = index;
            WeaponsArray[curr_weapon_index].SetActive(true);
        }
    }
}
