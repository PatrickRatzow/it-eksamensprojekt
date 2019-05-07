using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraControl : NetworkBehaviour
{

    [Header("Camera Options")] 

    public float m_rotationSpeed = 1f;
    GameObject m_jeffGameObject;
    GameObject m_jeffCam;
    GameObject m_tonyGameObject;
    GameObject m_tonyCam;

    private void Start() {
        m_jeffGameObject = GameObject.Find("Jeff");
        m_jeffCam = GameObject.Find("JeffCamera");

        m_tonyGameObject = GameObject.Find("Tony");
        m_tonyCam = GameObject.Find("TonyCamera");
    }
    private void Update() {
        if(GameObject.Find("GameManager").GetComponent<PlayerManager>().m_playingMultiplayer) {
            //playing multiplayer
            if(isServer) {
                //If host
                m_jeffCam.SetActive(true);
                m_tonyCam.SetActive(false);

                m_jeffCam.transform.localPosition = Vector3.Lerp(m_jeffCam.transform.localPosition, new Vector3(0,0,0), 0.05f);

                RotateCamera(1);
            } else {
                //if client

                m_jeffCam.SetActive(false);
                m_tonyCam.SetActive(true);

                m_tonyCam.transform.localPosition = Vector3.Lerp(m_tonyCam.transform.localPosition, new Vector3(0,0,0), 0.05f);

                RotateCamera(2);
            }
        } else {
            //playing singleplayer

            if(GameObject.FindGameObjectWithTag("Player") != null) {
                if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controlling == 1) {
                    m_jeffCam.SetActive(true);
                    m_tonyCam.SetActive(false);

                    m_jeffCam.transform.position = Vector3.Lerp(m_jeffCam.transform.position, m_jeffGameObject.transform.position + new Vector3(0,0.75f,0),0.05f);
                    m_tonyCam.transform.position = Vector3.Lerp(m_tonyCam.transform.position, m_jeffGameObject.transform.position + new Vector3(0,0.75f,0),0.05f);


                    RotateCamera(1);
                } else if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controlling == 2) {
                    m_jeffCam.SetActive(false);
                    m_tonyCam.SetActive(true);

                    m_jeffCam.transform.position = Vector3.Lerp(m_jeffCam.transform.position, m_tonyGameObject.transform.position + new Vector3(0,0.75f,0),0.05f);
                    m_tonyCam.transform.position = Vector3.Lerp(m_tonyCam.transform.position, m_tonyGameObject.transform.position + new Vector3(0,0.75f,0),0.05f);

                    RotateCamera(2);
                }
            }
        }

    }

    private void RotateCamera(int playerId) {
        float rotY = -Input.GetAxis("Mouse Y") * m_rotationSpeed;
        float rotX = Input.GetAxis("Mouse X") * m_rotationSpeed;

        if(playerId == 1) {
            m_jeffCam.transform.Rotate(new Vector3(rotY,0,0));
            m_jeffGameObject.transform.Rotate(new Vector3(0,rotX,0));

        } else if(playerId == 2) {
            m_tonyCam.transform.Rotate(new Vector3(rotY,0,0));
            m_tonyGameObject.transform.Rotate(new Vector3(0,rotX,0));

        }
    }
}
