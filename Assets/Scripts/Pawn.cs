using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public GameManager manager;
    public int team, AP, power;
    public float HP;
    public int leftAP;

    bool isMove = false;
    public int x, y;
    Vector3 targetPos;

    public bool isDead = false;

    Rigidbody rigid;
    void Update()
    {
        if (isMove)
        {
            move();
        }
        if (HP <= 0 && !isDead)
        {
            isDead = true;
            gameObject.layer = 11;
            rigid.isKinematic = false;
            rigid.AddForce(new Vector3(-2, 2, 3), ForceMode.Impulse);
            Invoke("EXEKilled", 0.4f);
        }
    }

    public void Initialize(int TEAM, int ActivePoint, int Power, float HealthPoint, int X, int Y)
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        team = TEAM;
        AP = ActivePoint;
        leftAP = AP;
        power = Power;
        HP = HealthPoint;
        x = X;
        y = Y;
        isMove = false;
        isDead = false;
        gameObject.layer = 10;
    }

    public void attack(Pawn pawn)
    {
        manager.isKeyPressAvailable = false;
        pawn.HP -= power;
        if (pawn.HP > 0) Invoke("EXETellManagerTurnOver", 0.5f);
        leftAP = 0;
    }

    void EXETellManagerTurnOver()
    {
        recoverAP();
        manager.isKeyPressAvailable = true;
        manager.phase = 0;
        manager.playingTeam = (manager.playingTeam + 1) % 2;
        string str= "\n";
        for (int j = 7; j >= 0; j--)
        {
            for (int i = 0; i <= 7; i++)
            {
                if (manager.pawns[i, j] != null) str += "O";
                else str += "X";
            }
            str += "\n";
        }
        Debug.Log(str);
    }

    public void recoverAP()
    {
        leftAP = AP;
    }

    void decreaseAP()
    {
        leftAP--;
        if (leftAP <= 0) Invoke("EXETellManagerTurnOver", 0.5f);
        else manager.isKeyPressAvailable = true;
    }

    public void moveTo(int x, int y)
    {
        manager.isKeyPressAvailable = false;
        isMove = true;
        targetPos = new Vector3(x, 0, y);
    }

    void move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.05f * Time.timeScale);
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isMove = false;
            decreaseAP();
            x = Mathf.RoundToInt(targetPos.x);
            y = Mathf.RoundToInt(targetPos.z);
        }
    }

    void EXEKilled()
    {
        Destroy(gameObject);
    }
}
