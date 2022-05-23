using System.Collections.Generic;
using UnityEngine;

public class Spielbrett : MonoBehaviour
{
    [Header("     Ships:")]
    [Header("Sprites:")]
    public Sprite SpriteUboot;
    public Sprite SpriteSchlachtschiff;
    public Sprite SpriteKreuzer;
    public Sprite SpriteZerstörer;
    public Sprite SpriteFlugzeugträger;
    public Sprite Square;
    [Header("     Marks:")]
    public Sprite MarkDaneben;
    public Sprite MarkTreffer;
    public Sprite MarkZerstört;
    [Header("Spiel:")]
    public float zugZeitInSekunden;
    public bool Zug;
    public int zNr;
    public bool placedships;

    private Vector2Int size = new Vector2Int(16, -16); //Größe eines Spielfeldes (bitte noch nicht verändern!)
    [SerializeField]
    private Vector2 origin1 = new Vector2(-0.5f, 0.5f); //World Position
    [SerializeField]
    private Vector2 origin2 = new Vector2(15.5f, 0.5f); //World Position
    private int[,] besetzung1 = new int[64, 64]; //koordinatenweise Besetzung des ersten Spielfeldes
    private int[,] besetzung2 = new int[64, 64]; //koordinatenweise Besetzung des zweiten Spielfeldes
    private List<int> ids = new List<int>(); //Liste der IDs aller Schiffe im Spiel

    public bool zug { get; private set; } //wenn true, Spieler am Zug, wenn false Feind am Zug
    public int zugNummer { get; private set; }
    public bool loose { get; private set; }
    public bool win { get; private set; }
    public bool tryingToStart { get; private set; }

    public int difficulty;

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
        //Variablen für Inspektor
        Zug = zug;
        zNr = zugNummer;
        placedships = CheckPlacedShips();


        //Countdown-------------------------------------------
        if (zugNummer == -1) //Schiffe Platzieren
        {
            countD.SetColor(Color.blue);
            if (countD.GetProgress() == 0)
            {
                zug = true;
                if (!countD.GetStatus())
                    countD.SetPercentage(100);
                countD.CountdownStart(-10, true);
            }
            if (countD.GetProgress() == 0 && countD.GetStatus())
            {
                tryingToStart = true;
                if (CheckPlacedShips())
                {
                    tryingToStart = false;
                    zugNummer++;
                }
            }
        }
        if (zugNummer >= 0 && !Methoden.Odd(zugNummer)) //Gegner am Zug
        {
            countD.SetColor(Color.red);
            if (countD.GetProgress() == 0)
            {
                zug = false;
                countD.CountdownStart(20, true);
            }
            else if (countD.GetProgress() == 100)
            {
                zug = true;
                zugNummer++;
            }
        }
        if (zugNummer >= 0 && Methoden.Odd(zugNummer)) //Spieler am Zug
        {
            countD.SetColor(Color.green);
            if (countD.GetProgress() == 100)
            {
                zug = true;
                countD.CountdownStart(-20, true);
            }
            else if (countD.GetProgress() == 0)
            {
                zug = false;
                zugNummer++;
            }
        }

        //Inspektor Variable Zugzeit in Sekunden
        if (countD.GetSpeed() > 0)
            zugZeitInSekunden = (100 - countD.GetProgress()) / countD.GetSpeed();
        else if (countD.GetSpeed() != 0)
            zugZeitInSekunden = countD.GetProgress() / - countD.GetSpeed();


        //Lautstärke langsam erhöhen bis 0.1f am Anfang des Kampfes
        if (!win && !loose)
            transform.parent.GetComponent<AudioSource>().volume = Mathf.Lerp(transform.parent.GetComponent<AudioSource>().volume, 0.1f, Time.deltaTime * 0.25f);

        //Win/Loose-Titel langsam einblenden
        if (winOBJ.activeSelf)
            winOBJ.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(winOBJ.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 0.25f));
        if (looseOBJ.activeSelf)
            looseOBJ.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(looseOBJ.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 0.25f));

        //Gefahrenwarnung (ziemlich laut und aufdringlich grell - muss noch geändert werden)
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
    /// Generiert die Spielfelder des Spielbrettes und passt Kammera und andere Optische Elemente an die Größe und Bildschirm an.
    /// </summary>
    /// <param name="x">-Größe der Spielfelder auf der X-Achse; immer positiv</param>
    /// <param name="y">-Größe der Spielfelder auf der Y-Achse; immer negativ</param>
    /// <param name="randabstand">-Randabstand zwischen Spielfeldern und Rand </param>
    /// <param name="verhältnis">-Verhältnis der Bildschirmbreite zur Bildschirmhöhe (B/H)</param>
    public void SetSize(int x, int y, float randabstand = 3, float verhältnis = 16f / 9f)
    {
        Camera cam = GameObject.Find("Spielbrett").GetComponentInParent<Camera>();
        verhältnis = Mathf.Abs(verhältnis);

        Methoden.CheckDimensions(ref x, ref y);

        size = new Vector2Int(x, y);
        origin1 = new Vector3(-0.5f, 0.5f, 20);
        origin2 = origin1 + new Vector2(size.x + 2 * randabstand, 0);

        Methoden.AdaptCamera(ref cam, size, origin1, verhältnis, randabstand);

        Methoden.AdaptOverlays(ref cam, size, origin1, origin2, verhältnis, randabstand);

        //Erzeugung aller Kästchen und Koordinaten beider Spielfelder
        for (int p = 0; p < 2; p++)
        {
            Methoden.CreateBoxes(cam, size, origin1, origin2, p);
            Methoden.CreateCoordinates(cam, size, origin1, origin2, p);
        }
    }

    /// <summary>
    /// Erstellt ein gameobject Schiff mit allen nötigen Components und Eigenschaften.
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

        GOship.AddComponent<PolygonCollider2D>().points = ship.schifftyp.ÄußerePunkte();

        ship.SetRotation(Random.Range(0, 3));

        if (!TryPlaceSchiff(ship, debug))
        {
            DestroySchiff(ship, true, name + " konnte nicht platziert werden und wurde zerstört", true);
            return;
        }
        if (placeRandom)
            RandomPlaceSchiff(ship, debug);
    }

    /// <summary>
    /// Fügt ein Schiff zum Koordinatensystem des Spielfeldes hinzu. Gibt false zurück, wenn es nicht platziert werden kann, weil es Grenzen überschreitet oder weil ein anderes Schiff bereits an dieser Stelle vorliegt.
    /// </summary>
    public bool AddSchiff(Schiff schiff, bool debug = false)
    {
        int posX = schiff.position.x;
        int posY = schiff.position.y;
        List<Vector3Int> kästchenstruktur = schiff.schifftyp.kästchenstruktur;
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

        foreach (Vector2Int kästchen in kästchenstruktur)
        {
            if (posX + kästchen.x < 0 + origin.x || posX + kästchen.x > size.x - 1 + origin.x || -posY - kästchen.y < 0 - origin.y || -posY - kästchen.y > -size.y - 1 - origin.y)
            {
                schiff.placed = false;
                Debug.Log(schiff.gameObject.name + " konnte nicht platziert werden ; Grenzüberschreitung!");
                return false;
            }
            else if (besetzung[posX + kästchen.x - (int)origin.x, -posY - kästchen.y + (int)origin.y] != 0 && besetzung[posX + kästchen.x - (int)origin.x, -posY - kästchen.y + (int)origin.y] != schiff.GetID())
            {
                schiff.placed = false;
                Debug.Log(schiff.gameObject.name + " konnte nicht platziert werden ; blockiert!");
                return false;
            }
        }
        foreach (Vector2Int kästchen in kästchenstruktur)
        {
            besetzung[posX + kästchen.x - (int)origin.x, -posY - kästchen.y + (int)origin.y] = schiff.GetID();
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
    /// Entfernt ein Schiff vom Koordinatensystem des Spielfeldes. Gibt false zurück, wenn kein Schiff entfernt werden kann, weil keines mehr existiert.
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
        List<Vector3Int> kästchenstruktur = schiff.schifftyp.kästchenstruktur;
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

        foreach (Vector2Int kästchen in kästchenstruktur)
        {
            if (besetzung[posX + kästchen.x - (int)origin.x, -posY - kästchen.y + (int)origin.y] != schiff.GetID())
            {
                Debug.Log(schiff.gameObject.name + " konnte nicht entfernt werden");
                return false;
            }
        }
        foreach (Vector2Int kästchen in kästchenstruktur)
        {
            besetzung[posX + kästchen.x - (int)origin.x, -posY - kästchen.y + (int)origin.y] = 0;
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
    /// Versucht ein Schiff irgendwo auf dem Spielfeld zu platzieren und gibt false zurück, wenn das Schiff nicht platziert werden kann.
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
    /// Platziert ein Schiff an zufälliger Position auf dem Spielfeld.
    /// </summary>
    public void RandomPlaceSchiff(Schiff schiff, bool debug = false)
    {
        bool placed = false;
        Vector2Int oldpos = schiff.position;

        for (int i = 0; i <= Mathf.Abs(size.x) * Mathf.Abs(size.y); i++)
        {
            if (schiff.spielfeld == 1)
                placed = schiff.SetPosition(new Vector2(origin1.x + 0.5f + Random.Range(0, Mathf.Abs(size.x) - schiff.schifftyp.ausmaße.x + 1), origin1.y - 0.5f - Random.Range(0, Mathf.Abs(size.y) - schiff.schifftyp.ausmaße.y + 1)), debug);
            else if (schiff.spielfeld == 2)
                placed = schiff.SetPosition(new Vector2(origin2.x + 0.5f + Random.Range(0, Mathf.Abs(size.x) - schiff.schifftyp.ausmaße.x + 1), origin2.y - 0.5f - Random.Range(0, Mathf.Abs(size.y) - schiff.schifftyp.ausmaße.y + 1)), debug);
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
    /// Verhindert, dass das Schiff die Grenzen des Spielfeldes überschreitet.
    /// </summary>
    public void AntiGrenzüberschreitung(Schiff schiff, bool debug = false)
    {
        Vector2 ausmaßehalbe = (Vector2)schiff.schifftyp.ausmaße / 2;
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

        //Verhindere Grenzüberschreitungen auf X-Achse
        if (schiff.gameObject.transform.position.x - ausmaßehalbe.x < origin.x)
        {
            schiff.SetPosition(new Vector2(origin.x + 0.5f, schiff.position.y), debug);
        }          //Linke Grenze
        if (schiff.gameObject.transform.position.x + ausmaßehalbe.x > origin.x + size.x)
        {
            schiff.SetPosition(new Vector2(origin.x + size.x + 0.5f - 2 * ausmaßehalbe.x, schiff.position.y), debug);
        } //Rechte Grenze
        //Verhindere Grenzüberschreitungen auf Y-Achse
        if (schiff.gameObject.transform.position.y + ausmaßehalbe.y > origin.y)
        {
            schiff.SetPosition(new Vector2(schiff.position.x, origin.y - 0.5f), debug);
        }          //Obere Grenze
        if (schiff.gameObject.transform.position.y - ausmaßehalbe.y < origin.y + size.y)
        {
            schiff.SetPosition(new Vector2(schiff.position.x, origin.y + size.y - 0.5f + 2 * ausmaßehalbe.y), debug);
        } //Untere Grenze
        schiff.gameObject.transform.position = new Vector3(schiff.position.x + ausmaßehalbe.x - 0.5f, schiff.position.y - ausmaßehalbe.y + 0.5f, transform.parent.position.z);
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
        else if (type == 1 && spielfeld == 1) sprMark.sprite = MarkZerstört;

        Rigidbody2D RBmark = GOmark.GetComponent<Rigidbody2D>();
        RBmark.isKinematic = true;
        return true;
    }

    /// <summary>
    /// Löscht eine Marke an vorgesehener Position.
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
    /// Platziert eine Marke an vorgesehener Position entsprechend der Belegung des Kästchens (Schiff/kein Schiff) und des Spielfeldes und spielt Musik ab.
    /// </summary>
    /// <param name="ship">Übergabe des getroffenen Schiffes</param>
    /// <returns>true, wenn eine Marke platziert wurde; false wenn keine platziert werden konnte</returns>
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
            if (schiffe[i].MarkKästchen(worldPosition))
            {
                platziert = schiffe[i].MarkKästchen(worldPosition, 1);
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
    /// Führt verschiedene spezialattacken aus. type = 1 -> Bombe; type = 2 -> Sonar
    /// </summary>
    /// <param name="ship">(leeres Array)</param>
    /// <param name="type">Typ der Attacke</param>
    /// <returns>Gibt die Anzahl an Treffern an. Gibt -1 zurück, wenn keine platziert werden konnte</returns>
    public int SpecialMarks(Vector2Int worldPosition, ref Schiff[] ship, int type)
    {
        int spielfeld;
        if (worldPosition.x > origin2.x && worldPosition.x < origin2.x + size.x && worldPosition.y < origin2.y && worldPosition.y > origin2.y + size.y)
            spielfeld = 2;
        else if (worldPosition.x > origin1.x && worldPosition.x < origin1.x + size.x && worldPosition.y < origin1.y && worldPosition.y > origin1.y + size.y)
            spielfeld = 1;
        else
        {
            ship = null;
            return 0;
        }

        ship = new Schiff[9];
        int hit = 0;

        if (type == 1)
        {
            List<Schiff> ships = new List<Schiff>();

            if (PlaceMark(worldPosition + new Vector2Int(1, 1), ref ship[0]))
            {
                ships.Add(ship[0]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(0, 1), ref ship[1]))
            {
                ships.Add(ship[1]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(-1, 1), ref ship[2]))
            {
                ships.Add(ship[2]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(1, 0), ref ship[3]))
            {
                ships.Add(ship[3]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(0, 0), ref ship[4]))
            {
                ships.Add(ship[4]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(-1, 0), ref ship[5]))
            {
                ships.Add(ship[5]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(1, -1), ref ship[6]))
            {
                ships.Add(ship[6]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(0, -1), ref ship[7]))
            {
                ships.Add(ship[7]);
                hit++;
            }

            if (PlaceMark(worldPosition + new Vector2Int(-1, -1), ref ship[8]))
            {
                ships.Add(ship[8]);
                hit++;
            }

            ship = ships.ToArray();
            if (hit == 0)
                hit = -1;
            return hit;
        }

        if (type == 2)
        {
            ship = null;
            Schiff[] schiffe = GameObject.Find("Spielbrett").transform.GetChild(spielfeld - 1).GetComponentsInChildren<Schiff>();
            foreach (Schiff schiff in schiffe)
            {

                if (schiff.MarkKästchen(worldPosition))
                    hit++;

                for (int x = 1; x < size.x; x++)
                {
                    if (schiff.MarkKästchen(worldPosition + new Vector2Int(x, 0)))
                        hit++;
                    if (schiff.MarkKästchen(worldPosition - new Vector2Int(x, 0)))
                        hit++;
                }
                for (int y = 1; y < size.x; y++)
                {
                    if (schiff.MarkKästchen(worldPosition + new Vector2Int(0, y)))
                        hit++;
                    if (schiff.MarkKästchen(worldPosition - new Vector2Int(0, y)))
                        hit++;
                }
            }
            return hit;
        }

        return -1;
    }

    /// <summary>
    /// Überprüft die Vorhandenheit der Schiffe pro Spielfeld und beendet gegebenenfalls den Kampf (gibt in diesem Fall true zurück).
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
            countD.CountdownPause();
            return true;
        }
        else if (intactShipsSpF2 == 0) //win
        {
            winOBJ.SetActive(true);
            GameObject.Find("Spielbrett").GetComponentsInParent<AudioSource>()[2].Play(); //Music from Pixabay
            GameObject.Find("Spielbrett").GetComponentsInParent<AudioSource>()[0].volume = 0.01f;
            win = true;
            countD.CountdownPause();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gibt zurück, ob alle Schiffe auf dem Spielfeld platziert sind (spielfeld = 0 -> alle Spielfelder).
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
    /// Aktiviert die Gefahrenwarnung (zur "Dekoration" mit Audio) für einen kurzen Moment.
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

        CreateShip("Flugzeugträger1", SpriteFlugzeugträger, Schifftyp.Flugzeugträger(), 1);
        CreateShip("Schlachtschiff1", SpriteSchlachtschiff, Schifftyp.Schlachtschiff(), 1);
        CreateShip("Kreuzer1", SpriteKreuzer, Schifftyp.Kreuzer(), 1);
        CreateShip("Kreuzer2", SpriteKreuzer, Schifftyp.Kreuzer(), 1);
        CreateShip("Zerstörer1", SpriteZerstörer, Schifftyp.Zerstörer(), 1);
        CreateShip("Zerstörer2", SpriteZerstörer, Schifftyp.Zerstörer(), 1);
        CreateShip("Uboot1", SpriteUboot, Schifftyp.Uboot(), 1);
        CreateShip("Uboot2", SpriteUboot, Schifftyp.Uboot(), 1);

        CreateShip("Flugzeugträger1", SpriteFlugzeugträger, Schifftyp.Flugzeugträger(), 2);
        CreateShip("Schlachtschiff1", SpriteSchlachtschiff, Schifftyp.Schlachtschiff(), 2);
        CreateShip("Kreuzer1", SpriteKreuzer, Schifftyp.Kreuzer(), 2);
        CreateShip("Kreuzer2", SpriteKreuzer, Schifftyp.Kreuzer(), 2);
        CreateShip("Zerstörer1", SpriteZerstörer, Schifftyp.Zerstörer(), 2);
        CreateShip("Zerstörer2", SpriteZerstörer, Schifftyp.Zerstörer(), 2);
        CreateShip("Uboot1", SpriteUboot, Schifftyp.Uboot(), 2);
        CreateShip("Uboot2", SpriteUboot, Schifftyp.Uboot(), 2);
    }

    /// <summary>
    /// Beendet das Spiel
    /// </summary>
    public void EndGame()
    {
        ids.Clear();
        countD.CountdownStop();

        //Löschen aller Schiffe
        for (int i = 0; i < transform.GetChild(0).childCount - 2; i++)
            DestroySchiff(transform.GetChild(0).GetChild(i + 2).GetComponent<Schiff>());
        for (int i = 0; i < transform.GetChild(1).childCount - 2; i++)
            DestroySchiff(transform.GetChild(1).GetChild(i + 2).GetComponent<Schiff>());
        //Löschen aller Marken
        for (int i = 0; i < transform.GetChild(0).GetChild(1).childCount; i++)
            Destroy(transform.GetChild(0).GetChild(1).GetChild(i).gameObject);
        for (int i = 0; i < transform.GetChild(1).GetChild(1).childCount; i++)
            Destroy(transform.GetChild(1).GetChild(1).GetChild(i).gameObject);
        //Löschen aller Kästchen und Koordinaten
        for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
            Destroy(transform.GetChild(0).GetChild(0).GetChild(i).gameObject);
        for (int i = 0; i < transform.GetChild(1).GetChild(0).childCount; i++)
            Destroy(transform.GetChild(1).GetChild(0).GetChild(i).gameObject);
        //Zurücksetzen des Ladebalkens
        GetComponentInChildren<LoadingBar>().Initialize();
        GetComponentInChildren<LoadingBar>().SetProgess(0);

        transform.parent.GetComponent<AudioSource>().Stop();
    }
}
