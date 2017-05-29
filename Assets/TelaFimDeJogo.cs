using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelaFimDeJogo : MonoBehaviour {

    private Text label;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		label.color = Color.HSVToRGB(0.5f + Mathf.Abs(Mathf.Sin(Time.time)) * 0.33f, 0.25f + Mathf.Abs(Mathf.Sin(Time.time)) * 0.75f, 1);
    }

    private IEnumerator irInicio()
    {
        yield return new WaitForSeconds(3);
        Globals.showTelaInicial();
    }

    void OnEnable()
    {
        if(Globals.telaFim) { 
            if(Globals.vitoria)
            {
                label = Globals.telaFim.transform.Find("vitoria").GetComponent<Text>();
                Globals.telaFim.transform.Find("derrota").GetComponent<Text>().enabled = false;
            } else
            {
                label = Globals.telaFim.transform.Find("derrota").GetComponent<Text>();
                Globals.telaFim.transform.Find("vitoria").GetComponent<Text>().enabled = false;
            }

            label.enabled = true;
            StartCoroutine(irInicio());
        }
    }
}
