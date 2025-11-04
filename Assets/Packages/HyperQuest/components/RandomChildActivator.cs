using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChildActivator : MonoBehaviour
{
    public bool RandOnAwake = true;
    public bool RandOnEnable = false;
    public List<KeyCode> ActivationKeys;

    private void OnEnable()
    {
        if (RandOnEnable) RandomlyActivate();
    }

    private void Awake()
    {
        if (RandOnAwake) RandomlyActivate();
    }

    private void Update()
    {
        if (ActivationKeys.GetKeyDown()) RandomlyActivate();
    }

    public void RandomlyActivate()
    {
        int selectedIndex = Random.Range(0, transform.childCount);
        //Debug.Log(selectedIndex);
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == selectedIndex);
        }
    }
}
