using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGuard : MonoBehaviour
{

    [Header("Options")]

    public float visualRange = 10f;

    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Tony") == null || GameObject.Find("Jeff") == null) {
            return;
        }
        GameObject[] characters = {GameObject.Find("Tony"), GameObject.Find("Jeff")};
        GameObject closestCharacter = characters[0];
        
        foreach(GameObject player in  characters) {

            if(player.GetComponent<NPCDetection>().canBeSeen) {
                if(Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(transform.position, closestCharacter.transform.position)) {
                    closestCharacter = player;
                }
            }
        }

        if(Vector3.Distance(transform.position, closestCharacter.transform.position) < visualRange) {
            if(closestCharacter.GetComponent<NPCDetection>().canBeSeen) {
                transform.LookAt(closestCharacter.transform.position);
                

                if(Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0,0,visualRange)), out hit)) {
                    if(hit.transform.tag == "Jeff" || hit.transform.tag == "Tony") {
                        if(!hit.transform.gameObject.GetComponent<Health>().m_isDead) {
                            hit.transform.gameObject.GetComponent<Health>().m_isDead = true;

                            float baseSpeed = 0;

                            var players = GameObject.FindGameObjectsWithTag("Player");

                            if(hit.transform.name == "Jeff") {
                                baseSpeed = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMovement>().m_jeffSpeed;
                            } else if (hit.transform.name == "Tony") {
                                baseSpeed = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMovement>().m_tonySpeed;
                            }

                            StartCoroutine(RespawnThing(hit.transform.gameObject, 10f));
                        }
                    }
                }
            }
        }        
    }
    
    IEnumerator RespawnThing(GameObject thing, float baseSpeed) {

        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players) {
            if(thing.transform.name == "Jeff") {
                player.GetComponent<PlayerMovement>().m_jeffSpeed = 0;
            } else if (thing.transform.name == "Tony") {
                player.GetComponent<PlayerMovement>().m_tonySpeed = 0;
            }
        }


        yield return new WaitForSeconds(2f);

        foreach(GameObject player in players) {
            if(thing.transform.name == "Jeff") {
                player.GetComponent<PlayerMovement>().m_jeffSpeed = baseSpeed;
            } else if (thing.transform.name == "Tony") {
                player.GetComponent<PlayerMovement>().m_tonySpeed = baseSpeed;
            }
        }

        thing.transform.position = GameObject.Find("GameManager").transform.Find("SpawnPoint").transform.position;
        thing.GetComponent<Health>().m_isDead = false;

    }
}




