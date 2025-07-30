using TMPro;
using UnityEngine;

public class TouchToStartBlink : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI touchText;
    [SerializeField] private float blinkSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
        Color color = touchText.color;
        color.a = alpha;
        touchText.color = color;
    }
}
