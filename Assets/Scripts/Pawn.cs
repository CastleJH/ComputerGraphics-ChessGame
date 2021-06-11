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
    public TextMesh powerText, APText, HPText;

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
        powerText.text = power.ToString();
        APText.text = AP.ToString();
        HPText.text = "¡Ü¡Ü¡Ü¡Ü¡Ü";
    }

    public void attack(Pawn pawn)
    {
        manager.isKeyPressAvailable = false;
        pawn.HP -= power;
        string tmpstr = "";
        for (int i = 0; i < pawn.HP; i++) tmpstr += "¡Ü";
        for (int i = 0; i < 5 - pawn.HP; i++) tmpstr += "¡Û";
        pawn.HPText.text = tmpstr;
        if (pawn.HP > 0) Invoke("EXETellManagerTurnOver", 0.5f);
        leftAP = 0;
        APText.text = leftAP.ToString();
    }

    void EXETellManagerTurnOver()
    {
        recoverAP();
        manager.TurnOver();
    }

    public void recoverAP()
    {
        leftAP = AP;
        APText.text = leftAP.ToString();
    }

    void decreaseAP()
    {
        leftAP--;
        APText.text = leftAP.ToString();
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
