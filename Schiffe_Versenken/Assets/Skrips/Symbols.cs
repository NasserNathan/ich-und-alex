using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbols : MonoBehaviour
{
    [SerializeField] private Sprite Symbol_A;
    [SerializeField] private Sprite Symbol_B;
    [SerializeField] private Sprite Symbol_C;
    [SerializeField] private Sprite Symbol_D;
    [SerializeField] private Sprite Symbol_E;
    [SerializeField] private Sprite Symbol_F;
    [SerializeField] private Sprite Symbol_G;
    [SerializeField] private Sprite Symbol_H;
    [SerializeField] private Sprite Symbol_I;
    [SerializeField] private Sprite Symbol_J;
    [SerializeField] private Sprite Symbol_K;
    [SerializeField] private Sprite Symbol_L;
    [SerializeField] private Sprite Symbol_M;
    [SerializeField] private Sprite Symbol_N;
    [SerializeField] private Sprite Symbol_O;
    [SerializeField] private Sprite Symbol_P;
    [SerializeField] private Sprite Symbol_Q;
    [SerializeField] private Sprite Symbol_R;
    [SerializeField] private Sprite Symbol_S;
    [SerializeField] private Sprite Symbol_T;
    [SerializeField] private Sprite Symbol_U;
    [SerializeField] private Sprite Symbol_V;
    [SerializeField] private Sprite Symbol_W;
    [SerializeField] private Sprite Symbol_X;
    [SerializeField] private Sprite Symbol_Y;
    [SerializeField] private Sprite Symbol_Z;
    [SerializeField] private Sprite Symbol_ß;
    [SerializeField] private Sprite Symbol_0;
    [SerializeField] private Sprite Symbol_1;
    [SerializeField] private Sprite Symbol_2;
    [SerializeField] private Sprite Symbol_3;
    [SerializeField] private Sprite Symbol_4;
    [SerializeField] private Sprite Symbol_5;
    [SerializeField] private Sprite Symbol_6;
    [SerializeField] private Sprite Symbol_7;
    [SerializeField] private Sprite Symbol_8;
    [SerializeField] private Sprite Symbol_9;
    [SerializeField] private Sprite Symbol_RI;
    [SerializeField] private Sprite Symbol_RII;
    [SerializeField] private Sprite Symbol_RIII;
    [SerializeField] private Sprite Symbol_RIV;
    [SerializeField] private Sprite Symbol_RV;
    [SerializeField] private Sprite Symbol_RVI;
    [SerializeField] private Sprite Symbol_Schiff;

    private Sprite[] symbols = new Sprite[44];

    private void Awake()
    {
        symbols[00] = Symbol_A;
        symbols[01] = Symbol_B;
        symbols[02] = Symbol_C;
        symbols[03] = Symbol_D;
        symbols[04] = Symbol_E;
        symbols[05] = Symbol_F;
        symbols[06] = Symbol_G;
        symbols[07] = Symbol_H;
        symbols[08] = Symbol_I;
        symbols[09] = Symbol_J;
        symbols[10] = Symbol_K;
        symbols[11] = Symbol_L;
        symbols[12] = Symbol_M;
        symbols[13] = Symbol_N;
        symbols[14] = Symbol_O;
        symbols[15] = Symbol_P;
        symbols[16] = Symbol_Q;
        symbols[17] = Symbol_R;
        symbols[18] = Symbol_S;
        symbols[19] = Symbol_T;
        symbols[20] = Symbol_U;
        symbols[21] = Symbol_V;
        symbols[22] = Symbol_W;
        symbols[23] = Symbol_X;
        symbols[24] = Symbol_Y;
        symbols[25] = Symbol_Z;
        symbols[26] = Symbol_ß;
        symbols[27] = Symbol_0;
        symbols[28] = Symbol_1;
        symbols[29] = Symbol_2;
        symbols[30] = Symbol_3;
        symbols[31] = Symbol_4;
        symbols[32] = Symbol_5;
        symbols[33] = Symbol_6;
        symbols[34] = Symbol_7;
        symbols[35] = Symbol_8;
        symbols[36] = Symbol_9;
        symbols[37] = Symbol_RI;
        symbols[38] = Symbol_RII;
        symbols[39] = Symbol_RIII;
        symbols[40] = Symbol_RIV;
        symbols[41] = Symbol_RV;
        symbols[42] = Symbol_RVI;
        symbols[43] = Symbol_Schiff;
    }
    public static Sprite GetSprite(int index)
    {
        return GameObject.Find("Spielbrett").GetComponent<Symbols>().symbols[index];
    }
    public static Sprite GetSprite(string name)
    {
        if (name == "A") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_A;
        if (name == "B") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_B;
        if (name == "C") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_C;
        if (name == "D") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_D;
        if (name == "E") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_E;
        if (name == "F") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_F;
        if (name == "G") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_G;
        if (name == "H") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_H;
        if (name == "I") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_I;
        if (name == "J") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_J;
        if (name == "K") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_K;
        if (name == "L") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_L;
        if (name == "M") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_M;
        if (name == "N") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_N;
        if (name == "O") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_O;
        if (name == "P") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_P;
        if (name == "Q") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_Q;
        if (name == "R") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_R;
        if (name == "S") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_S;
        if (name == "T") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_T;
        if (name == "U") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_U;
        if (name == "V") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_V;
        if (name == "W") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_W;
        if (name == "X") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_X;
        if (name == "Y") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_Y;
        if (name == "Z") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_Z;
        if (name == "ß") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_ß;
        if (name == "0") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_0;
        if (name == "1") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_1;
        if (name == "2") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_2;
        if (name == "3") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_3;
        if (name == "4") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_4;
        if (name == "5") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_5;
        if (name == "6") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_6;
        if (name == "7") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_7;
        if (name == "8") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_8;
        if (name == "9") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_9;
        if (name == "RI") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_RI;
        if (name == "RII") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_RII;
        if (name == "RIII") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_RIII;
        if (name == "RIV") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_RIV;
        if (name == "RV") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_RV;
        if (name == "RVI") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_RVI;
        if (name == "Schiff") return GameObject.Find("Spielbrett").GetComponent<Symbols>().Symbol_Schiff;
        return null;
    }
}
