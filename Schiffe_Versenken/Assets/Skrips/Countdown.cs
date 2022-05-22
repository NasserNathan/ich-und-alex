using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField]
    private float directionalSpeed = 0;
    private float oldSpeed = 0;
    [SerializeField]
    private bool status = false;
    public float percentage = 0;
    private LoadingBar loadingBar;

    // Start is called before the first frame update
    void Start()
    {
        loadingBar = GetComponent<LoadingBar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (status)
        {
            percentage += directionalSpeed * Time.deltaTime;
            if (percentage <= 0)
            {
                if (directionalSpeed < 0)
                    CountdownPause();
                percentage = 0;
            }
            if (percentage >= 100)
            {
                if (directionalSpeed > 0)
                    CountdownPause();
                percentage = 100;
            }
            loadingBar.SetProgess(percentage / 100);
        }
    }

    /// <summary>
    /// Startet den Countdown und ändert die Änderungsrate. (directionalSpeed != 0)
    /// </summary>
    /// <param name="directionalSpeed">Änderungsrate in Prozent pro Minute</param>
    /// /// <param name="timeInSec">True -> directionalSpeed entspricht der Zeit von 0 auf 100 Prozent (bzw. 100 auf 0 je nach Vorzeichen).</param>
    /// <returns>Gibt false zurück, falls directionalSpeed = 0 ist.</returns>
    public bool CountdownStart(float directionalSpeed, bool timeInSec = false)
    {
        if (directionalSpeed == 0)
            return false;

        if (timeInSec) this.directionalSpeed = 100 / directionalSpeed;
        else this.directionalSpeed = directionalSpeed;

        status = true;
        return true;
    }

    public void CountdownStop()
    {
        directionalSpeed = 0;
        status = false;
    }

    /// <summary>
    /// Pausiert einen laufenden Countdown.
    /// </summary>
    /// <returns>Gibt false zurück, falls der Countdown deaktiviert oder bereits pausiert ist.</returns>
    public bool CountdownPause()
    {
        if (status && directionalSpeed != 0)
        {
            oldSpeed = directionalSpeed;
            directionalSpeed = 0;
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Setzt den pausierten Countdown fort.
    /// </summary>
    /// <returns>Gibt false zurück, falls der Countdown deaktiviert ist oder bereits fortläuft.</returns>
    public bool CountdownContinue()
    {
        if (status && directionalSpeed == 0)
        {
            directionalSpeed = oldSpeed;
            oldSpeed = 0;
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Setzt die Füllmenge in Prozent.
    /// </summary>
    public void SetPercentage(float percentage)
    {
        if (percentage <= 0)
        {
            if (directionalSpeed < 0)
                CountdownPause();
            percentage = 0;
        }
        if (percentage >= 100)
        {
            if (directionalSpeed > 0)
                CountdownPause();
            percentage = 100;
        }
        this.percentage = percentage;
    }

    /// <summary>
    /// Gibt die aktuelle Füllmenge in Prozent an.
    /// </summary>
    /// <param name="decimalNumber">True -> Angabe in Dezimalzahl</param>
    /// <returns></returns>
    public float GetProgress(bool decimalNumber = false)
    {
        if (decimalNumber)
            return loadingBar.GetProgess();
        return percentage;
    }
}
