using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBar_Bar : MonoBehaviour
{
    private Transform parent;
    private bool barRight = false;
    private bool barLeft = false;

    public void Initialize()
    {
        parent = transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).TryGetComponent(out LoadingBar_Right _))
                barRight = true;
            if (parent.GetChild(i).TryGetComponent(out LoadingBar_Left _))
                barLeft = true;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (!barRight || !barLeft)
            return;

        transform.localScale = new Vector3(transform.localScale.x, (parent.GetComponent<SpriteRenderer>().bounds.size.y - (3 * Mathf.Abs(parent.lossyScale.y) / parent.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit)) / (GetComponent<SpriteRenderer>().sprite.bounds.size.y * parent.lossyScale.y), transform.localScale.z);

        if (GetComponent<SpriteRenderer>().bounds.size.x > (parent.GetComponentInChildren<LoadingBar_Right>().transform.position
            - parent.GetComponentInChildren<LoadingBar_Left>().transform.position).magnitude)
        {
            transform.localScale = new Vector3((parent.GetComponentInChildren<LoadingBar_Right>().transform.position
                - parent.GetComponentInChildren<LoadingBar_Left>().transform.position).magnitude /
                (GetComponent<SpriteRenderer>().sprite.bounds.size.x * parent.lossyScale.x),
                transform.localScale.y, transform.localScale.z);
        }

        transform.position = parent.GetComponentInChildren<LoadingBar_Right>().transform.position - new Vector3(GetComponent<SpriteRenderer>().bounds.extents.x,
            -0.5f * parent.lossyScale.y / parent.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit, 0);
    }
}
