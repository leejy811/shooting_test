using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float bulletSpeed;
    public float maxShotDelay;      //총알의 재장전 속도
    public float curShotDelay;      //현재 총알의 재장전 시간
    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }

    //프레임당 한번 돌아가는 함수 Update 선언
    void Update()
    {
        Watch();
        Follow();
        Fire();     //총알 발사를 관리하는 함수
        Reload();   //총알 재장전 관련 함수
    }

    void Watch()
    {
        if (!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
    }

    void Follow()
    {
        transform.position = followPos;
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

        Shot("BulletFollower", 0f);

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
}
