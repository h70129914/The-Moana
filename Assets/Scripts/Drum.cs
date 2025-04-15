using UnityEngine;

public class Drum : MonoBehaviour
{
    public int _score = 10;

    private float speed;
    private float perfectY = -3f;

    private bool active = false;
    private float maxDistance = 1f;

    public int ColumnIndex { get; set; }

    void Start()
    {
        DrumManager.Instance.RegisterDrum(this);
    }

    public void Initialize(float fallSpeed, float targetY, int index)
    {
        speed = fallSpeed;
        perfectY = targetY;
        ColumnIndex = index;
        active = true;
    }

    void Update()
    {
        if (!active) return;

        transform.position += speed * Time.deltaTime * Vector3.down;

        if (transform.position.y <= -6)
        {
            active = false;
            DrumManager.Instance.PopDrum(ColumnIndex);
            Destroy(gameObject);
        }
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
