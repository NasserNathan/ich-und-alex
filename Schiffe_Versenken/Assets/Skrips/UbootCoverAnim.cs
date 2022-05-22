using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UbootCoverAnim : MonoBehaviour
{
    private void Start()
    {
        GameObject.Find("Spielbrett").transform.parent.GetChild(3).gameObject.SetActive(false);
    }


    public static void AnimStart()
    {
        GameObject.Find("Spielbrett").transform.parent.GetChild(3).gameObject.SetActive(true);
    }

    public static void AnimRocket()
    {
        if (GameObject.Find("Spielbrett").transform.parent.GetChild(3).gameObject.activeSelf)
            GameObject.Find("Spielbrett").transform.parent.GetChild(3).GetComponent<Animator>().SetTrigger("Rocket");
    }

    public static void AnimEnd()
    {
        GameObject.Find("Spielbrett").transform.parent.GetChild(3).GetComponent<Animator>().SetTrigger("End");
        GameObject.Find("Spielbrett").transform.parent.GetChild(3).gameObject.SetActive(false);
    }
}
