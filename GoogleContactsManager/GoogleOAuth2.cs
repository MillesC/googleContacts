using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleContactsManager
{
    public class GoogleOAuth2Tokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdToken { get; set; }
        public string Email { get; set; }
        static Regex json = new Regex(@"\""(?<PARAM>[^\""]+)\"".*:.*\""(?<VALOR>[^\""]+)\""", RegexOptions.Compiled | RegexOptions.Singleline);
        /// <summary>
        /// Faz o parse de um texto em formato JSON com uma expressão regular.
        /// Trata todas as linhas com o formato
        ///      "access_token":"1/fFAGRNJru1FTz70BzhT3Zg",
        /// </summary>
        /// <param name="theText">O texto a a tratar</param>
        public void ParseResponse(string theText)
        {
            string[] linhas = theText.Split('\n', '\r');
            foreach (string linha in linhas)
            {
                Match m = json.Match(linha);
                if (m.Success)
                {
                    switch (m.Groups["PARAM"].Value)
                    {
                        case "access_token":
                            AccessToken = m.Groups["VALOR"].Value;
                            break;
                        case "refresh_token":
                            RefreshToken = m.Groups["VALOR"].Value;
                            break;
                        case "id_token":
                            IdToken = m.Groups["VALOR"].Value;
                            break;
                        case "email":
                            Email = m.Groups["VALOR"].Value;
                            break;
                    }
                }
            }
        }
    }
    public class GoogleOAuth2
    {
        #region Configuração
        private static string buildAuthorizationUrl(string[] scopes, string redirectUri, string clientId, string email)
        {
            return string.Format(
                    @"https://accounts.google.com/o/oauth2/v2/auth?scope={0}&redirect_uri={1}&response_type=code&client_id={2}&login_hint={3}",
                    WebUtility.UrlEncode(string.Join(" ", scopes)),
                    WebUtility.UrlEncode(redirectUri),
                    clientId,
                    email);
        }
        private const string getTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        private static string buildGetIdUrl(string idToken)
        {
            return string.Format("https://www.googleapis.com/oauth2/v2/tokeninfo?id_token={0}", idToken);
        }
        public const string ScopeContactos = "https://www.google.com/m8/feeds/";
        public const string ScopeUserInfoEmail = "https://www.googleapis.com/auth/userinfo.email";
        #endregion
        /// <summary>
        /// Pede autorização ao Google para aceder aos dados de um utilizador, de acordo com o scope indicado.
        /// Usa um HttpListener para obter o resultado.
        /// </summary>
        /// <param name="clientId">O clienteId da aplicação Developers Google Console</param>
        /// <param name="clientSecret">O clienteSecret da aplicação Developers Google Console</param>
        /// <param name="scopes">Os scopes pretendidos. É preferível incluir https://www.googleapis.com/auth/userinfo.email </param>
        /// <param name="email">Este endereço será usado como sugestão se o utilizador não tiver nenhuma conta aberta</param>
        /// <param name="ct">Token que é passado para PostAsync e GetAsync</param>
        public static async Task<GoogleOAuth2Tokens> AuthorizeAsync(string clientId, string clientSecret, string[] scopes, string email, CancellationToken ct)
        {
            GoogleOAuth2Tokens resultTokens = new GoogleOAuth2Tokens();
            // um endereço em localhost para o Google retornar o código de acesso
            string redirectUri = string.Format("http://localhost:{0}/authorizegoogle/", getRandomUnusedPort());

            ///
            /// Criar um Listener para tratar o endereço definido em redirectUri e receber o authorization code
            /// 
            string authotizationCode = null;
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(redirectUri);
                try
                {
                    listener.Start();

                    // Abre um browser para o utilizador dar acesso
                    Process.Start(buildAuthorizationUrl(scopes, redirectUri, clientId, email));

                    // Espera pela resposta e guarda o URL no qual estará o código que pretendemos: {redirectUri}/?code=<codigo>
                    var context = await listener.GetContextAsync().ConfigureAwait(false);
                    NameValueCollection coll = context.Request.QueryString;
                    foreach (var k in coll.AllKeys)
                    {
                        if (k.Equals("code", System.StringComparison.InvariantCultureIgnoreCase))
                        {
                            authotizationCode = coll[k];
                        }
                    }
                    sendOutput(context, authotizationCode);
                }
                finally
                {
                    listener.Close();
                }
            }
            if (string.IsNullOrEmpty(authotizationCode))
            {
                return null; 
            }
            
            /// 
            /// Fazer o pedido do token com o código obtido                    
            /// 
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                var values = new Dictionary<string, string>();
                values.Add("code", authotizationCode);
                values.Add("client_id", clientId);
                values.Add("client_secret", clientSecret);
                values.Add("redirect_uri", redirectUri);
                values.Add("grant_type", "authorization_code");
                var postContent = new FormUrlEncodedContent(values);
                using (HttpResponseMessage response = await client.PostAsync(getTokenUrl, postContent, ct).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        resultTokens.ParseResponse(result);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return resultTokens;
        }
        private static void sendOutput(HttpListenerContext context, string authotizationCode)
        {
            // Para que o browser não fique com a janela vazia escrevo qualquer coisa lá
            string mensagem = (string.IsNullOrEmpty(authotizationCode) ? "A autorização não foi concedida" : "Foi recebido o código de autorização");
            string closePageResponse =
@"<!doctype html>
<html>
<head>
<meta charset='utf-8'>
<title>" + mensagem + @"</title>
<meta name='viewport' content='width=device-width, initial-scale=1'>
<style> 
* {line-height: 1.2; margin: 0;}
html {display: table; font-family: sans-serif; height: 100%;text-align: center;width: 100%;} 
body {display: table-cell;vertical-align: middle;margin: 2em auto;} 
h1 {color: #555;font-size: 2em;font-weight: 400;}
</style>
</head>
<body>
<h1>Esta janela pode ser fechada.</h1>
<p>" + mensagem + @".</p>
</body>
</html>";
            byte[] output = System.Text.Encoding.UTF8.GetBytes(closePageResponse);
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = output.Length;
            Stream outputStream = context.Response.OutputStream;
            outputStream.Write(output, 0, output.Length);
            outputStream.Close();
        }
        private static int getRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
