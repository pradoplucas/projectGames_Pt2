using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptGhost : MonoBehaviour
{

    public GameObject pac;

    public Material[] material;
    private Renderer rend;

    private UnityEngine.AI.NavMeshAgent agent;

    public bool isEscape = false;

    public int whichGhost;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
        
        rend = GetComponentInChildren<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material [0];
    }

    // Update is called once per frame
    void Update()
    {
        if(pac.GetComponent<scriptPac>().isPower[whichGhost]){
            pac.GetComponent<scriptPac>().isPower[whichGhost] = false;
            isEscape = true;
            agent.speed = 2.5f;
            rend.sharedMaterial = material [1];
        }

        if(!isEscape){
            agent.SetDestination(pac.transform.position);
            agent.speed = pac.GetComponent<scriptPac>().velGhosts;
        }
        else{
            agent.SetDestination(new Vector3(0, 0, 0));

            if(Vector3.Distance(transform.position, new Vector3(0, 0, 0)) < 1f){
                rend.sharedMaterial = material [0];
                isEscape = false;
            }
        }

    }

    private void isEscapeFalse(){
        isEscape = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && isEscape){
            gameObject.SetActive(false);
            Invoke("isAlive", 10f);
        }
    }

    private void isAlive(){
        transform.position = new Vector3 (0, 0.08333242f, 0);
        gameObject.SetActive(true);
    }
}
