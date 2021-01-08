using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Item Script
 * 이 스크립트는 아이템의 속도와 타입을 정의하기 위한 스크립트입니다.
 */

public class Item : MonoBehaviour
{
    public string type;         //아이템의 타입
    public float speed;         //아이템의 속도

    Rigidbody2D rigid;          //물리적 이동을 위한 Rigidbody2D 컴포넌트 변수

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rigid.velocity = Vector2.down * speed;
    }

    //Trigger Enter 충돌처리를 위한 함수 OnTriggerEnter2D 선언(경계 충돌)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")     //경계에 충돌했을 때
        {
            gameObject.SetActive(false);    //아이템을 제거한다.
        }
    }
}
