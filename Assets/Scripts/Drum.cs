using UnityEngine;

public class Drum : MonoBehaviour
{
    public int ColumnIndex { get; set; }
    public float perfectY = -3f;
    public int _score = 10;

    private float speed;

    private bool active = false;
    private float maxDistance = 1f;

    void Start()
    {
        DrumManager.Instance.RegisterDrum(this);
    }

    public void Initialize(float fallSpeed, float targetY, int index)
    {
        this.speed = fallSpeed;

        this.ColumnIndex = index;
        active = true;
    }

    void Update()
    {
        if (!active) return;

        transform.position += Vector3.down * speed * Time.deltaTime;

        //if (transform.position.y <= targetY)
        //{
        //    transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        //    active = false;
        //    // Reached destination - maybe notify or destroy
        //    Destroy(gameObject);
        //}
    }

    public void TryCatch()
    {
        float distance = Mathf.Abs(transform.position.y - perfectY);
        int score = CalculateScore(distance);

        GameManager.Instance.UpdateScore(score);
        DrumManager.Instance.PopDrum(ColumnIndex);
        Destroy(gameObject);
    }

    private int CalculateScore(float distance)
    {
        Debug.Log("Distance: " + distance); 
        float threshold = 0.2f;

        if (distance > maxDistance)
            return 0;

        if (distance <= threshold)
            return _score;

        float normalizedDistance = 1 - (distance / maxDistance);
        return Mathf.RoundToInt(normalizedDistance * _score);
    }
}
