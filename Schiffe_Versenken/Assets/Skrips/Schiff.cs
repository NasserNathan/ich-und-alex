using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schiff : MonoBehaviour
{
    public Vector2Int position { get; private set; } //World position
    public int spielfeld { get; private set; }
    public Spielbrett spielbrett { get; private set; }
    public Schifftyp schifftyp;
    [SerializeField]
    private int ID;
    public bool selected = false;
    public bool placed = false;
    public bool destroyed = false;
    private enum Ausrichtung
    {
        rechts,
        unten,
        links,
        oben
    }
    private Ausrichtung rotation;

    /// <summary>
    /// Setzt die Position der Schiffsstruktur und die dazugeh�rige Position des gameObjects und gibt zur�ck, ob es Erfolgreich oder nicht war.
    /// </summary>
    public bool SetPosition(Vector2 position, bool debug = false)
    {
        Vector2 ausma�ehalbe = (Vector2)schifftyp.ausma�e / 2;
        if (placed)
            spielbrett.RemoveSchiff(this);
        this.position = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if (!spielbrett.AddSchiff(this, debug))
            return false;
        transform.position = new Vector3(this.position.x + ausma�ehalbe.x - 0.5f, this.position.y - ausma�ehalbe.y + 0.5f);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        return true;
    }
    /// <summary>
    /// Bewegt das Schiff und dessen transform.position gemeinsam
    /// </summary>
    public void MovePosition(int x, int y, bool debug = false)
    {
        spielbrett.RemoveSchiff(this);
        transform.localPosition += new Vector3(x, y, 0);
        position += new Vector2Int(x, y);
        spielbrett.AntiGrenz�berschreitung(this, debug);
        spielbrett.AddSchiff(this, debug);
    }
    /// <summary>
    /// Legt einmalig das Spielfeld eines Schiffes und fest und weist diesem eine entsprechende ID zu.
    /// </summary>
    public void SetSpielfeld(int spielfeld)
    {
        if (spielfeld == 0)
            spielfeld = 1;
        if (this.spielfeld == 0)
        {
            this.spielfeld = spielfeld;
            spielbrett = GameObject.Find("Spielbrett").GetComponent<Spielbrett>();
        }
        else 
        { 
            Debug.LogWarning("Spielfeld eines Schiffes kann nur einmalig festgelegt werden!");
            return;
        }
        ID = spielbrett.GenerateID(this);
    }
    /// <summary>
    /// mark = -1 -> gibt aus, ob das Schiff auf der angegebenen Position liegt; mark != -1 -> setzt/entfernt eine Marke und gibt aus, ob es erfolgreich war.
    /// </summary>
    public bool MarkK�stchen(Vector2Int worldPosition, int mark = -1)
    {
        bool destr = true;
        bool hit = false;
        for (int i = 0; i < schifftyp.k�stchenstruktur.Count; i++)
        {
            if (this.position + (Vector2Int)schifftyp.k�stchenstruktur[i] == worldPosition)
            {
                if (mark == -1)
                {
                    hit = true;
                    break;
                }
                else if (mark == 0)
                {
                    hit = true;
                    spielbrett.DeleteMark(worldPosition);
                    schifftyp.SetK�stchenstruktur(i, 0);
                }
                else
                {
                    hit = spielbrett.CreateMark(worldPosition, 1);
                    schifftyp.SetK�stchenstruktur(i, 1);
                }
                break;
            }
        }
        if (hit && mark != -1)
        {
            foreach (Vector3Int k�stchen in schifftyp.k�stchenstruktur)
            {
                if (k�stchen.z == 0)
                {
                    destr = false;
                }
            }
            if (!destroyed && destr)
            {
                destroyed = destr;
                Debug.Log(name + " zerst�rt! (Spielfeld " + spielfeld + ")");
                spielbrett.GetComponentsInParent<AudioSource>()[4].volume = 0.025f * schifftyp.k�stchenstruktur.Count;
                spielbrett.GetComponentsInParent<AudioSource>()[4].Play(); //Music from Pixabay
                spielbrett.CheckIntactShips();
            }
            else destroyed = destr;
        }
        return hit;
    }
    /// <summary>
    /// �ndert die Rotation der Schiffsstruktur und die transform.rotation
    /// </summary>
    public void SetRotation(int ausrichtung)
    {
        while(ausrichtung < 0) { ausrichtung += 4; }
        while(ausrichtung > 3) { ausrichtung -= 4; }
        if (ausrichtung == 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (ausrichtung == 1)
            transform.rotation = Quaternion.Euler(0, 0, 270);
        else if (ausrichtung == 2)
            transform.rotation = Quaternion.Euler(0, 0, 180);
        else if (ausrichtung == 3)
            transform.rotation = Quaternion.Euler(0, 0, 90);


        if (placed)
            spielbrett.RemoveSchiff(this);
        RotateK�stchenstruktur();
        transform.position = new Vector3(this.position.x + (float)schifftyp.ausma�e.x / 2 - 0.5f, this.position.y - (float)schifftyp.ausma�e.y / 2 + 0.5f);
        spielbrett.AntiGrenz�berschreitung(this, true);
        spielbrett.AddSchiff(this, true);
    }
    //�ndert die Schiffsstruktur entsprechend einer Rotation ab
    private void RotateK�stchenstruktur()
    {
        float transformRotationZ = transform.rotation.eulerAngles.z;
        List<Vector3Int> neueStruktur = new List<Vector3Int>();
        List<Vector3Int> alteStruktur = schifftyp.k�stchenstruktur;

        if ((int)rotation == 1 && transformRotationZ == 0)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.y, k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)0;
            }
        }        //270 -> 000 ( 90)
        else if ((int)rotation == 2 && transformRotationZ ==   0)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.x, -k�stchen.y, k�stchen.z));
                rotation = (Ausrichtung)0;
            }
        } //180 -> 000 (180)
        else if ((int)rotation == 3 && transformRotationZ ==   0)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(k�stchen.y, -k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)0;
            }
        } //090 -> 000 (-90)
        else if ((int)rotation == 0 && transformRotationZ == 270)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(k�stchen.y, -k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)1;
            }
        } //000 -> 270 (-90)
        else if ((int)rotation == 2 && transformRotationZ == 270)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.y, k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)1;
            }
        } //180 -> 270 ( 90)
        else if ((int)rotation == 3 && transformRotationZ == 270)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.x, -k�stchen.y, k�stchen.z));
                rotation = (Ausrichtung)1;
            }
        } //090 -> 270 (180)
        else if ((int)rotation == 0 && transformRotationZ == 180)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.x, -k�stchen.y, k�stchen.z));
                rotation = (Ausrichtung)2;
            }
        } //000 -> 180 (180)
        else if ((int)rotation == 1 && transformRotationZ == 180)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(k�stchen.y, -k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)2;
            }
        } //270 -> 180 (-90)
        else if ((int)rotation == 3 && transformRotationZ == 180)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.y, k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)2;
            }
        } //090 -> 180 ( 90)
        else if ((int)rotation == 0 && transformRotationZ ==  90)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.y, k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)3;
            }
        } //000 -> 090 ( 90)
        else if ((int)rotation == 1 && transformRotationZ ==  90)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(-k�stchen.x, -k�stchen.y, k�stchen.z));
                rotation = (Ausrichtung)3;
            }
        } //270 -> 090 (180)
        else if ((int)rotation == 2 && transformRotationZ ==  90)
        {
            foreach (Vector3Int k�stchen in alteStruktur)
            {
                neueStruktur.Add(new Vector3Int(k�stchen.y, -k�stchen.x, k�stchen.z));
                rotation = (Ausrichtung)3;
            }
        } //180 -> 090 (-90)
        else neueStruktur = alteStruktur;

        schifftyp.SetK�stchenstruktur(neueStruktur, (int)rotation);
    }

    public int GetRotation() { return (int)this.rotation; }
    public int GetID() { return ID; }
}
