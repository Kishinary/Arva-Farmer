using UnityEngine;

public class Ability : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public new string name;
    public float cooldownTime;
    public float activeTime;

    public virtual void Activate(GameObject parent)
    {
        
        
    }
}
