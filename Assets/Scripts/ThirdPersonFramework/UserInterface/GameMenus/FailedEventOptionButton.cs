using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdPersonFramework.UserInterface
{
    public class FailedEventOptionButton : MonoBehaviour
    {
        [SerializeField] Image m_Icon;
        [SerializeField] TMP_Text m_Description;
        [SerializeField] Button m_AttachedButton;

        private Action m_Action;

        public void SetOption(Sprite iconSprite, string description, Action action)
        {
            m_Icon.sprite = iconSprite;

            // TODO : GET THE ICON FROM SOME REGISTRY
            if (iconSprite == null)
                throw new NotImplementedException();

            m_Description.text = description;
            m_Action = action;

            m_AttachedButton.onClick.AddListener(() =>
            {
                CallAction();
            });
        }

        private void CallAction()
        {
            m_Action?.Invoke();

            // TODO : Play Sound

            //Close Failed Option Window

            if (FailEventScreen.Instance)
                FailEventScreen.Instance.Hide();
        }
    }
}