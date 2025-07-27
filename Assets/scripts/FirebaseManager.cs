using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;


public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore DB { get; private set; }
    public FirebaseUser User { get; private set; }

    private string currentUsername;
    public bool IsFirebaseReady { get; private set; } = false;

    void Awake()
    {
        Debug.Log("[FirebaseManager] 🚀 Awake: iniziata configurazione Firebase.");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log("[FirebaseManager] 🔍 Dipendenze Check task completato.");

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("[FirebaseManager] ❌ Errore nel controllo dipendenze Firebase: " + task.Exception);
                    return;
                }

                var status = task.Result;
                Debug.Log($"[FirebaseManager] DependencyStatus = {status}");

                if (status == DependencyStatus.Available)
                {
                    Debug.Log("[FirebaseManager] ✅ Dependencies available. Inizializzo Auth...");

                    try
                    {
                        Auth = FirebaseAuth.DefaultInstance;
                        Debug.Log("[FirebaseManager] ✅ FirebaseAuth.DefaultInstance ottenuto.");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("[FirebaseManager] ❌ Errore Auth init: " + ex.Message);
                    }

                    try
                    {
                        DB = FirebaseFirestore.DefaultInstance;
                        Debug.Log("[FirebaseManager] ✅ FirebaseFirestore.DefaultInstance ottenuto.");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("[FirebaseManager] ❌ Errore Firestore init: " + ex.Message);
                    }

                    try
                    {
                        Debug.Log($"[FirebaseManager] 📦 FirebaseApp name: {FirebaseApp.DefaultInstance?.Name}");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("[FirebaseManager] ❌ Errore accesso FirebaseApp.DefaultInstance: " + ex.Message);
                    }

                    IsFirebaseReady = true;
                    Debug.Log("[FirebaseManager] ✅ Firebase pronto.");
                }
                else
                {
                    Debug.LogError($"[FirebaseManager] ❌ Firebase non disponibile: {status}");
                }
            });
        }
        else
        {
            Debug.Log("[FirebaseManager] 🔁 Istanza già esistente, distruggo duplicato.");
            Destroy(gameObject);
        }
    }


    public void Register(string email, string password, string username, System.Action<bool, string> callback)
    {
        Debug.Log($"[FirebaseManager] Tentativo registrazione: {email}, username={username}");

        if (!IsFirebaseReady || Auth == null || DB == null)
        {
            Debug.LogWarning("[FirebaseManager] Register chiamato ma Firebase non pronto.");
            callback(false, "Firebase non inizializzato. Riprova tra poco.");
            return;
        }

        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                foreach (var e in task.Exception.Flatten().InnerExceptions)
                {
                    Debug.LogError($"[FirebaseManager] Register exception: {e.GetType()} - {e.Message}");
                }

                callback(false, task.Exception?.Message ?? "Errore registrazione");
                return;
            }

            User = task.Result.User;
            currentUsername = username;

            Debug.Log($"[FirebaseManager] Utente registrato, uid={User.UserId}");

            var userData = new Dictionary<string, object>
            {
                { "username", username }
            };

            DB.Collection("users").Document(User.UserId)
              .SetAsync(userData)
              .ContinueWithOnMainThread(setTask =>
            {
                Debug.Log("[FirebaseManager] ➡️ Entrato in ContinueWith per salvataggio username");
                if (setTask.IsFaulted)
                    Debug.LogError($"[FirebaseManager] Errore salvataggio username su Firestore: {setTask.Exception}");
                else
                    Debug.Log("[FirebaseManager] Username salvato su Firestore correttamente.");

                callback(true, "Registrazione completata");
            });
        });
    }

    public void Login(string email, string password, System.Action<bool, string> callback)
    {
        Debug.Log($"[FirebaseManager] Tentativo login: {email}");

        if (!IsFirebaseReady || Auth == null || DB == null)
        {
            Debug.LogWarning("[FirebaseManager] Login chiamato ma Firebase non pronto.");
            callback(false, "Firebase non inizializzato. Riprova tra poco.");
            return;
        }

        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"[FirebaseManager] Login fallito: {task.Exception}");
                callback(false, task.Exception?.Message ?? "Errore login");
                return;
            }

            User = task.Result.User;
            Debug.Log($"[FirebaseManager] Utente loggato, uid={User.UserId}");

            DB.Collection("users").Document(User.UserId)
              .GetSnapshotAsync()
              .ContinueWithOnMainThread(snapshotTask =>
            {
                if (snapshotTask.IsFaulted)
                {
                    Debug.LogError($"[FirebaseManager] Errore ottenimento user doc: {snapshotTask.Exception}");
                    callback(false, "Errore lettura username");
                    return;
                }

                var snapshot = snapshotTask.Result;
                if (snapshot.Exists)
                {
                    currentUsername = snapshot.GetValue<string>("username");
                    Debug.Log($"[FirebaseManager] Username caricato: {currentUsername}");
                    callback(true, "Login riuscito");
                }
                else
                {
                    Debug.LogWarning("[FirebaseManager] Document snapshot non esiste.");
                    callback(false, "Username non trovato");
                }
            });
        });
    }

    public string GetUsername() => currentUsername;
    public string GetUserID() => User?.UserId;
    public string GetEmail() => User?.Email;
}
