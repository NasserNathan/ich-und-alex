using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Methoden
{
    /// <summary>
    /// Nimmt einen Integer auf und gibt 1 zur�ck, wenn der Integer > 1 ist und gibt 0 zur�ck, wenn er < 1 ist.
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
    /// Gibt zur�ck, ob i ungerade oder gerade ist.
    /// </summary>
    public static bool Odd(int i)
    {
            if (i % 2 == 0) return false;
            else if (i == 0) return false;
            else return true;
    }

    /// <summary>
    /// Stellt sicher, dass die Ausma�e nicht die Limits �berschreiten.
    /// </summary>
    public static bool CheckDimensions(ref int  x, ref int y)
    {
        if (x < 5)
        {
            Debug.LogWarning("Minimale Gr��e des Spielfeldes unterschritten! Gr��e wurde angepasst!");
            x = 5;
        }
        else if (x > 64)
        {
            Debug.LogWarning("Maximale Gr��e des Spielfeldes �berschritten! Gr��e wurde angepasst!");
            x = 64;
        }
        if (y > -5)
        {
            Debug.LogWarning("Minimale Gr��e des Spielfeldes unterschritten! Gr��e wurde angepasst!");
            y = -5;
        }
        else if (x < -64)
        {
            Debug.LogWarning("Maximale Gr��e des Spielfeldes �berschritten! Gr��e wurde angepasst!");
            y = -64;
        }
        while (Mathf.Abs(x) * Mathf.Abs(y) <= 25)
        {
            Debug.LogWarning("Minimale Gr��e des Spielfeldes unterschritten! Gr��e wurde angepasst!");
            x++;
            y--;
        }
        return true;
    }

    /// <summary>
    /// Anpassung der Kammera an die Spielfeldausma�e.
    /// </summary>
    public static bool AdaptCamera(ref Camera cam, Vector2 size, Vector2 origin, float verh�ltnis, float borderDistance)
    {
        Vector3 oldpos = cam.transform.GetChild(0).position;

        cam.transform.position = origin + new Vector2(size.x + borderDistance, size.y / 2);

        cam.transform.GetChild(0).position = oldpos;

        cam.orthographicSize = Mathf.Abs(cam.transform.position.y) + origin.y + borderDistance;

        if (2 * size.x + 4 * borderDistance > 2 * verh�ltnis * cam.orthographicSize)
            cam.orthographicSize = 1 / verh�ltnis * ((2 * size.x + 4 * borderDistance) / 2);

        return true;
    }

    /// <summary>
    /// Anpassung der "YouWin", "YouLoose", "Overlay" und "Gefahrenwarnung" an Kammera und Seitenverh�ltnisses.
    /// </summary>
    public static bool AdaptOverlays(ref Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, float verh�ltnis, float borderDistance)
    {
        Vector3 boundsSize;
        
        Transform firstChild = cam.transform.GetChild(0);

        for (int i = 3; i < 6; i++)
        {
            firstChild.GetChild(i).position = (Vector3)(Vector2)cam.transform.position + new Vector3(0, 0, firstChild.GetChild(i).position.z);
            boundsSize = firstChild.GetChild(i).GetComponent<SpriteRenderer>().sprite.bounds.size;
            if (verh�ltnis >= 1)
                firstChild.GetChild(i).localScale = new Vector3(2 * cam.orthographicSize * verh�ltnis / 
                    boundsSize.x, 2 * cam.orthographicSize * verh�ltnis / boundsSize.x, 1);
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

        firstChild.GetComponentInChildren<LoadingBar>().SetSize(new Vector2(2 * cam.orthographicSize * verh�ltnis - borderDistance, cam.orthographicSize * 0.1f));
        firstChild.GetComponentInChildren<LoadingBar>().transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y + cam.orthographicSize - (firstChild.GetComponentInChildren<LoadingBar>().GetSize().y + 0.2f * borderDistance) / 2, firstChild.position.z + 10);

        return true;
    }

    /// <summary>
    /// Erstellt Koordinaten f�r ein Spielfeld.
    /// </summary>
    public static bool CreateCoordinates(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        CreateXCoords(cam, size, origin1, origin2, spielfeld);

        CreateYCoords(cam, size, origin1, origin2, spielfeld);

        return true;
    }

    /// <summary>
    /// Erstellt K�stchen f�r ein Spielfeld.
    /// </summary>
    public static bool CreateBoxes(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        GameObject k�stchen;

        for (int i = 0; i < Mathf.Abs(size.y); i++)
        {
            for (int j = 0; j < Mathf.Abs(size.x); j++)
            {
                k�stchen = new GameObject(j + "," + i);
                k�stchen.transform.parent = cam.transform.GetChild(0).GetChild(spielfeld).GetChild(0);
                if (spielfeld == 0) k�stchen.transform.position = origin1 + new Vector2(0.5f + j, -0.5f - i);
                else k�stchen.transform.position = origin2 + new Vector2(0.5f + j, -0.5f - i);
                k�stchen.transform.localPosition = new Vector3(k�stchen.transform.localPosition.x, k�stchen.transform.localPosition.y, 0);
                k�stchen.AddComponent<SpriteRenderer>();
                k�stchen.GetComponent<SpriteRenderer>().sprite = cam.transform.GetComponentInChildren<Spielbrett>().Square;
                if ((Odd(j) && !Odd(i)) || (!Odd(j) && Odd(i)))
                    k�stchen.GetComponent<SpriteRenderer>().color = new Color(0.4941177f, 0.6352941f, 0.764706f);
                else k�stchen.GetComponent<SpriteRenderer>().color = new Color(0.5333334f, 0.6745098f, 0.8078432f);
            }
        }

        return true;
    }

    private static bool CreateYCoords(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        GameObject k�stchen, k�stchen2;

        for (int anzahl = 0; anzahl < Mathf.Abs(size.y); anzahl++)
        {
            for (int seite = 0; seite < 2; seite++)
            {
                k�stchen = new GameObject("Ky" + (anzahl + 1) + "." + seite);
                k�stchen.transform.parent = cam.transform.GetChild(0).GetChild(spielfeld).GetChild(0);
                if (seite == 0)
                {
                    if (spielfeld == 0) k�stchen.transform.position = origin1 + new Vector2(-0.5f, -0.5f - anzahl);
                    else k�stchen.transform.position = origin2 + new Vector2(-0.5f, -0.5f - anzahl);
                }
                else
                {
                    if (spielfeld == 0) k�stchen.transform.position = origin1 + new Vector2(-0.5f + size.x + seite, -0.5f - anzahl);
                    else k�stchen.transform.position = origin2 + new Vector2(-0.5f + size.x + seite, -0.5f - anzahl);
                }
                k�stchen.transform.localPosition = new Vector3(k�stchen.transform.localPosition.x, k�stchen.transform.localPosition.y, 0);
                k�stchen.AddComponent<SpriteRenderer>();
                if (anzahl < 9)
                    k�stchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(28 + anzahl);
                else
                {
                    k�stchen.transform.position += new Vector3(0.25f, 0);
                    k�stchen2 = new GameObject("first digit");
                    k�stchen2.transform.parent = k�stchen.transform;
                    k�stchen2.transform.localPosition = new Vector3(-0.5f, 0, 0);
                    k�stchen2.AddComponent<SpriteRenderer>();
                    k�stchen2.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite("" + ((anzahl + 1) / 10));
                    k�stchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite("" + (anzahl + 1 - (anzahl + 1) / 10 * 10));
                }
            }
        }

        return true;
    }

    private static bool CreateXCoords(Camera cam, Vector2 size, Vector2 origin1, Vector2 origin2, int spielfeld)
    {
        GameObject k�stchen;

        for (int anzahl = 0; anzahl < Mathf.Abs(size.x); anzahl++)
        {
            for (int seite = 0; seite < 2; seite++)
            {
                k�stchen = new GameObject("Kx" + (anzahl + 1) + "." + seite);
                k�stchen.transform.parent = cam.transform.GetChild(0).GetChild(spielfeld).GetChild(0);
                if (seite == 0)
                {
                    if (spielfeld == 0) k�stchen.transform.position = origin1 + new Vector2(0.5f + anzahl, 0.5f);
                    else k�stchen.transform.position = origin2 + new Vector2(0.5f + anzahl, 0.5f);
                }
                else
                {
                    if (spielfeld == 0) k�stchen.transform.position = origin1 + new Vector2(0.5f + anzahl, 0.5f + size.y - seite);
                    else k�stchen.transform.position = origin2 + new Vector2(0.5f + anzahl, 0.5f + size.y - seite);
                }
                k�stchen.transform.localPosition = new Vector3(k�stchen.transform.localPosition.x, k�stchen.transform.localPosition.y, 0);
                k�stchen.AddComponent<SpriteRenderer>();
                if (anzahl < 27) k�stchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(anzahl);
                else if (anzahl >= 27 && anzahl < 54) k�stchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(anzahl - 27);
                else k�stchen.GetComponent<SpriteRenderer>().sprite = Symbols.GetSprite(anzahl - 54);
            }
        }

        return true;
    }
}
