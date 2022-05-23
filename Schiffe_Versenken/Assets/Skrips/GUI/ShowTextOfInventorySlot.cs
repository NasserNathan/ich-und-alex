using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowTextOfInventorySlot : MonoBehaviour
{
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            transform.Find("Text").gameObject.SetActive(true);
        else transform.Find("Text").gameObject.SetActive(false);
    }
}
