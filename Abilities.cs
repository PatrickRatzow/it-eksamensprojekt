using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Abilities : NetworkBehaviour {
    [Header("Jeff Abilities")] public string[] m_jeffAbilities;
    public float[] m_jeffCooldownTimes = {4f, 4f, 4f};
    public float[] m_jeffCooldownUsed = {0f, 0f, 0f};
    
    private bool[] m_jeffCanUseAbility = {true, true, true};
    private GameObject m_jeffGameObject;

    [Space] [Header("Tony Abilities")] public string[] m_tonyAbilities;
    public float[] m_tonyCooldownTimes = {4f, 4f, 4f};
    public float[] m_tonyCooldownUsed = {0f, 0f, 0f};
    private bool[] m_tonyCanUseAbility = {true, true, true};
    private GameObject m_tonyGameObject;
    private bool m_tonyPlatformActive = false;
    private GameObject m_tonyPlatformGameObject;

    [Space] [Header("Ability Options")] public float m_superSpeedMultiplier = 1.5f;
    public float m_superSpeedActiveTime = 2f;
    public float m_superJumpMultiplier = 2f;
    public float m_invisabilityActiveTime = 4f;
    public float m_phasingActiveTime = 2f;
    public Vector3 m_teleportationOffset = new Vector3(0, 2, 0);
    public Vector3 m_platformScaleActive = new Vector3(2, 0.1f, 2);
    public Vector3 m_platformPositionOffset = new Vector3(0, 1.2f, 0);

    
    private void Start() {           
        
    }

    private void Update() {

        if(m_jeffGameObject == null || m_tonyGameObject == null || m_tonyPlatformGameObject == null) {
            m_jeffGameObject = GameObject.Find("Jeff");
        
            m_tonyGameObject = GameObject.Find("Tony");
            m_tonyPlatformGameObject = m_tonyGameObject.transform.Find("Platform").gameObject;
        }
        CheckCooldown();
        KeyCode[] m_abilities = {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3
        };
        
        if(m_tonyPlatformActive) {
            m_tonyPlatformGameObject.transform.localScale = Vector3.Lerp(m_tonyPlatformGameObject.transform.localScale, m_platformScaleActive, 0.1f);
            m_tonyPlatformGameObject.transform.localPosition = Vector3.Lerp(m_tonyPlatformGameObject.transform.localPosition, m_platformPositionOffset, 0.1f);
        } else {
            m_tonyPlatformGameObject.transform.localScale = Vector3.Lerp(m_tonyPlatformGameObject.transform.localScale, new Vector3(0.01f,0.01f,0.01f), 0.1f);
            m_tonyPlatformGameObject.transform.localPosition = Vector3.Lerp(m_tonyPlatformGameObject.transform.localPosition, new Vector3(0.1f,0.01f,0.1f), 0.1f);
        }

        int characterId = GetComponent<PlayerMovement>().controlling;

        int index = -1;
        for (int i = 0; i < m_abilities.Length; i++) {
            if (!Input.GetKeyDown(m_abilities[i])) {
                continue;
            }
            
            index = i;
                
            break;
        }


        if (index == -1) {
            return;
        }
        
        bool canUse;
        string[] abilities;

        if (characterId == 1) {
            canUse = m_jeffCanUseAbility[index];
            abilities = m_jeffAbilities;
        }
        else {
            canUse = m_tonyCanUseAbility[index];
            abilities = m_tonyAbilities;
        }

        if (!canUse) {
            return;
        }

        if(GameObject.Find("GameManager").GetComponent<PlayerManager>().m_playingMultiplayer) {
            CmdUseAbility(abilities[index], index, characterId);
        } else {
            UseAbility(abilities[index], index, characterId);
        }
        if (characterId == 1) {
            m_jeffCanUseAbility[index] = false;
        }
        else {
            m_tonyCanUseAbility[index] = false;
        }
        StartCooldown(index, characterId);
    }

    [Command]
    private void CmdUseAbility(string ability, int abilityId, int playerId) {
        RpcUseAbility(ability, abilityId, playerId);
    }

    [ClientRpc]
    private void RpcUseAbility(string ability, int abilityId, int playerId) {
        int playerCount = GameObject.Find("GameManager").GetComponent<PlayerManager>().GetPlayerCount();
        switch(ability) {
            case "Super Jump":
                
                if(playerId == 1) {
                    m_jeffGameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0,GetComponent<PlayerMovement>().m_jeffJumpForce*m_superJumpMultiplier/playerCount,0));
                } else if (playerId == 2) {
                    m_tonyGameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0,GetComponent<PlayerMovement>().m_tonyJumpForce*m_superJumpMultiplier/playerCount,0));
                }
                break;
            
            case "Super Speed":
                    if(playerId == 1) {
                        StartCoroutine(SuperSpeed(m_superSpeedActiveTime, 1, 10f));
                    } else if (playerId == 2) {
                        StartCoroutine(SuperSpeed(m_superSpeedActiveTime, 2, 10f));
                    }
                break;

            case "Invisability":
                StartCoroutine(Invisability(m_invisabilityActiveTime,playerId));

                break;

            case "Platform":
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players) {
                    player.GetComponent<Abilities>().m_tonyPlatformActive = !m_tonyPlatformActive;
                }
                
                break;

            case "Phasing":
                StartCoroutine(Phasing(m_phasingActiveTime, playerId));

                break;

            case "Teleportation":
                if(playerId == 1) {
                    m_jeffGameObject.transform.position = m_tonyGameObject.transform.position + m_teleportationOffset;
                } else if (playerId == 2) {
                    m_tonyGameObject.transform.position = m_jeffGameObject.transform.position + m_teleportationOffset;
                }
                break;
        }
    }

    private void StartCooldown(int ability, int playerId) {
        if(playerId == 1) {
            m_jeffCooldownUsed[ability] = Time.time + m_jeffCooldownTimes[ability];
        } else if (playerId == 2) {
            m_tonyCooldownUsed[ability] = Time.time + m_tonyCooldownTimes[ability];
        }
    }

    private void CheckCooldown() {

        int temp = 0;
        foreach (float t in m_jeffCooldownUsed) {
            if(t - Time.time < 0f) {
                m_jeffCanUseAbility[temp] = true;
            }
            temp++;
        }

        temp = 0;
        foreach (float t in m_tonyCooldownUsed) {
            if(t - Time.time < 0f) {
                m_tonyCanUseAbility[temp] = true;
            }
            temp++;
        }
    }

    IEnumerator SuperSpeed(float activeTime, int playerId, float normalSpeed) {
        if(playerId == 1) {
            GetComponent<PlayerMovement>().m_jeffSpeed = normalSpeed * m_superSpeedMultiplier;
            yield return new WaitForSeconds(activeTime);
            GetComponent<PlayerMovement>().m_jeffSpeed = normalSpeed;

        } else if (playerId == 2) {
            GetComponent<PlayerMovement>().m_tonySpeed = normalSpeed * m_superSpeedMultiplier;
            yield return new WaitForSeconds(activeTime);
            GetComponent<PlayerMovement>().m_tonySpeed = normalSpeed;
        }
    }

    IEnumerator Phasing(float activeTime, int playerId) {
        if(playerId == 1) {
            Physics.IgnoreLayerCollision(9,10,true);
            yield return new WaitForSeconds(activeTime);
            Physics.IgnoreLayerCollision(9,10,false);
        } else if (playerId == 2) {
            Physics.IgnoreLayerCollision(9,11,true);
            yield return new WaitForSeconds(activeTime);
            Physics.IgnoreLayerCollision(9,11,false);
        }

    }

    IEnumerator Invisability(float activeTime, int playerId) {

        if(playerId == 1) {
            var rend = m_jeffGameObject.transform.Find("Model").Find("Body").GetComponent<Renderer>();
            Color newColor = rend.material.color;
            newColor.a = 25f/255f;
            rend.material.color = Color.Lerp(rend.material.color, newColor, 0.5f);
            m_jeffGameObject.GetComponent<NPCDetection>().canBeSeen = false;
            yield return new WaitForSeconds(activeTime);
            m_jeffGameObject.GetComponent<NPCDetection>().canBeSeen = true;
            newColor.a = 255f/255f;
            rend.material.color = newColor;

        } else if (playerId == 2) {
            var rend = m_tonyGameObject.transform.Find("Model").Find("Body").GetComponent<Renderer>();
            Color newColor = rend.material.color;
            newColor.a = 25f/255f;
            rend.material.color = Color.Lerp(rend.material.color, newColor, 0.5f);
            m_tonyGameObject.GetComponent<NPCDetection>().canBeSeen = false;
            yield return new WaitForSeconds(activeTime);
            m_tonyGameObject.GetComponent<NPCDetection>().canBeSeen = true;
            newColor.a = 255f/255f;
            rend.material.color = newColor;

        }

    }

    private void UseAbility(string ability, int abilityId, int playerId) {
        int playerCount = GameObject.Find("GameManager").GetComponent<PlayerManager>().GetPlayerCount();
        switch(ability) {
            case "Super Jump":
                
                if(playerId == 1) {
                    m_jeffGameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0,GetComponent<PlayerMovement>().m_jeffJumpForce*m_superJumpMultiplier/playerCount,0));
                } else if (playerId == 2) {
                    m_tonyGameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0,GetComponent<PlayerMovement>().m_tonyJumpForce*m_superJumpMultiplier/playerCount,0));
                }
                break;
            
            case "Super Speed":
                    if(playerId == 1) {
                        StartCoroutine(SuperSpeed(m_superSpeedActiveTime, 1, 10f));
                    } else if (playerId == 2) {
                        StartCoroutine(SuperSpeed(m_superSpeedActiveTime, 2, 10f));
                    }
                break;

            case "Invisability":
                StartCoroutine(Invisability(m_invisabilityActiveTime,playerId));

                break;

            case "Platform":
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players) {
                    player.GetComponent<Abilities>().m_tonyPlatformActive = !m_tonyPlatformActive;
                }
                
                break;

            case "Phasing":
                StartCoroutine(Phasing(m_phasingActiveTime, playerId));

                break;

            case "Teleportation":
                if(playerId == 1) {
                    m_jeffGameObject.transform.position = m_tonyGameObject.transform.position + m_teleportationOffset;
                } else if (playerId == 2) {
                    m_tonyGameObject.transform.position = m_jeffGameObject.transform.position + m_teleportationOffset;
                }
                break;
        }
    }


}
