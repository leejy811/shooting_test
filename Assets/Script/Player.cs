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
    public int life;        //플레이어의 목숨
    public int score;       //플레이어의 점수

    //총알 관련 변수
    public int power;       //총알의 파워
    public int maxPower;    //총알의 최대 파워
    public int boom;        //현재 갖고 있는 필살기 개수
    public int maxBoom;     //최대 필살기 개수
    public float bulletSpeed;       //총알의 속도
    public float maxShotDelay;      //총알의 재장전 속도
    public float curShotDelay;      //현재 총알의 재장전 시간

    public GameObject boomEffect;       //필살기 오브젝트

    //플레이어 경계 관련 변수
    public bool isTouchTop;         //위쪽
    public bool isTouchBottom;      //아래쪽
    public bool isTouchRight;       //오른쪽
    public bool isTouchLeft;        //왼쪽

    public bool isHit;          //플레이어가 총알에 맞았는지 판단하는 변수
    public bool isBoomTime;     //필살기가 사용중인지 판단하는 변수

    public GameManager gameManager;     //GameManager 스크립트를 불러오는 변수
    public ObjectManager objectManager;
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
        Boom();     //필살기 발동 함수
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
                Shot("BulletPlayerA", 0f);
                break;
            case 2:     //A타입 총알 두개를 거리를 두고 발사
                Shot("BulletPlayerA", 0.1f);
                Shot("BulletPlayerA", -0.1f);
                break;
            case 3:     //B타입 총알 한개와 양쪽에 A타입 총알 두개를 거리 두고 발사
                Shot("BulletPlayerA", 0.35f);
                Shot("BulletPlayerB", 0f);
                Shot("BulletPlayerA", -0.35f);
                break;
        }

        curShotDelay = 0;       //발사후 현재 재장전 시간을 초기화
    }

    //총알을 발사하는 함수 Shot 선언 (bulletDistance가 양수면 오른쪽 음수면 왼쪽을 뜻함)
    void Shot(string bulletType, float bulletDistance)
    {
        //총알을 (총알타입, 생성위치, 생성회전)에 따라 생성한다.
        GameObject bullet = objectManager.MakeObj(bulletType);
        bullet.transform.position = transform.position + Vector3.right * bulletDistance;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();     //총알의 Rigidbody2D 컴포넌트를 가져와서 rigid 컴포넌트 변수로 선언
        rigid.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);      //총알을 위 방향으로 발사
    }

    //총알 재장전 관련 함수 Reload 선언
    void Reload()
    {
        curShotDelay += Time.deltaTime;     //현재 재장전 시간을 재기위해 curShotDelay에 프레임당 시간을 더해준다.
    }

    //필살기를 발동하는 함수 Boom 선언
    void Boom()
    {
        //만약 Fire2버튼이 눌리지 않는다면 함수를 나간다.
        if (!Input.GetButton("Fire2"))
            return;

        //만약 필살기가 발동 중이라면 함수를 나간다.
        if (isBoomTime)
            return;

        //필살기를 모두 사용했다면 함수를 나간다.
        if (boom == 0)
            return;

        boom--;                             //필살기 개수 차감
        isBoomTime = true;                  //필살기를 사용중
        gameManager.UpdateBoomIcon(boom);       //필살기 UI 업데이트 

        boomEffect.SetActive(true);         //필살기 오브젝트를 활성화
        Invoke("OffBoomEffect", 4f);        //4초 뒤에 필살기 오브젝트를 비활성화

        DestoryEnemies("EnemyL");
        DestoryEnemies("EnemyM");
        DestoryEnemies("EnemyS");

        DestoryEnemies("BulletEnemyA");
        DestoryEnemies("BulletEnemyB");
    }

    void DestoryEnemies(string enemyType)
    {
        if (objectManager.GetPool(enemyType) == null)
            return;

        GameObject[] enemyObj = objectManager.GetPool(enemyType);

        for (int index = 0; index < enemyObj.Length; index++)
        {
            if (enemyObj[index].activeSelf)
            {
                if (enemyType == "EnemyL" || enemyType == "EnemyL" || enemyType == "EnemyL")
                {
                    Enemy enemyLogic = enemyObj[index].GetComponent<Enemy>();        //Enemy 스크립트를 가져와 enemyLogic에 초기화
                    enemyLogic.OnHit(1000);     //모든 적들에게 1000의 데미지를 가함(모두 파괴)
                }
                else
                    enemyObj[index].SetActive(false);
            }
        }
    }

    //Trigger Enter 충돌처리를 위한 함수 OnTriggerEnter2D 선언(경계 충돌, 적 충돌, 아이템 충돌)
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
            //만약 이미 충돌했다면 충돌 함수를 나간다.
            if (isHit)
                return;

            isHit = true;       //지금은 충돌한 상태이다.
            life--;             //플레이어의 목숨 차감
            gameManager.UpdateLifeIcon(life);       //플레이어 목숨 UI 업데이트

            //만약 목숨이 남지 않았다면
            if(life == 0)
                gameManager.GameOver();     //GameManager 스크립트의 GameOver 함수 호출
            else
                gameManager.RespawnPlayer();        //GameManager안에 있는 RespawnPlayer 함수 호출

            gameObject.SetActive(false);    //player를 비활성화 상태로 만듬
            collision.gameObject.SetActive(false);      //충돌한 적 또는 적의 총알 파괴
        }
        //만약 충돌한 물체가 아이템이면
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();      //Item 스크립트를 사용하기위한 item 변수 선언
            //Item의 type이 무엇인가
            switch (item.type)
            {
                case "Coin":        //Item의 type이 Coin이면 점수를 1000점 더한다.
                    score += 1000;
                    break;
                case "Power":       //Item의 type이 Power일때 최대 Power개수 보다 적으면 하나를 더하고 크면 점수를 500점 더한다.
                    if (power == maxPower)
                        score += 500;
                    else
                        power++;
                    break;
                case "Boom":        //Item의 type이 Boom일때 최대 Boom개수 보다 적으면 하나를 더하고 크면 점수를 500점 더한다.
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);       //필살기 UI 업데이트
                    }
                    break;
            }

            collision.gameObject.SetActive(false);      //충돌한 아이템 파괴
        }
    }

    //필살기의 효과를 종료하는 함수 OffBoomEffect 선언
    void OffBoomEffect()
    {
        boomEffect.SetActive(false);        //필살기의 효과를 끈다.
        isBoomTime = false;                 //필살기가 사용 중이 아니다.
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