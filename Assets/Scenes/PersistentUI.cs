using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại khi sang Scene mới
        }
        else
        {
            Destroy(gameObject); // Tránh bị nhân bản nếu quay lại Scene cũ
        }
    }
}