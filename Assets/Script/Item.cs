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

    //초기 변수 초기화를 위한 Awake함수 선언
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();        //Rigidbody2D 컴포넌트로 초기화
        rigid.velocity = Vector2.down * speed;      //아이템의 속도를 아래 방향으로 초기화
    }
}
