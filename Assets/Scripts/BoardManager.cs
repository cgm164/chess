using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;
public class BoardManager : MonoBehaviour
{
    public GameObject cell;
    public GameObject pawn;
    public GameObject horse;
    public GameObject rook;
    public GameObject sharp;
    public GameObject queen;
    public GameObject king;
    public GameObject bordeXSup;
    public GameObject bordeYDer;
    public GameObject bordeXInf;
    public GameObject bordeYIzq;
    public Material material_madera_clara;
    public Material material_madera_oscura;
    public Material material_marmol_claro;
    public Material material_marmol_oscuro;
    public Material material_oro;
    public Material material_plata;
    public CountDown timerW;
    public CountDown timerB; 
    public GameObject camera;
    public Material[] actualW;

    public GameObject islandW;
    public GameObject islandB;

    public float time;

    public Color black;
    public Color white;
    private Turn turn;
    public TypeGame typeGame = TypeGame.IA;

    Ray ray;
    RaycastHit hit;

    private bool isGameEnding = false;
    private GameObject pieceSelect;
    private GameObject oldCell;
    private GameObject cellSelect;
    private Chess chess;
    private Rotation moveManager;
    private Status status = Status.SELECT_PIECE;
    private List<string> castlingsMoves = new List<string> { "e1g1", "e1c1", "e8g8", "e8c8"};
    private List<string> castlingsRookL = new List<string> { "h1", "a1", "h8", "a8"};
    private List<string> castlingsRookR = new List<string> { "f1", "d1", "f8", "d8" };
    private enum Status
    {
        SELECT_PIECE, 
        SELECT_CELL, 
        MOVE_PIECE
    };

    public enum TypeGame
    {
        IA, 
        HUMAN
    };

    public enum Turn
    {
        BLACK, 
        WHITE
    };


    private void GeneratePiece(GameObject obj, string name, int points, GameObject piece, float height, float rotation, Color color, Turn player)
    {
        GameObject iPiece = Instantiate(piece);
        iPiece.name = name + (player == Turn.BLACK? "black" : "white");
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float z = obj.transform.position.z;

        iPiece.transform.position += new Vector3(x, y + height, z);
        iPiece.transform.rotation  = Quaternion.Euler(piece.transform.rotation.eulerAngles.x, piece.transform.eulerAngles.y + rotation + 180f, piece.transform.rotation.eulerAngles.z);
        iPiece.GetComponent<State>().color = color;
        iPiece.GetComponent<State>().player = player;
        iPiece.GetComponent<State>().points = points;
        iPiece.GetComponent<State>().namePrefab = piece.name;
        addColor(iPiece, color);

        obj.GetComponent<State>().piece = iPiece;
    }

    private void addColor(GameObject obj, Color color)
    {
        obj.GetComponent<MeshRenderer>().material.color = color;
        obj.GetComponent<State>().color = color;
    }

    private void GenerateBoard()
    {
        GenerateBorders();
        float size = cell.GetComponent<Collider>().bounds.size.x;
        float height = cell.GetComponent<Collider>().bounds.size.y;

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++) {
                GameObject obj;

                obj = Instantiate(cell);
                obj.transform.position += new Vector3(size * i, 0, size * j);

                if (i % 2 == 0 && j % 2 == 0 || i % 2 != 0 && j % 2 != 0)
                    addColor(obj, black);
                else
                    addColor(obj, white);


                obj.name = char.ConvertFromUtf32(i + 97) + "" + (j + 1);
                string name = "piece" + obj.name;

                if (j == 1 || j == 6)
                {
                    GeneratePiece(obj, name, 1, pawn, height, 0, j == 1? white : black, j == 1 ? Turn.WHITE : Turn.BLACK);

                } else if (j == 0 || j == 7)
                {
                    float rotation = j == 0 ? 0 : -180;
                    Color color = j == 0 ? white : black;
                    Turn turn = j == 0 ? Turn.WHITE : Turn.BLACK;

                    if (i == 0 || i == 7)
                        GeneratePiece(obj, name, 3, rook, height, rotation, color, turn);
                    else if (i == 1 || i == 6)
                        GeneratePiece(obj, name, 2, horse, height, rotation, color, turn);
                    else if (i == 2 || i == 5)
                        GeneratePiece(obj, name, 2, sharp, height, rotation, color, turn);
                    else if (i == 3)
                        GeneratePiece(obj, name, 4, queen, height, rotation, color, turn);
                    else 
                        GeneratePiece(obj, name, 100, king, height, rotation, color, turn);
                }
            }
        cell.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard();
        moveManager = gameObject.AddComponent<Rotation>();
        chess = gameObject.AddComponent<Chess>();
        turn = Turn.WHITE;
        typeGame = PlayerPrefs.GetInt("modoJuego") == 0 ? TypeGame.HUMAN : TypeGame.IA;
        isGameEnding = true;
        islandB.GetComponent<MeshRenderer>().material.color = Color.white;
        time = PlayerPrefs.GetInt("tiempo");
        timerW.startTimer(time);
        timerB.startTimer(time);
        int texture = PlayerPrefs.GetInt("textura");
        
        if (texture == 1)
            ChangeMaterialWood();
        else if (texture == 2)
            ChangeMaterialGS();
        else
            ChangeMaterialMarble();        
    }

    public delegate void EndToMove();
    public delegate void EndToMoveStatus(bool a);

    public void StartGame()
    {
        timerW.stop = false;
        isGameEnding = false;
    }
    public void StopGame()
    {
       timerW.stop = true;
       timerB.stop = true;
       isGameEnding = true;
    }

    public void ContinueGame()
    {
        timerW.stop = turn != Turn.WHITE;
        timerB.stop = turn != Turn.BLACK;
        isGameEnding = false;
    }


    private int CountPointsIsland(GameObject island)
    {
        int points = 0;

        foreach (Transform child in island.transform.parent)
        {
            Debug.Log(child.gameObject.name);
            if (child.gameObject.name.Contains("piece"))
                points += child.GetComponent<State>().points;
        }

        return points;
    }

    private void CheckTimeout()
    {
        float timeB = timerB.timeLeft;
        float timeW = timerW.timeLeft;

        if ((timeB < 0 || timeW < 0) && !isGameEnding)
        {
            //int whitePoints = CountPointsIsland(islandW);
            //int blackPoints = CountPointsIsland(islandB);
            isGameEnding = true;
            if (timeB < 0)
                Debug.Log("Negras ganan");
            else
                Debug.Log("Blancas ganan");
        } 
    }

    private int iIslandW = 0;
    private int iIslandB = 0;
    void MovePiece()
    {
        void generic(bool change)
        {
            pieceSelect.GetComponent<MeshRenderer>().material.color = pieceSelect.GetComponent<State>().color;
            cellSelect.GetComponent<MeshRenderer>().material.color = cellSelect.GetComponent<State>().color;
            pieceSelect = null;
            cellSelect = null;
            if (change)
                status = Status.SELECT_PIECE;

        }

        void AddIntoIslandW(GameObject _piece)
        {
            GameObject piece = Instantiate(_piece);
            int i = iIslandW % 8;
            int j = iIslandW / 8;
            GameObject cellIsland = Instantiate(islandW, islandW.transform.parent);
            cellIsland.SetActive(true);
            float size   = cellIsland.GetComponent<Collider>().bounds.size.x;
            float height = cellIsland.GetComponent<Collider>().bounds.size.y;

            cellIsland.transform.position += new Vector3(j * size, 0, -i * size);
            piece.transform.position = cellIsland.transform.position + new Vector3(0, piece.transform.position.y, 0);
            piece.transform.rotation = Quaternion.Euler(piece.transform.rotation.eulerAngles.x, piece.transform.eulerAngles.y + 180, piece.transform.rotation.eulerAngles.z);
            piece.transform.parent = islandW.transform.parent;
            iIslandW++;
        }

        void AddIntoIslandB(GameObject _piece)
        {
            GameObject piece = Instantiate(_piece);
            int i = iIslandB % 8;
            int j = iIslandB / 8;
            GameObject cellIsland = Instantiate(islandB, islandB.transform.parent);
            cellIsland.SetActive(true);
            float size = cellIsland.GetComponent<Collider>().bounds.size.x;
            float height = cellIsland.GetComponent<Collider>().bounds.size.y;

            cellIsland.transform.position += new Vector3(-j * size, 0, -i * size);
            piece.transform.position = cellIsland.transform.position + new Vector3(0, piece.transform.position.y, 0);
            piece.transform.parent = islandB.transform.parent;
            iIslandB++;
        }

        bool promotionQueen(GameObject cell, GameObject piece)
        {
            return piece.GetComponent<State>().namePrefab == "Pawn" && 
                  (turn == Turn.WHITE && cell.name.Contains('8') ||
                   turn == Turn.BLACK && cell.name.Contains('1')); 
        }

        void movePiece(string o, string n)
        {
            if (cellSelect.GetComponent<State>().piece != null)
            {
                if (turn == Turn.WHITE)
                   AddIntoIslandB(cellSelect.GetComponent<State>().piece);
                else
                    AddIntoIslandW(cellSelect.GetComponent<State>().piece);
                StartCoroutine(cellSelect.GetComponent<State>().piece.GetComponent<PieceBehaviour>().Capture());
            }

            pieceSelect.GetComponent<State>().Move(cellSelect, () =>
            {   
                if (promotionQueen(cellSelect, pieceSelect))
                {
                    GameObject obj = Instantiate(queen);
                    obj.transform.position += new Vector3(pieceSelect.transform.position.x, cellSelect.GetComponent<Collider>().bounds.size.y, pieceSelect.transform.position.z);
                    obj.GetComponent<MeshRenderer>().material = new Material(pieceSelect.GetComponent<MeshRenderer>().material);
                    obj.GetComponent<State>().color = pieceSelect.GetComponent<State>().color;
                    obj.GetComponent<State>().player = turn;
                    obj.name = pieceSelect.name;
                    Destroy(pieceSelect);
                    pieceSelect = obj;
                }

                cellSelect.GetComponent<State>().piece = pieceSelect;
                oldCell.GetComponent<State>().piece = null;

                int index = castlingsMoves.IndexOf(o + n);

                chess.GetStatusGame((Chess.GameStatus st) =>
                {
                    if (st == Chess.GameStatus.in_progress)
                    {
                        generic(index == -1);

                        if (index != -1)
                        {
                            cellSelect = GameObject.Find(castlingsRookR[index]);
                            oldCell = GameObject.Find(castlingsRookL[index]);
                            pieceSelect = oldCell.GetComponent<State>().piece;
                            movePiece(castlingsRookL[index], castlingsRookR[index]);
                        }
                        else
                        {
                            //timerW.stop = turn == Turn.WHITE;
                            //timerB.stop = turn == Turn.BLACK;
                            turn = turn == Turn.WHITE ? Turn.BLACK : Turn.WHITE;
                        }
                            
                        return;
                    }
                    Debug.Log("GAME OVER");
                });
            });
        }

        if (typeGame == TypeGame.HUMAN || turn == Turn.WHITE)
        {
            string oldC = oldCell.name;
            string newC = cellSelect.name;

            chess.Move(oldC + newC, () => {
                movePiece(oldC, newC);
            }, generic);
        } else
        {
            chess.Move((o, n) =>
            {
                cellSelect = GameObject.Find(n);
                oldCell = GameObject.Find(o);
                pieceSelect = oldCell.GetComponent<State>().piece;
                movePiece(o, n);
            });            
        }  
    }

    private bool IsHumanVSIA()
    {
        return typeGame == TypeGame.IA;
    }

    private bool IsTurnIA()
    {
        return status == Status.SELECT_PIECE && turn == Turn.BLACK; 
    }

    private GameObject GetClickedCell()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            return hit.collider.gameObject;

        return null;
    }

    private GameObject GetPiece(GameObject obj)
    {
        return obj.GetComponent<State>().piece;
    }

    private Color GetOriginalColor(GameObject obj)
    {
        return obj.GetComponent<State>().color;
    }

    private Turn GetPlayer(GameObject obj)
    {
        return obj.GetComponent<State>().player;
    }

    private void SetColor(GameObject obj, Color color)
    {
        obj.GetComponent<MeshRenderer>().material.color = color;
    }

    private bool IsSamePieceThatSelected(GameObject piece)
    {
        return pieceSelect == piece && pieceSelect != null;
    }

    private void ResetSelection()
    {
        pieceSelect = null;
        cellSelect = null;
        oldCell = null;
    }

    public void ResetGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameEnding)
            return;

        //CheckTimeout();

        if (IsHumanVSIA() && IsTurnIA())
        {
            status = Status.MOVE_PIECE;
            MovePiece();
            return;
        }

        GameObject cell = GetClickedCell();

        if (cell)
        {
            GameObject piece = GetPiece(cell);

            if (IsSamePieceThatSelected(piece))
            {
                status = Status.SELECT_PIECE;
                SetColor(pieceSelect, GetOriginalColor(pieceSelect));

                if (cellSelect)
                    SetColor(cellSelect, GetOriginalColor(cellSelect));

                ResetSelection();
                return;
            }

            if (status == Status.SELECT_PIECE && piece)
            {
                pieceSelect = piece;
                oldCell     = cell;

                if (GetPlayer(piece) == turn)
                {
                    status = Status.SELECT_CELL;
                    SetColor(pieceSelect, Color.red);
                }
            }
            else if (status == Status.SELECT_CELL)
            {
                status = Status.MOVE_PIECE;
                cellSelect = hit.collider.gameObject;
                SetColor(cellSelect, Color.red);
                MovePiece();
            }

        }
    }
    private void addMaterial(GameObject obj, Material m)
    {
        obj.GetComponent<MeshRenderer>().material = m;
        obj.GetComponent<State>().color = m.color;
    }

    public void ChangeMaterialWood()
    {
        ChangeMaterial(material_madera_oscura, material_madera_clara);
    }

    public void ChangeMaterialMarble(){
        ChangeMaterial(material_marmol_oscuro, material_marmol_claro);
    }
    public void ChangeMaterialGS(){
        ChangeMaterial(material_plata, material_oro);
    }

    private void ChangeMaterial(Material m1, Material m2) 
    {
        GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects().Where(c => c.name.Contains("piece")).ToArray();
        black = m1.color;
        white = m2.color;

        foreach (GameObject piece in objects)
            addMaterial(piece, piece.name.Contains("black")? m1 : m2);
    }
    public void GenerateBorders(){
        GameObject go;
        float size=0;

        //Borde Inferior
        size = bordeXInf.GetComponent<Collider>().bounds.size.x;
        go = Instantiate(bordeXInf);
        go.transform.position += new Vector3(size, 0, size);

        //Borde Izquierdo
        size = bordeYIzq.GetComponent<Collider>().bounds.size.x;
        go = Instantiate(bordeYIzq);
        go.transform.position += new Vector3(size, 0, size);

        //BordeSuperior
        size = bordeXSup.GetComponent<Collider>().bounds.size.x;
        go = Instantiate(bordeXSup);
        go.transform.position += new Vector3(size, 0, size);

        //Borde Derecho
        size = bordeYDer.GetComponent<Collider>().bounds.size.x;
        go = Instantiate(bordeYDer);
        go.transform.position += new Vector3(size, 0, size);

    }

  
}

