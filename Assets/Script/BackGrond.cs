using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Item Script
 * 이 스크립트는 무한 배경을 위한 스크립트입니다.
 */

public class BackGrond : MonoBehaviour
{
    public float speed;                 //배경이 내려가는 속도(멀수록 느려서 원근감이 느껴지는 페럴렉스 기법 사용)
    public int startIndex;              //배경이 시작하는 Index
    public int endIndex;                //배경이 끝나는 Index
    public Transform[] sprites;         //배경들의 Transform 변수

    float ViewHeight;                   //카메라의 길이를 받아오는 변수


    //초기 변수 초기화를 위한 Awake함수 선언
    void Awake()
    {
        ViewHeight = Camera.main.orthographicSize * 2;      //카메라 수직 사이즈에 2를 곱해 길이를 구해서 초기화
    }

    //프레임당 한번 돌아가는 함수 Update 선언
    void Update()
    {
        Move();             //배경을 움직이게하는 함수
        Scrolling();        //배경이 무한히 반복되도록 하는 함수
    }

    //배경을 움직이게하는 함수 Move 선언
    void Move()
    {
        Vector3 curPos = transform.position;        //배경의 현재 위치
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;        //배경이 움직일 위치
        transform.position = curPos + nextPos;      //배경의 위치를 현재 위치 + 움직일 위치로 초기화
    }

    //배경이 무한히 반복되도록 하는 함수 Scrolling 선언
    void Scrolling()
    {
        //만약 배경이 카메라 밖을 벗어났다면
        if (sprites[endIndex].position.y < ViewHeight * (-1))
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;      //시작 지점의 배경의 로컬 위치를 받아오는 변수 선언
            //끝 지점의 로컬 위치를 시작 지점의 로컬 위치에 카메라 길이만큼 위로 바꿔준다.
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * ViewHeight;

            //startIndex와 endIndex를 교체해준다.
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;   //예외 처리와 함께 endIndex에 startIndex - 1을 대입
        }
    }
}
