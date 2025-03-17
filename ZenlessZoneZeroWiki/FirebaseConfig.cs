
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
namespace ZenlessZoneZeroWiki
{
    public class FirebaseConfig
    {
        public static void Initialize()
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("zenlesszonezerowikiauth-firebase-adminsdk-fbsvc-8ea1214dac.json")
            });
        }

    }

}
