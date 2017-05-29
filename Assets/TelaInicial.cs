using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelaInicial : MonoBehaviour
{
    public GameObject labelEspera;

    private KeyCode[] keyCodesAlpha = {
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    private KeyCode[] keyCodesKeypad = {
         KeyCode.Keypad2,
         KeyCode.Keypad3,
         KeyCode.Keypad4,
         KeyCode.Keypad5,
         KeyCode.Keypad6,
         KeyCode.Keypad7,
         KeyCode.Keypad8,
         KeyCode.Keypad9,
     };

    // Use this for initialization
    void Start()
    {
        labelEspera.SetActive(false);
    }

    void OnEnable ()
    {
        Globals.n2 = Globals.n1 = -1;
        GameObject.FindGameObjectWithTag("jogon1").GetComponent<Text>().text = "";
        GameObject.FindGameObjectWithTag("jogon2").GetComponent<Text>().text = "";
        labelEspera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        updateTelaInicial();
    }

    private void setNumber (string name, int n1)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(name);
        for(int i = 0; i < objs.Length; i++)
        {
            Text t = objs[i].GetComponent<Text>();
            t.text = n1 + "";
        }
    }

    private IEnumerator TimeoutDigitacaoTelaInicio()
    {
        yield return new WaitForSeconds(2f);
        Globals.showTelaGame();
    }

    private IEnumerator BlinkLoading()
    {
        yield return new WaitForSeconds(0.33f);
        if(Globals.initScreen.activeSelf) { 
            labelEspera.SetActive(!labelEspera.activeInHierarchy);
            StartCoroutine(BlinkLoading());
        }
    }

    private void startGame ()
    {
        labelEspera.SetActive(true);
        StartCoroutine(TimeoutDigitacaoTelaInicio());
        StartCoroutine(BlinkLoading());
    }

    private void updateTelaInicial()
    {
        bool interrupt = false;

        if (Globals.n1 == -1 || Globals.n2 == -1)
        {
            for (int i = 1; i < 9 && !interrupt; i++)
            {
                if (Input.GetKeyDown(keyCodesAlpha[i - 1]) || Input.GetKeyUp(keyCodesKeypad[i - 1]))
                {
                    int numberPressed = i + 1;
                    if (Globals.n1 == -1)
                    {
                        Globals.n1 = numberPressed;
                        setNumber("jogon1", Globals.n1);
                    }
                    else if (Globals.n1 != numberPressed)
                    {
                        Globals.n2 = numberPressed;
                        setNumber("jogon2", Globals.n2);
                        interrupt = true;
                        startGame();
                    }
                }
            }
        }

    }

}