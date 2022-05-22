using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (SpriteRenderer))]
public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Sprite Loading_bar_left;
    [SerializeField] private Sprite Loading_bar_middle;
    [SerializeField] private Sprite Loading_bar_right;
    [SerializeField] private Sprite Loading_bar;
    [SerializeField] private Color Loading_bar_color;
    [SerializeField] private float process;
    [SerializeField] private Vector2 size;

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        process = GetProgess();
        size = GetSize();
    }

    public void Initialize()
    {
        if (Loading_bar_middle != null)
            GetComponent<SpriteRenderer>().sprite = Loading_bar_middle;

        GetComponent<SpriteRenderer>().sortingOrder = 4;

        if (transform.GetComponentInChildren<LoadingBar_Right>() == null)
        {
            GameObject GO = new GameObject("LoadingBar_right");
            GO.transform.parent = transform;
            GO.transform.localPosition = Vector3.zero;
            GO.AddComponent<SpriteRenderer>().sprite = Loading_bar_right;
            GO.GetComponent<SpriteRenderer>().sortingOrder = 4;
            GO.AddComponent<LoadingBar_Right>();
        }
        if (transform.GetComponentInChildren<LoadingBar_Left>() == null)
        {
            GameObject GO = new GameObject("LoadingBar_left");
            GO.transform.parent = transform;
            GO.transform.localPosition = Vector3.zero;
            GO.AddComponent<SpriteRenderer>().sprite = Loading_bar_left;
            GO.GetComponent<SpriteRenderer>().sortingOrder = 4;
            GO.AddComponent<LoadingBar_Left>();
        }
        if (transform.GetComponentInChildren<LoadingBar_Bar>() == null)
        {
            GameObject GO = new GameObject("LoadingBar_Bar");
            GO.transform.parent = transform;
            GO.transform.localPosition = Vector3.zero;
            GO.AddComponent<SpriteRenderer>().sprite = Loading_bar;
            GO.GetComponent<SpriteRenderer>().color = Loading_bar_color;
            GO.GetComponent<SpriteRenderer>().sortingOrder = 3;
            GO.AddComponent<LoadingBar_Bar>();
        }

        GetComponentInChildren<LoadingBar_Right>().Refresh();
        GetComponentInChildren<LoadingBar_Left>().Refresh();
        GetComponentInChildren<LoadingBar_Bar>().Initialize();
    }

    /// <summary>
    /// Gibt den Fortschritt der Fortschrittsleiste in Prozent als Dezimalzahl zurück.
    /// </summary>
    public float GetProgess()
    {
        return GetComponentInChildren<LoadingBar_Bar>().transform.GetComponent<SpriteRenderer>().bounds.size.x 
            / (GetComponentInChildren<LoadingBar_Right>().transform.position - GetComponentInChildren<LoadingBar_Left>().transform.position).magnitude;
    }

    /// <summary>
    /// Setzt den Fortschritt der Fortschrittsleiste. Könnte in der Start-Methode eventuell nicht funktionieren!
    /// </summary>
    /// <param name="percent">Fortschritt als Dezimalzahl</param>
    /// <returns></returns>
    public bool SetProgess(float percent)
    {
        GetComponentInChildren<LoadingBar_Right>().Refresh();
        GetComponentInChildren<LoadingBar_Left>().Refresh();

        if (percent > 1 || percent < 0)
        {
            Debug.LogError("Angegebene Zahl liegt außerhalb des Bereiches!");
            return false;
        }

        GetComponentInChildren<LoadingBar_Bar>().transform.localScale = new Vector3(percent * (GetComponentInChildren<LoadingBar_Right>().transform.position 
            - GetComponentInChildren<LoadingBar_Left>().transform.position).magnitude /
            (GetComponentInChildren<LoadingBar_Bar>().transform.GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.lossyScale.x),
            transform.localScale.y, transform.localScale.z);

        GetComponentInChildren<LoadingBar_Bar>().Refresh();

        return true;
    }

    /// <summary>
    /// Gibt die Ausmaße der Gesammten Fortschrittsleiste zurück.
    /// </summary>
    public Vector2 GetSize()
    {
        return new Vector3((GetComponentInChildren<LoadingBar_Right>().transform.position - GetComponentInChildren<LoadingBar_Left>().transform.position).magnitude
            + GetComponentInChildren<LoadingBar_Right>().GetComponent<SpriteRenderer>().bounds.extents.x 
            + GetComponentInChildren<LoadingBar_Left>().GetComponent<SpriteRenderer>().bounds.extents.x,
            GetComponent<SpriteRenderer>().bounds.size.y);
    }

    /// <summary>
    /// Setzt die Größe der Fortschrittsleiste. Beachte, dass die Größe der Leiste in x-Richtung ein Minimum je nach Größe in y-Richtung aufweist.
    /// </summary>
    /// <returns>Gibt false zurück, wenn x oder y = 0 ist und ändert in dem Fall nichts.</returns>
    public bool SetSize(Vector2 size)
    {
        float oldProgress = GetProgess();

        GetComponentInChildren<LoadingBar_Right>().Refresh();
        GetComponentInChildren<LoadingBar_Left>().Refresh();

        if (size.x == 0 || size.y == 0)
            return false;
        if (Mathf.Abs(size.x) < 0.001f)
            size.x = 0.001f * size.x / Mathf.Abs(size.x);
        if (Mathf.Abs(size.y) < 0.001f)
            size.y = 0.001f * size.y / Mathf.Abs(size.y);

        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);

        if (GetSize() == size)
            return true;

        Transform child1 = GetComponentInChildren<LoadingBar_Right>().transform;
        Transform child2 = GetComponentInChildren<LoadingBar_Left>().transform;
        float spSizeChild1 = child1.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float spSizeChild2 = child2.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float spSizeMain = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float Minimum;

        transform.localScale = new Vector2(transform.localScale.x, size.y / (GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.parent.lossyScale.y));

        Minimum = spSizeChild1 * GetComponent<SpriteRenderer>().bounds.size.y / child1.GetComponent<SpriteRenderer>().sprite.bounds.size.y
            + spSizeChild2 * GetComponent<SpriteRenderer>().bounds.size.y / child2.GetComponent<SpriteRenderer>().sprite.bounds.size.y
            + 1 * transform.lossyScale.y / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        if (size.x < Minimum)
            size.x = Minimum;

        transform.localScale = new Vector2((size.x - spSizeChild1 * child1.lossyScale.y - spSizeChild2 * child2.lossyScale.y) 
            / (spSizeMain * transform.parent.lossyScale.x),
            transform.localScale.y);

        GetComponentInChildren<LoadingBar_Right>().Refresh();
        GetComponentInChildren<LoadingBar_Left>().Refresh();
        GetComponentInChildren<LoadingBar_Bar>().Refresh();

        SetProgess(oldProgress);

        return true;
    }

    public void SetColor(Color color)
    {
        Loading_bar_color = color;
        transform.GetComponentInChildren<LoadingBar_Bar>().GetComponent<SpriteRenderer>().color = Loading_bar_color;
    }
    public Color GetColor()
    {
        return Loading_bar_color;
    }
}
