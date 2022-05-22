using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBar_Left : MonoBehaviour
{

    public void Refresh()
    {
        transform.position = transform.parent.position - new Vector3(
            transform.parent.GetComponent<SpriteRenderer>().bounds.extents.x + GetComponent<SpriteRenderer>().bounds.extents.x, 0, 0);
        transform.localScale = new Vector3(GetComponent<SpriteRenderer>().bounds.size.y / (GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.parent.lossyScale.x), 1, 1);
    }
}
