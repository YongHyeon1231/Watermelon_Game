using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> planets = new List<GameObject>();
    public GameObject NextObject;
    public GameObject Clone;
    public TextMeshProUGUI scoreText;

    public Image nextPlanetImage;

    float timeCount = 0;
    private int score = 0;

    public static GameManager instance;

    public bool isGameOver = false;

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
        SetObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
            return;

        // 시간 누적
        timeCount += Time.deltaTime;

        currentObjectPos();
        
        if(Mouse.current.leftButton.wasPressedThisFrame && timeCount >= 0.5f) {
            // 누적 시간 초기화
            timeCount = 0;

            currentobjectCondition();
            SetObject();
        }
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
        Debug.Log("Game Over!");
    }

    // 물체가 마우스 X 좌표에 따라오게 만들기
    void currentObjectPos() {
        if (Clone == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Clone.transform.position = new Vector3(Mathf.Clamp(mousePos.x, -3.5f, 3.5f), 4.2f, 0);
    }

    void currentobjectCondition() {
        if (Clone == null) return;
        Clone.GetComponent<CircleCollider2D>().enabled = true;
        Clone.GetComponent<Rigidbody2D>().gravityScale = 1;
        Clone = null; // 참조 해제
    }

    void SelectNextObject() {
        int n = Random.Range(0, 3); // 0이상 3이하 정수
        NextObject = planets[n];
    }

    void SetObject() {
        if (NextObject == null) SelectNextObject();
        // 1. 대기 중인 행성 소환
        Clone = Instantiate(NextObject);
        Clone.GetComponent<Planet>().gameManager = this;

        // 2. 다음 행성 미리 뽑기
        SelectNextObject();

        // 3. UI 갱신 (다음 행성 이미지 보여주기)
        if (nextPlanetImage != null) {
            // 프리팹에 SpriteRenderer가 있다고 가정
            SpriteRenderer sr = NextObject.GetComponent<SpriteRenderer>();
            if (sr != null) {
                nextPlanetImage.sprite = sr.sprite;
            }
        }
    }

    void StartSetting() {
        Time.timeScale = 1;
        isGameOver = false;
        score = 0;
        scoreText.text = "0";
    }
}
