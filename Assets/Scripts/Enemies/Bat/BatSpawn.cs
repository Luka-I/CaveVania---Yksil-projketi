using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSpawn : MonoBehaviour
{
    public GameObject Bat;
    public Transform spawnPoint;
    private bool hasSpawnedBat = false;
    private BatFly batScript;

    private void Start()
    {
        //batScript = Bat.GetComponent<BatFly>();

        //Bat.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasSpawnedBat)
        {
            SpawnBat();
        }
    }
    void SpawnBat()
    {
        //Bat.transform.position = spawnPoint.position;
        GameObject batIinstance = Instantiate(Bat, spawnPoint.position, Quaternion.identity);

        batIinstance.GetComponent<BatFly>().ActivateBat();
        //batScript.ActivateBat();
        hasSpawnedBat = true;
    }
}
