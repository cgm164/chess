using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Material material_madera_clara;
    public Material material_madera_oscura;
    public Material material_marmol_claro;
    public Material material_marmol_oscuro;
    public Material material_oro;
    public Material material_plata;
    public GameObject camera;

    public Color black;
    public Color white;

    private Color turn;
    public TypeGame typeGame = TypeGame.IA;

    Ray ray;
    RaycastHit hit;

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


    private void GeneratePiece(GameObject obj, GameObject piece, float height, float rotation, Color color)
    {
        GameObject iPiece = Instantiate(piece);
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float z = obj.transform.position.z;

        iPiece.transform.position += new Vector3(x, y + height, z);
        iPiece.transform.rotation  = Quaternion.Euler(piece.transform.rotation.eulerAngles.x, piece.transform.eulerAngles.y + rotation, piece.transform.rotation.eulerAngles.z);
        iPiece.GetComponent<State>().color = color;
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

                if (j == 1 || j == 6)
                {
                    GeneratePiece(obj, pawn, height, 0, j == 1? white : black);

                } else if (j == 0 || j == 7)
                {
                    float rotation = j == 0 ? 0 : -180;
                    Color color = j == 0 ? white : black;

                    if (i == 0 || i == 7)
                        GeneratePiece(obj, rook, height, rotation, color);
                    else if (i == 1 || i == 6)
                        GeneratePiece(obj, horse, height, rotation, color);
                    else if (i == 2 || i == 5)
                        GeneratePiece(obj, sharp, height, rotation, color);
                    else if (i == 3)
                        GeneratePiece(obj, queen, height, rotation, color);
                    else 
                        GeneratePiece(obj, king, height, rotation, color);
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
        turn = white;
        typeGame = PlayerPrefs.GetInt("modoJuego") == 0 ? TypeGame.HUMAN : TypeGame.IA;
    }

    public delegate void EndToMove();
    public delegate void EndToMoveStatus(bool a);

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

        void movePiece(string o, string n)
        {
            pieceSelect.GetComponent<State>().Move(cellSelect, () =>
            {
                //if (typeGame == TypeGame.HUMAN)
                //{
                //    Vector3 v = camera.transform.rotation.eulerAngles;

                //    StartCoroutine(moveManager.Rotate(camera.transform, Quaternion.Euler(v.x, v.y + 180, v.z), 1f));
                //    StartCoroutine(moveManager.Translation(camera.transform, new Vector3(0, 0, turn == white ? 37 : -37), 1f, Rotation.MoveType.Time));
                //}

                if (cellSelect.GetComponent<State>().piece != null)
                    Destroy(cellSelect.GetComponent<State>().piece);

                cellSelect.GetComponent<State>().piece = pieceSelect;
                oldCell.GetComponent<State>().piece = null;
                
                int index = castlingsMoves.IndexOf(o + n);
                generic(index == -1);

                if (index != -1)
                {
                    cellSelect = GameObject.Find(castlingsRookR[index]);
                    oldCell = GameObject.Find(castlingsRookL[index]);
                    pieceSelect = oldCell.GetComponent<State>().piece;
                    movePiece(castlingsRookL[index], castlingsRookR[index]);
                } else
                    turn = turn == white ? black : white;
            });
        }

        if (typeGame == TypeGame.HUMAN || turn == white)
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
        return status == Status.SELECT_PIECE && turn == black; 
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

                if (GetOriginalColor(piece) == turn)
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
    public void ChangeMaterialWood(){
        ChangeMaterial(material_madera_oscura, material_madera_clara);

        
        //this.gameObject.GetComponent<MeshRenderer>().material = null;
        //obj.GetComponent<MeshRenderer>().material = material_madera;
        
        //obj.GetComponent<State>().color = color;

    }
    private void ChangePieceMaterial(){
        
    }
    private void ChangeCellMaterial(){

    }
    public void ChangeMaterialMarble(){
        ChangeMaterial(material_marmol_oscuro, material_marmol_claro);
    }
    public void ChangeMaterialGS(){
        ChangeMaterial(material_oro, material_plata);
    }
    //Material1=oscuro, Material2=claro
    private void ChangeMaterial(Material material1, Material material2){
        int y=0;
        GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(var root in objects){
            if(root.GetComponent<MeshRenderer>()!=null && Char.IsUpper(root.name, 0)){

                if(y==3) {
                    root.GetComponent<MeshRenderer>().material = material1;
                    y=4;
                }
                else if(y==4){
                    root.GetComponent<MeshRenderer>().material = material1;
                    y=1;
                }
                else{
                    root.GetComponent<MeshRenderer>().material = material2;
                    y++;
                }


            }
        }
    }
    public void ResetMaterialToColour(){
        int y=0;
        GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(var root in objects){
            if(root.GetComponent<MeshRenderer>()!=null && Char.IsUpper(root.name, 0)){
                if(y==3) {
                    root.GetComponent<MeshRenderer>().material.color = black;
                    root.GetComponent<State>().color = black;
                    y=4;
                }
                else if(y==4){
                    root.GetComponent<MeshRenderer>().material.color = black;
                    root.GetComponent<State>().color = black;
                    y=1;
                }
                else{
                    root.GetComponent<MeshRenderer>().material.color = white;
                    root.GetComponent<State>().color = white;
                    y++;
                }
            }
            
        }
    }
}
