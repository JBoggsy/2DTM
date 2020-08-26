using UnityEngine;
using UnityEngine.UI;

using GameCore;

namespace MonoBehaviours {
    namespace UI {
        public class SimulationControlCanvasMonobehavior : MonoBehaviour {
            public Text SpeedLabel;
            public InputField SeedField;
            public Text GenerationLabel;
            public PlayButtonMonobehaviour PlayButton;

            public void Start() {
                UpdateSpeedLabel();
                UpdateSeedLabel();
                UpdateGenerationLabel();
            }

            public void Play() {
                GameMaster.Instance.HandlePlayPauseButton();
                PlayButton.UpdateTexture();
            }

            public void Restart() {
                GameMaster.Instance.HandleResetButton();
                PlayButton.UpdateTexture();
            }

            public void Step() {
                GameMaster.Instance.HandleStepButton();
                PlayButton.UpdateTexture();
            }

            public void Faster() {
                GameMaster.Instance.HandleSimSpeedIncreaseButton();
                UpdateSpeedLabel();
            }

            public void Slower() {
                GameMaster.Instance.HandleSimSpeedDecreaseButton();
                UpdateSpeedLabel();
            }

            private void UpdateSpeedLabel() {
                //float speed = GameMaster.Instance.SimulationSpeedSetting;
                SpeedLabel.text = "x1/2";
            }

            public void Randomize() {
                GameMaster.Instance.HandleChangeSeedButton();
                UpdateSeedLabel();
            }

            public void ChangeSeed(string value) {
                if (int.TryParse(value, out int seed)) {
                    GameMaster.Instance.SetSeed(seed);
                }
                UpdateSeedLabel();
            }

            private void UpdateSeedLabel() {
                SeedField.text = GameMaster.Instance.RandomSeed.ToString();
            }

            private void UpdateGenerationLabel() {
                int generation = 1;
                string text = generation.ToString();
                while (text.Length < 8) {
                    text = "0" + text;
                }
                GenerationLabel.text = text;
            }
        }
    }
}