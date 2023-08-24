using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTreasure : MonoBehaviour
{
    public GetKey agent;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Agent") && agent.GetKeyCount() > 0)
        {
            agent.DecrementKeyCount(); 
            gameObject.SetActive(false);
        }
    }
}
