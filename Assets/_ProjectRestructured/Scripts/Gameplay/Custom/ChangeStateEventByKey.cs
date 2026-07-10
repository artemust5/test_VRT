using UnityEngine;
using UnityEngine.Events;

namespace _ProjectRestructured.Scripts.Gameplay.Custom
{
    public class ChangeStateEventByKey : MonoBehaviour
    {
        [System.Serializable]
        public class State
        {
            public string stateName;
            public UnityEvent onEnter;
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
            currentStateIndex = (currentStateIndex + 1) % states.Length;
            ActivateState(currentStateIndex);
            UpdateCurrentStateName();
        }

        private void ActivateState(int stateIndex)
        {
            if (stateIndex < 0 || stateIndex >= states.Length) return;
            states[stateIndex].onEnter.Invoke();
        }

        private void UpdateCurrentStateName()
        {
            if (currentStateIndex >= 0 && currentStateIndex < states.Length)
                currentStateName = states[currentStateIndex].stateName;
            else
                currentStateName = "No State";
        }
    }
}