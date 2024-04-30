using TMPro;
using UnityEngine;
using ThirdPersonFramework.UserInterface;

public class TitleScreenPrompt : PromptScreen
{
    [SerializeField] TMP_Text m_TitleScreenText; //'Press Any Key' Text
    [SerializeField][Range(1, 10)] float m_AlphaOccsillationSpeed = 1;
    [SerializeField][Range(0, 1)] float m_MinAlpha, m_Intensity = 1;

    private void Start() { }

    protected override void Update() { }
}
