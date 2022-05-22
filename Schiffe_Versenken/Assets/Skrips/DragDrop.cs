using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private Schiff schiff;
    private Vector3 offset;

    private void Start()
    {
        schiff = GetComponent<Schiff>();
    }
    private void OnMouseDown()
    {
        offset = transform.position - MouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        if (schiff.placed)
            schiff.spielbrett.RemoveSchiff(schiff, true);
        transform.position = MouseWorldPosition() + offset;
        if (Input.GetKeyDown(KeyCode.R)) //Rotation
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 90);
            //transform.RotateAround(transform.position + offset, Vector3.forward, -90);
        }
    }

    private void OnMouseUp()
    {
        if (schiff.selected)
        {
            schiff.SetPosition(MouseWorldPosition() + offset + new Vector3(-(float)schiff.schifftyp.ausmaße.x / 2 + 0.5f, (float)schiff.schifftyp.ausmaße.y / 2 - 0.5f), true);
        }
    }

    //Aus Internet -->
    private Vector3 MouseWorldPosition()
    {
        Vector3 mousescreenPos = Input.mousePosition;
        mousescreenPos.z = Camera.main.WorldToScreenPoint(mousescreenPos).z;
        return Camera.main.ScreenToWorldPoint(mousescreenPos);
    }
}
