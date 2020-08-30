using TuringMachines;
using UnityEngine;

namespace MonoBehaviours {
    namespace UI {
        /// <summary>
        /// Monobehaviour for the Turing machine editor UI element.
        /// 
        /// Handles the UI side of editing a Turing machine's FSM.
        /// </summary>
        /// <seealso cref="TuringMachine"/>
        public class TuringMachineEditorMonobehaviour : MonoBehaviour {

            /// <value>
            /// The <see cref="TuringMachine"/> actively being edited.
            /// </value>
            protected TuringMachine targetMachine;

            // Use this for initialization
            void Start() {

            }

            // Update is called once per frame
            void Update() {

            }

            /// <summary>
            /// Set the <see cref="TuringMachine"/> instance which is being edited.
            /// </summary>
            /// <param name="tm">The <see cref="TuringMachine"/> instance to be edited.</param>
            public void SetMachine(TuringMachine tm) {
                targetMachine = tm;
                _RefreshView();
            }

            /// <summary>
            /// Refresh the editor view by destroying all extant elements and
            /// creating new elements based on the current value of
            /// <see cref="targetMachine"/>.
            /// </summary>
            private void _RefreshView() {

            }
        }
    }
}