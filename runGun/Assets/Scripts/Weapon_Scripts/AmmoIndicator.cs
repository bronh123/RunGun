using UnityEngine;

public class AmmoIndicator : MonoBehaviour
{

    public AmmoTracker weapon;
    public int ammo_count;

    // Update is called once per frame
    void Update()
    {
        if (weapon.getAmmo() >= ammo_count) {
            GetComponent<Renderer>().enabled = true;
        } else {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
