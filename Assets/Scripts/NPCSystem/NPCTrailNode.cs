using UnityEngine;

public class NPCTrailNode : MonoBehaviour
{
    [SerializeField] NPCTrailNode[] m_NextPath;

    [SerializeField] bool m_DrawPath;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawSphere(transform.position, 0.25f);

        if (m_DrawPath)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < m_NextPath.Length; i++)
                Gizmos.DrawLine(transform.position, m_NextPath[i].transform.position);
        }
    }

    public NPCTrailNode[] GetNextNode()
    {
        return m_NextPath;
    }
}