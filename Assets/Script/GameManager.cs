using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> planets = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image nextPlanetImage;
    [SerializeField] private GameObject gameOverPanel;

    private GameObject nextObject;
    private GameObject currentObject;
    private float timeCount = 0;
    private int score = 0;

    public static GameManager instance;

    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;
    public int Score => score;
    public List<GameObject> Planets => planets;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartSetting();
        SelectNextObject(); // 다음 행성 미리 뽑기
        SpawnNextObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
            return;

        // 시간 누적
        timeCount += Time.deltaTime;

        UpdateCurrentObjectPosition();
        
        if(Mouse.current.leftButton.wasPressedThisFrame && timeCount >= 0.5f) {
            // 누적 시간 초기화
            timeCount = 0;

            ActivateCurrentObject();
            SpawnNextObject();
        }
    }

    public void ReportCollision(GameObject thisObject, GameObject collisionObject) {
        // 태그가 없거나 충돌 객체가 유효하지 않으면 반환
        if (collisionObject.tag == "Untagged" || thisObject.tag == "Untagged") return;

        // 같은 태그의 행성들만 병합
        if (thisObject.tag != collisionObject.tag) return;

        Planet thisPlanet = thisObject.GetComponent<Planet>();
        Planet otherPlanet = collisionObject.GetComponent<Planet>();

        // 이미 병합 중인 행성이면 무시
        if ((thisPlanet != null && thisPlanet.IsMerging) || (otherPlanet != null && otherPlanet.IsMerging)) return;

        // InstanceID가 더 큰 쪽이 병합을 처리 (한 번만 처리하기 위함)
        if (thisObject.GetInstanceID() <= collisionObject.GetInstanceID()) return;

        // 병합 처리 시작
        if (thisPlanet != null) thisPlanet.SetMergingState(true);
        if (otherPlanet != null) otherPlanet.SetMergingState(true);

        int currentLevel = int.Parse(thisObject.tag);
        
        // 마지막 단계가 아닌 경우만 업그레이드
        if (currentLevel < planets.Count - 1)
        {
            GameObject nextPrefab = planets[currentLevel];
            
            if (nextPrefab == null) {
                Debug.LogError($"Planets list missing prefab at index {currentLevel}!");
            } else {
                Debug.Log($"Merge: Tag {thisObject.tag} + Tag {collisionObject.tag} -> Spawning Index {currentLevel} (Prefab: {nextPrefab.name})");
                
                // 새 행성 생성
                GameObject mergedPlanet = Instantiate(nextPrefab);
                mergedPlanet.transform.position = thisObject.transform.position;
                mergedPlanet.GetComponent<CircleCollider2D>().enabled = true;
                mergedPlanet.GetComponent<Rigidbody2D>().gravityScale = 1;
                
                Planet mergedPlanetComponent = mergedPlanet.GetComponent<Planet>();
                if (mergedPlanetComponent != null) {
                    mergedPlanetComponent.gameManager = this;
                }
                
                // 점수 추가
                AddScore((currentLevel + 1) * 10);
            }
        }

        // 병합된 행성 파괴
        Destroy(thisObject);
        Destroy(collisionObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0; // 게임 정지
        gameOverPanel.SetActive(true);
    }

    public void ReGame() {
        SceneManager.LoadScene("GameScene");
    }

    // 현재 물체가 마우스 X 좌표에 따라오게 만들기
    private void UpdateCurrentObjectPosition() {
        if (currentObject == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        currentObject.transform.position = new Vector3(Mathf.Clamp(mousePos.x, -3.5f, 3.5f), 4.0f, 0);
    }

    private void ActivateCurrentObject() {
        if (currentObject == null) return;
        currentObject.GetComponent<CircleCollider2D>().enabled = true;
        currentObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        currentObject = null; // 참조 해제
    }

    private void SelectNextObject() {
        int n = Random.Range(0, 3); // 0이상 3이하 정수
        nextObject = planets[n];
    }

    private void SpawnNextObject() {
        if (nextObject == null) SelectNextObject();
        // 1. 대기 중인 행성 소환
        currentObject = Instantiate(nextObject);
        currentObject.GetComponent<Planet>().gameManager = this;

        // 2. 다음 행성 미리 뽑기
        SelectNextObject();

        // 3. UI 갱신 (다음 행성 이미지 보여주기)
        if (nextPlanetImage != null) {
            // 프리팹에 SpriteRenderer가 있다고 가정
            SpriteRenderer sr = nextObject.GetComponent<SpriteRenderer>();
            if (sr != null) {
                nextPlanetImage.sprite = sr.sprite;
            }
        }
    }

    private void StartSetting() {
        Time.timeScale = 1;
        isGameOver = false;
        score = 0;
        scoreText.text = "0";
    }
}
