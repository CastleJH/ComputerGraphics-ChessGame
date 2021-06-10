using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pawnPrefab1, pawnPrefab2;
    public GameObject[,] pawns = new GameObject[8, 8];
    public bool isKeyPressAvailable = true;

    public int playingTeam;
    public GameObject selectedObj;
    Pawn selectedPawn;
    public int phase = 0;  //0: selecting, 1: moving

    int prevX, prevY;
    int newX, newY;

    void Start()
    {
        phase = 0;
        InitializeGame();   
    }

    // Update is called once per frame
    void Update()
    {
        checkPhaseChange();
        selectPawn();
        actPawn();
    }

    void selectPawn()
    {
        if (phase == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction * 100, out hit))
                {
                    selectedObj = hit.collider.transform.gameObject;
                    if (selectedObj.tag != "Pawn")
                    {
                        selectedObj = null;
                        selectedPawn = null;
                        return;
                    }
                    selectedPawn = selectedObj.GetComponent<Pawn>();
                    if (selectedPawn.team != playingTeam)
                    {
                        selectedObj = null;
                        selectedPawn = null;
                        return;
                    }
                }
            }
        }
    }

    void checkPhaseChange()
    {
        if (phase == 0 && selectedObj != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction * 100, out hit))
                {
                    if (selectedObj == hit.collider.transform.gameObject)
                    {
                        Debug.Log("seleted.");
                        phase = 1;
                    }
                }
            }
        }
    }

    void actPawn()
    {
        if (phase == 1 && isKeyPressAvailable)
        {
            prevX = selectedPawn.x; prevY = selectedPawn.y;
            if (selectedPawn.leftAP == selectedPawn.AP &&
                Input.GetKeyDown(KeyCode.Backspace)) phase = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectedPawn.x <= 0) return;
                if (pawns[selectedPawn.x, selectedPawn.y] == null) return;
                movePawn(-1, 0);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectedPawn.x >= 7) return;
                if (pawns[selectedPawn.x, selectedPawn.y] == null) return;
                movePawn(1, 0);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedPawn.y >= 7) return;
                if (pawns[selectedPawn.x, selectedPawn.y] == null) return;
                movePawn(0, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedPawn.y <= 0) return;
                if (pawns[selectedPawn.x, selectedPawn.y] == null) return;
                movePawn(0, -1);
            }
        }
    }

    void movePawn(int xdiff, int ydiff)
    {
        newX = selectedPawn.x + xdiff;
        newY = selectedPawn.y + ydiff;
        if (pawns[newX, newY] != null)
        {
            Pawn targetPawn = pawns[newX, newY].GetComponent<Pawn>();
            if (targetPawn.team == selectedPawn.team) return;
            if (targetPawn.HP - selectedPawn.power <= 0)
            {
                selectedPawn.moveTo(selectedPawn.x + xdiff, selectedPawn.y + ydiff);
                pawns[newX, newY] = selectedObj;
                pawns[prevX, prevY] = null;
            }
            selectedPawn.attack(targetPawn);
        }
        else
        {
            selectedPawn.moveTo(selectedPawn.x + xdiff, selectedPawn.y + ydiff);
            pawns[newX, newY] = selectedObj;
            pawns[prevX, prevY] = null;
        }
    }

    void InitializeGame()
    {
        playingTeam = 0;

        for (int i = 0; i< 8; i++)
            for (int j = 0; j < 8; j++)
                pawns[i, j] = null;

        GameObject obj;
        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 0, 0, 4, 0);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 1, 0, 4, 0);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 6, 0, 4, 0);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 7, 0, 4, 0);
        obj.SetActive(true);

        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 3, 1, 3, 0);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 4, 1, 3, 0);
        obj.SetActive(true);

        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 2, 0, 2, 0);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 5, 0, 2, 0);
        obj.SetActive(true);

        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 3, 0, 1, 0);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab1); addFirstTimeToArray(obj, 4, 0, 1, 0);
        obj.SetActive(true);

        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 0, 7, 4, 1);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 1, 7, 4, 1);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 6, 7, 4, 1);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 7, 7, 4, 1);
        obj.SetActive(true);

        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 3, 6, 3, 1);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 4, 6, 3, 1);
        obj.SetActive(true);

        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 2, 7, 2, 1);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 5, 7, 2, 1);
        obj.SetActive(true);

        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 3, 7, 1, 1);
        obj.SetActive(true);
        obj = Instantiate(pawnPrefab2); addFirstTimeToArray(obj, 4, 7, 1, 1);
        obj.SetActive(true);
    }

    void addFirstTimeToArray(GameObject obj, int x, int y, int AP, int team)
    {
        Pawn onePawn = obj.GetComponent<Pawn>();
        onePawn.manager = this;
        if (AP == 1) onePawn.Initialize(team, AP, 5, 5, x, y);
        else onePawn.Initialize(team, AP, 5 - AP, 5, x, y);
        obj.transform.position = new Vector3(x, 0, y);
        pawns[x, y] = obj;
    }
}
