using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Options")]
    public bool m_isDead = false;

    [Space]

    [Header("References")]
    public GameObject m_deathTextGO;

    [Space]

    [Header("Other")]
    public bool m_iControl = false;

    void Start() {
        m_deathTextGO.SetActive(false);
    }

    void Update() {
        if(m_iControl) {
            if(m_isDead) {
                m_deathTextGO.SetActive(true);

            } else {
                m_deathTextGO.SetActive(false);
            }
        }
    }

}
