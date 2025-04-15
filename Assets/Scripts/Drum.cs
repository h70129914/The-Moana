using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Drum : MonoBehaviour
{
    public int _score = 10;

    private float speed;
    private Transform perfect;

    private bool active = false;
    private float maxDistance = 1f;

    public int ColumnIndex { get; set; }

    public List<GameObject> effects;

    private GameObject GetEffect(float distance)
    {
        if (distance > 1)
            return effects[4];
        else if (distance > 0.8f)
            return effects[3];
        else if (distance > 0.6f)
            return effects[2];
        else if (distance > 0.4f)
            return effects[1];
        else
            return effects[0];
    }

    void Start()
    {
        DrumManager.Instance.RegisterDrum(this);
    }

    public void Initialize(float fallSpeed, Transform target, int index)
    {
        speed = fallSpeed;
        perfect = target;
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
        float distance = Mathf.Abs(transform.position.y - perfect.position.y);
        int score = CalculateScore(distance);

        GameManager.Instance.UpdateScore(score);
        DrumManager.Instance.PopDrum(ColumnIndex);
        Destroy(gameObject);

        GameObject effect = GetEffect(distance);
        if (effect != null)
        {
            Vector3 randomOffset = new(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Quaternion randomRotation = Quaternion.Euler(0, 0, 30);

            GameObject instantiatedEffect = Instantiate(effect, perfect.position + randomOffset, randomRotation);
            instantiatedEffect.transform.localScale = Vector3.zero;
            instantiatedEffect.transform.DOScale(0.3f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                instantiatedEffect.transform.DOScale(0.4f, 0.5f).SetEase(Ease.InBounce).OnComplete(() =>
                {
                    Destroy(instantiatedEffect);
                });
            });

            Vector3 originalPosition = perfect.position;
            perfect.DOShakePosition(0.5f, 0.5f, 10, 90, false, true).OnComplete(() =>
            {
                perfect.position = originalPosition;
            });
        }
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
