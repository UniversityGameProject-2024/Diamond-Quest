using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseTest : MonoBehaviour
{
    void Start()
    {
        var options = new AppOptions()
        {
            ProjectId = "diamond-quest-59676",
            ApiKey = "AIzaSyBbCK_47ajHXvR12RirvvtvDBEPodEOJ6w",
            AppId = "1:662789458154:android:67229a7ba6e1f52bf66f93",
            DatabaseUrl = new System.Uri("https://diamond-quest-59676-default-rtdb.firebaseio.com")
        };

        FirebaseApp.Create(options);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("✅ Firebase is ready!");
                var db = FirebaseDatabase.DefaultInstance.RootReference;
                db.Child("test").SetValueAsync("נבדק בהצלחה!");
            }
            else
            {
                Debug.LogError("❌ Firebase Init Error: " + task.Result);
            }
        });
    }
}
