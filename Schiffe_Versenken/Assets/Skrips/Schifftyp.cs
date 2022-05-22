using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schifftyp
{
    public string name { get; private set; }
    public Vector2Int ausma�e { get; private set; }
    public List<Vector3Int> k�stchenstruktur { get; private set; } //xy-Koordinate: Position; z-Koordinate: 0 = nicht markiert, 1 = getroffen

    /// <summary>
    /// Setzt die K�stchenstruktur und f�gt dem Namen einen Kommentar hinzu
    /// </summary>
    public void SetK�stchenstruktur(List<Vector3Int> k�stchenstruktur, int rotation)
    {
        this.k�stchenstruktur = SetNullpunkt(k�stchenstruktur);
        this.name += ", r=" + rotation;
        GetAusma�e();
    }
    /// <summary>
    /// Setzt die z-Koordinate eines K�stchens der K�stchenstruktur und f�gt dem Namen einen Kommentar hinzu.
    /// </summary>
    public void SetK�stchenstruktur(int index, int z)
    {
        if (z >= 1)
            z = 1;
        else z = 0;
        k�stchenstruktur[index] = new Vector3Int(k�stchenstruktur[index].x, k�stchenstruktur[index].y, z);
        name += ", z" + (Vector2Int)k�stchenstruktur[index] + "=" + z;
    }
    /// <summary>
    /// Berechnet und sortiert die Punkte relativ zur transform.position, die miteinander verbunden den Umriss einer K�stchenstruktur ergeben und gibt diese dann in einem Array aus.
    /// </summary>
    public Vector2[] �u�erePunkte() 
    {
        Vector2 ausma�ehalbe = (Vector2)ausma�e / 2;
        List<Vector2> punkte = new List<Vector2>();
        List<Vector2> newpunkte = new List<Vector2>();

        foreach (Vector3 k�stchen in k�stchenstruktur) //Entfernung der unrelevanten Punkte.
        {
            if (!punkte.Remove(new Vector2(k�stchen.x + 0.5f, k�stchen.y + 0.5f))) 
                punkte.Add(new Vector2(k�stchen.x + 0.5f, k�stchen.y + 0.5f));
            if (!punkte.Remove(new Vector2(k�stchen.x - 0.5f, k�stchen.y + 0.5f)))
                punkte.Add(new Vector2(k�stchen.x - 0.5f, k�stchen.y + 0.5f));
            if (!punkte.Remove(new Vector2(k�stchen.x + 0.5f, k�stchen.y - 0.5f)))
                punkte.Add(new Vector2(k�stchen.x + 0.5f, k�stchen.y - 0.5f));
            if (!punkte.Remove(new Vector2(k�stchen.x - 0.5f, k�stchen.y - 0.5f)))
                punkte.Add(new Vector2(k�stchen.x - 0.5f, k�stchen.y - 0.5f));
        }
        foreach(Vector2 punkt in punkte) //Berechnung der Punkte relativ zur transform.position.
        {
            newpunkte.Add(new Vector2(punkt.x - ausma�ehalbe.x + 0.5f, punkt.y + ausma�ehalbe.y - 0.5f)); 
        }
        punkte.Clear();

        Vector2 nextPoint = newpunkte[0];
        Vector2 lastPoint = newpunkte[0];
        punkte.Add(nextPoint);
        Vector2 lastvector;
        Vector2 newvector;
        int neighbourMin;
        int count;

        for (int i = 0; i <= newpunkte.Count -2; i++) //Sortierung der Punkte.
        {
            lastvector = nextPoint - lastPoint;
            lastPoint = nextPoint;
            neighbourMin = 5;
            foreach (Vector2 neighbourpoint in NeighbourPoints(newpunkte, newpunkte.IndexOf(lastPoint)))
            {
                newvector = neighbourpoint - lastPoint;
                if (newvector.normalized != lastvector.normalized && newvector.normalized != -lastvector.normalized)
                {
                    count = NeighbourPoints(newpunkte, newpunkte.IndexOf(neighbourpoint)).Length;
                    if (count < neighbourMin || (count == neighbourMin && newvector.magnitude < (nextPoint - lastPoint).magnitude))
                    {
                        nextPoint = neighbourpoint;
                        neighbourMin = count;
                    }
                }
            }
            punkte.Add(nextPoint);
        }
        return punkte.ToArray();
    }
    //Berechnung der Ausma�e
    private void GetAusma�e() 
    {
        if(k�stchenstruktur == null) { ausma�e = new Vector2Int(0, 0); }

        Vector2Int min = new Vector2Int(k�stchenstruktur[0].x, k�stchenstruktur[0].y);
        Vector2Int max = new Vector2Int(k�stchenstruktur[0].x, k�stchenstruktur[0].y);

        foreach (Vector2Int k�stchen in k�stchenstruktur)
        {
            if (k�stchen.x < min.x) min.x = k�stchen.x;
            else if (k�stchen.x > max.x) max.x = k�stchen.x;
            if (k�stchen.y < min.y) min.y = k�stchen.y;
            else if (k�stchen.y > max.y) max.y = k�stchen.y;
        }
        ausma�e = new Vector2Int(Mathf.Abs(max.x - min.x) + 1, Mathf.Abs(max.y - min.y) + 1);
    }
    //Setzt den Nullpunkt der K�stchenstruktur in die obere linke Ecke
    private List<Vector3Int> SetNullpunkt(List<Vector3Int> k�stchenstruktur)
    {
        List<Vector3Int> newk�stchenstruktur = new List<Vector3Int>();
        Vector2Int min = new Vector2Int(k�stchenstruktur[0].x, k�stchenstruktur[0].y);
        Vector2Int max = new Vector2Int(k�stchenstruktur[0].x, k�stchenstruktur[0].y);
        foreach (Vector2Int k�stchen in k�stchenstruktur)
        {
            if (k�stchen.x < min.x) min.x = k�stchen.x;
            else if (k�stchen.x > max.x) max.x = k�stchen.x;
            if (k�stchen.y < min.y) min.y = k�stchen.y;
            else if (k�stchen.y > max.y) max.y = k�stchen.y;
        }
        foreach (Vector3Int k�stchen in k�stchenstruktur)
        {
            newk�stchenstruktur.Add(new Vector3Int(k�stchen.x - min.x, k�stchen.y - max.y, k�stchen.z));
        }
        return newk�stchenstruktur;
    }
    //Gibt ein Array mit allen benachbarten Punkten eine Punktes aus einer List<Vector2> zur�ck. Benachbart sind zwei Punkte wenn diese gemeinsam auf einer Horizontalen bzw. Vertikalen nebeneinander liegen.
    private Vector2[] NeighbourPoints(List<Vector2> vectorList, int index) 
    {
        List<Vector2> points = new List<Vector2>();
        int newindex;
        for (int i = 0; i <= ausma�e.y; i++)
        {
            newindex = vectorList.IndexOf(vectorList[index] + new Vector2(0, -1 - i));
            if (newindex != -1)
            {
                points.Add(vectorList[newindex]);
                break;
            }
        }
        for (int i = 0; i <= ausma�e.y; i++)
        {
            newindex = vectorList.IndexOf(vectorList[index] + new Vector2(0, 1 + i));
            if (newindex != -1)
            {
                points.Add(vectorList[newindex]);
                break;
            }
        }
        for (int i = 0; i <= ausma�e.x; i++)
        {
            newindex = vectorList.IndexOf(vectorList[index] + new Vector2(-1 - i, 0));
            if (newindex != -1)
            {
                points.Add(vectorList[newindex]);
                break;
            }
        }
        for (int i = 0; i <= ausma�e.x; i++)
        {
            newindex = vectorList.IndexOf(vectorList[index] + new Vector2(1 + i, 0));
            if (newindex != -1)
            {
                points.Add(vectorList[newindex]);
                break;
            }
        }

        return points.ToArray();
    }

    //vorgefertigte Schiffsstrukturen:
    public static Schifftyp Uboot()
    {
        List<Vector3Int> KS = new List<Vector3Int>();
        KS.Add(new Vector3Int(0, 0, 0));
        return new Schifftyp("Uboot", KS);
    }
    public static Schifftyp Zerst�rer()
    {
        List<Vector3Int> KS = new List<Vector3Int>();
        KS.Add(new Vector3Int(0, 0, 0));
        KS.Add(new Vector3Int(1, 0, 0));
        return new Schifftyp("Zerst�rer", KS);
    }
    public static Schifftyp Kreuzer()
    {
        List<Vector3Int> KS = new List<Vector3Int>();
        KS.Add(new Vector3Int(0, 0, 0));
        KS.Add(new Vector3Int(1, 0, 0));
        KS.Add(new Vector3Int(2, 0, 0));
        return new Schifftyp("Kreuzer", KS);
    }
    public static Schifftyp Schlachtschiff()
    {
        List<Vector3Int> KS = new List<Vector3Int>();
        KS.Add(new Vector3Int(0, 0, 0));
        KS.Add(new Vector3Int(1, 0, 0));
        KS.Add(new Vector3Int(2, 0, 0));
        KS.Add(new Vector3Int(3, 0, 0));
        return new Schifftyp("Schlachtschiff", KS);
    }
    public static Schifftyp Flugzeugtr�ger()
    {
        List<Vector3Int> KS = new List<Vector3Int>();
        KS.Add(new Vector3Int(0, 0, 0));
        KS.Add(new Vector3Int(1, 0, 0));
        KS.Add(new Vector3Int(1, -1, 0));
        KS.Add(new Vector3Int(2, 0, 0));
        KS.Add(new Vector3Int(3, 0, 0));
        return new Schifftyp("Flugzeugtr�ger", KS);
    }

    //Konstruktor
    public Schifftyp(string name, List<Vector3Int> k�stchenstruktur)
    {
        this.name = name;
        this.k�stchenstruktur = SetNullpunkt(k�stchenstruktur);
        GetAusma�e();
    } 
}
