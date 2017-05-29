using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelaDoJogo : MonoBehaviour
{

    public GameObject n1;
    public GameObject n2;

    private Object TabuadaSquarePrefab;
    private List<GameObject> maze;

    private const int X_AXIS = 17;
    private const int Y_AXIS = 11;

    private const int ANDAR_LESTE = 1;
    private const int ANDAR_OESTE = -1;
    private const int ANDAR_NORTE = -X_AXIS;
    private const int ANDAR_SUL = X_AXIS;

    private const int MAX_DIR = 4;

    private int cursorMovimento = -1;

    private bool JOGO_INICIADO = false;

    private Image[] hp;

    enum Direcao
    {
        Norte = 0,
        Sul = 1,
        Leste = 2,
        Oeste = 3
    }

    // Use this for initialization
    void Start()
    {
        int extN1 = Globals.n1;
        int extN2 = Globals.n2;

        Text t = n1.GetComponent<Text>();
        t.text = extN1 + "";
        t = n2.GetComponent<Text>();
        t.text = extN2 + "";
    }

    // inherited
    void OnEnable()
    {
        if (Globals.gameScreen == null) return;

        hp = new Image[3];
        Transform hpBar = Globals.gameScreen.transform.Find("hpBar");
        hp[0] = hpBar.Find("h1").GetComponent<Image>();
        hp[1] = hpBar.Find("h2").GetComponent<Image>();
        hp[2] = hpBar.Find("h3").GetComponent<Image>();

        foreach (Image heart in hp)
        {
            heart.enabled = true;
        }

        TabuadaSquarePrefab = Resources.Load("TabuadaSquarePrefab");
        if (Globals.gameScreen)
        {
            criarGridTabuadaSquare();
        }
    }

    private int currentHealth()
    {
        int ret = 0;

        for (int i = 0; i < hp.Length; i++)
        {
            ret += hp[i].enabled ? 1 : 0;
        }

        return ret;
    }

    private void damage()
    {
        bool hit = false;

        int i;

        for (i = 0; i < hp.Length && !hit; i++)
        {
            if (hp[i].enabled)
            {
                hp[i].enabled = false;
                hit = true;
            }
        }

        if(i == hp.Length)
        {
            encerrarGame(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (JOGO_INICIADO)
        {
            animarCursor();

            if (Input.GetKeyDown(KeyCode.DownArrow) && possivelAndar(cursorMovimento, Direcao.Sul))
            {
                decidirJogadorAndar(Direcao.Sul);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && possivelAndar(cursorMovimento, Direcao.Oeste))
            {
                decidirJogadorAndar(Direcao.Oeste);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && possivelAndar(cursorMovimento, Direcao.Leste))
            {
                decidirJogadorAndar(Direcao.Leste);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && possivelAndar(cursorMovimento, Direcao.Norte))
            {
                decidirJogadorAndar(Direcao.Norte);
            }
        }
    }

    bool decidirJogadorPisou(Color c)
    {
        return c.Equals(new Color(1f, 0f, 0f)) || c.Equals(new Color(0f, 1f, 0f));
    }

    bool decidirCampoLimpo(int idx)
    {
        return maze[idx].transform.Find("bg").GetComponent<Image>().color.Equals(new Color(1f, 1f, 1f));
    }

    private void revelarCasa(int pos)
    {
        int num = int.Parse(maze[pos].transform.Find("number").GetComponent<Text>().text);
        bool divisivel = num % Globals.n1 == 0 || num % Globals.n2 == 0;

        maze[pos].transform.Find("bg").GetComponent<Image>().color = !divisivel ? new Color(1f, 0f, 0f) : new Color(0f, 1f, 0f);

        if (divisivel)
        {
            maze[pos].transform.Find("math1").GetComponent<Text>().enabled = true;
            maze[pos].transform.Find("math2").GetComponent<Text>().enabled = true;
        }
    }

    private void moverJogador(int pos)
    {
        int posAntiga = cursorMovimento;
        cursorMovimento = pos;
        revelarCasa(posAntiga);
        if(cursorMovimento == maze.Count - 1) // vitoria
        {
            encerrarGame(true);
        }
    }

    void decidirJogadorAndar(Direcao dir)
    {
        int peek = posDestino(cursorMovimento, dir);

        int num = int.Parse(maze[peek].transform.Find("number").GetComponent<Text>().text);
        bool divisivel = num % Globals.n1 == 0 || num % Globals.n2 == 0;
        bool pisado = decidirJogadorPisou(maze[peek].transform.Find("bg").GetComponent<Image>().color);

        if (pisado)
        {
            if (divisivel)
            {
                moverJogador(peek);
                // mover jogador, tem que pintar o quadrado antigo
            }
            else
            {
                ;// faz nada, jogador ja penalizado pela casa
            }
        }
        else
        {
            revelarCasa(peek);

            if (divisivel)
            {
                moverJogador(peek);
                // mover jogador, tem que pintar o quadrado antigo
            }
            else
            {
                damage();
            }
        }
    }

    private void animarCursor()
    {
        Image bg = maze[cursorMovimento].transform.FindChild("bg").GetComponent<Image>();
        // tentar sair do vermelho
        bg.color = Color.HSVToRGB(0.5f + Mathf.Abs(Mathf.Sin(Time.time)) * 0.33f, 0.25f + Mathf.Abs(Mathf.Sin(Time.time)) * 0.75f, 1);
    }

    private void criarTabuadaSquare(float x, float y)
    {
        GameObject go = GameObject.Instantiate(TabuadaSquarePrefab) as GameObject;
        Transform tr = go.GetComponent<Transform>();
        tr.localPosition = new Vector3(x, y, tr.localPosition.z);
        tr.parent = Globals.gameScreen.transform;
        maze.Add(go);
    }

    public void criarGridTabuadaSquare()
    {
        float startX = 140f,
                startY = 474f,
                diffX = 42.5f,
                diffY = 42.5f;

        maze = new List<GameObject>();

        for (int j = 0; j < Y_AXIS; j++)
        {
            for (int i = 0; i < X_AXIS; i++)
            {
                criarTabuadaSquare(startX + i * diffX, startY - diffY * j);
            }
        }

        cursorMovimento = 0;
        marcarCasaPossivel(cursorMovimento);
        criarCaminho(cursorMovimento);
        marcarParedes();
        marcarCasaPossivel(maze.Count - 1);
        JOGO_INICIADO = true;
    }

    void criarCaminho2(int pos)
    {
        int rng = Random.Range(0, MAX_DIR);
        for (int i = 0; i < MAX_DIR; i++, rng = ++rng % MAX_DIR)
        {
            Direcao dir = (Direcao)rng;
            int dest = posDestino(pos, dir);
            if (possivelAndar(dest, dir) && ehParede(dest))
            {
                marcarCasaPossivel(dest);
                criarCaminho2(dest);
            }
        }
    }

    private IEnumerator recWait(bool vitoria)
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = maze.Count - 1; i >= 0; i--)
        {
            GameObject o = maze[i];
            maze.RemoveAt(i);
            Destroy(o);
        }

        Globals.vitoria = vitoria;
        Globals.showTelaFim();
    }

    private void criarCaminho(int pos)
    {
        HashSet<int> rngs = new HashSet<int>();

        while (rngs.Count != MAX_DIR)
        {
            rngs.Add(Random.Range(0, MAX_DIR));
        }

        foreach (int rng in rngs)
        {
            Direcao dir = (Direcao)rng;
            int destPre = posDestino(pos, dir);
            int dest = posDestino(destPre, dir);
            if (!possivelAndar(pos, dir) || !casaNaoPisada(dest)) continue;
            marcarCasaPossivel(dest);
            marcarCasaPossivel(destPre);
            criarCaminho(dest);
        }
    }

    bool ehParede(int pos)
    {
        bool ret = false;
        if (pos >= 0 && pos < maze.Count)
        {
            string txt = maze[pos].transform.Find("number").GetComponent<Text>().text;
            if (string.Compare(txt, "?") != 0)
            {
                int num = int.Parse(txt);
                ret = !(num % Globals.n1 == 0 || num % Globals.n2 == 0);
            }
        }
        return ret;
    }

    private void encerrarGame(bool vitoria)
    {

        JOGO_INICIADO = false;
        StartCoroutine(revelarTodos(0, vitoria));
    }

    private IEnumerator revelarTodos(int idx, bool vitoria)
    {
        if (idx != maze.Count)
        {
            if (decidirCampoLimpo(idx))
            {
                yield return new WaitForSeconds(0.01f);
            }
            revelarCasa(idx);
            StartCoroutine(revelarTodos(idx + 1, vitoria));
        } else
        {
            StartCoroutine(recWait(vitoria));
        }
    }

    private bool possivelAndar(int pos, Direcao dir)
    {
        bool ret = false;

        if (pos >= 0 && pos < maze.Count)
        {
            if (dir == Direcao.Leste)
            {
                ret = !((pos + 1) % X_AXIS == 0);
            }
            else if (dir == Direcao.Oeste)
            {
                ret = !(pos % X_AXIS == 0);
            }
            else if (dir == Direcao.Sul)
            {
                ret = !(pos >= (Y_AXIS - 1) * X_AXIS);
            }
            else /* Direcao.Norte */
            {
                ret = !(pos < X_AXIS);
            }
        }

        return ret;
    }

    private bool casaNaoPisada(int pos)
    {
        bool ret = false;
        if (pos >= 0 && pos < maze.Count)
        {
            Text t = maze[pos].transform.Find("number").GetComponent<Text>();
            ret = string.Compare(t.text, "?") == 0;
        }
        return ret;
    }

    private int posDestino(int pos, Direcao dir)
    {
        int ret = pos;
        switch (dir)
        {
            case Direcao.Leste:
                ret += ANDAR_LESTE;
                break;
            case Direcao.Norte:
                ret += ANDAR_NORTE;
                break;
            case Direcao.Oeste:
                ret += ANDAR_OESTE;
                break;
            default: /* Sul */
                ret += ANDAR_SUL;
                break;
        }
        return ret;
    }

    private void marcarCasaPossivel(int pos)
    {
        int rng = Random.Range(0, 3);
        Text label = maze[pos].transform.FindChild("number").GetComponent<Text>();
        int n1 = rng != 1 ? Globals.n1 : 1;
        int n2 = rng != 0 ? Globals.n2 : 1;
        int generated;
        do
        {
            generated = Random.Range(1, 200) * n1 * n2;
        } while (generated >= 1000);

        label.text = "" + generated;

        Text math1 = maze[pos].transform.FindChild("math1").GetComponent<Text>();
        Text math2 = maze[pos].transform.FindChild("math2").GetComponent<Text>();

        if (generated % Globals.n1 == 0)
        {
            math1.text = (generated / Globals.n1) + "*" + Globals.n1;
        }
        else
        {
            math1.text = "!=" + Globals.n1;
        }

        if (generated % Globals.n2 == 0)
        {
            math2.text = (generated / Globals.n2) + "*" + Globals.n2;
        }
        else
        {
            math2.text = "!=" + Globals.n2;
        }

    }

    private void marcarParedes()
    {
        for (int i = 0; i < maze.Count; i++)
        {
            if (casaNaoPisada(i))
            {
                int rng = Random.Range(0, 10);

                if (rng < 3)
                {
                    marcarCasaPossivel(i);
                }
                else
                {
                    criarParede(i);
                }
            }

            Text math1 = maze[i].transform.FindChild("math1").GetComponent<Text>();
            Text math2 = maze[i].transform.FindChild("math2").GetComponent<Text>();
            math1.enabled = false;
            math2.enabled = false;
        }
    }

    private void criarParede(int pos)
    {
        Text label = maze[pos].transform.FindChild("number").GetComponent<Text>();
        int rng = -1;
        do
        {
            rng = Random.Range(1, 1000);
        } while (rng % Globals.n1 == 0 || rng % Globals.n2 == 0);
        label.text = "" + rng;
    }

}