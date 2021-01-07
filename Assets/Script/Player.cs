using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Player Script
 * 이 스크립트는 플레이어의 이동과 오브젝트와의 상호작용을 담은 스크립트입니다.
 * 담겨 있는 기능은 이동, 총알 발사, 피격 이벤트, 경계가 있다.
 */

public class Player : MonoBehaviour
{
    //플레이어 관련 변수
    public float speed;     //플레이어의 속도
    public int life;
    public int score;

    //총알 관련 변수
    public float power;     //총알의 파워
    public float bulletSpeed;       //총알의 속도
    public float maxShotDelay;      //총알의 재장전 속도
    public float curShotDelay;      //현재 총알의 재장전 시간

    public GameObject bulletObjA;       //A타입 총알
    public GameObject bulletObjB;       //B타입 총알

    //플레이어 경계 관련 변수
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public bool isHit;

    public GameManager manager;     //GameManager 스크립트를 불러오는 변수
    Animator anim;      //Animator 컴포넌트 변수

    //초기 변수 초기화를 위한 Awake함수 선언
    void Awake()
    {
        anim = GetComponent<Animator>();    //Animator 컴포넌트 변수 초기화
    }

    //프레임당 한번 돌아가는 함수 Update 선언
    void Update()
    {
        Move();     //플레이어 이동 관련 함수
        Fire();     //총알 발사를 관리하는 함수
        Reload();   //총알 재장전 관련 함수
    }

    //플레이어 이동 관련 함수 Move 선언
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");   //좌우 입력을 받는 변수 h를 GetAxisRaw로 초기화
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))   //경계의 부딪힌 상태에서 그 방향으로 움직이려 하면
            h = 0;  //입력 변수 h를 0으로 초기화

        float v = Input.GetAxisRaw("Vertical");     //상하 입력을 받는 변수 v를 GetAxisRaw로 초기화
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))   //경계의 부딪힌 상태에서 그 방향으로 움직이려 하면
            v = 0;  //입력 변수 v를 0으로 초기화

        Vector3 curPos = transform.position;    //플레이어의 현재위치를 curPos로 초기화
        Vector3 nextPos = new Vector3(h, v, transform.position.z) * speed * Time.deltaTime;     //플레이어의 다음위치를 nextPos로 초기화(입력변수 * 속도 * 프레임)

        transform.position = curPos + nextPos;      //플레이어의 위치를 현재위치 + 다음위치로 초기화

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))   //버튼이 눌리거나 떼어질 때
            anim.SetInteger("Input", (int)h);   //Animator의 int형 parameter인 Input을 h로 초기화
    }

    //총알 발사를 관리하는 함수 Fire 선언
    void Fire()
    {
        //만약 Fire1버튼이 눌리지 않는다면 함수를 나간다.
        if (!Input.GetButton("Fire1"))
            return;

        //현재 재장전 시간이 최대 재장전 시간보다 낮다면 함수를 나간다.
        if (curShotDelay < maxShotDelay)
            return;

        //power의 값에 따라 다른 총알을 발사
        switch (power)
        {
            case 1:     //A타입 총알을 하나만 발사
                Shot(bulletObjA, 0f);
                break;
            case 2:     //A타입 총알 두개를 거리를 두고 발사
                Shot(bulletObjA, 0.1f);
                Shot(bulletObjA, -0.1f);
                break;
            case 3:     //B타입 총알 한개와 양쪽에 A타입 총알 두개를 거리 두고 발사
                Shot(bulletObjA, 0.35f);
                Shot(bulletObjB, 0f);
                Shot(bulletObjA, -0.35f);
                break;
        }

        curShotDelay = 0;       //발사후 현재 재장전 시간을 초기화
    }

    //총알을 발사하는 함수 Shot 선언 (bulletDistance가 양수면 오른쪽 음수면 왼쪽을 뜻함)
    void Shot(GameObject bulletType, float bulletDistance)
    {
        //총알을 (총알타입, 생성위치, 생성회전)에 따라 생성한다.
        GameObject bullet = Instantiate(bulletType, transform.position + Vector3.right * bulletDistance, transform.rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();     //총알의 Rigidbody2D 컴포넌트를 가져와서 rigid 컴포넌트 변수로 선언
        rigid.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);      //총알을 위 방향으로 발사
    }

    //총알 재장전 관련 함수 Reload 선언
    void Reload()
    {
        curShotDelay += Time.deltaTime;     //현재 재장전 시간을 재기위해 curShotDelay에 프레임당 시간을 더해준다.
    }

    //Trigger Enter 충돌처리를 위한 함수 OnTriggerEnter2D 선언(경계 충돌, 적 충돌)
    //Enter함수는 충돌했을 때 그 순간을 감지한다.
    void OnTriggerEnter2D(Collider2D collision)
    {
        //만약 충돌한 물체가 경계이면
        if(collision.gameObject.tag == "Border")
        {
            //충돌한 물체의 이름이 무엇인가
            switch (collision.gameObject.name)
            {
                case "Top":     //위의 경계에 닿은 경우
                    isTouchTop = true;
                    break;
                case "Bottom":  //아래의 경계에 닿은 경우
                    isTouchBottom = true;
                    break;
                case "Right":   //오른쪽의 경계에 닿은 경우
                    isTouchRight = true;
                    break;
                case "Left":    //왼쪽의 경계에 닿은 경우
                    isTouchLeft = true;
                    break;
            }
        }
        //만약 충돌한 물체가 적 또는 적의 총알이면
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit)
                return;

            isHit = true;
            life--;
            manager.UpdateLifeIcon(life);

            if(life == 0)
                manager.GameOver();
            else
                manager.RespawnPlayer();        //GameManager안에 있는 RespawnPlayer함수 호출

            gameObject.SetActive(false);    //player를 비활성화 상태로 만듬
            Destroy(collision.gameObject);
        }
    }

    //Trigger Exit 충돌처리를 위한 함수 OnTriggerExit2D 선언(경계 충돌)
    //Exit함수는 충돌이 끝났을 때 그 순간을 감지한다.
    void OnTriggerExit2D(Collider2D collision)
    {
        //충돌이 끝난 물체가 경계이면
        if (collision.gameObject.tag == "Border")
        {
            //충돌이 끝난 물체의 이름이 무엇인가
            switch (collision.gameObject.name)
            {
                case "Top":     //위의 경계에 닿은 경우
                    isTouchTop = false;
                    break;
                case "Bottom":  //아래의 경계에 닿은 경우
                    isTouchBottom = false;
                    break;
                case "Right":   //오른쪽의 경계에 닿은 경우
                    isTouchRight = false;
                    break;
                case "Left":    //왼쪽의 경계에 닿은 경우
                    isTouchLeft = false;
                    break;
            }
        }
    }
}