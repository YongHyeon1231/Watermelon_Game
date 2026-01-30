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

    void OnCollisionEnter2D(Collision2D collision) {
        // 부딪혔을 때 둘 중 하나만 작동하게 하는 법
        if (this.gameObject.tag == collision.gameObject.tag) {
            if (this.gameObject.GetInstanceID() > collision.gameObject.GetInstanceID()) {
                GameObject clone = Instantiate(gameManager.planets[int.Parse(this.gameObject.tag)]);
                clone.transform.position = this.gameObject.transform.position;
                clone.GetComponent<CircleCollider2D>().enabled = true;
                clone.GetComponent<Rigidbody2D>().gravityScale = 1;
                clone.GetComponent<Planet>().gameManager = this.gameManager;
            }
            Destroy(this.gameObject);
        }
    }
}
