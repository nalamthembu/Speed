using UnityEngine;
[
    RequireComponent
    (
        typeof(SphereCollider)
    )
]

public class PlayerSounds : MonoBehaviour
{
    new private SphereCollider collider;

    [SerializeField] float minWhooshSpeed = 100F;

    [SerializeField] float maxObjectSizeBeforeBigWhoosh = 10.0F;

    [SerializeField] float minObjectSize = 2.0F;

    [SerializeField] LayerMask layerToIgnore;

    [SerializeField] PlayerSoundIDs soundIDs;

    private AudioSource source;

    private void Awake()
    {
        if (source is null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 0;
        }
    }

    private void OnValidate()
    {
        if (collider is null)
            collider = GetComponent<SphereCollider>();

        //If its still null after trying to find it.
        if (collider is null)
        {
            collider = gameObject.AddComponent<SphereCollider>();
        }

        if (!collider.isTrigger)
            collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player.instance == null || Player.instance.Vehicle == null)
            return;

        if (other.bounds.extents.sqrMagnitude <= minObjectSize * minObjectSize)
            return;

        if (Player.instance.Vehicle.SpeedKMH >= minWhooshSpeed)
        {
            if (other.bounds.extents.sqrMagnitude < maxObjectSizeBeforeBigWhoosh * maxObjectSizeBeforeBigWhoosh)
            {
                SoundManager.instance.PlaySound(soundIDs.regularWhoosh, other.transform.position, false, new RandomPitch(1, 1.5f));
            }
            else
            {
                SoundManager.instance.PlaySound(soundIDs.bigWhoosh, other.transform.position, false, new RandomPitch(1, 1.5f));
            }
        }
    }
}

[System.Serializable]
public struct PlayerSoundIDs
{
    public string regularWhoosh;
    public string bigWhoosh;
}
