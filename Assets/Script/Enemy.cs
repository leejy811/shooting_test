using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;

    public float maxShotDelay;      //총알의 재장전 속도
    public float curShotDelay;      //현재 총알의 재장전 시간
    public float bulletSpeed;       //총알의 속도

    public GameObject bulletObjA;       //A타입 총알
    public GameObject bulletObjB;       //B타입 총알
    public GameObject player;

    public Sprite[] sprites;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //프레임당 한번 돌아가는 함수 Update 선언
    void Update()
    {
        Fire();     //총알 발사를 관리하는 함수
        Reload();   //총알 재장전 관련 함수
    }

    //총알 발사를 관리하는 함수 Fire 선언
    void Fire()
    {

        //현재 재장전 시간이 최대 재장전 시간보다 낮다면 함수를 나간다.
        if (curShotDelay < maxShotDelay)
            return;

        if(enemyName == "S")
        {
            Shot(bulletObjA, 0f, 3f);
        }
        else if (enemyName == "L")
        {
            Shot(bulletObjB, 0.3f, 4f);
            Shot(bulletObjB, -0.3f, 4f);
        }

        curShotDelay = 0;
    }

    //총알을 발사하는 함수 Shot 선언 (bulletDistance가 양수면 오른쪽 음수면 왼쪽을 뜻함)
    void Shot(GameObject bulletType, float bulletDistance, float bulletSpeed)
    {
        //총알을 (총알타입, 생성위치, 생성회전)에 따라 생성한다.
        GameObject bullet = Instantiate(bulletType, transform.position + Vector3.right * bulletDistance, transform.rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();     //총알의 Rigidbody2D 컴포넌트를 가져와서 rigid 컴포넌트 변수로 선언
        Vector3 dirVec = player.transform.position - (transform.position + Vector3.right * bulletDistance);
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);      //총알을 위 방향으로 발사
    }

    //총알 재장전 관련 함수 Reload 선언
    void Reload()
    {
        curShotDelay += Time.deltaTime;     //현재 재장전 시간을 재기위해 curShotDelay에 프레임당 시간을 더해준다.
    }

    void OnHit(int damage)
    {
        health -= damage;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
            Destroy(gameObject);
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.damage);

            Destroy(bullet.gameObject);
        }
    }
}
