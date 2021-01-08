using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

/* GameManager Script
 * 이 스크립트는 게임의 전반적인 관리를 위한 스크립트입니다.
 * 담겨 있는 기능은 enemy의 랜덤소환, 플레이어의 리스폰, UI 제어, 게임 오버 및 재시작이 있다.
 */

public class GameManager : MonoBehaviour
{
    public string[] enemyobjs;      //enemy 오브젝트들을 담을 배열 변수
    public Transform[] spawnPoints;     //소환할 위치를 담을 배열 변수

    public float nextSpawnDelay;         //재소환까지 걸리는 시간
    public float curSpawnDelay;         //현재 측정한 재소환까지 시간

    public GameObject player;           //player 오브젝트를 담을 변수
    public Text scoreText;              //점수를 담을 Text UI
    public Image[] lifeImage;           //플레이어의 남은 목숨을 담을 Image UI
    public Image[] boomImage;           //플레이어의 남은 필살기를 담을 Image UI
    public GameObject gameOverSet;      //게임이 오버되었을때 띄울 Text와 Button UI를 담은 빈 오브젝트
    public ObjectManager objectManager;

    Player playerLogic;                 //Player 스크립트를 가져오기 위한 변수

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    //초기 변수 초기화를 위한 Awake함수 선언
    void Awake()
    {
        spawnList = new List<Spawn>();
        playerLogic = player.GetComponent<Player>();         //player 오브젝트 안에 Player 스크립트를 가져와 playerLogic을 초기화
        enemyobjs = new string[] { "EnemyS", "EnemyM", "EnemyL" };
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        TextAsset textFile = Resources.Load("Stage0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        stringReader.Close();

        nextSpawnDelay = spawnList[0].delay;
    }

    //프레임당 한번 돌아가는 함수 Update 선언
    void Update()
    {
        curSpawnDelay += Time.deltaTime;    //시간을 측정하기 위해 curSpawnDelay에 deltaTime을 더해줌

        //만약 현재까지 측장한 시간이 재소환까지 걸리는 시간보다 길면
        if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();       //enemy를 소환하는 함수 호출
            curSpawnDelay = 0;      //소환후 현재 측정하던 시간을 초기화
        }

        scoreText.text = string.Format("{0:n0}", playerLogic.score);    //scoreText UI를 3자리마다 콤마를 찍는 식으로 Player 스크립트의 score 대입
    }

    //enemy를 소환하는 함수 SpawnEnemy 선언
    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;

        //enemy를 (enemy타입, 생성위치, 생성회전)에 따라 소환한다.
        GameObject enemy = objectManager.MakeObj(enemyobjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();      //enemy의 Rigidbody2D 컴포넌트를 가져올 rigid 변수 선언
        Enemy enemyLogic = enemy.GetComponent<Enemy>();             //enemy의 스크립트를 가져올 enemyLogic 변수 선언
        enemyLogic.player = player;         //Enemy 스크립트 안의 player를 GameManager에서 가져온 player로 지정해줌
        enemyLogic.objectManager = objectManager;         //Enemy 스크립트 안의 player를 GameManager에서 가져온 player로 지정해줌

        //랜덤으로 생성하는 위치가 5번혹은 6번이라면(오른쪽)
        if (enemyPoint == 5 || enemyPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back * 90);      //오른쪽에 맞춰 왼쪽을 바라보도록 회전
            rigid.velocity = new Vector2(enemyLogic.enemySpeed * (-1), -1);      //왼쪽 아래로 가도록 속도 조정
        }
        //랜덤으로 생성하는 위치가 7번혹은 8번이라면(왼쪽)
        else if (enemyPoint == 7 || enemyPoint == 8)
        {
            enemy.transform.Rotate(Vector3.forward * 90);   //왼쪽에 맞춰 오른쪽을 바라보도록 회전
            rigid.velocity = new Vector2(enemyLogic.enemySpeed, -1);     //오른쪽 아래로 가도록 속도 조정
        }
        //랜덤으로 생성하는 위치가 1~4번이라면(위쪽)
        else
            rigid.velocity = new Vector2(0, enemyLogic.enemySpeed * (-1));       //아래쪽으로 가도록 속도 조정

        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    /*플레이어의 남은 목숨을 표시해주는 lifeImage를 
    현재 남은 목숨으로 업데이트 해주는 public 함수 UpdateLifeIcon 선언 */
    public void UpdateLifeIcon(int life)
    {
        //우선 모든 UI를 비활성화
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        //남은 목숨 만큼의 UI만 활성화
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    /*플레이어의 남은 필살기의 개수를 표시해주는 boomImage를 
    현재 남은 필살기의 개수로 업데이트 해주는 public 함수 UpdateBoomIcon 선언 */
    public void UpdateBoomIcon(int boom)
    {
        //우선 모든 UI를 비활성화
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);
        }

        //남은 필살기의 개수 만큼의 UI만 활성화
        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    //게임오버 UI 작동 시키는 public 함수 GameOver 선언
    public void GameOver()
    {
        gameOverSet.SetActive(true);        //GameOver UI인 gameOverSet를 활성화
    }

    //게임을 재시작 하는 기능을 하고 재시작 버튼 UI의 OnClick 함수이자 public 함수 GameRetry 선언
    public void GameRetry()
    {
        SceneManager.LoadScene(0);          //Scene을 맨처음으로 되돌린다
    }

    //플레이어를 재소환하는 함수 RespawnPlayer 선언(재소환 함수 호출)
    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);     //2초뒤에 실제로 재소환을 실행하는 함수인 RespawnPlayerExe 호출
    }

    //플레이어를 재소환하는 함수 RespawnPlayerExe 선언(실제 재소환 실행)
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;        //플레이어의 시작 위치 지정
        player.SetActive(true);         //플레이어를 다시 활성화

        playerLogic.isHit = false;      //플레이어가 맞고 있는 중이 아닌 것을 Player 스크립트의 isHit 변수로 넘긴다.
    }
}
