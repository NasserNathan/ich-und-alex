using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpielfeldFeind : MonoBehaviour
{
    private BoxCollider[,] BC;
    [SerializeField]
    private Schiff hitShip;
    private Spielbrett spielbrett;

    void Start()
    {
        spielbrett = GameObject.Find("Spielbrett").GetComponent<Spielbrett>();
        BC = new BoxCollider[Mathf.Abs(spielbrett.getSize().x), Mathf.Abs(spielbrett.getSize().y)];

        for (int x = 0; x <= Mathf.Abs(spielbrett.getSize().x) - 1; x++)
        {
            for (int y = 0; y <= Mathf.Abs(spielbrett.getSize().y) - 1; y++)
            {
                BC[x, y] = gameObject.AddComponent<BoxCollider>();
                BC[x, y].size = new Vector3(0.99f, 0.99f, 0);
                BC[x, y].center = (Vector3)spielbrett.getOrigin2() + new Vector3(0.5f - transform.position.x + x, -0.5f - transform.position.y - y, -1);
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        bool platziert = false;
        if (spielbrett.zug && spielbrett.zugNummer > 0)
        {   //Unity Documentation -->
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            foreach (BoxCollider bc in BC)
            {
                if (bc.Raycast(ray, out _, 20.0f))
                {
                    platziert = spielbrett.PlaceMark(Methoden.Vector2ToVector2Int(transform.position + bc.center), ref hitShip);
                }
            }
            if (platziert && hitShip == null)
            {
                spielbrett.CountdownEnd();
            }
        }
    }
}
