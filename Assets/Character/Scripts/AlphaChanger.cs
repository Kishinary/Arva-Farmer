using UnityEngine;

public class AlphaChanger : MonoBehaviour
{
    public Collider2D bodyCollider;
    public string obstructionTag = "Decorations";

    [Header("Alpha Settings")]
    [Range(0f, 1f)] public float transparentAlpha = 0.5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Decorations"))
        {
            if (bodyCollider.IsTouching(other))
            {
                SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                  Color curColor = sr.color;
                    sr.color = new Color(curColor.r, curColor.g, curColor.b, transparentAlpha);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Decorations"))
        {
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color curColor = sr.color;
                sr.color = new Color(curColor.r, curColor.g, curColor.b, 1f);
            }
        }
    }
}