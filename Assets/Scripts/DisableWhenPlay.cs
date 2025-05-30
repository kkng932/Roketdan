using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenPlay : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.SetActive(false);
    }
}
