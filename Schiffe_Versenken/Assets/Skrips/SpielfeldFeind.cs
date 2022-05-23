using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpielfeldFeind : MonoBehaviour
{
    private BoxCollider[,] BC;
    [SerializeField]
    private Schiff hitShip;
    private Spielbrett spielbrett;
    private Schiff[] ships;
    private bool Sonar = false;
    private int SonarUses = 1;
    private bool Bomb = false;
    private int BombUses = 1;
    public int Schiffsteile;//{ get; private set; }

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
        if (spielbrett.zug && spielbrett.zugNummer > 0)
        {   
            if (Sonar && SonarUses > 0)
            {
                Vector2 textposition = new Vector2();
                int platzierungen = -1;
                //Unity Documentation -->
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                foreach (BoxCollider bc in BC)
                {
                    if (bc.Raycast(ray, out _, 20.0f))
                    {
                        platzierungen = spielbrett.SpecialMarks(Methoden.Vector2ToVector2Int(transform.position + bc.center), ref ships, 2);
                        textposition = transform.position + bc.center;
                    }
                }
                if (platzierungen != -1)
                {
                    spielbrett.countD.SetPercentage(0);
                    Schiffsteile = platzierungen;

                    GameObject.Find("Canvas").transform.Find("Inventar Sonar").Find("Count Text").GetComponent<Text>().text = Schiffsteile.ToString() + "  bei: " +
                        Symbols.GetFromAlphabet(Mathf.RoundToInt(textposition.x - spielbrett.getOrigin2().x)) + " " +
                        Mathf.RoundToInt(textposition.y + 1).ToString();
                }
                Sonar = false;
                SonarUses--;
            }
            else if (Bomb && BombUses > 0)
            {
                int platzierungen = -1;
                //Unity Documentation -->
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                foreach (BoxCollider bc in BC)
                {
                    if (bc.Raycast(ray, out _, 20.0f))
                    {
                        platzierungen = spielbrett.SpecialMarks(Methoden.Vector2ToVector2Int(transform.position + bc.center), ref ships, 1);
                    }
                }
                if (platzierungen != -1)
                    spielbrett.countD.SetPercentage(0);
                Bomb = false;
                BombUses--;
            }
            else
            {
                bool platziert = false;
                //Unity Documentation -->
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
                foreach (BoxCollider bc in BC)
                {
                    if (bc.Raycast(ray, out _, 20.0f))
                    {
                        platziert = spielbrett.PlaceMark(Methoden.Vector2ToVector2Int(transform.position + bc.center), ref hitShip);
                    }
                }
                if (platziert && hitShip != null)
                {
                    spielbrett.countD.CountdownStart(spielbrett.countD.GetSpeed() * 1.5f);
                    if (spielbrett.countD.GetProgress() + 10 >= 100)
                        spielbrett.countD.SetPercentage(99.9f);
                    else spielbrett.countD.SetPercentage(spielbrett.countD.GetProgress() + 10);
                }
                if (platziert && hitShip == null)
                    spielbrett.countD.SetPercentage(0);
            }

        }
    }

    public void NextShotIsBomb()
    {
        Sonar = false;
        Bomb = true;
    }

    public void NextShotIsSonar()
    {
        Bomb = false;
        Sonar = true;
    }

    public void NextShotIsNormal()
    {
        Bomb = false;
        Sonar = false;
    }

}
