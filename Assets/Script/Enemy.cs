using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Enemy Script
 * 이 스크립트는 적의 이동과 오브젝트와의 상호작용을 담은 스크립트입니다.
 * 담겨 있는 기능은 자동 이동, 총알 발사, 피격 이벤트, 경계가 있다.
 */

public class Enemy : MonoBehaviour
{
    public string enemyName;    //적 비행기의 이름
    public float enemySpeed;    //적 비행기의 속도
    public int health;          //적 비행기의 체력
    public int enemyScore;      //적 비행기의 점수

    public float maxShotDelay;      //총알의 재장전 속도
    public float curShotDelay;      //현재 총알의 재장전 시간

    public GameObject bulletObjA;       //A타입 총알
    public GameObject bulletObjB;       //B타입 총알
    public GameObject itemCoin;         //Coin 아이템
    public GameObject itemPower;        //Power 아이템
    public GameObject itemBoom;         //Boom 아이템
    public GameObject player;           //플레이어 호출 변수
    public ObjectManager objectManager;

    public Sprite[] sprites;            //피격시 바꿀 스프라이트 변수
    SpriteRenderer spriteRenderer;      //스프라이트를 바꾸기 위한 컴포넌트 변수
    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    int index;
    int roundNum;

    //초기 변수 초기화를 위한 Awake함수 선언
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    //spriteRenderer변수 초기화

        if (enemyName == "B")
            anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        switch (enemyName)
        {
            case "B":
                health = 3000;
                Invoke("Stop", 2f);
                break;
            case "L":
                health = 40;
                break;
            case "M":
                health = 10;
                break;
            case "S":
                health = 3;
                break;
        }
    }

    void Stop()
    {
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        Shot("BulletBossA", 0.3f, 8f);
        Shot("BulletBossA", 0.45f, 8f);
        Shot("BulletBossA", -0.3f, 8f);
        Shot("BulletBossA", -0.45f, 8f);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFoward", 2);
        else
            Invoke("Think", 3);
    }

    void FireShot()
    {
        for (index = 0; index < 5; index++)
            Shot("BulletEnemyB", 0f, 3f);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3);
    }

    void FireArc()
    {
        Shot("BulletEnemyA", 0f, 5f);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 3);
    }

    void FireAround()
    {
        int roundNumA = 50;
        int roundNumB = 40;
        roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

        for (index = 0; index < roundNumA; index++)
            Shot("BulletBossB", 0f, 2f);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 0.7f);
        else
            Invoke("Think", 3);
    }

    //프레임당 한번 돌아가는 함수 Update 선언
    void Update()
    {
        if (enemyName == "B")
            return;

        Fire();     //총알 발사를 관리하는 함수
        Reload();   //총알 재장전 관련 함수
    }

    //총알 발사를 관리하는 함수 Fire 선언
    void Fire()
    {

        //현재 재장전 시간이 최대 재장전 시간보다 낮다면 함수를 나간다.
        if (curShotDelay < maxShotDelay)
            return;

        //만약 enemy가 작은 것이라면
        if(enemyName == "S")
        {
            //A타입의 총알을 3의 속도로 하나만 발사
            Shot("BulletEnemyA", 0f, 3f);
        }
        //만약 enemy가 큰 것이라면
        else if (enemyName == "L")
        {
            //B타입의 총알을 4의 속도로 두개 발사
            Shot("BulletEnemyB", 0.3f, 4f);
            Shot("BulletEnemyB", -0.3f, 4f);
        }

        curShotDelay = 0;       //발사후 현재 재장전 시간을 초기화
    }

    //총알을 발사하는 함수 Shot 선언 (bulletDistance가 양수면 오른쪽 음수면 왼쪽을 뜻함)
    void Shot(string bulletType, float bulletDistance, float bulletSpeed)
    {
        GameObject bullet = objectManager.MakeObj(bulletType);
        bullet.transform.position = transform.position + Vector3.right * bulletDistance;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();     //총알의 Rigidbody2D 컴포넌트를 가져와서 rigid 컴포넌트 변수로 선언

        if(enemyName == "B")
        {
            switch (patternIndex)
            {
                case 0:     //FireFoward
                    rigid.AddForce(Vector2.down * bulletSpeed, ForceMode2D.Impulse);
                    break;
                case 1:     //FireShot
                    //적에게서 플레이어를 가르키는 방향으로의 벡터 선언
                    Vector2 dirVec1 = player.transform.position - transform.position;
                    Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
                    dirVec1 += ranVec;
                    rigid.AddForce(dirVec1.normalized * bulletSpeed, ForceMode2D.Impulse);     //총알을 플레이어를 가르키는 방향으로 발사 (이때, 단위 벡터로의 변환이 필요)
                    break;
                case 2:     //FireArc
                    bullet.transform.rotation = Quaternion.identity;
                    Vector2 dirVec2 = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]), -1);
                    rigid.AddForce(dirVec2.normalized * bulletSpeed, ForceMode2D.Impulse);     //총알을 플레이어를 가르키는 방향으로 발사 (이때, 단위 벡터로의 변환이 필요)
                    break;
                case 3:
                    bullet.transform.rotation = Quaternion.identity;
                    Vector2 dirVec3 = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));
                    rigid.AddForce(dirVec3.normalized * bulletSpeed, ForceMode2D.Impulse);     //총알을 플레이어를 가르키는 방향으로 발사 (이때, 단위 벡터로의 변환이 필요)

                    Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
                    bullet.transform.Rotate(rotVec);
                    break;
            }
        }
        else
        {
            //적에게서 플레이어를 가르키는 방향으로의 벡터 선언
            Vector3 dirVec = player.transform.position - (transform.position + Vector3.right * bulletDistance);
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);      //총알을 플레이어를 가르키는 방향으로 발사 (이때, 단위 벡터로의 변환이 필요)
        }
    }

    //총알 재장전 관련 함수 Reload 선언
    void Reload()
    {
        curShotDelay += Time.deltaTime;     //현재 재장전 시간을 재기위해 curShotDelay에 프레임당 시간을 더해준다.
    }

    //적 비행기의 피격 이벤트 함수 선언
    public void OnHit(int damage)
    {
        //만약 체력이 0 이하라면 이미 죽은 것이므로 함수를 나간다.
        if (health <= 0)
            return;

        health -= damage;   //체력이 데미지만큼 줄어든다.

        if(enemyName == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];     //sprite가 피격 sprite로 변경됨
            Invoke("ReturnSprite", 0.1f);           //0.1초뒤에 원래대로의 sprite로 돌아옴
        }
        
        //만약 체력이 0보다 작거나 같으면 오브젝트 파괴
        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();     //Player 스크립트를 가져오기 위한 playerLogic변수 선언
            playerLogic.score += enemyScore;        //적 비행기의 타입별 점수를 Player의 점수에 더함

            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);      //아이템의 확률적 드랍을 위한 랜덤 int 변수 선언
            if (ran < 5)        //아이템이 나오지 않을 확률 50%
                Debug.Log("Not Item");
            else if (ran < 8)//Coin이 나올 확률 30%
                MakeItem("ItemCoin");
            else if (ran < 9)       //Power가 나올 확률 10%
                MakeItem("ItemPower");
            else if (ran < 10)      //Boom이 나올 확률 10%
                MakeItem("ItemBoom");

            gameObject.SetActive(false);        //적 비행기 오브젝트를 파괴
            transform.rotation = Quaternion.identity;
        }
    }

    void MakeItem(string itemType)
    {
        GameObject itemObj = objectManager.MakeObj(itemType);
        itemObj.transform.position = transform.position;
    }

    //원래의 sprite로 돌아오게 하는 함수 ReturnSprite 선언
    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];     //sprite가 원래 sprite로 변경됨
    }

    //Trigger Enter 충돌처리를 위한 함수 OnTriggerEnter2D 선언(경계 충돌, 총알 충돌)
    //Enter함수는 충돌했을 때 그 순간을 감지한다.
    void OnTriggerEnter2D(Collider2D collision)
    {
        //만약 충돌한 물체가 경계라면 오브젝트 파괴
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        //만약 충돌한 물체가 플레이어의 총알이라면
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();    //Bullet 스크립트를 호출할 변수 bullet 선언
            OnHit(bullet.damage);       //피격함수에 bullet의 damage를 인수로 넣음

            bullet.gameObject.SetActive(false);     //오브젝트에게 맞은 총알을 파괴
        }
    }
}
