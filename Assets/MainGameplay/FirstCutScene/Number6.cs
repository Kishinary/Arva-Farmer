using UnityEngine;

public class Number6 : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        GameObject.FindFirstObjectByType<FirstScene>().finallyy = true;
    }
}
