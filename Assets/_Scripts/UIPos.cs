using DG.Tweening;
using UnityEngine;

public class UIPos : MonoBehaviour {

    [SerializeField] Vector3 idle;
    [SerializeField] Vector3 tutorial;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 won;
    [SerializeField] Vector3 lose;

    RectTransform rect;

    void Start() {
        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = idle;
        GameManager.AddOnGameStateChanged(_gameState => {
            if (_gameState == GameState.Idle) {
                rect.DOAnchorPos(idle, UIManager.ANIM_DUR);
            } else if (_gameState == GameState.Tutorial) {
                rect.DOAnchorPos(tutorial, UIManager.ANIM_DUR);
            } else if (_gameState == GameState.Start) {
                rect.DOAnchorPos(start, UIManager.ANIM_DUR);
            } else if (_gameState == GameState.Win) {
                rect.DOAnchorPos(won, UIManager.ANIM_DUR);
            } else if (_gameState == GameState.Lose) {
                rect.DOAnchorPos(lose, UIManager.ANIM_DUR);
            }
        });
    }
}