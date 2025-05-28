// 배경 스크롤러 

using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField]
    GameObject groundObj;
    [SerializeField]
    GameObject backgroundObj;
    [SerializeField]
    float groundSpeed;
    [SerializeField]
    float backgroundSpeed;

    Vector2 groundStartPos;
    Vector2 backgroundStartPos;

    float groundWidth;
    float backgroundWidth;
    private void Start()
    {
        groundStartPos = groundObj.transform.position;
        backgroundStartPos = backgroundObj.transform.position;

        groundWidth = groundObj.GetComponent<SpriteRenderer>().bounds.size.x;
        backgroundWidth = backgroundObj.GetComponent<SpriteRenderer>().bounds.size.x;
        
    }
    private void Update()
    {
        // 배경 이동 
        groundObj.transform.Translate(Vector2.left * groundSpeed * Time.deltaTime);
        backgroundObj.transform.Translate(Vector2.left * backgroundSpeed * Time.deltaTime);

        if(groundObj.transform.position.x <= groundStartPos.x - groundWidth)
        {
            groundObj.transform.position = groundStartPos;
        }
        if(backgroundObj.transform.position.x <= backgroundStartPos.x - backgroundWidth)
        {
            backgroundObj.transform.position = backgroundStartPos;
        }
    }
}
