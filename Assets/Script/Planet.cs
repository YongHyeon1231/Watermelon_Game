using UnityEngine;

public class Planet : MonoBehaviour
{
    // 프리팹에 있는 것을 연동시켜줄 때는 생성할때 연동시켜주면됩니다.
    public GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isMerging = false;

    void OnCollisionEnter2D(Collision2D collision) {
        if (isMerging) return;
        if (collision.gameObject.tag == "Untagged") return;

        // 부딪혔을 때 같은 태그라면
        if (this.gameObject.tag == collision.gameObject.tag) {
            
            Planet otherPlanet = collision.gameObject.GetComponent<Planet>();
            if (otherPlanet != null && otherPlanet.isMerging) return;

            if (this.gameObject.GetInstanceID() > collision.gameObject.GetInstanceID()) {
                
                this.isMerging = true;
                if(otherPlanet != null) otherPlanet.isMerging = true;

                int currentLevel = int.Parse(this.gameObject.tag);
                
                // 만약 마지막 단계의 행성이 아니라면 업그레이드
                if (currentLevel < GameManager.instance.planets.Count - 1)
                {
                    int nextIndex = currentLevel;
                    GameObject nextPrefab = GameManager.instance.planets[nextIndex];
                    
                    if (nextPrefab == null) {
                        Debug.LogError($"Planets list missing prefab at index {nextIndex}!");
                    } else {
                        Debug.Log($"Merge: Tag {this.gameObject.tag} + Tag {collision.gameObject.tag} -> Spawning Index {nextIndex} (Prefab: {nextPrefab.name})");
                    }

                    GameObject clone = Instantiate(nextPrefab);
                    clone.transform.position = this.gameObject.transform.position;
                    clone.GetComponent<CircleCollider2D>().enabled = true;
                    clone.GetComponent<Rigidbody2D>().gravityScale = 1;
                    clone.GetComponent<Planet>().gameManager = GameManager.instance;
                    
                    // 점수 추가 (예: 레벨 * 10점)
                    GameManager.instance.AddScore((currentLevel + 1) * 10);
                }

                // 합체 처리한 쪽에서 둘 다 파괴
                Destroy(this.gameObject);
                Destroy(collision.gameObject);
            }
        }
    }
}
