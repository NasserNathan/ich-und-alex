using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpielbrettFreund : MonoBehaviour
{
    private bool platziert;
    private bool shoot;
    private float delay;
    private Vector2Int lastshot = new Vector2Int(999, 999);
    private Schiff hitShip;
    private Vector2Int[] neighbours = new Vector2Int[4];
    private List<Vector2Int> notDestroyedHits = new List<Vector2Int>();
    private Spielbrett spielbrett;

    private void Start()
    {
        spielbrett = GameObject.Find("Spielbrett").GetComponent<Spielbrett>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Spielbrett.zug && Spielbrett.zugNummer > -1 && Spielbrett.difficulty == 1)
        if (spielbrett.zugNummer > -1)
        {
            if (delay > 0)
                delay -= Time.deltaTime;
            if (delay <= 0)
                shoot = true;

            if (!spielbrett.CountdownReset() && shoot && spielbrett.countdown != 2)
            {
                if (spielbrett.difficulty == 1) //Difficulty Easy-------------------------------------------------------------
                    platziert = GetComponentInParent<Spielbrett>().PlaceMark(RandomPoint(), ref hitShip); //zufälliger Schuss
                else if (spielbrett.difficulty == 2) //Difficulty Normal------------------------------------------------------
                {
                    if (lastshot == new Vector2Int(999, 999)) //zufälliger Schuss
                    {
                        lastshot = RandomPoint();
                        platziert = GetComponentInParent<Spielbrett>().PlaceMark(lastshot, ref hitShip);
                        if (!platziert || hitShip == null)
                            SetToRandomShot();
                        else if (hitShip.destroyed)
                        {
                            foreach (Vector2Int k in hitShip.schifftyp.kästchenstruktur)
                                notDestroyedHits.Remove(hitShip.position + k);
                            SetToRandomShot();
                        }
                        else notDestroyedHits.Add(lastshot);
                    }
                    else //geplanter Schuss
                    {
                        neighbours = ShuffledNeighbourPoints();
                        foreach (Vector2Int point in neighbours)
                        {
                            platziert = GetComponentInParent<Spielbrett>().PlaceMark(lastshot + point, ref hitShip);
                            if (platziert)
                            {
                                if (hitShip == null)
                                    SetToRandomShot();
                                else if (hitShip.destroyed)
                                {
                                    foreach (Vector2Int k in hitShip.schifftyp.kästchenstruktur)
                                        notDestroyedHits.Remove(hitShip.position + k);
                                    SetToRandomShot();
                                }
                                else
                                {
                                    lastshot += point;
                                    notDestroyedHits.Add(lastshot);
                                }
                                break;
                            }
                        }
                        if (!platziert)
                        {
                            notDestroyedHits.Remove(lastshot);
                            SetToRandomShot();
                        }
                    }
                }
                else if (spielbrett.difficulty == 3) // Difficulty Hard ----------------------------------------------------
                {

                }
                else if (spielbrett.difficulty == 4) // Difficulty Impossible ----------------------------------------------
                {
                    if (Random.Range(0, 3) == 0)
                        platziert = GetComponentInParent<Spielbrett>().PlaceMark(RandomPoint(), ref hitShip); //zufälliger Schuss
                    else 
                    {
                        foreach (Schiff ship in transform.GetComponentsInChildren<Schiff>())
                        {
                            for (int i = 0; i < ship.schifftyp.kästchenstruktur.Count; i++)
                            {
                                platziert = GetComponentInParent<Spielbrett>().PlaceMark(ship.position + (Vector2Int)ship.schifftyp.kästchenstruktur[i], ref hitShip);
                                if (platziert)
                                {
                                    break;
                                }
                            }
                            if (platziert)
                                break;
                        }
                    }
                }

                if (platziert || transform.GetChild(1).childCount == Mathf.Abs(spielbrett.getSize().x) * Mathf.Abs(spielbrett.getSize().y))
                {
                    shoot = false;
                    platziert = false;

                    if (hitShip != null)
                    {
                        spielbrett.Gefahrenwarnung();
                        delay = Random.Range(0.5f, 2f);
                    }
                    else if (!spielbrett.zug)
                    {
                        spielbrett.CountdownStart(0.1f);
                        delay = 0;
                    }
                }
            }
        }
    }

    private void SetToRandomShot()
    {
        if (notDestroyedHits.Count > 0)
            lastshot = notDestroyedHits[Random.Range(0, notDestroyedHits.Count)];
        else lastshot = new Vector2Int(999, 999);
    }

    private Vector2Int RandomPoint()
    {
        return new Vector2Int((int)(spielbrett.getOrigin1().x + 0.5f + Random.Range(0, spielbrett.getSize().x)), (int)(spielbrett.getOrigin1().y - 0.5f - Random.Range(0, -spielbrett.getSize().y)));
    }

    private Vector2Int RandomNeighbourPoint(Vector2Int Point, List<Vector2Int> excludedRelativePoints = null)
    {
        List<int> lx = new List<int>();
        List<int> ly = new List<int>();
        lx.Add(1);
        lx.Add(-1);
        ly.Add(1);
        ly.Add(-1);
        Vector2Int neighbour;
        if (excludedRelativePoints != null)
            foreach (Vector2Int q in excludedRelativePoints)
            {
                lx.Remove(q.x);
                ly.Remove(q.y);
            }
        if (lx.Count > 0 && ly.Count > 0)
        {
            if (Random.Range(0, 2) == 0)
                neighbour = new Vector2Int(lx[Random.Range(0, lx.Count)], 0);
            else
                neighbour = new Vector2Int(0, ly[Random.Range(0, ly.Count)]);
        }
        else if (lx.Count < 1 && ly.Count > 0)
            neighbour = new Vector2Int(0, ly[Random.Range(0, ly.Count)]);
        else if (ly.Count < 1 && lx.Count > 0)
            neighbour = new Vector2Int(lx[Random.Range(0, lx.Count)], 0);
        else neighbour = Vector2Int.zero;

        return Point + neighbour;
    }

    private Vector2Int[] ShuffledNeighbourPoints()
    {
        Vector2Int[] randomOrder = new Vector2Int[4];
        List<Vector2Int> excluded = new List<Vector2Int>();
        randomOrder[0] = RandomNeighbourPoint(Vector2Int.zero);
        excluded.Add(randomOrder[0]);
        randomOrder[1] = RandomNeighbourPoint(Vector2Int.zero, excluded);
        excluded.Add(randomOrder[1]);
        randomOrder[2] = RandomNeighbourPoint(Vector2Int.zero, excluded);
        excluded.Add(randomOrder[2]);
        randomOrder[3] = RandomNeighbourPoint(Vector2Int.zero, excluded);

        return randomOrder;
    }
}
