using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMovement : MonoBehaviour
{
    private Schiff schiff;
    // Start is called before the first frame update
    void Start()
    {
        schiff = GetComponent<Schiff>();
    }

    // Update is called once per frame
    void Update()
    {
        if (schiff.selected && schiff.spielbrett.zug && schiff.spielbrett.zugNummer == 0)
        {
            if (Input.GetKeyDown(KeyCode.W)) //Oben
            {
                schiff.MovePosition(0, 1, true); 
            }
            if (Input.GetKeyDown(KeyCode.A)) //Links
            {
                schiff.MovePosition(-1, 0, true);
            }
            if (Input.GetKeyDown(KeyCode.S)) //Unten
            {
                schiff.MovePosition(0, -1, true);
            }
            if (Input.GetKeyDown(KeyCode.D)) //Rechts
            {
                schiff.MovePosition(1, 0, true);
            }
            if (Input.GetKeyDown(KeyCode.R)) //Rotation
            {
                schiff.SetRotation(schiff.GetRotation() + 1);
            }
        }

        if (!schiff.placed && schiff.spielbrett.tryingToStart)
            if (!schiff.spielbrett.TryPlaceSchiff(schiff, true))
                schiff.spielbrett.DestroySchiff(schiff, true, schiff.name + " konnte nicht platziert werden und wurde zerstört", true);
    }
}
