using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingplayerByPass : MonoBehaviour
{

    public GameObject[] thingsToEnable;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject thing in thingsToEnable) {
            thing.SetActive(true);
        }
    }
}
