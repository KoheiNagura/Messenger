using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource)), DisallowMultipleComponent]
public class SEManager : MonoBehaviour {
    [SerializeField] private AudioData[] datas;
    private static SEManager instance;
    private static float defaultVolume;
    private AudioSource source;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        source = GetComponent<AudioSource>();
        defaultVolume = source.volume;
    }

    public static async void Play(AudioType type, float delay = 0)
    {
        if (delay > 0) await Task.Delay(Mathf.FloorToInt(delay * 1000));
        var data = instance.datas.First(i => i.Type == type);
        var volume = defaultVolume * data.Volume;
        instance.source.volume = volume;
        instance.source.pitch = data.Pitch;
        instance.source.PlayOneShot(data.Clip);
    }
}