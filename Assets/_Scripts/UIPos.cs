using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UIPos : MonoBehaviour {

    [SerializeField] Vector3 idle;
    [SerializeField] Vector3 tutorial;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 won;
    [SerializeField] Vector3 lose;

    Dictionary<GameState, Vector3> posDict;

    RectTransform rect;

    void Start() {
        posDict = new Dictionary<GameState, Vector3>();
        posDict.Add(GameState.Idle, idle);
        posDict.Add(GameState.Tutorial, tutorial);
        posDict.Add(GameState.Start, start);
        posDict.Add(GameState.Win, won);
        posDict.Add(GameState.Lose, lose);

        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = idle;
        GameManager.AddOnGameStateChanged(_gameState => {
            rect.DOAnchorPos(posDict[_gameState], UIManager.ANIM_DUR);
        });
    }
}