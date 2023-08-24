using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyToCollect : MonoBehaviour
{
    public GetKey agent; 

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Agent"))
        {
            agent.IncrementKeyCount(); 
            gameObject.SetActive(false);
        }
    }
}
