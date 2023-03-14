using DG.Tweening;
using UnityEngine;

public class LoadingImage : MonoBehaviour {

    void Start() {
        Rotate();
    }

    void Rotate() {
        transform.DOLocalRotate(new Vector3(0, 0, 180), 1).OnComplete(() => {
            transform.rotation = Quaternion.identity;
            transform.DOScale(Vector3.zero, 0.4f).OnComplete(() => {
                transform.DOScale(new Vector3(1, 1, 1), 0.4f).OnComplete(Rotate);
            });
        });
    }

}