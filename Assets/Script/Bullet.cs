using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Bullet Script
 * 이 스크립트는 총알의 제거를 위한 스크립트입니다.
 */

public class Bullet : MonoBehaviour
{
    public int damage;      //총알의 데미지

    //Trigger Enter 충돌처리를 위한 함수 OnTriggerEnter2D 선언(경계 충돌)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")     //경계에 충돌했을 때
        {
            Destroy(gameObject);    //총알을 제거한다.
        }
    }
}
