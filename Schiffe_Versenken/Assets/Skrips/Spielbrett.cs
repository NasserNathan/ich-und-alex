using System.Collections.Generic;
using UnityEngine;

public class Spielbrett : MonoBehaviour
{
    [Header("     Ships:")]
    [Header("Sprites:")]
    public Sprite SpriteUboot;
    public Sprite SpriteSchlachtschiff;
    public Sprite SpriteKreuzer;
    public Sprite SpriteZerst�rer;
    public Sprite SpriteFlugzeugtr�ger;
    public Sprite Square;
    [Header("     Marks:")]
    public Sprite MarkDaneben;
    public Sprite MarkTreffer;
    public Sprite MarkZerst�rt;
    [Header("Spiel:")]
    public float zugZeitInMinuten;
    public float countdownInSek;
    public bool Zug;
    public int zNr;

    private Vector2Int size = new Vector2Int(16, -16); //Gr��e eines Spielfeldes (bitte noch nicht ver�ndern!)
    [SerializeField]
    private Vector2 origin1 = new Vector2(-0.5f, 0.5f); //World Position
    [SerializeField]
    private Vector2 origin2 = new Vector2(15.5f, 0.5f); //World Position
    private int[,] besetzung1 = new int[64, 64]; //koordinatenweise Besetzung des ersten Spielfeldes
    private int[,] besetzung2 = new int[64, 64]; //koordinatenweise Besetzung des zweiten Spielfeldes
    private List<int> ids = new List<int>(); //Liste der IDs aller Schiffe im Spiel

    public bool zug { get; private set; } //wenn true, Spieler am Zug, wenn false Feind am Zug
    public int zugNummer { get; private set; }
    public int countdown { get; private set; }
    public bool loose { get; private set; }
    public bool win { get; private set; }
    public bool tryingToStart { get; private set; }
    public int difficulty;
    private int oldcountdown;
    public float y { get; set; }
    private bool gefahrenwarnung;
    private float timer;
    private bool oldgefahrenwarnung;
    private GameObject winOBJ;
    private GameObject looseOBJ;
    public Countdown countD;
    private void Awake()
    {
        countD = GetComponentInChildren<Countdown>();
    }

    void Start()
    {
        StartGame(2);
    }

    private void Update()
    {
        //Variablen f�r Inspektor
        Zug = zug;
        zNr = zugNummer;

        //Anti-Division-durch-0
        if (zugZeitInMinuten == 0)
            zugZeitInMinuten = 0.0001f;

        //Zugzeit-Countdown
        //if (countdown == 1) //Verringerung
        //{
        //    if (y >= 36)
        //        zugNummer++;
        //    if (y <= 0)
        //    {
        //        y = 0;
        //        if (CheckPlacedShips(1))
        //        {
        //            zug = false;
        //            countdown = 0;
        //            tryingToStart = false;
        //        }
        //        else tryingToStart = true;
        //    }
        //    else y -= Time.deltaTime * (1 / (zugZeitInMinuten * (5f / 3f)));

        //    transform.parent.GetChild(1).localScale = new Vector3(transform.GetChild(1).localScale.x - 0.5f, y, transform.GetChild(1).localScale.z);
        //}
        //else if (countdown == -1) //Erh�hung
        //{
        //    if (y >= 36)
        //    {
        //        y = 36;
        //        countdown = 0;
        //    }
        //    else y += Time.deltaTime * (y + 1) * 5;

        //    transform.parent.GetChild(1).localScale = new Vector3(transform.GetChild(1).localScale.x - 0.5f, y, transform.GetChild(1).localScale.z);
        //}
        //countdownInSek = y * (5f / 3f) * zugZeitInMinuten;

        if (countD.GetProgress() == 0)
        {
            if (zugNummer == -1)
            {
                countD.SetPercentage(100);
                countD.CountdownStart(-15, true);
                zug = true;
            }
            else
            {
                zug = false;
                countD.CountdownStart(0.75f, true);
            }
            zugNummer++;
        }
        if (countD.GetProgress() == 100)
        {

        }


        //Lautst�rke langsam erh�hen bis 0.1f am Anfang des Kampfes
        if (!win && !loose)
            transform.parent.GetComponent<AudioSource>().volume = Mathf.Lerp(transform.parent.GetComponent<AudioSource>().volume, 0.1f, Time.deltaTime * 0.25f);

        //Win/Loose-Titel langsam einblenden
        if (winOBJ.activeSelf)
            winOBJ.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(winOBJ.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 0.25f));
        if (looseOBJ.activeSelf)
            looseOBJ.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(looseOBJ.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 0.25f));

        //Gefahrenwarnung
        if (gefahrenwarnung)
        {
            if (gefahrenwarnung != oldgefahrenwarnung)
            {
                oldgefahrenwarnung = gefahrenwarnung;
                transform.GetChild(3).gameObject.SetActive(gefahrenwarnung);
                GetComponentsInParent<AudioSource>()[1].Play();
            }
            timer += 1 * Time.deltaTime;
            if (timer >= 0.75f)
            {
                gefahrenwarnung = false;
                oldgefahrenwarnung = gefahrenwarnung;
                transform.GetChild(3).gameObject.SetActive(false);
                timer = 0;
            }
        }
    }

    public Vector2Int getSize() { return size; }
    public Vector2 getOrigin1() { return origin1; }
    public Vector2 getOrigin2() { return origin2; }

    /// <summary>
    /// Generiert die Spielfelder des Spielbrettes und passt Kammera und andere Optische Elemente an die Gr��e und Bildschirm an.
    /// </summary>
    /// <param name="x">-Gr��e der Spielfelder auf der X-Achse; immer positiv</param>
    /// <param name="y">-Gr��e der Spielfelder auf der Y-Achse; immer negativ</param>
    /// <param name="randabstand">-Randabstand zwischen Spielfeldern und Rand </param>
    /// <param name="verh�ltnis">-Verh�ltnis der Bildschirmbreite zur Bildschirmh�he (B/H)</param>
    public void SetSize(int x, int y, float randabstand = 3, float verh�ltnis = 16f / 9f)
    {
        Camera cam = GameObject.Find("Spielbrett").GetComponentInParent<Camera>();
        verh�ltnis = Mathf.Abs(verh�ltnis);

        Methoden.CheckDimensions(ref x, ref y);

        size = new Vector2Int(x, y);
        origin1 = new Vector3(-0.5f, 0.5f, 20);
        origin2 = origin1 + new Vector2(size.x + 2 * randabstand, 0);

        Methoden.AdaptCamera(ref cam, size, origin1, verh�ltnis, randabstand);

        Methoden.AdaptOverlays(ref cam, size, origin1, origin2, verh�ltnis, randabstand);

        //Erzeugung aller K�stchen und Koordinaten beider Spielfelder
        for (int p = 0; p < 2; p++)
        {
            Methoden.CreateBoxes(cam, size, origin1, origin2, p);
            Methoden.CreateCoordinates(cam, size, origin1, origin2, p);
        }
    }

    /// <summary>
    /// Erstellt ein gameobject Schiff mit allen n�tigen Components und Eigenschaften.
    /// </summary>
    public void CreateShip(string name, Sprite sprite, Schifftyp schifftyp, int spielfeld, bool placeRandom = false, bool debug = false)
    {
        GameObject GOship = new GameObject(name);
        GOship.AddComponent<SpriteRenderer>();
        if (spielfeld == 1)
            GOship.transform.parent = GameObject.Find("Spielbrett").transform.GetChild(0).transform;
        else if (spielfeld == 2)
        {
            GOship.transform.parent = GameObject.Find("Spielbrett").transform.GetChild(1).transform;
            placeRandom = true;
        }
        else
        {
            Debug.LogError("Dieses Spielfeld existiert nicht!");
            Destroy(GOship);
            return;
        }
        GOship.transform.localPosition = new Vector3(GOship.transform.localPosition.x, GOship.transform.localPosition.y, 0);

        SpriteRenderer sprShip = GOship.GetComponent<SpriteRenderer>();
        sprShip.sortingOrder = 1;
        sprShip.sprite = sprite;

        GOship.AddComponent<Rigidbody2D>().isKinematic = true;
        GOship.AddComponent<Schiff>();
        GOship.AddComponent<Select>();
        GOship.AddComponent<KeyboardMovement>();

        Schiff ship = GOship.GetComponent<Schiff>();
        ship.schifftyp = schifftyp;
        ship.SetSpielfeld(spielfeld);

        GOship.AddComponent<PolygonCollider2D>().points = ship.schifftyp.�u�erePunkte();

        ship.SetRotation(Random.Range(0, 3));

        if (!TryPlaceSchiff(ship, debug))
        {
            DestroySchiff(ship, true, name + " konnte nicht platziert werden und wurde zerst�rt", true);
            return;
        }
        if (placeRandom)
            RandomPlaceSchiff(ship, debug);
    }

    /// <summary>
    /// F�gt ein Schiff zum Koordinatensystem des Spielfeldes hinzu. Gibt false zur�ck, wenn es nicht platziert werden kann, weil es Grenzen �berschreitet oder weil ein anderes Schiff bereits an dieser Stelle vorliegt.
    /// </summary>
    public bool AddSchiff(Schiff schiff, bool debug = false)
    {
        int posX = schiff.position.x;
        int posY = schiff.position.y;
        List<Vector3Int> k�stchenstruktur = schiff.schifftyp.k�stchenstruktur;
        Vector2 origin;
        int[,] besetzung;
        if (schiff.spielfeld == 1)
        {
            origin = origin1 + new Vector2(0.5f, -0.5f);
            besetzung = besetzung1;
        }
        else if (schiff.spielfeld == 2)
        {
            origin = origin2 + new Vector2(0.5f, -0.5f);
            besetzung = besetzung2;
        }
        else
        {
            Debug.LogError("Spielfeld " + schiff.spielfeld + " existiert nicht!");
            return false;
        }

        foreach (Vector2Int k�stchen in k�stchenstruktur)
        {
            if (posX + k�stchen.x < 0 + origin.x || posX + k�stchen.x > size.x - 1 + origin.x || -posY - k�stchen.y < 0 - origin.y || -posY - k�stchen.y > -size.y - 1 - origin.y)
            {
                schiff.placed = false;
                Debug.Log(schiff.gameObject.name + " konnte nicht platziert werden ; Grenz�berschreitung!");
                return false;
            }
            else if (besetzung[posX + k�stchen.x - (int)origin.x, -posY - k�stchen.y + (int)origin.y] != 0 && besetzung[posX + k�stchen.x - (int)origin.x, -posY - k�stchen.y + (int)origin.y] != schiff.GetID())
            {
                schiff.placed = false;
                Debug.Log(schiff.gameObject.name + " konnte nicht platziert werden ; blockiert!");
                return false;
            }
        }
        foreach (Vector2Int k�stchen in k�stchenstruktur)
        {
            besetzung[posX + k�stchen.x - (int)origin.x, -posY - k�stchen.y + (int)origin.y] = schiff.GetID();
        }

        if (debug)
        {
            string line;
            Debug.Log("----------------------SPIELFELD-" + schiff.spielfeld + "------------------------");
            for (int i = 0; i < Mathf.Abs(size.y); i++)
            {
                line = "" + (i + 1);
                for (int j = 0; j < Mathf.Abs(size.x); j++)
                    line += "      " + Methoden.XLIntToInt(besetzung[j, i]);
                Debug.Log(line);
            }
            Debug.Log("----------------------SPIELFELD-" + schiff.spielfeld + "------------------------");
        }

        schiff.placed = true;
        Debug.Log(schiff.gameObject.name + " erfolgreich platziert");
        return true;
    }

    /// <summary>
    /// Entfernt ein Schiff vom Koordinatensystem des Spielfeldes. Gibt false zur�ck, wenn kein Schiff entfernt werden kann, weil keines mehr existiert.
    /// </summary>
    public bool RemoveSchiff(Schiff schiff, bool debug = false)
    {
        if (!schiff.placed)
        {
            Debug.Log(schiff.gameObject.name + " wurde bereits entfernt");
            return false;
        }
        int posX = schiff.position.x;
        int posY = schiff.position.y;
        List<Vector3Int> k�stchenstruktur = schiff.schifftyp.k�stchenstruktur;
        Vector2 origin;
        int[,] besetzung;
        if (schiff.spielfeld == 1)
        {
            origin = origin1 + new Vector2(0.5f, -0.5f);
            besetzung = besetzung1;
        }
        else if (schiff.spielfeld == 2)
        {
            origin = origin2 + new Vector2(0.5f, -0.5f);
            besetzung = besetzung2;
        }
        else
        {
            Debug.LogError("Spielfeld " + schiff.spielfeld + " existiert nicht!");
            return false;
        }

        foreach (Vector2Int k�stchen in k�stchenstruktur)
        {
            if (besetzung[posX + k�stchen.x - (int)origin.x, -posY - k�stchen.y + (int)origin.y] != schiff.GetID())
            {
                Debug.Log(schiff.gameObject.name + " konnte nicht entfernt werden");
                return false;
            }
        }
        foreach (Vector2Int k�stchen in k�stchenstruktur)
        {
            besetzung[posX + k�stchen.x - (int)origin.x, -posY - k�stchen.y + (int)origin.y] = 0;
        }

        if (debug)
        {
            string line;
            Debug.Log("----------------------SPIELFELD-" + schiff.spielfeld + "------------------------");
            for (int i = 0; i < Mathf.Abs(size.y); i++)
            {
                line = "" + (i + 1);
                for (int j = 0; j < Mathf.Abs(size.x); j++)
                    line += "      " + Methoden.XLIntToInt(besetzung[j, i]);
                Debug.Log(line);
            }
            Debug.Log("----------------------SPIELFELD-" + schiff.spielfeld + "------------------------");
        }

        schiff.placed = false;
        Debug.Log(schiff.gameObject.name + " erfolgreich entfernt");
        return true;
    }

    /// <summary>
    /// Entfernt das gesammte Schiff inklusive Gameobject.
    /// </summary>
    public void DestroySchiff(Schiff schiff, bool debug = false, string message = "", bool warning = false)
    {
        RemoveSchiff(schiff, debug);
        Destroy(schiff.gameObject);
        if (debug)
        {
            if (warning)
                Debug.LogWarning(message);
            else Debug.Log(message);
        }
    }

    /// <summary>
    /// Versucht ein Schiff irgendwo auf dem Spielfeld zu platzieren und gibt false zur�ck, wenn das Schiff nicht platziert werden kann.
    /// </summary>
    public bool TryPlaceSchiff(Schiff schiff, bool debug = false)
    {
        Vector2 origin;
        if (schiff.spielfeld == 1)
            origin = origin1 + new Vector2(0.5f, -0.5f);
        else if (schiff.spielfeld == 2)
            origin = origin2 + new Vector2(0.5f, -0.5f);
        else
        {
            Debug.LogError("Spielfeld " + schiff.spielfeld + " existiert nicht!");
            return false;
        }

        for (int y = 0; y > size.y; y--)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (schiff.SetPosition(new Vector2(x + origin.x, y + origin.y), debug))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Platziert ein Schiff an zuf�lliger Position auf dem Spielfeld.
    /// </summary>
    public void RandomPlaceSchiff(Schiff schiff, bool debug = false)
    {
        bool placed = false;
        Vector2Int oldpos = schiff.position;

        for (int i = 0; i <= Mathf.Abs(size.x) * Mathf.Abs(size.y); i++)
        {
            if (schiff.spielfeld == 1)
                placed = schiff.SetPosition(new Vector2(origin1.x + 0.5f + Random.Range(0, Mathf.Abs(size.x) - schiff.schifftyp.ausma�e.x + 1), origin1.y - 0.5f - Random.Range(0, Mathf.Abs(size.y) - schiff.schifftyp.ausma�e.y + 1)), debug);
            else if (schiff.spielfeld == 2)
                placed = schiff.SetPosition(new Vector2(origin2.x + 0.5f + Random.Range(0, Mathf.Abs(size.x) - schiff.schifftyp.ausma�e.x + 1), origin2.y - 0.5f - Random.Range(0, Mathf.Abs(size.y) - schiff.schifftyp.ausma�e.y + 1)), debug);
            else
            {
                Debug.LogError(schiff.name + ": Ein solches Spielfeld existiert nicht!");
                return;
            }
            if (placed)
                break;
        }
        if (!placed)
            schiff.SetPosition(oldpos, debug);
    }

    /// <summary>
    /// Verhindert, dass das Schiff die Grenzen des Spielfeldes �berschreitet.
    /// </summary>
    public void AntiGrenz�berschreitung(Schiff schiff, bool debug = false)
    {
        Vector2 ausma�ehalbe = (Vector2)schiff.schifftyp.ausma�e / 2;
        Vector2 origin;
        if (schiff.spielfeld == 1)
            origin = origin1;
        else if (schiff.spielfeld == 2)
            origin = origin2;
        else
        {
            Debug.LogError("Spielfeld " + schiff.spielfeld + " existiert nicht!");
            return;
        }

        //Verhindere Grenz�berschreitungen auf X-Achse
        if (schiff.gameObject.transform.position.x - ausma�ehalbe.x < origin.x)
        {
            schiff.SetPosition(new Vector2(origin.x + 0.5f, schiff.position.y), debug);
        }          //Linke Grenze
        if (schiff.gameObject.transform.position.x + ausma�ehalbe.x > origin.x + size.x)
        {
            schiff.SetPosition(new Vector2(origin.x + size.x + 0.5f - 2 * ausma�ehalbe.x, schiff.position.y), debug);
        } //Rechte Grenze
        //Verhindere Grenz�berschreitungen auf Y-Achse
        if (schiff.gameObject.transform.position.y + ausma�ehalbe.y > origin.y)
        {
            schiff.SetPosition(new Vector2(schiff.position.x, origin.y - 0.5f), debug);
        }          //Obere Grenze
        if (schiff.gameObject.transform.position.y - ausma�ehalbe.y < origin.y + size.y)
        {
            schiff.SetPosition(new Vector2(schiff.position.x, origin.y + size.y - 0.5f + 2 * ausma�ehalbe.y), debug);
        } //Untere Grenze
        schiff.gameObject.transform.position = new Vector3(schiff.position.x + ausma�ehalbe.x - 0.5f, schiff.position.y - ausma�ehalbe.y + 0.5f, transform.parent.position.z);
        schiff.gameObject.transform.localPosition = new Vector3(schiff.gameObject.transform.localPosition.x, schiff.gameObject.transform.localPosition.y, 0);
    }

    /// <summary>
    /// Generiert biszu 10000 einmalige IDs.
    /// </summary>
    public int GenerateID(Schiff schiff)
    {
        if (schiff.spielfeld > 2)
        {
            Debug.LogError("Spielfeld " + schiff.spielfeld + " existiert nicht!");
            return 0;
        }

        int id = Random.Range(0, 10000) + schiff.spielfeld * 10000;
        while (ids.Remove(id))
        {
            ids.Add(id);
            id = Random.Range(0, 10000) + schiff.spielfeld * 10000;
            if (ids.Count > 9999)
                return 0;
        }
        ids.Add(id);
        return id;
    }

    /// <summary>
    /// Platziert auf einem Spielfeld eine Marke (type: 0 / 1) oder ersetzt eine vorhandene an vorgesehener Position. Gibt false bei bereits vorhandener selben Marke aus.
    /// </summary>
    public bool CreateMark(Vector2 worldPosition, int type)
    {
        int spielfeld;
        if (worldPosition.x > origin2.x && worldPosition.x < origin2.x + size.x && worldPosition.y < origin2.y && worldPosition.y > origin2.y + size.y)
            spielfeld = 2;
        else if (worldPosition.x > origin1.x && worldPosition.x < origin1.x + size.x && worldPosition.y < origin1.y && worldPosition.y > origin1.y + size.y)
            spielfeld = 1;
        else spielfeld = 1;

        Transform child;
        Transform parent = transform.GetChild(spielfeld - 1).GetChild(1);
        if (type < 0)
            type = 0;
        if (type > 1)
            type = 1;

        for (int i = 0; i < parent.childCount; i++)
        {
            child = parent.GetChild(i);
            if (worldPosition == (Vector2)child.position)
            {
                if (type != int.Parse(child.name))
                {
                    Destroy(child.gameObject);
                }
                else return false;
            }
        }

        GameObject GOmark = new GameObject("" + type);
        GOmark.AddComponent<SpriteRenderer>();
        GOmark.AddComponent<Rigidbody2D>();
        GOmark.transform.position = worldPosition;
        GOmark.transform.parent = parent;
        GOmark.transform.localPosition = new Vector3(GOmark.transform.localPosition.x, GOmark.transform.localPosition.y, 0);

        SpriteRenderer sprMark = GOmark.GetComponent<SpriteRenderer>();
        sprMark.sortingOrder = 2;
        if (type == 0) sprMark.sprite = MarkDaneben;
        if (type == 1 && spielfeld == 2) sprMark.sprite = MarkTreffer;
        else if (type == 1 && spielfeld == 1) sprMark.sprite = MarkZerst�rt;

        Rigidbody2D RBmark = GOmark.GetComponent<Rigidbody2D>();
        RBmark.isKinematic = true;
        return true;
    }

    /// <summary>
    /// L�scht eine Marke an vorgesehener Position.
    /// </summary>
    public void DeleteMark(Vector2 worldPosition)
    {
        int spielfeld;
        if (worldPosition.x > origin2.x && worldPosition.x < origin2.x + size.x && worldPosition.y < origin2.y && worldPosition.y > origin2.y + size.y)
            spielfeld = 2;
        else if (worldPosition.x > origin1.x && worldPosition.x < origin1.x + size.x && worldPosition.y < origin1.y && worldPosition.y > origin1.y + size.y)
            spielfeld = 1;
        else spielfeld = 1;

        Transform child;
        Transform parent = transform.GetChild(spielfeld - 1).GetChild(1);

        for (int i = 0; i < parent.childCount; i++)
        {
            child = parent.GetChild(i);
            if (worldPosition == (Vector2)child.position)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Platziert eine Marke an vorgesehener Position entsprechend der Belegung des K�stchens (Schiff/kein Schiff) und des Spielfeldes und spielt Musik ab.
    /// </summary>
    /// <param name="ship">�bergabe des getroffenen Schiffes</param>
    public bool PlaceMark(Vector2Int worldPosition, ref Schiff ship)
    {
        int spielfeld;
        if (worldPosition.x > origin2.x && worldPosition.x < origin2.x + size.x && worldPosition.y < origin2.y && worldPosition.y > origin2.y + size.y)
            spielfeld = 2;
        else if (worldPosition.x > origin1.x && worldPosition.x < origin1.x + size.x && worldPosition.y < origin1.y && worldPosition.y > origin1.y + size.y)
            spielfeld = 1;
        else
        {
            ship = null;
            return false;
        }

        Schiff[] schiffe = GameObject.Find("Spielbrett").transform.GetChild(spielfeld - 1).GetComponentsInChildren<Schiff>();

        bool hit = false;
        bool platziert = false;

        for (int i = 0; i < schiffe.Length; i++)
        {
            if (schiffe[i].MarkK�stchen(worldPosition))
            {
                platziert = schiffe[i].MarkK�stchen(worldPosition, 1);
                ship = schiffe[i];
                hit = true;
                GameObject.Find("Spielbrett").GetComponentsInParent<AudioSource>()[3].Play(); //Music by SergeQuadrado from Pixabay
                break;
            }
        }
        if (!hit)
        {
            platziert = CreateMark(worldPosition, 0);
            ship = null;
            if (platziert)
                GetComponentsInParent<AudioSource>()[5].Play(); //Music from Pixabay
        }
        return platziert;
    }

    /// <summary>
    /// Startet den Countdown der Zugzeit und gibt true zur�ck solange er l�uft.
    /// </summary>
    public bool CountdownStart(float ZeitInMinuten)
    {
        if (countdown == 0 && y > 0)
        {
            GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zug = true;
            GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zugZeitInMinuten = ZeitInMinuten;
            countdown = 1;
        }
        if (y <= 0)
        {
            GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zug = false;
            y = 0;
            if (countdown != 2)
                countdown = 0;
            return false;
        }
        else return true;
    }

    /// <summary>
    /// Unterbricht den Countdown und gibt true zur�ck, solange er unterbrochen ist.
    /// </summary>
    public bool CountdownPause(bool zug = false)
    {
        if (countdown != 2)
        {
            oldcountdown = countdown;
            GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zug = zug;
            countdown = 2;
        }
        if (countdown == 2)
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Setzt den unterbrochenen Countdown fort.
    /// </summary>
    public void CountdownContinue(bool zug = false)
    {
        if (countdown == 2)
        {
            countdown = 0;
            if (oldcountdown == 1)
            {
                CountdownStart(GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zugZeitInMinuten);
            }
            else if (oldcountdown == -1)
                CountdownReset();
        }
    }

    /// <summary>
    /// Beendet den Countdown der Zugzeit vorzeitig.
    /// </summary>
    public void CountdownEnd()
    {
        if (countdown == 1)
        {
            GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zug = false;
            GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zugZeitInMinuten = 0.01f;
        }
    }

    /// <summary>
    /// Setzt den Countdown der Zugzeit zur�ck und gibt true zur�ck solange er noch zur�ckgesetzt wird.
    /// </summary>
    public bool CountdownReset()
    {
        if (countdown == 0 && y < 36)
        {
            GameObject.Find("Spielbrett").GetComponent<Spielbrett>().zug = false;
            countdown = -1;
        }
        if (y >= 36)
        {
            y = 36;
            if (countdown != 2)
                countdown = 0;
            return false;
        }
        else return true;
    }

    /// <summary>
    /// �berpr�ft die Vorhandenheit der Schiffe pro Spielfeld und beendet gegebenenfalls den Kampf (gibt in diesem Fall true zur�ck).
    /// </summary>
    public bool CheckIntactShips()
    {
        Schiff[] shipsSpF1 = transform.GetChild(0).GetComponentsInChildren<Schiff>();
        Schiff[] shipsSpF2 = transform.GetChild(1).GetComponentsInChildren<Schiff>();
        int intactShipsSpF1 = shipsSpF1.Length;
        int intactShipsSpF2 = shipsSpF2.Length;
        for (int i = 0; i < shipsSpF1.Length; i++)
            if (shipsSpF1[i].destroyed)
                intactShipsSpF1--;
        for (int i = 0; i < shipsSpF2.Length; i++)
            if (shipsSpF2[i].destroyed)
                intactShipsSpF2--;
        if (intactShipsSpF1 == 0) //loose
        {
            looseOBJ.SetActive(true);
            GetComponentsInParent<AudioSource>()[0].volume = 0.01f;
            loose = true;
            CountdownPause();
            return true;
        }
        else if (intactShipsSpF2 == 0) //win
        {
            winOBJ.SetActive(true);
            GameObject.Find("Spielbrett").GetComponentsInParent<AudioSource>()[2].Play(); //Music from Pixabay
            GameObject.Find("Spielbrett").GetComponentsInParent<AudioSource>()[0].volume = 0.01f;
            win = true;
            CountdownPause();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gibt zur�ck, ob alle Schiffe auf dem Spielfeld platziert sind (spielfeld = 0 -> alle Spielfelder).
    /// </summary>
    public bool CheckPlacedShips(int spielfeld = 0)
    {
        Spielbrett spielbrett = GameObject.Find("Spielbrett").GetComponent<Spielbrett>();
        bool placed = true;
        if (spielfeld == 0)
        {
            for (int i = 0; i < spielbrett.gameObject.transform.GetChild(0).childCount - 2; i++)
                if (!spielbrett.gameObject.transform.GetChild(0).GetChild(i + 2).GetComponent<Schiff>().placed)
                    placed = false;
            for (int i = 0; i < spielbrett.gameObject.transform.GetChild(1).childCount - 2; i++)
                if (!spielbrett.gameObject.transform.GetChild(1).GetChild(i + 2).GetComponent<Schiff>().placed)
                    placed = false;
        }
        else if (spielfeld == 1)
        {
            for (int i = 0; i < spielbrett.gameObject.transform.GetChild(0).childCount - 2; i++)
                if (!spielbrett.gameObject.transform.GetChild(0).GetChild(i + 2).GetComponent<Schiff>().placed)
                    placed = false;
        }
        else if (spielfeld == 2)
        {
            for (int i = 0; i < spielbrett.gameObject.transform.GetChild(1).childCount - 2; i++)
                if (!spielbrett.gameObject.transform.GetChild(1).GetChild(i + 2).GetComponent<Schiff>().placed)
                    placed = false;
        }
        else
        {
            Debug.LogError("Spielfeld " + spielfeld + " existiert nicht!");
            return false;
        }

        return placed;
    }

    /// <summary>
    /// Aktiviert die Gefahrenwarnung (zur "Dekoration" mit Audio) f�r einen kurzen Moment.
    /// </summary>
    public void Gefahrenwarnung()
    {
        if (!gefahrenwarnung)
            gefahrenwarnung = true;
    }

    /// <summary>
    /// Startet das Spiel.
    /// </summary>
    public void StartGame(int difficulty)
    {
        
        EndGame();

        SetSize(10, -10, 3, 2f / 1f);
        this.difficulty = difficulty;

        winOBJ = transform.GetChild(4).gameObject;
        winOBJ.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        winOBJ.SetActive(false);
        looseOBJ = transform.GetChild(5).gameObject;
        looseOBJ.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        looseOBJ.SetActive(false);

        win = false;
        loose = false;
        //y = 36;
        //transform.parent.GetChild(1).localScale = new Vector3(transform.GetChild(1).localScale.x - 0.5f, 36, transform.GetChild(1).localScale.z);
        zugNummer = -1;
        zug = false;

        tryingToStart = false;
        gefahrenwarnung = false;
        oldgefahrenwarnung = false;
        //timer = 0;
        transform.parent.GetComponent<AudioSource>().volume = 0;
        transform.parent.GetComponent<AudioSource>().Play();

        CreateShip("Flugzeugtr�ger1", SpriteFlugzeugtr�ger, Schifftyp.Flugzeugtr�ger(), 1);
        CreateShip("Schlachtschiff1", SpriteSchlachtschiff, Schifftyp.Schlachtschiff(), 1);
        CreateShip("Kreuzer1", SpriteKreuzer, Schifftyp.Kreuzer(), 1);
        CreateShip("Kreuzer2", SpriteKreuzer, Schifftyp.Kreuzer(), 1);
        CreateShip("Zerst�rer1", SpriteZerst�rer, Schifftyp.Zerst�rer(), 1);
        CreateShip("Zerst�rer2", SpriteZerst�rer, Schifftyp.Zerst�rer(), 1);
        CreateShip("Uboot1", SpriteUboot, Schifftyp.Uboot(), 1);
        CreateShip("Uboot2", SpriteUboot, Schifftyp.Uboot(), 1);

        CreateShip("Flugzeugtr�ger1", SpriteFlugzeugtr�ger, Schifftyp.Flugzeugtr�ger(), 2);
        CreateShip("Schlachtschiff1", SpriteSchlachtschiff, Schifftyp.Schlachtschiff(), 2);
        CreateShip("Kreuzer1", SpriteKreuzer, Schifftyp.Kreuzer(), 2);
        CreateShip("Kreuzer2", SpriteKreuzer, Schifftyp.Kreuzer(), 2);
        CreateShip("Zerst�rer1", SpriteZerst�rer, Schifftyp.Zerst�rer(), 2);
        CreateShip("Zerst�rer2", SpriteZerst�rer, Schifftyp.Zerst�rer(), 2);
        CreateShip("Uboot1", SpriteUboot, Schifftyp.Uboot(), 2);
        CreateShip("Uboot2", SpriteUboot, Schifftyp.Uboot(), 2);

        CountdownContinue();
        CountdownStart(0.5f);

        countD.SetPercentage(100);
        countD.CountdownStart(-30, true);
    }

    /// <summary>
    /// Beendet das Spiel
    /// </summary>
    public void EndGame()
    {
        ids.Clear();
        CountdownPause();
        countD.CountdownStop();

        //L�schen aller Schiffe
        for (int i = 0; i < transform.GetChild(0).childCount - 2; i++)
            DestroySchiff(transform.GetChild(0).GetChild(i + 2).GetComponent<Schiff>());
        for (int i = 0; i < transform.GetChild(1).childCount - 2; i++)
            DestroySchiff(transform.GetChild(1).GetChild(i + 2).GetComponent<Schiff>());
        //L�schen aller Marken
        for (int i = 0; i < transform.GetChild(0).GetChild(1).childCount; i++)
            Destroy(transform.GetChild(0).GetChild(1).GetChild(i).gameObject);
        for (int i = 0; i < transform.GetChild(1).GetChild(1).childCount; i++)
            Destroy(transform.GetChild(1).GetChild(1).GetChild(i).gameObject);
        //L�schen aller K�stchen und Koordinaten
        for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
            Destroy(transform.GetChild(0).GetChild(0).GetChild(i).gameObject);
        for (int i = 0; i < transform.GetChild(1).GetChild(0).childCount; i++)
            Destroy(transform.GetChild(1).GetChild(0).GetChild(i).gameObject);
        //Zur�cksetzen des Ladebalkens
        GetComponentInChildren<LoadingBar>().Initialize();
        GetComponentInChildren<LoadingBar>().SetProgess(0f);

        transform.parent.GetComponent<AudioSource>().Stop();
    }
}