using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GetKey : Agent
{
    public Transform targetTransform;  
    public GameObject key;
    public float moveSpeed;

    public GameObject ChestParent;
    public GameObject BadChestPrefab;
    public GameObject TreasurePrefab;
    public GameObject ExplosionPrefab;
    private bool chestsSpawned = false;
    private int keyCount;

    private bool isSearchingForChest = false;
    private bool isSearchingForKey= true;

    private float maxRotationSpeed = 270.0f;

   

    public override void OnEpisodeBegin()
    {
       
        key.SetActive(true);
        transform.localPosition = new Vector3(7.0f, 0f, 7.0f);
        //transform.localPosition = new Vector3(3.0f,0f,3.0f);
        targetTransform.localPosition = new Vector3(4.0f, 0f, 4.0f);
        ChestParent.transform.localPosition = Vector3.zero;

        if (chestsSpawned)
        {
            foreach (Transform child in ChestParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        chestsSpawned = false;
        keyCount = 0;
      
    }
    public void IncrementKeyCount()
    {
        keyCount += 1; 
    }
    public int GetKeyCount()
    {
        return keyCount;
    }

    public void DecrementKeyCount()
    {
        keyCount--;
    }

    private void SpawnChests()    {
        

         Vector3 chestPosition = ChestParent.transform.position;
    
        
        Instantiate(BadChestPrefab,chestPosition+ new Vector3(21.0f, 0f, -1.0f), Quaternion.Euler(270f, 270f, 0f), ChestParent.transform);
        Instantiate(BadChestPrefab, chestPosition + new Vector3(-7.0f, 0f, 7.0f), Quaternion.Euler(270f, 270f, 0f), ChestParent.transform);
        Instantiate(BadChestPrefab, chestPosition + new Vector3(9.0f, 0f,20.0f ), Quaternion.Euler(270f, 270f, 0f), ChestParent.transform);
        Instantiate(BadChestPrefab, chestPosition + new Vector3(12.0f, 0f,-18.0f), Quaternion.Euler(270f, 270f, 0f), ChestParent.transform);
        Instantiate(BadChestPrefab, chestPosition + new Vector3(18.0f, 0f,12.0f), Quaternion.Euler(270f, 270f, 0f), ChestParent.transform);
        Instantiate(BadChestPrefab, chestPosition + new Vector3(-25.0f, 0f, -22.0f), Quaternion.Euler(270f, 270f, 0f), ChestParent.transform);


        Instantiate(TreasurePrefab,  chestPosition+ new Vector3(15.0f, 0f, 8.0f), Quaternion.Euler(270f, 270f, 0f), ChestParent.transform);
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(TreasurePrefab.transform.localPosition);
        sensor.AddObservation(BadChestPrefab.transform.localPosition);

    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continuousActions = actions.ContinuousActions;
        float horizontalInput = continuousActions[0];
        float verticalInput = continuousActions[1];

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 inputDirection = (horizontalInput * right + verticalInput * forward).normalized;
        Vector3 movement = inputDirection * moveSpeed * Time.deltaTime;
        transform.position += movement;

        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);
            transform.rotation = newRotation;
        }

       /** 
        if (isSearchingForKey)
        {
            float distance = Vector3.Distance(targetTransform.localPosition, transform.localPosition);
            float maxDistance = Vector3.Distance(new Vector3(-28f, 0f, -28f), new Vector3(28f, 0f, 28f));
            float distanceReward = (distance / maxDistance) / 100f;
            Debug.Log(-distanceReward);
            AddReward(-distanceReward);
        }

        if (isSearchingForChest)
        {
            float distance = Vector3.Distance(new Vector3(15.0f, 0f, 10.0f), transform.localPosition);
            float maxDistance = Vector3.Distance(new Vector3(-28f, 0f, -28f), new Vector3(28f, 0f, 28f));
            float distanceReward = (distance / maxDistance) / 100f;
            Debug.Log(-distanceReward);
            AddReward(-distanceReward);
        }**/
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Key")
        {
            AddReward(+1f);
           // isSearchingForChest = true;
           //isSearchingForKey = false;

            if (!chestsSpawned)
            {
                SpawnChests();
                chestsSpawned = true;                
            }
        }
        else if (collision.gameObject.tag == "Treasure")
        {
            AddReward(+1f);
            GameObject[] badChests = GameObject.FindGameObjectsWithTag("BadChest");
            foreach (GameObject badChest in badChests)
            {
                Destroy(badChest);
            }

            Destroy(GameObject.FindGameObjectWithTag("Treasure"));
            EndEpisode();
        }
        else if (collision.gameObject.tag == "BadChest")
        {
            AddReward(-1f);
            var expl = Instantiate(ExplosionPrefab, transform.localPosition, Quaternion.identity);
            Destroy(expl, 2);

            GameObject[] badChests = GameObject.FindGameObjectsWithTag("BadChest");
            foreach (GameObject badChest in badChests)
            {

                GameObject.Destroy(badChest);
            }

            Destroy(GameObject.FindGameObjectWithTag("Treasure"));
            EndEpisode();
        }
        else if (collision.gameObject.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();           
        }
    }
  

}

