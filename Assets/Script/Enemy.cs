﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Enemy Script
 * 이 스크립트는 적의 이동과 오브젝트와의 상호작용을 담은 스크립트입니다.
 * 담겨 있는 기능은 자동 이동, 총알 발사, 피격 이벤트, 경계가 있다.
 */

public class Enemy : MonoBehaviour
{
    public string enemyName;    //적 비행기의 이름
    public float speed;         //적 비행기의 속도
    public int health;          //적 비행기의 체력
    public int enemyScore;

    public float maxShotDelay;      //총알의 재장전 속도
    public float curShotDelay;      //현재 총알의 재장전 시간
    public float bulletSpeed;       //총알의 속도

    public GameObject bulletObjA;       //A타입 총알
    public GameObject bulletObjB;       //B타입 총알
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject player;           //플레이어 호출 변수

    public Sprite[] sprites;            //피격시 바꿀 스프라이트 변수
    SpriteRenderer spriteRenderer;      //스프라이트를 바꾸기 위한 컴포넌트 변수

    //초기 변수 초기화를 위한 Awake함수 선언
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    //spriteRenderer변수 초기화
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

        //만약 enemy가 작은 것이라면
        if(enemyName == "S")
        {
            //A타입의 총알을 3의 속도로 하나만 발사
            Shot(bulletObjA, 0f, 3f);
        }
        //만약 enemy가 큰 것이라면
        else if (enemyName == "L")
        {
            //B타입의 총알을 4의 속도로 두개 발사
            Shot(bulletObjB, 0.3f, 4f);
            Shot(bulletObjB, -0.3f, 4f);
        }

        curShotDelay = 0;       //발사후 현재 재장전 시간을 초기화
    }

    //총알을 발사하는 함수 Shot 선언 (bulletDistance가 양수면 오른쪽 음수면 왼쪽을 뜻함)
    void Shot(GameObject bulletType, float bulletDistance, float bulletSpeed)
    {
        //총알을 (총알타입, 생성위치, 생성회전)에 따라 생성한다.
        GameObject bullet = Instantiate(bulletType, transform.position + Vector3.right * bulletDistance, transform.rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();     //총알의 Rigidbody2D 컴포넌트를 가져와서 rigid 컴포넌트 변수로 선언

        //적에게서 플레이어를 가르키는 방향으로의 벡터 선언
        Vector3 dirVec = player.transform.position - (transform.position + Vector3.right * bulletDistance);
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);      //총알을 플레이어를 가르키는 방향으로 발사 (이때, 단위 벡터로의 변환이 필요)
    }

    //총알 재장전 관련 함수 Reload 선언
    void Reload()
    {
        curShotDelay += Time.deltaTime;     //현재 재장전 시간을 재기위해 curShotDelay에 프레임당 시간을 더해준다.
    }

    //적 비행기의 피격 이벤트 함수 선언
    public void OnHit(int damage)
    {
        if (health <= 0)
            return;

        health -= damage;   //체력이 데미지만큼 줄어든다.
        spriteRenderer.sprite = sprites[1];     //sprite가 피격 sprite로 변경됨
        Invoke("ReturnSprite", 0.1f);           //0.1초뒤에 원래대로의 sprite로 돌아옴
        
        //만약 체력이 0보다 작거나 같으면 오브젝트 파괴
        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            int ran = Random.Range(0, 10);
            if (ran < 5)
                Debug.Log("Not Item");
            else if (ran < 8)
                Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);
            else if (ran < 9)
                Instantiate(itemPower, transform.position, itemPower.transform.rotation);
            else if (ran < 10)
                Instantiate(itemBoom, transform.position, itemBoom.transform.rotation);

            Destroy(gameObject);
        }
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
        if (collision.gameObject.tag == "BorderBullet")
            Destroy(gameObject);
        //만약 충돌한 물체가 플레이어의 총알이라면
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();    //Bullet 스크립트를 호출할 변수 bullet 선언
            OnHit(bullet.damage);       //피격함수에 bullet의 damage를 인수로 넣음

            Destroy(bullet.gameObject);     //오브젝트에게 맞은 총알을 파괴
        }
    }
}
