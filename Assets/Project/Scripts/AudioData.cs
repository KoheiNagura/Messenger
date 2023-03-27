using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Messenger/AudioData", order = 0)]
public class AudioData : ScriptableObject
{
    public AudioType Type => type;
    public AudioClip Clip => clip;
    public float Volume => volume;
    public float Pitch => pitch;

    [SerializeField] private AudioType type;
    [SerializeField] private AudioClip clip;
    [SerializeField, Range(0, 1)] private float volume = 1;
    [SerializeField, Range(-3, 3)] private float pitch = 1;
}

public enum AudioType
{
    Stuck,
    Drop,
    Spawn,
    OpenModal,
    CloseModal,
}