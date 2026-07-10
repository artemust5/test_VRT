using UnityEngine;

namespace _ProjectRestructured.Scripts.Gameplay.Custom
{
    public class ChangeStateByKey : MonoBehaviour
    {
        [System.Serializable]
        public class State
        {
            public string stateName;
            public GameObject[] objects;
        }
        
        private const KeyCode SwitchKeyForNumpad = KeyCode.KeypadPlus;
        private const KeyCode SwitchKeyForKeypad = KeyCode.Space;
        public State[] states;

        [Space]
        [Header("▶ Numpad - KeypadPlus")]
        [Header("▶ Keypad - Space")]
        public string currentStateName;

        private int currentStateIndex;

        private void Start()
        {
            if (states.Length <= 0) return;
            ActivateState(currentStateIndex);
            UpdateCurrentStateName();
        }

        private void Update()
        {
            if (Input.GetKeyDown(SwitchKeyForNumpad) || Input.GetKeyDown(SwitchKeyForKeypad))
            {
                SwitchToNextState();
            }
        }

        private void SwitchToNextState()
        {
            DeactivateState(currentStateIndex);

            currentStateIndex = (currentStateIndex + 1) % states.Length;

            ActivateState(currentStateIndex);
            UpdateCurrentStateName();
        }

        private void ActivateState(int stateIndex)
        {
            if (stateIndex < 0 || stateIndex >= states.Length)
                return;

            foreach (var obj in states[stateIndex].objects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        private void DeactivateState(int stateIndex)
        {
            if (stateIndex < 0 || stateIndex >= states.Length)
                return;

            foreach (var obj in states[stateIndex].objects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        private void UpdateCurrentStateName()
        {
            if (currentStateIndex >= 0 && currentStateIndex < states.Length)
            {
                currentStateName = states[currentStateIndex].stateName;
            }
            else
            {
                currentStateName = "No State";
            }
        }
    }
}