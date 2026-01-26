using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public List<GameObject> planets = new List<GameObject>();
    public GameObject NextObject;
    public GameObject Clone;

    float timeCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetObject();
    }

    // Update is called once per frame
    void Update()
    {
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

    // 물체가 마우스 X 좌표에 따라오게 만들기
    void currentObjectPos() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Clone.transform.position = new Vector3(Mathf.Clamp(mousePos.x, -3.5f, 3.5f), 4.2f, 0);
    }

    void currentobjectCondition() {
        Clone.GetComponent<Collider2D>().enabled = true;
        Clone.GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    void SelectNextObject() {
        int n = Random.Range(0, 3); // 0이상 3이하 정수
        NextObject = planets[n];
    }

    void SetObject() {
        SelectNextObject();
        Clone = Instantiate(NextObject);
    }
}
