using UnityEngine;

public class GuideScroller : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }
}
