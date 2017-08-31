using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveDataBundle
{
    // public members are serialized automatically
    // for private members use [SerializeField] attribute
    public float m_Cash;
    public int totalDeliveriesCompleted, totalTimePlayed;

    public float globalAudio;
    public float musicAudio;
    public float sfxAudio;

    public bool muted;
    public bool hasDoneTutorial;

    public SaveDataBundle() { }
    public SaveDataBundle(float initialCash)
    {
        m_Cash = initialCash;
        totalDeliveriesCompleted = 0;
        totalTimePlayed = 0;
        globalAudio = 1f;
        musicAudio = 1f;
        sfxAudio = 1f;
        muted = false;
        hasDoneTutorial = false;
    }

    public static SaveDataBundle FromByteArray(byte[] data)
    {

        using (var stream = new MemoryStream())
        {

            var formatter = new BinaryFormatter();
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);

            SaveDataBundle block = (SaveDataBundle)formatter.Deserialize(stream);
            return block;
        }
    }

    public static byte[] ToByteArray(SaveDataBundle bundle)
    {
        var formatter = new BinaryFormatter();

        using (var stream = new MemoryStream())
        {

            formatter.Serialize(stream, bundle);
            return stream.ToArray();
        }
    }
}