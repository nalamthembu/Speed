using UnityEngine;

public class NPCTrailManager : MonoBehaviour
{
    public NPCTrailManager instance;

    private void Awake()
    {
        if (instance is null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}