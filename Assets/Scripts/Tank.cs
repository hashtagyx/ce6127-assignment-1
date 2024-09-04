using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public float Speed = 12f;
    public float TurnSpeed = 180f;
    private void Awake()
    {
        Debug.Log("Awake");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update - delta time: " + Time.deltaTime);
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate - delta time: " + Time.deltaTime);
    }
}
