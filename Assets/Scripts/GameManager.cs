using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pawnPrefab1, pawnPrefab2, collisionPrefab;
    public GameObject[,] pawns = new GameObject[8, 8];
    public bool isKeyPressAvailable = true;

    public int playingTeam;
    public GameObject selectedObj;
    Pawn selectedPawn;
    public int phase = 0;  //0: selecting, 1: moving

    int prevX, prevY;
    int newX, newY;
    bool isRotatingCamera = false;

    Vector3 defaultCamPos, mapCenter;
    Quaternion defaultCamRot;
    float defaultCamFOV;

    public GameObject UIRedTurn, UIBlueTurn;
    public GameObject ClickedParticle, SelectedParticle;

    public GameObject[] lightSource;
    void Start()
    {
        phase = 0;
        InitializeGame();
        defaultCamPos = Camera.main.transform.position;
        defaultCamRot = Camera.main.transform.rotation;
        defaultCamFOV = Camera.main.fieldOfView;
        mapCenter = new Vector3(3.5f, 0, 3.5f);
        ClickedParticle.GetComponent<ParticleSystem>().Play();
        SelectedParticle.GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        checkPhaseChange();
        selectPawn();
        actPawn();
        if (phase == 1)
        {
            SelectedParticle.transform.position = selectedObj.transform.position + Vector3.up * 0.5f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (selectedPawn != null) selectedPawn.recoverAP();
                TurnOver();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (pawns[i, j] != null) Destroy(pawns[i, j]);
            InitializeGame();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Camera.main.transform.position = defaultCamPos;
            Camera.main.transform.rotation = defaultCamRot;
        }
    }

    Vector2 prevClickPos, curClickPos;
    void LateUpdate()
    {
        if (!isRotatingCamera) return;
        if (Input.GetMouseButtonDown(0))
        {
            prevClickPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            curClickPos = Input.mousePosition;
            Camera.main.transform.RotateAround(mapCenter, Vector3.up, (prevClickPos.x - curClickPos.x) / 10);
            prevClickPos = curClickPos;
        }
    }

    public void TurnOver()
    {
        isKeyPressAvailable = true;
        phase = 0;
        playingTeam = (playingTeam + 1) % 2;
        UIControl();
        for (int i = 0; i < lightSource.Length; i++) lightSource[i].SetActive(true);
    }

    void selectPawn()
    {
        if (phase == 0)
        {
            SelectedParticle.SetActive(false);
            if (Input.GetMouseButtonDown(0))
            {
                isRotatingCamera = true;
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
                    ClickedParticle.transform.position = selectedObj.transform.position + Vector3.up * 0.5f;
                    isRotatingCamera = false;
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
                        SelectedParticle.transform.position = selectedObj.transform.position;
                        SelectedParticle.SetActive(true);
                        SelectedParticle.GetComponent<ParticleSystem>().Play();
                        ClickedParticle.transform.position = new Vector3(-10, -10, -10);
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
            Vector3 collPos = (selectedPawn.gameObject.transform.position + targetPawn.gameObject.transform.position) / 2;
            for (int i = 0; i < lightSource.Length; i++) lightSource[i].SetActive(false);
            GameObject particle = Instantiate(collisionPrefab);
            particle.transform.position = collPos + Vector3.up * 0.5f;
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            selectedPawn.moveTo(selectedPawn.x + xdiff, selectedPawn.y + ydiff);
            pawns[newX, newY] = selectedObj;
            pawns[prevX, prevY] = null;
        }
    }

    public void UIControl()
    {
        switch (playingTeam)
        {
            case 0:
                UIRedTurn.SetActive(true);
                UIBlueTurn.SetActive(false);
                break;
            case 1:
                UIRedTurn.SetActive(false);
                UIBlueTurn.SetActive(true);
                break;
        }
    }

    void InitializeGame()
    {
        for (int i = 0; i < lightSource.Length; i++) lightSource[i].SetActive(true);
        ClickedParticle.transform.position = new Vector3(-10, -10, -10);
        SelectedParticle.transform.position = new Vector3(-10, -10, -10);
        playingTeam = 0;
        UIControl();
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
