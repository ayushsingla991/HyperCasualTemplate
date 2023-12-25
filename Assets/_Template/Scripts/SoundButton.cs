using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FM.Template {
    public class SoundButton : Button {

        bool playingSound = false;

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
            if (!playingSound) {
                playingSound = true;
                Sound.PlaySound("button_click");
                Timer.Delay(0.2f, () => {
                    playingSound = false;
                });
            }
        }

        public override void OnSubmit(BaseEventData eventData) {
            base.OnSubmit(eventData);
            Sound.PlaySound("button_click");
        }

    }
}