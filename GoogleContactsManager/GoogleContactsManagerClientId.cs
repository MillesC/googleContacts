using System;
using System.Collections.Generic;
using System.IO;

namespace GoogleContactsManager
{
    public class GoogleContactsManagerClientId
    {
        private static RootObject ro;
        private static RootObject Ro
        {
            get 
            {
                if (ro == null)
                {
                    // se o ficheiro não existir, o erro dá algures na Task e não aparece
                    // simplesmente não acontece nada
                    string json = File.ReadAllText(
                        Path.Combine(
                            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                            "client_id.json")
                    );
                    ro = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(json);
                }
                return ro;
            }
        }
        public static string ClientId { get { return Ro.installed.client_id; } }
        public static string ClientSecret { get { return Ro.installed.client_secret; } }
        public static string ProjectId { get { return Ro.installed.project_id; } }
        public const string ApplicationName = "WAYD-Google-Contacts-Manager";
    }
    public class Installed
    {
        public string client_id { get; set; }
        public string project_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_secret { get; set; }
        public List<string> redirect_uris { get; set; }
    }
    public class RootObject
    {
        public Installed installed { get; set; }
    }
}
