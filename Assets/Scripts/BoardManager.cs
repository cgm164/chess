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

    public Color black;
    public Color white;

    Ray ray;
    RaycastHit hit;

    private GameObject pieceSelect;
    private GameObject oldCell;
    private GameObject cellSelect;
    private Chess chess;

    private enum Status
    {
        SELECT_PIECE, 
        SELECT_CELL, 
        MOVE_PIECE
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
        iPiece.GetComponent<StatePiece>().color = color;
        addColor(iPiece, color);

        obj.GetComponent<StatePiece>().cell = iPiece;
        
        cells.Add(iPiece);
    }

    private void addColor(GameObject obj, Color color)
    {
        obj.GetComponent<MeshRenderer>().material.color = color;
        obj.GetComponent<StatePiece>().color = color;
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
                    GeneratePiece(obj, pawn, height, 0, j == 1? black : white);

                } else if (j == 0 || j == 7)
                {
                    float rotation = j == 0 ? 0 : -180;
                    Color color = j == 0 ? black : white;

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
        gameObject.AddComponent<Chess>();
        chess = gameObject.GetComponent<Chess>();
    }

    public delegate void EndToMove();

    void MovePiece()
    {
        //pieceSelect.transform.position += new Vector3(0, 5, 0);
        pieceSelect.GetComponent<StatePiece>().Move(cellSelect, () => {
            status = Status.SELECT_PIECE;
            pieceSelect.GetComponent<MeshRenderer>().material.color = pieceSelect.GetComponent<StatePiece>().color;
            cellSelect.GetComponent<MeshRenderer>().material.color = cellSelect.GetComponent<StatePiece>().color;
            cellSelect.GetComponent<StatePiece>().cell = pieceSelect;
            oldCell.GetComponent<StatePiece>().cell = null;
            pieceSelect = null;
            cellSelect = null;
        });
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject temp = hit.collider.gameObject.GetComponent<StatePiece>().cell;

                if (status == Status.SELECT_PIECE || pieceSelect == temp && pieceSelect != null)
                {
                    if (pieceSelect == temp && pieceSelect != null)
                    {
                        status = Status.SELECT_PIECE;
                        pieceSelect.GetComponent<MeshRenderer>().material.color = pieceSelect.GetComponent<StatePiece>().color;
                        
                        if (cellSelect != null)
                            cellSelect.GetComponent<MeshRenderer>().material.color = cellSelect.GetComponent<StatePiece>().color;

                        pieceSelect = null;
                        cellSelect  = null;
                        oldCell     = null;
                        return;
                    }

                    pieceSelect = hit.collider.gameObject.GetComponent<StatePiece>().cell;
                    oldCell = hit.collider.gameObject;

                    if (pieceSelect != null)
                    {
                        status = Status.SELECT_CELL;
                        pieceSelect.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                }
                else if (status == Status.SELECT_CELL)
                {
                    status = Status.MOVE_PIECE;
                    cellSelect  = hit.collider.gameObject;
                    cellSelect.GetComponent<MeshRenderer>().material.color = Color.red;
                    MovePiece();
                } 
            }
        }
    }
}
