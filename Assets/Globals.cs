using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour {

    public static GameObject initScreen;
    public static GameObject gameScreen;
    public static GameObject telaFim;

    public static bool vitoria = false;

    public static int n1 = -1;
    public static int n2 = -1;

    // Use this for initialization
    void Start () {

        Object TelaDoJogoPrefab = Resources.Load("TelaDoJogoPrefab");
        Object TelaFimDeJogo = Resources.Load("TelaFimDeJogoPrefab");
        Object TelaInicialPrefab = Resources.Load("TelaInicialPrefab");

        gameScreen = GameObject.Instantiate(TelaDoJogoPrefab) as GameObject;
        gameScreen.SetActive(false);
        telaFim = GameObject.Instantiate(TelaFimDeJogo) as GameObject;
        telaFim.SetActive(false);
        initScreen = GameObject.Instantiate(TelaInicialPrefab) as GameObject;
        initScreen.SetActive(false);

        GameObject go = GameObject.Find("Canvas");

        go.transform.localPosition = new Vector3(0, 0, 10);

        gameScreen.transform.parent = go.transform;
        telaFim.transform.parent = go.transform;
        initScreen.transform.parent = go.transform;

        showTelaInicial();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public static void showTelaInicial()
    {
        gameScreen.SetActive(false);
        telaFim.SetActive(false);
        initScreen.SetActive(true);
    }

    public static void showTelaGame()
    {
        gameScreen.SetActive(true);
        telaFim.SetActive(false);
        initScreen.SetActive(false);
    }

    public static void showTelaFim()
    {
        gameScreen.SetActive(false);
        telaFim.SetActive(true);
        initScreen.SetActive(false);
    }

}
