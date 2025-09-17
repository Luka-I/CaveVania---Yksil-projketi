using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCamera : MonoBehaviour
{
    [SerializeField] private GameObject virtualCamera1;
    [SerializeField] private GameObject virtualCamera2;
    [SerializeField] private GameObject virtualCamera3;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera1.SetActive(true);
        virtualCamera2.SetActive(false);
        virtualCamera3.SetActive(false);
    }

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            virtualCamera2.SetActive(false);
            virtualCamera3.SetActive(true);
        }
    }
}
