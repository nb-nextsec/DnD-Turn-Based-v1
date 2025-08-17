using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float lifetime = 0.9f;
    public float riseSpeed = 1.2f;

    Text text;
    float t;
    Vector3 velocity;

    void Awake()
    {
        text = GetComponent<Text>();
        velocity = new Vector3(0, riseSpeed, 0);
    }

    void Update()
    {
        t += Time.deltaTime;
        transform.Translate(velocity * Time.deltaTime, Space.Self);

        // fade out
        if (text != null)
        {
            var c = text.color;
            c.a = Mathf.Lerp(1f, 0f, t / lifetime);
            text.color = c;
        }

        if (t >= lifetime) Destroy(transform.parent.gameObject); // parent = little canvas
    }
}
