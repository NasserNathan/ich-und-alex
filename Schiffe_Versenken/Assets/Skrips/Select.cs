using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    private Schiff schiff;
    private SpriteRenderer SprRenderer;
    private bool focus = false;
    private bool zug = true;

    // Start is called before the first frame update
    void Start()
    {
        schiff = GetComponent<Schiff>();
        SprRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseUp()
    {
        if (schiff.spielfeld == 1 && !schiff.destroyed && zug)
            schiff.selected = true;
    }

    private void OnMouseEnter()
    {
        focus = true;
    }
    private void OnMouseExit()
    {
        focus = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!schiff.spielbrett.zug)
        {
            zug = false;
            schiff.selected = false;
        }

        if (schiff.destroyed)
        {
            schiff.selected = false;
            transform.localScale = new Vector3(1, 1, 0);
            SprRenderer.color = new Color(0.8f, 0.8f, 1, 0.9f);
            SprRenderer.sortingOrder = 1;
        }
        else if (schiff.selected && schiff.placed)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 0);
            SprRenderer.color = new Color(1, 1, 1, 0.5f);
            SprRenderer.sortingOrder = 10;
        }
        else if (!schiff.placed)
            SprRenderer.color = new Color(1, 0.2f, 0.2f, 0.5f);
        else if (!schiff.selected && !schiff.spielbrett.loose)
        {
            transform.localScale = new Vector3(1, 1, 0);
            SprRenderer.color = new Color(1, 1, 1, 1);
            SprRenderer.sortingOrder = 1;
            if (schiff.spielfeld == 2)
            {
                SprRenderer.color = new Color(1, 1, 1, 0);
            }
        }

        if (schiff.spielbrett.loose && !schiff.destroyed && schiff.placed && schiff.spielfeld == 2)
        {
            SprRenderer.color = new Color(1, 0.9f, 0.1f, Mathf.Lerp(SprRenderer.color.a, 1f,Time.deltaTime * 0.5f));
        }

        if (Input.GetMouseButtonDown(0) && !focus) //Unselect wenn nicht auf das Schiff geklickt wurde
            schiff.selected = false;
    }
}
