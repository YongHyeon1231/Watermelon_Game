using UnityEngine;

public class Planet : MonoBehaviour
{
    // 프리팹에 있는 것을 연동시켜줄 때는 생성할때 연동시켜주면됩니다.
    [HideInInspector] public GameManager gameManager;
    
    private bool isMerging = false;
    
    public bool IsMerging => isMerging;
    
    public void SetMergingState(bool state)
    {
        isMerging = state;
    }

    float t = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 4.1f) {
            t += Time.deltaTime;

            if(t > 3) {
                gameManager.GameOver();
            }
        } else {
            t = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
       gameManager.ReportCollision(this.gameObject, collision.gameObject);
    }
}
