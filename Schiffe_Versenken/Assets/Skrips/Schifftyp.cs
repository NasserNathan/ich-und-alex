using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schifftyp
{
    public string name { get; private set; }
    public Vector2Int ausmaße { get; private set; }
    public List<Vector3Int> kästchenstruktur { get; private set; } //xy-Koordinate: Position; z-Koordinate: 0 = nicht markiert, 1 = getroffen

    /// <summary>
    /// Setzt die Kästchenstruktur und fügt dem Namen einen Kommentar hinzu
    /// </summary>
    public void SetKästchenstruktur(List<Vector3Int> kästchenstruktur, int rotation)
    {
        this.kästchenstruktur = SetNullpunkt(kästchenstruktur);
        this.name += ", r=" + rotation;
        GetAusmaße();
    }
    /// <summary>
    /// Setzt die z-Koordinate eines Kästchens der Kästchenstruktur und fügt dem Namen einen Kommentar hinzu.
    /// </summary>
    public void SetKästchenstruktur(int index, int z)
    {
        if (z >= 1)
            z = 1;
        else z = 0;
        kästchenstruktur[index] = new Vector3Int(kästchenstruktur[index].x, kästchenstruktur[index].y, z);
        name += ", z" + (Vector2Int)kästchenstruktur[index] + "=" + z;
    }
    /// <summary>
    /// Berechnet und sortiert die Punkte relativ zur transform.position, die miteinander verbunden den Umriss einer Kästchenstruktur ergeben und gibt diese dann in einem Array aus.
    /// </summary>
    public Vector2[] ÄußerePunkte() 
    {
        Vector2 ausmaßehalbe = (Vector2)ausmaße / 2;
        List<Vector2> punkte = new List<Vector2>();
        List<Vector2> newpunkte = new List<Vector2>();

        foreach (Vector3 kästchen in kästchenstruktur) //Entfernung der unrelevanten Punkte.
        {
            if (!punkte.Remove(new Vector2(kästchen.x + 0.5f, kästchen.y + 0.5f))) 
                punkte.Add(new Vector2(kästchen.x + 0.5f, kästchen.y + 0.5f));
            if (!punkte.Remove(new Vector2(kästchen.x - 0.5f, kästchen.y + 0.5f)))
                punkte.Add(new Vector2(kästchen.x - 0.5f, kästchen.y + 0.5f));
            if (!punkte.Remove(new Vector2(kästchen.x + 0.5f, kästchen.y - 0.5f)))
                punkte.Add(new Vector2(kästchen.x + 0.5f, kästchen.y - 0.5f));
            if (!punkte.Remove(new Vector2(kästchen.x - 0.5f, kästchen.y - 0.5f)))
                punkte.Add(new Vector2(kästchen.x - 0.5f, kästchen.y - 0.5f));
        }
        foreach(Vector2 punkt in punkte) //Berechnung der Punkte relativ zur transform.position.
        {
            newpunkte.Add(new Vector2(punkt.x - ausmaßehalbe.x + 0.5f, punkt.y + ausmaßehalbe.y - 0.5f)); 
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
    //Berechnung der Ausmaße
    private void GetAusmaße() 
    {
        if(kästchenstruktur == null) { ausmaße = new Vector2Int(0, 0); }

        Vector2Int min = new Vector2Int(kästchenstruktur[0].x, kästchenstruktur[0].y);
        Vector2Int max = new Vector2Int(kästchenstruktur[0].x, kästchenstruktur[0].y);

        foreach (Vector2Int kästchen in kästchenstruktur)
        {
            if (kästchen.x < min.x) min.x = kästchen.x;
            else if (kästchen.x > max.x) max.x = kästchen.x;
            if (kästchen.y < min.y) min.y = kästchen.y;
            else if (kästchen.y > max.y) max.y = kästchen.y;
        }
        ausmaße = new Vector2Int(Mathf.Abs(max.x - min.x) + 1, Mathf.Abs(max.y - min.y) + 1);
    }
    //Setzt den Nullpunkt der Kästchenstruktur in die obere linke Ecke
    private List<Vector3Int> SetNullpunkt(List<Vector3Int> kästchenstruktur)
    {
        List<Vector3Int> newkästchenstruktur = new List<Vector3Int>();
        Vector2Int min = new Vector2Int(kästchenstruktur[0].x, kästchenstruktur[0].y);
        Vector2Int max = new Vector2Int(kästchenstruktur[0].x, kästchenstruktur[0].y);
        foreach (Vector2Int kästchen in kästchenstruktur)
        {
            if (kästchen.x < min.x) min.x = kästchen.x;
            else if (kästchen.x > max.x) max.x = kästchen.x;
            if (kästchen.y < min.y) min.y = kästchen.y;
            else if (kästchen.y > max.y) max.y = kästchen.y;
        }
        foreach (Vector3Int kästchen in kästchenstruktur)
        {
            newkästchenstruktur.Add(new Vector3Int(kästchen.x - min.x, kästchen.y - max.y, kästchen.z));
        }
        return newkästchenstruktur;
    }
    //Gibt ein Array mit allen benachbarten Punkten eine Punktes aus einer List<Vector2> zurück. Benachbart sind zwei Punkte wenn diese gemeinsam auf einer Horizontalen bzw. Vertikalen nebeneinander liegen.
    private Vector2[] NeighbourPoints(List<Vector2> vectorList, int index) 
    {
        List<Vector2> points = new List<Vector2>();
        int newindex;
        for (int i = 0; i <= ausmaße.y; i++)
        {
            newindex = vectorList.IndexOf(vectorList[index] + new Vector2(0, -1 - i));
            if (newindex != -1)
            {
                points.Add(vectorList[newindex]);
                break;
            }
        }
        for (int i = 0; i <= ausmaße.y; i++)
        {
            newindex = vectorList.IndexOf(vectorList[index] + new Vector2(0, 1 + i));
            if (newindex != -1)
            {
                points.Add(vectorList[newindex]);
                break;
            }
        }
        for (int i = 0; i <= ausmaße.x; i++)
        {
            newindex = vectorList.IndexOf(vectorList[index] + new Vector2(-1 - i, 0));
            if (newindex != -1)
            {
                points.Add(vectorList[newindex]);
                break;
            }
        }
        for (int i = 0; i <= ausmaße.x; i++)
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
    public static Schifftyp Zerstörer()
    {
        List<Vector3Int> KS = new List<Vector3Int>();
        KS.Add(new Vector3Int(0, 0, 0));
        KS.Add(new Vector3Int(1, 0, 0));
        return new Schifftyp("Zerstörer", KS);
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
    public static Schifftyp Flugzeugträger()
    {
        List<Vector3Int> KS = new List<Vector3Int>();
        KS.Add(new Vector3Int(0, 0, 0));
        KS.Add(new Vector3Int(1, 0, 0));
        KS.Add(new Vector3Int(1, -1, 0));
        KS.Add(new Vector3Int(2, 0, 0));
        KS.Add(new Vector3Int(3, 0, 0));
        return new Schifftyp("Flugzeugträger", KS);
    }

    //Konstruktor
    public Schifftyp(string name, List<Vector3Int> kästchenstruktur)
    {
        this.name = name;
        this.kästchenstruktur = SetNullpunkt(kästchenstruktur);
        GetAusmaße();
    } 
}
