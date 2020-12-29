using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject cell;
    public GameObject pawn;
    public GameObject horse;
    public GameObject rook;
    public GameObject sharp;
    public GameObject queen;
    public GameObject king;

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

    private Status status = Status.SELECT_PIECE;
    private List<GameObject> cells = new List<GameObject>();

    private void GeneratePiece(GameObject obj, GameObject piece, float height, float rotation, Color color)
    {
        GameObject iPiece = Instantiate(piece);
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float z = obj.transform.position.z;

        iPiece.transform.position += new Vector3(x, y + height, z);
        iPiece.transform.rotation  = Quaternion.Euler(cell.transform.rotation.eulerAngles.x, rotation, cell.transform.rotation.eulerAngles.z);
        iPiece.GetComponent<State>().color = color;
        addColor(iPiece, color);

        obj.GetComponent<State>().piece = iPiece;
        
        cells.Add(iPiece);
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
                GameObject obj = Instantiate(cell);
                obj.transform.position += new Vector3(size * i, 0,  size * j);

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
                    if (i == 1 || i == 6)
                        GeneratePiece(obj, horse, height, rotation, color);
                    if (i == 2 || i == 5)
                        GeneratePiece(obj, sharp, height, rotation, color);
                    if (i == 3)
                        GeneratePiece(obj, queen, height, rotation, color);
                    else 
                        GeneratePiece(obj, king, height, rotation, color);
                }
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard();
        moveManager = gameObject.AddComponent<Rotation>();
        chess = gameObject.AddComponent<Chess>();
        turn = white;
    }

    public delegate void EndToMove();

    void MovePiece()
    {
        void generic()
        {
            status = Status.SELECT_PIECE;
            pieceSelect.GetComponent<MeshRenderer>().material.color = pieceSelect.GetComponent<State>().color;
            cellSelect.GetComponent<MeshRenderer>().material.color = cellSelect.GetComponent<State>().color;
            pieceSelect = null;
            cellSelect = null;
        }

        void movePiece()
        {
            pieceSelect.GetComponent<State>().Move(cellSelect, () =>
            {
                turn = turn == white ? black : white;

                if (typeGame == TypeGame.HUMAN)
                    StartCoroutine(moveManager.Rotate(camera.transform, new Vector3(0, 0, 180), 1.0f));

                if (cellSelect.GetComponent<State>().piece != null)
                    Destroy(cellSelect.GetComponent<State>().piece);

                cellSelect.GetComponent<State>().piece = pieceSelect;
                oldCell.GetComponent<State>().piece = null;

                generic();
            });
        }

        if (typeGame == TypeGame.HUMAN || turn == white)
        {
            string oldC = oldCell.name;
            string newC = cellSelect.name;
            
            chess.Move(oldC + newC, () => {
                movePiece();
            }, generic);
        } else
        {
            chess.Move((o, n) =>
            {
                status = Status.SELECT_PIECE;
                cellSelect = GameObject.Find(n);
                oldCell = GameObject.Find(o);
                pieceSelect = oldCell.GetComponent<State>().piece;
                movePiece();
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

    // Update is called once per frame
    void Update()
    {
        if (IsHumanVSIA() && IsTurnIA())
        {
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
}
