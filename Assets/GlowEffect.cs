using System.Collections;
using UnityEngine;

public class GlowEffect : MonoBehaviour
{

    private Renderer Component;
    public Color targetColor = Color.red;
    public float glowInterval = 0.01f;
    public float maxOpacity = 1.5f;
    public float minOpacity = 0.0f;
    public float pulseDuration = 0.7f;
    private float elapsedTime = 0.0f;
    private float targetOpacity = 0.0f;
    private int opacityDirection = 1;
    private bool isGlowing = true;

    void Start()
    {
        Component = GetComponent<Renderer>();
    }

    void Update()
    {
        if (!isGlowing)
            return;
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= glowInterval)
        {
            elapsedTime = 0.0f;
            if (Component.material.color.a >= maxOpacity || Component.material.color.a <= minOpacity)
            {
                opacityDirection *= -1;
            }
            targetOpacity = Component.material.color.a + (opacityDirection * (maxOpacity - minOpacity));
            Component.material.color = new Color(targetColor.r, targetColor.g, targetColor.b, Mathf.Lerp(Component.material.color.a, targetOpacity, glowInterval / pulseDuration));
        }
    }

    public void StopGlowing()
    {
        Component.material.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1.0f);
        isGlowing = false;
    }
}
