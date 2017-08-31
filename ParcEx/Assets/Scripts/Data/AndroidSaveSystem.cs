using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using System.Xml.Serialization;
using System.IO;

public class AndroidSaveSystem
{
    public AndroidSaveSystem()
    {
        m_currentSaveBundle = new SaveDataBundle(5f);
    }
    public string DataPath()
    {
        if (Application.isMobilePlatform)
            return (Application.persistentDataPath + "/Data/");
        else
            return (Application.dataPath + "/Data/");
    }

    public static Action OnSaveGameSelected;
    public static Action<SaveDataBundle> OnSaveLoaded;

    public static SerialEvent onSaveLoaded = new SerialEvent();
    public static SerialEvent postSaveLoaded = new SerialEvent();
    public static SerialEvent onSaveGame = new SerialEvent();

    private static SaveDataBundle m_currentSaveBundle;
    private static ISavedGameMetadata m_saveBundleMetadata = null;
    public bool hasLoaded = false;


    /// <summary>
    /// Static reference to current save data. Automatically refreshed by save system.
    /// </summary>
    /// <value>The current save.</value>

    public SaveDataBundle CurrentSave
    {
        get
        {
            return m_currentSaveBundle;
        }
    }

    public void ShowSelectUI()
    {
        bool allowCreateNew = false;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ShowSelectSavedGameUI("Select saved game",
            10,
            allowCreateNew,
            allowDelete,
            OnSavedGameSelected);
    }


    private void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
    {
        if (status == SelectUIStatus.SavedGameSelected)
        {
            // handle selected game save
        }
        else
        {
            // handle cancel or error
        }
    }

    // Read the save game
    private void ReadSaveGame(string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        // Check if signed in
        if (GPServices.instance.Authenticated)
        {

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            savedGameClient.OpenWithAutomaticConflictResolution(filename,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                callback);
        }
    }

    // Write the save game
    private void WriteSaveGame(ISavedGameMetadata game, byte[] savedData, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        // Check if signed in
        if (GPServices.instance.Authenticated)
        {
            SavedGameMetadataUpdate updatedMetadata = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
                .WithUpdatedDescription("Saved at: " + DateTime.Now)
                .Build();

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
        }
    }

    // Save the game
    public void GPlay_SaveGame()
    {

        // Check if signed in
        if (GPServices.instance.Authenticated)
        {

            // Save game callback
            Action<SavedGameRequestStatus, ISavedGameMetadata> writeCallback =
                (SavedGameRequestStatus status, ISavedGameMetadata game) => {
                    onSaveGame.Invoke(m_currentSaveBundle);
                };

            // Read binary callback
            Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
                (SavedGameRequestStatus status, byte[] data) => {
                    //m_currentSaveBundle = SaveDataBundle.FromByteArray(data);
                };

            // Read game callback
            Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
                (SavedGameRequestStatus status, ISavedGameMetadata game) => {

                    // Check if read was successful
                    if (status == SavedGameRequestStatus.Success)
                    {

                        m_saveBundleMetadata = game;
                        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, readBinaryCallback);
                    }
                };

            // Replace "MySaveGame" with whatever you would like to save file to be called
            ReadSaveGame("playerSave", readCallback);
            WriteSaveGame(m_saveBundleMetadata, SaveDataBundle.ToByteArray(m_currentSaveBundle), writeCallback);
        }
        else
        {
            SaveGame();
        }
    }

    // Load the game
    public void GPlay_LoadGame(MonoBehaviour mb)
    {
        Debug.Log(mb);
        // Check if signed in
        if (GPServices.instance.Authenticated)
        {

            // Read binary callback
            Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
                (SavedGameRequestStatus status, byte[] data) => {

                    // Check if read was successful
                    if (status == SavedGameRequestStatus.Success)
                    {

                        // Load game data
                        try
                        {

                            m_currentSaveBundle = SaveDataBundle.FromByteArray(data);
                            hasLoaded = true;
                            onSaveLoaded.Invoke(m_currentSaveBundle);
                            mb.StartCoroutine(Init());
                        }

                        catch (Exception e)
                        {
                            m_currentSaveBundle = new SaveDataBundle(100f);
                            onSaveLoaded.Invoke(m_currentSaveBundle);
                            Debug.LogError("Failed to read binary data: " + e.ToString());
                        }
                    }
                };

            // Read game callback
            Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
                (SavedGameRequestStatus status, ISavedGameMetadata game) => {

                    // Check if read was successful
                    if (status == SavedGameRequestStatus.Success)
                    {

                        m_saveBundleMetadata = game;
                        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, readBinaryCallback);
                    }
                };

            // Replace "MySaveGame" with whatever you would like to save file to be called
            ReadSaveGame("MySaveGame", readCallback);
        }
        else //new serialization for when no google play.
        {
            LoadGame(mb);
        }

    }

    public void LoadGame(MonoBehaviour mb)
    {
        if (CheckIfFolderExists())
            m_currentSaveBundle = Deserialize<SaveDataBundle>();
        else
        {
            m_currentSaveBundle = new SaveDataBundle(5f);
            SaveGame();
        }
        onSaveLoaded.Invoke(CurrentSave);
        mb.StartCoroutine(Init());
    }

    public void SaveGame()
    {
        Serialize(CurrentSave);
    }

    public bool CheckIfFolderExists()
    {
        if (Directory.Exists(DataPath()))
        {
            return true;
        }
        else
        {
            Directory.CreateDirectory(DataPath());
            return false;
        }
    }

    private void Serialize(SaveDataBundle item)
    {
        using(var stream = new FileStream(DataPath() + "data.xml", FileMode.Create))
        {
            var XML = new XmlSerializer(typeof(SaveDataBundle));
            XML.Serialize(stream, item);
        }
        onSaveGame.Invoke(item);
    }

    private T Deserialize<T>()
    {
        using(var stream = new FileStream(DataPath() + "data.xml", FileMode.Open))
        {
            var XML = new XmlSerializer(typeof(T));
            return (T)XML.Deserialize(stream);
        }
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(0.1f);
        postSaveLoaded.Invoke(CurrentSave);
    }
}