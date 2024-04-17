using UnityEngine;

public class CapsuleGlowController : MonoBehaviour
{
    public Renderer capsuleRenderer; // Reference to the Renderer component of the capsule object
    public Color targetColor = Color.white; // Target color of the material
    public float glowInterval = 0.5f; // Interval between opacity changes
    public float maxOpacity = 1.0f; // Maximum opacity value
    public float minOpacity = 0.0f; // Minimum opacity value
    public float pulseDuration = 1.0f; // Duration of the pulsating effect

    private bool isGlowing = true; // Indicates whether the object is currently glowing
    private float elapsedTime = 0.0f; // Time elapsed since the last opacity change
    private float opacityDirection = 1.0f; // Direction of opacity change
    private float targetOpacity;

    void Update()
    {
        if (!isGlowing)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= glowInterval)
        {
            elapsedTime = 0.0f;

            if (capsuleRenderer.material.color.a >= maxOpacity || capsuleRenderer.material.color.a <= minOpacity)
            {
                opacityDirection *= -1;
            }
            targetOpacity = capsuleRenderer.material.color.a + (opacityDirection * (maxOpacity - minOpacity));
            capsuleRenderer.material.color = new Color(targetColor.r, targetColor.g, targetColor.b, Mathf.Lerp(capsuleRenderer.material.color.a, targetOpacity, glowInterval / pulseDuration));
        }
    }

    public void StopGlowing()
    {
        capsuleRenderer.material.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1.0f);
        isGlowing = false;
    }
}
