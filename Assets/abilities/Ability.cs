using UnityEngine;

public class Ability : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public new string name;
    public float cooldownTime;
    public float activeTime;
    public Transform transform;

    public virtual void BeginAim(GameObject parent, Transform playerTransform, Rigidbody2D rb) { }

    public virtual void DuringAim(GameObject parent, Transform playerTransform, Rigidbody2D rb) { }

    public virtual void Activate(GameObject parent, Transform transform, Rigidbody2D rb){  }
    public virtual void BeginCoolDown(GameObject parent) { }
}
