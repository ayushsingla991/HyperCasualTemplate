using UnityEngine;

public class Game : MonoBehaviour {

    static Game instance;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

}