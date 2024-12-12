using FM.Template;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Goal {
    public string color;
    public Sprite sprite;
    public int target;

    public int count;
    public GoalUI ui;

    public Goal(string _color, Sprite _sprite) {
        color = _color;
        sprite = _sprite;
        count = 0;
    }
}

namespace FM.Template {
    public class GoalManager : MonoBehaviour {

        static GoalManager instance;

        Dictionary<string, Goal> goals;

        [SerializeField] GameObject goalUIPrefab;
        [SerializeField] RectTransform goalsBG;
        [SerializeField] RectTransform goalsContainer;
        [SerializeField] Animator combo;
        [SerializeField] Animator perfect;
        [SerializeField] ProgressBar goalBar;

        [SerializeField] Image[] comboCountImages;
        int comboCount;

        public static bool ComboActive;

        int totalCount;

        void Awake() {
            instance = this;
            Sprite[] sprites = Resources.LoadAll<Sprite>("Goal/");
            goals = sprites.ToDictionary(x => x.name, x => new Goal(x.name, x));
            comboCount = 0;
            totalCount = 0;
            ComboActive = false;
        }

        public static void SetGoalSize(float size) {
            instance.goalsBG.sizeDelta = new Vector2(size, instance.goalsBG.sizeDelta.y);
        }

        public static void SetGoal(Dictionary<string, int> goals) {
            instance.goals = instance.goals.Where(pair => goals.ContainsKey(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (KeyValuePair<string, int> goal in goals) {
                instance.goals[goal.Key].target = goal.Value;
            }

            float gapX = 200;
            float startX = -gapX * (goals.Count - 1) / 2f;

            int index = 0;

            foreach (KeyValuePair<string, int> goal in goals) {
                RectTransform goalRect = Instantiate(instance.goalUIPrefab).GetComponent<RectTransform>();
                goalRect.SetParent(instance.goalsContainer);
                goalRect.localScale = Vector3.one;
                goalRect.anchoredPosition = new Vector3(startX + gapX * index, 0, 0);
                index++;

                GoalUI goalUI = goalRect.GetComponent<GoalUI>();
                goalUI.goalText.text = goal.Value.ToString();
                goalUI.goalImage.sprite = instance.goals[goal.Key].sprite;

                instance.goals[goal.Key].ui = goalUI;
            }

            instance.goalBar.minValue = 0;
            instance.goalBar.maxValue = goals.Values.Sum();
            instance.goalBar.progress = 0;
            instance.goalBar.text = instance.totalCount + "/" + instance.goalBar.maxValue;

        }

        public static void DoneCount(int count) {
            if (count >= 10) {
                instance.perfect.gameObject.SetActive(true);
                instance.perfect.enabled = true;
                Sound.PlaySound("combo", false);
            }
        }

        public static void Done(string color, int count) {
            instance.totalCount += count;
            instance.goalBar.text = instance.totalCount + "/" + instance.goalBar.maxValue;

            instance.goalBar.DOProgress(instance.totalCount);

            if (instance.goals.ContainsKey("any")) {
                color = "any";
            }
            if (!instance.goals.ContainsKey(color)) {
                return;
            }
            instance.goals[color].count += count;
            instance.goals[color].ui.tickImage.gameObject.SetActive(instance.goals[color].count >= instance.goals[color].target);
            instance.goals[color].ui.goalText.gameObject.SetActive(instance.goals[color].count < instance.goals[color].target);
            instance.goals[color].ui.goalText.text = "" + (instance.goals[color].target - instance.goals[color].count);

            if (instance.goals[color].count < instance.goals[color].target) {
                CoinsManager.CoinsToAdd += count;
            }

            if (instance.goals.All(goal => goal.Value.count >= goal.Value.target)) {
                GameManager.GameWon();
            }
            Taptic.Success();
        }

        public static void AddCombo() {
            instance.comboCount++;
            if (ComboActive) {
                return;
            }
            if (instance.comboCount >= 2) {
                ComboActive = true;
                int comboIndex = instance.comboCount - 2;
                if (comboIndex > instance.comboCountImages.Length - 1) {
                    comboIndex = instance.comboCountImages.Length - 1;
                }
                instance.combo.gameObject.SetActive(true);
                instance.combo.enabled = true;
                instance.comboCountImages[comboIndex].gameObject.SetActive(true);
                Taptic.Success();
                Sound.PlaySound("combo", false);
            }
        }

        public static void ResetCombo() {
            instance.comboCount = 0;
        }

    }
}