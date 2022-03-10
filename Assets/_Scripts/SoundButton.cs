using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundButton : Button {

    bool pointerClickSound = true;

    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);
        if (pointerClickSound) {
            Sound.PlayButton();
            Taptic.Light();
        }
        pointerClickSound = true;
    }

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);
        Sound.PlayButton();
        pointerClickSound = false;
        Timer.Delay(0.1f, () => {
            pointerClickSound = true;
        });
    }

    public override void OnSubmit(BaseEventData eventData) {
        base.OnSubmit(eventData);
        Sound.PlayButton();
    }

}