using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ThirdPersonFramework.UserInterface
{
    /* [PURPOSE]
     * This represents the screen that comes up during a 
     * failed event (eg. a failed mission, death, etc.)
     * It is easily modifiable so that it caters to any fail
     * condition and does anything the designer needs it to do.
     */

    [System.Serializable]
    public struct FailedEventOption
    {
        public string description;
        public Action callback;
    }

    public class FailEventScreen : BaseGameMenu
    {
        [SerializeField] TMP_Text m_FailedScreenText;
        [SerializeField] GameObject m_FailedOptionButtonPrefab;
        [SerializeField] Transform m_OptionsPanel;
        List<FailedEventOptionButton> m_FailedOptionsButtonList = new();

        public static FailEventScreen Instance;

        public bool IsActive { get { return !IsHidden(); } }

        // This shows the event screen and displays the available options 
        // to the player.
        public void OnFailedEvent(string failedMessage, FailedEventOption[] options)
        {
            m_FailedScreenText.text = failedMessage;

            foreach (var option in options)
            {
                var buttonGO = Instantiate(m_FailedOptionButtonPrefab, m_OptionsPanel.position, Quaternion.identity, m_OptionsPanel);

                var buttonObj = buttonGO.GetComponent<FailedEventOptionButton>();

                buttonObj.SetOption(null, option.description, option.callback);

                m_FailedOptionsButtonList.Add(buttonObj);
            }

            Show();
        }

        public override void Hide(bool quick = false)
        {
            base.Hide(quick);

            //Destroy all the buttons.
            foreach(var button in m_FailedOptionsButtonList)
                Destroy(button);

            //Clear List of Buttons
            m_FailedOptionsButtonList.Clear();
        }
    }
}