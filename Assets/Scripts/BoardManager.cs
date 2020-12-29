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

    Ray ray;
    RaycastHit hit;

    private GameObject pieceSelect;
    private GameObject cellSelect;

    private enum Status
    {
        SELECT_PIECE, 
        SELECT_CELL, 
        MOVE_PIECE
    };

    private Status status;

    private void GeneratePiece(GameObject obj, GameObject piece, float height, float rotation)
    {
        GameObject iPiece = Instantiate(piece);
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float z = obj.transform.position.z;

        iPiece.transform.position += new Vector3(x, y + height, z);
        iPiece.transform.rotation  = Quaternion.Euler(0, rotation, 0);
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
                    obj.GetComponent<MeshRenderer>().material.color = Color.black;
                else
                    obj.GetComponent<MeshRenderer>().material.color = Color.white;

                obj.name = char.ConvertFromUtf32(i + 97) + "" + (j + 1);

                if (j == 1 || j == 6)
                {
                    GeneratePiece(obj, pawn, height, 0);

                } else if (j == 0 || j == 7)
                {
                    float rotation = j == 0 ? 0 : -180;

                    if (i == 0 || i == 7)
                        GeneratePiece(obj, rook, height, rotation);
                    if (i == 1 || i == 6)
                        GeneratePiece(obj, horse, height, rotation);
                    if (i == 2 || i == 5)
                        GeneratePiece(obj, sharp, height, rotation);
                    if (i == 3)
                        GeneratePiece(obj, queen, height, rotation);
                    else 
                        GeneratePiece(obj, king, height, rotation);
                }
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                //print(hit.collider.name);
                MeshRenderer render = hit.collider.GetComponent<MeshRenderer>();

                if (render != null && status == Status.SELECT_PIECE)
                {
                    status = Status.SELECT_CELL;
                    pieceSelect = hit.collider.gameObject;
                    render.material.color = Color.red;

                } else if (render == null && status == Status.SELECT_CELL)
                {
                    status = Status.MOVE_PIECE;
                    cellSelect = hit.collider.gameObject;
                    


                }
            }
        }
    }
}
