using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRay : MonoBehaviour
{
    public bool FoundHider { get; private set; }

    public Material material;
    public Renderer rend;

    private void Start()
    {
       rend = GetComponent<Renderer>();
       rend.enabled = true;
       rend.material = material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hider")) FoundHider = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hider")) FoundHider = false;
    }
}
