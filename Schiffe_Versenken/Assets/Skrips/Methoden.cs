using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Methoden
{
    /// <summary>
    /// Nimmt einen Integer auf und gibt 1 zurück, wenn der Integer > 1 ist und gibt 0 zurück, wenn er < 1 ist.
    /// </summary>
    public static int XLIntToInt(int i)
    {
        if (i != 0) return 1;
        else return 0;
    }
    
    public static Vector2Int Vector2ToVector2Int(Vector2 vector)
    {
        return new Vector2Int((int)vector.x, (int)vector.y);
    }

    /// <summary>
    /// Gibt zurück, ob i ungerade oder gerade ist.
    /// </summary>
    public static bool Odd(int i)
    {
            if (i % 2 == 0) return false;
            else if (i == 0) return false;
            else return true;
    }

    /// <summary>
    /// Stellt sicher, dass die Ausmaße nicht die Limits überschreiten.
    /// </summary>
    public static bool CheckDimensions(ref int  x, ref int y)
    {
        if (x < 5)
        {
            Debug.LogWarning("Minimale Größe des Spielfeldes unterschritten! Größe wurde angepasst!");
            x = 5;
        }
        else if (x > 64)
        {
            Debug.LogWarning("Maximale Größe des Spielfeldes überschritten! Größe wurde angepasst!");
            x = 64;
        }
        if (y > -5)
        {
            Debug.LogWarning("Minimale Größe des Spielfeldes unterschritten! Größe wurde angepasst!");
            y = -5;
        }
        else if (x < -64)
        {
            Debug.LogWarning("Maximale Größe des Spielfeldes überschritten! Größe wurde angepasst!");
            y = -64;
        }
        while (Mathf.Abs(x) * Mathf.Abs(y) <= 25)
        {
            Debug.LogWarning("Minimale Größe des Spielfeldes unterschritten! Größe wurde angepasst!");
            x++;
            y--;
        }
        return true;
    }

    /// <summary>
    /// Anpassung der Kammera an die Spielfeldausmaße.
    /// </summary>
    public static bool AdaptCamera(ref Camera cam, Vector2 size, Vector2 origin, float verhältnis, float borderDistance)
    {
        Vector3 oldpos = cam.transform.GetChild(0).position;

        cam.transform.position = origin + new Vector2(size.x + borderDistance, size.y / 2);

        cam.transform.GetChild(0).position = oldpos;

        cam.orthographicSize = Mathf.Abs(cam.transform.position.y) + origin.y + borderDistance;

        if (2 * size.x + 4 * borderDistance > 2 * verhältnis * cam.orthographicSize)
            cam.orthographicSize = 1 / verhältnis * ((2 * size.x + 4 * borderDistance) / 2);

        return true;
    }

    /// <summary>
    /// Anpassung der "YouWin", "YouLoose", "Overlay" und "Gefahrenwarnung" an Kammera und Seitenverhältnisses.
    /// </summary>
    public static bool AdaptOverlays(ref Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, float verhältnis, float borderDistance)
    {
        Vector3 boundsSize;
        
        Transform firstChild = cam.transform.GetChild(0);

        for (int i = 3; i < 6; i++)
        {
            firstChild.GetChild(i).position = (Vector3)(Vector2)cam.transform.position + new Vector3(0, 0, firstChild.GetChild(i).position.z);
            boundsSize = firstChild.GetChild(i).GetComponent<SpriteRenderer>().sprite.bounds.size;
            if (verhältnis >= 1)
                firstChild.GetChild(i).localScale = new Vector3(2 * cam.orthographicSize * verhältnis / 
                    boundsSize.x, 2 * cam.orthographicSize * verhältnis / boundsSize.x, 1);
            else firstChild.GetChild(i).localScale = new Vector3(2 * cam.orthographicSize / 
                boundsSize.y, 2 * cam.orthographicSize / boundsSize.y, 1);
        }

        firstChild.GetChild(2).GetChild(0).position = origin1 + size / 2;
        firstChild.GetChild(2).GetChild(1).position = origin2 + size / 2;

        firstChild.GetChild(2).GetChild(0).localPosition = new Vector3(
            firstChild.GetChild(2).GetChild(0).localPosition.x, 
            firstChild.GetChild(2).GetChild(0).localPosition.y, 0);

        firstChild.GetChild(2).GetChild(1).localPosition = new Vector3(
            firstChild.GetChild(2).GetChild(1).localPosition.x, 
            firstChild.GetChild(2).GetChild(1).localPosition.y, 0);

        firstChild.GetChild(2).GetChild(0).localScale = size * new Vector2(1, -1) /
            firstChild.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.size;

        firstChild.GetChild(2).GetChild(1).localScale = size * new Vector2(1, -1) /
            firstChild.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>().sprite.bounds.size;

        firstChild.GetComponentInChildren<LoadingBar>().SetSize(new Vector2(2 * cam.orthographicSize * verhältnis - borderDistance, cam.orthographicSize * 0.1f));
        firstChild.GetComponentInChildren<LoadingBar>().transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y + cam.orthographicSize - (firstChild.GetComponentInChildren<LoadingBar>().GetSize().y + 0.2f * borderDistance) / 2, firstChild.position.z + 10);

        return true;
    }

    /// <summary>
    /// Erstellt Koordinaten für ein Spielfeld.
    /// </summary>
    public static bool CreateCoordinates(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        CreateXCoords(cam, size, origin1, origin2, spielfeld);

        CreateYCoords(cam, size, origin1, origin2, spielfeld);

        return true;
    }

    /// <summary>
    /// Erstellt Kästchen für ein Spielfeld.
    /// </summary>
    public static bool CreateBoxes(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        GameObject kästchen;

        for (int i = 0; i < Mathf.Abs(size.y); i++)
        {
            for (int j = 0; j < Mathf.Abs(size.x); j++)
            {
                kästchen = new GameObject(j + "," + i);
                kästchen.transform.parent = cam.transform.GetChild(0).GetChild(spielfeld).GetChild(0);
                if (spielfeld == 0) kästchen.transform.position = origin1 + new Vector2(0.5f + j, -0.5f - i);
                else kästchen.transform.position = origin2 + new Vector2(0.5f + j, -0.5f - i);
                kästchen.transform.localPosition = new Vector3(kästchen.transform.localPosition.x, kästchen.transform.localPosition.y, 0);
                kästchen.AddComponent<SpriteRenderer>();
                kästchen.GetComponent<SpriteRenderer>().sprite = cam.transform.GetComponentInChildren<Spielbrett>().Square;
                if ((Odd(j) && !Odd(i)) || (!Odd(j) && Odd(i)))
                    kästchen.GetComponent<SpriteRenderer>().color = new Color(0.4941177f, 0.6352941f, 0.764706f);
                else kästchen.GetComponent<SpriteRenderer>().color = new Color(0.5333334f, 0.6745098f, 0.8078432f);
            }
        }

        return true;
    }

    private static bool CreateYCoords(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        GameObject kästchen, kästchen2;

        for (int anzahl = 0; anzahl < Mathf.Abs(size.y); anzahl++)
        {
            for (int seite = 0; seite < 2; seite++)
            {
                kästchen = new GameObject("Ky" + (anzahl + 1) + "." + seite);
                kästchen.transform.parent = cam.transform.GetChild(0).GetChild(spielfeld).GetChild(0);
                if (seite == 0)
                {
                    if (spielfeld == 0) kästchen.transform.position = origin1 + new Vector2(-0.5f, -0.5f - anzahl);
                    else kästchen.transform.position = origin2 + new Vector2(-0.5f, -0.5f - anzahl);
                }
                else
                {
                    if (spielfeld == 0) kästchen.transform.position = origin1 + new Vector2(-0.5f + size.x + seite, -0.5f - anzahl);
                    else kästchen.transform.position = origin2 + new Vector2(-0.5f + size.x + seite, -0.5f - anzahl);
                }
                kästchen.transform.localPosition = new Vector3(kästchen.transform.localPosition.x, kästchen.transform.localPosition.y, 0);
                kästchen.AddComponent<SpriteRenderer>();
                if (anzahl < 9)
                    kästchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(28 + anzahl);
                else
                {
                    kästchen.transform.position += new Vector3(0.25f, 0);
                    kästchen2 = new GameObject("first digit");
                    kästchen2.transform.parent = kästchen.transform;
                    kästchen2.transform.localPosition = new Vector3(-0.5f, 0, 0);
                    kästchen2.AddComponent<SpriteRenderer>();
                    kästchen2.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite("" + ((anzahl + 1) / 10));
                    kästchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite("" + (anzahl + 1 - (anzahl + 1) / 10 * 10));
                }
            }
        }

        return true;
    }

    private static bool CreateXCoords(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        GameObject kästchen;

        for (int anzahl = 0; anzahl < Mathf.Abs(size.x); anzahl++)
        {
            for (int seite = 0; seite < 2; seite++)
            {
                kästchen = new GameObject("Kx" + (anzahl + 1) + "." + seite);
                kästchen.transform.parent = cam.transform.GetChild(0).GetChild(spielfeld).GetChild(0);
                if (seite == 0)
                {
                    if (spielfeld == 0) kästchen.transform.position = origin1 + new Vector2(0.5f + anzahl, 0.5f);
                    else kästchen.transform.position = origin2 + new Vector2(0.5f + anzahl, 0.5f);
                }
                else
                {
                    if (spielfeld == 0) kästchen.transform.position = origin1 + new Vector2(0.5f + anzahl, 0.5f + size.y - seite);
                    else kästchen.transform.position = origin2 + new Vector2(0.5f + anzahl, 0.5f + size.y - seite);
                }
                kästchen.transform.localPosition = new Vector3(kästchen.transform.localPosition.x, kästchen.transform.localPosition.y, 0);
                kästchen.AddComponent<SpriteRenderer>();
                if (anzahl < 27) kästchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(anzahl);
                else if (anzahl >= 27 && anzahl < 54) kästchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(anzahl - 27);
                else kästchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(anzahl - 54);
            }
        }

        return true;
    }
}
