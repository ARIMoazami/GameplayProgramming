using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    public GameObject newCam;
    public GameObject playerCam;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        playerCam.SetActive(false);
        newCam.SetActive(true);
        Player.GetComponent<SplineMove>().enabled = true;
    }



}