using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private GameObject doorOpen;
    [SerializeField]
    private GameObject doorClosed;

    public void Interact(bool open)
    {
        doorClosed.SetActive(!open);
        doorOpen.SetActive(open);
    }

}
