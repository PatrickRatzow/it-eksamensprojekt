using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {
    [Header("UI")] 
    public RectTransform m_abilities;

    [Header("Jeff Options")]
    public float m_jeffSpeed = 10f;
    public float m_jeffJumpForce = 300f;

    private GameObject m_jeffGameObject;
    private Rigidbody m_jeffRb;
    private Vector3 m_jeffPosition;
    private Quaternion m_jeffRotation;

    [Space]


    [Header("Tony Options")]
    public float m_tonySpeed = 10f;
    public float m_tonyJumpForce = 350f;

    private GameObject m_tonyGameObject;
    private Rigidbody m_tonyRb;
    private Vector3 m_tonyPosition;
    private Quaternion m_tonyRotation;
    private Animator m_tonyAnim;
    private Animator m_jeffAnim;

    public int controlling = 1;

    [Space]

    [Header("Both Options")]
    public float distToGroundBeforeJump = 1.1f;
       

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void UpdateUI() {
        int childrenCount = m_abilities.gameObject.transform.childCount;
        int characterId = controlling;
        Abilities abilities = GetComponent<Abilities>();
        for (int i = 0; i < childrenCount; i++) {
            Transform child = m_abilities.gameObject.transform.GetChild(i);


            float time;
            float cooldown;
            if (characterId == 1) {
                time = abilities.m_jeffCooldownUsed[i];
                cooldown = abilities.m_jeffCooldownTimes[i];
            } else {
                time = abilities.m_tonyCooldownUsed[i];
                cooldown = abilities.m_tonyCooldownTimes[i];               
            }
            
            float min = (time - Time.time);
            float max = cooldown;
            string text = "";
            float frac = Mathf.Clamp(min / max, 0f, 1f);

            Text textObj = child.GetComponentInChildren<Text>();
            Image img = child.GetComponentInChildren<Image>();
                
            if (Time.time < time) {
                text = Math.Round(time - Time.time, 1).ToString() + "s";
                textObj.fontSize = 40;
                textObj.color = new Color(0.9f, 0.9f, 0.9f);
                img.color = new Color(0f, 0f, 0f, 100f / 255f);
            }
            else {
                text = (i + 1).ToString();
                textObj.fontSize = 60;
                textObj.color = new Color(0.9f, 0.9f, 0.9f);
                img.color = new Color(46f / 255f, 204f / 255f, 113 / 255f);
            }

            RectTransform background = child.GetComponentsInChildren<RectTransform>()[1];
            background.sizeDelta = new Vector2(background.sizeDelta.x, frac * 90);
            background.position = new Vector3(background.position.x, frac * 74f, 0f);

            textObj.text = text;
        }
    }

    // Update is called once per frame
    void Update() 
    {

        if(m_jeffRb == null || m_jeffGameObject == null || m_tonyRb == null || m_tonyGameObject == null) {
            m_jeffRb = GameObject.Find("Jeff").GetComponent<Rigidbody>();
            m_jeffGameObject = GameObject.Find("Jeff");

            m_tonyRb = GameObject.Find("Tony").GetComponent<Rigidbody>();
            m_tonyGameObject = GameObject.Find("Tony");
        }

        if(m_jeffAnim == null || m_tonyAnim == null) {
            m_jeffAnim = GameObject.Find("Jeff").GetComponent<Animator>();
            m_tonyAnim = GameObject.Find("Tony").GetComponent<Animator>();

        }

        if(m_jeffRb.velocity.x > 0.2f || m_jeffRb.velocity.x < -0.2f || m_jeffRb.velocity.z > 0.2f || m_jeffRb.velocity.x < -0.2f) {
            m_jeffAnim.SetBool("running", true);
        } else {
            m_jeffAnim.SetBool("running", false);
        }
        if(m_tonyRb.velocity.x > 0.2f || m_tonyRb.velocity.x < -0.2f || m_tonyRb.velocity.z > 0.2f || m_tonyRb.velocity.z < -0.2f) {
            m_tonyAnim.SetBool("running", true);
        } else {
            m_tonyAnim.SetBool("running", false);
        }

        UpdateUI();
        
        if(GameObject.Find("GameManager").GetComponent<PlayerManager>().m_playingMultiplayer) {
            //this is multiplayer movement
            
            if(isServer) {
                if(controlling != 1) {
                    ChangeControl();
                }

                Transform tony = m_tonyGameObject.transform;

                tony.position = Vector3.Lerp(tony.position, m_tonyPosition, 0.8f);
                tony.rotation = Quaternion.Lerp(tony.rotation, m_tonyRotation, 0.8f);

            } else {
                if(controlling != 2) {
                    ChangeControl();
                }

                Transform jeff = m_jeffGameObject.transform;

                jeff.position = Vector3.Lerp(jeff.position, m_jeffPosition, 0.8f);
                jeff.rotation = Quaternion.Lerp(jeff.rotation, m_jeffRotation, 0.8f);
            }



        } else {
            //This is sigleplayer movement
            if(Input.GetKeyDown(KeyCode.Q)) {
                ChangeControl();
            }
        }

        MovePlayer(controlling);

        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded(controlling)) {
            PlayerJump(controlling);
        }
    }

    private void MovePlayer(int playerId) {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = Vector3.zero;

        if(playerId == 1) {
            moveX *= m_jeffSpeed;
            moveZ *= m_jeffSpeed;

            movement = new Vector3(moveX, m_jeffRb.velocity.y, moveZ);
            movement = m_jeffGameObject.transform.TransformDirection(movement);

            m_jeffRb.velocity = movement;

            if(GameObject.Find("GameManager").GetComponent<PlayerManager>().m_playingMultiplayer) {
                CmdUpdatePosition(1, m_jeffGameObject.transform.position, m_jeffGameObject.transform.rotation, movement);
            }
        } else if (playerId == 2) {
            moveX *= m_tonySpeed;
            moveZ *= m_tonySpeed;

            movement = new Vector3(moveX, m_tonyRb.velocity.y, moveZ);
            movement = m_tonyGameObject.transform.TransformDirection(movement);

            m_tonyRb.velocity = movement;

            if(GameObject.Find("GameManager").GetComponent<PlayerManager>().m_playingMultiplayer) {
                CmdUpdatePosition(2, m_tonyGameObject.transform.position, m_tonyGameObject.transform.rotation, movement);
            }
        }
    }

    private void PlayerJump(int playerId) {
        int playerCount = GameObject.Find("GameManager").GetComponent<PlayerManager>().GetPlayerCount();
        if(playerId == 1) {
            m_jeffRb.AddForce(new Vector3(0,m_jeffJumpForce/playerCount,0));
        } else if(playerId == 2) {
            m_tonyRb.AddForce(new Vector3(0,m_tonyJumpForce/playerCount,0));
        }
    }

    private void ChangeControl() {
        if(controlling == 1) {
            controlling = 2;
            GameObject.Find("Jeff").GetComponent<Health>().m_iControl = false;
            GameObject.Find("Tony").GetComponent<Health>().m_iControl = true;
        } else if (controlling == 2) {
            controlling = 1;
            GameObject.Find("Jeff").GetComponent<Health>().m_iControl = true;
            GameObject.Find("Tony").GetComponent<Health>().m_iControl = false;
        } else {
            controlling = 1;
            GameObject.Find("Jeff").GetComponent<Health>().m_iControl = true;
            GameObject.Find("Tony").GetComponent<Health>().m_iControl = false;
        }
    }

    private bool IsGrounded(int playerId) {
        if(playerId == 1) {
            if(Physics.Raycast(m_jeffGameObject.transform.position, -Vector3.up, distToGroundBeforeJump, ~(10<<11))) {
                return true;
            }
        } else if (playerId == 2) {
            if(Physics.Raycast(m_tonyGameObject.transform.position, -Vector3.up, distToGroundBeforeJump, ~(10<<11))) {
                return true;
            }
        }

        return false;
    }

    [Command]
    private void CmdUpdatePosition(int playerid, Vector3 pos, Quaternion rot, Vector3 velo) {
        RpcUpdatePosition(playerid, pos, rot, velo);
    }

    [ClientRpc]
    private void RpcUpdatePosition(int playerid, Vector3 pos, Quaternion rot, Vector3 velo) {
        if(playerid == 1) {
            //Debug.Log("Server sent something");
            if(!isServer) {
                m_jeffPosition = pos;
                m_jeffRotation = rot;

                if(m_jeffRb != null)
                    m_jeffRb.velocity = velo;    
            }
        } else if (playerid == 2) {
            //Debug.Log("Client sent something");
            if(isServer) {
                m_tonyPosition = pos;
                m_tonyRotation = rot;
                if(m_tonyRb != null)
                    m_tonyRb.velocity = velo;
            }
        }
    }
}
