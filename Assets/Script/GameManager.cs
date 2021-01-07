using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* GameManager Script
 * 이 스크립트는 게임의 전반적인 관리를 위한 스크립트입니다.
 * 담겨 있는 기능은 enemy의 랜덤소환, 플레이어의 리스폰이 있다.
 */

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyobjs;      //enemy 오브젝트들을 담을 배열 변수
    public Transform[] spawnPoints;     //소환할 위치를 담을 배열 변수

    public float maxSpawnDelay;         //재소환까지 걸리는 시간
    public float curSpawnDelay;         //현재 측정한 재소환까지 시간

    public GameObject player;       //player 오브젝트를 담을 변수
    public Text scoreText;
    public Image[] lifeImage;
    public GameObject gameOverSet;

    //프레임당 한번 돌아가는 함수 Update 선언
    void Update()
    {
        curSpawnDelay += Time.deltaTime;    //시간을 측정하기 위해 curSpawnDelay에 deltaTime을 더해줌

        //만약 현재까지 측장한 시간이 재소환까지 걸리는 시간보다 길면
        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();       //enemy를 소환하는 함수 호출
            maxSpawnDelay = Random.Range(0.5f, 3f);     //재소환까지 걸리는 시간을 0.5초에서 3초 사이의 랜덤 값으로 지정
            curSpawnDelay = 0;      //소환후 현재 측정하던 시간을 초기화
        }

        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    //enemy를 소환하는 함수 SpawnEnemy 선언
    void SpawnEnemy()
    {
        int randomEnemy = Random.Range(0, 3);       //어느 종류의 enemy를 소환할지를 랜덤 값으로 초기화한 변수
        int randomPoint = Random.Range(0, 9);       //어디에 enemy를 소환할지를 랜덤 값으로 초기화한 변수
        //enemy를 (enemy타입, 생성위치, 생성회전)에 따라 소환한다.
        GameObject enemy = Instantiate(enemyobjs[randomEnemy], spawnPoints[randomPoint].position, spawnPoints[randomPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();      //enemy의 Rigidbody2D 컴포넌트를 가져올 rigid 변수 선언
        Enemy enemyLogic = enemy.GetComponent<Enemy>();             //enemy의 스크립트를 가져올 enemyLogic 변수 선언
        enemyLogic.player = player;         //Enemy 스크립트 안의 player를 GameManager에서 가져온 player로 지정해줌

        //랜덤으로 생성하는 위치가 5번혹은 6번이라면(오른쪽)
        if (randomPoint == 5 || randomPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back * 90);      //오른쪽에 맞춰 왼쪽을 바라보도록 회전
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);      //왼쪽 아래로 가도록 속도 조정
        }
        //랜덤으로 생성하는 위치가 7번혹은 8번이라면(왼쪽)
        else if (randomPoint == 7 || randomPoint == 8)
        {
            enemy.transform.Rotate(Vector3.forward * 90);   //왼쪽에 맞춰 오른쪽을 바라보도록 회전
            rigid.velocity = new Vector2(enemyLogic.speed, -1);     //오른쪽 아래로 가도록 속도 조정
        }
        //랜덤으로 생성하는 위치가 1~4번이라면(위쪽)
        else
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));       //아래쪽으로 가도록 속도 조정

    }

    public void UpdateLifeIcon(int life)
    {
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
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

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }
}
