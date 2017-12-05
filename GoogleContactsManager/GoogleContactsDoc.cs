using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;


namespace GoogleContactsManager
{
    /// <summary>
    /// Esta classe contém o código necessário para fazer a ligação ao Google e obter autorização
    /// para obter os contactos
    /// </summary>
    public class GoogleContactsAccount
    {
        /// <summary>
        /// Endereço de e-mail da conta.
        /// Esta variável é preenchida após a autorização de acesso à conta.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Retorna um bool indicando se esta conta foi autorizada no Google
        /// </summary>
        public bool Connected
        {
            get
            {
                return ParametersAuth != null;
            }
        }
        /// <summary>
        /// Tokens para aceder aos contactos
        /// </summary>
        public OAuth2Parameters ParametersAuth
        {
            get { return parametersAuth; }
        }
        OAuth2Parameters parametersAuth;
        /// <summary>
        /// Construtor
        /// </summary>
        public GoogleContactsAccount()
        {
            Email = "";
            parametersAuth = null;
        }
        /// <summary>
        /// Método que vai obter autorização para aceder aos contactos
        /// </summary>
        public async Task AuthorizeAsync(CancellationToken ct)
        {
            GoogleOAuth2Tokens authTokens = await GoogleOAuth2.AuthorizeAsync(
                GoogleContactsManagerClientId.ClientId,
                GoogleContactsManagerClientId.ClientSecret,
                new string[] {GoogleOAuth2.ScopeContactos, GoogleOAuth2.ScopeUserInfoEmail},
                Email,
                ct).ConfigureAwait(false);
            if (authTokens == null)
            {
                throw new GoogleContactsException("Autorização não foi concedida.");
            }
            parametersAuth = new OAuth2Parameters();
            parametersAuth.AccessToken = authTokens.AccessToken;
            parametersAuth.RefreshToken = authTokens.RefreshToken;
            Email = authTokens.Email;
        }
    }
    
    /// <summary>
    /// Um tipo de exception apenas para distinguir as ocorrências geradas por estas classes
    /// </summary>
    public class GoogleContactsException : Exception
    {
        public GoogleContactsException(string message) : base(message) { }
    }

    /// <summary>
    /// Estado do documento
    /// </summary>
    public enum GoogleContactsDocStatusEnum
    {
        Offline,
        Online
    }

    /// <summary>
    /// Extensões para os contactos e algumas "configurações"
    /// </summary>
    public static class GoogleContactsExtensions
    {
        /// <summary>
        /// Número máximo de operações
        /// </summary>
        public const int MaxBatchEntryNumber = 100;
        public const string BatchGroupsUrl = "https://www.google.com/m8/feeds/groups/default/full/batch";

        /// <summary>
        /// Retorna uma string com um XML que contém os dados do contacto dentro de um elemento «feed», o mesmo formato
        /// que é utilizado na gravação para XML das listas Feed<Contact> ou Feed<Group>
        /// </summary>
        /// <returns></returns>
        public static string ToXmlFeedString(this Contact contato)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(ms))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("feed", "http://www.w3.org/2005/Atom");
                    xmlWriter.WriteAttributeString("xmlns", "gContact", null, "http://schemas.google.com/contact/2008");
                    xmlWriter.WriteAttributeString("xmlns", "gd", null, "http://schemas.google.com/g/2005");
                    xmlWriter.WriteAttributeString("xmlns", "batch", null, "http://schemas.google.com/gdata/batch");
                    xmlWriter.WriteAttributeString("xmlns", "openSearch", null, "http://a9.com/-/spec/opensearch/1.1/");
                    contato.ContactEntry.SaveToXml(xmlWriter);
                    xmlWriter.WriteEndElement();
                }
                ms.Position = 0;
                var sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Faz uma cópia do contacto, criando um novo objecto.
        /// </summary>
        /// <remarks>Isto foi feito para copiar um contacto de uma conta para outra
        /// permitindo que se façam alterações na cópia sem afetar o original, nomeadamente 
        /// a remoção do grupo original.</remarks>
        /// <returns>A cópia criada no método.</returns>
        public static Contact Duplicate(this Contact contatoOrigem)
        {
            string feedXml = contatoOrigem.ToXmlFeedString();
            //
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(feedXml)))
            {
                ContactsRequest contReq = new ContactsRequest(new RequestSettings(GoogleContactsManagerClientId.ApplicationName));
                Feed<Contact> fd = contReq.Parse<Contact>(ms, new Uri(ContactsQuery.CreateContactsUri("default")));
                if (fd.Entries.Count() != 1)
                {
                    throw new GoogleContactsException("Ocorreu um erro ao tentar duplicar o contacto.");
                }
                return fd.Entries.First();
            }
        }

        /// <summary>
        /// Lança uma exceção do tipo GoogleContactsException com os dados do GDataBatchInterrupt, elemento
        /// que pode constar na resposta aos pedidos e execução de batch
        /// </summary>
        public static void ThrowGoogleContactsException(this GDataBatchInterrupt interrupt, string entryTitle)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Ocorreu o seguinte erro: {0}", entryTitle); sb.AppendLine();
            sb.AppendFormat("Reason: {0}", interrupt.Reason); sb.AppendLine();
            sb.AppendFormat("Successes: {0}", interrupt.Successes); sb.AppendLine();
            sb.AppendFormat("Failures: {0}", interrupt.Failures); sb.AppendLine();
            sb.AppendFormat("Parsed: {0}", interrupt.Parsed); sb.AppendLine();
            sb.AppendFormat("Unprocessed: {0}", interrupt.Unprocessed); sb.AppendLine();
            throw new GoogleContactsException(sb.ToString());        
        }
    }

    /// <summary>
    /// Um objeto que contém os dados da conta: listas de grupos e contactos.
    /// </summary>
    public class GoogleContactsDoc
    {
        #region Métodos static que criam e preenchem os documentos
        /// <summary>
        /// Este é o método que cria um documento e preenche as lista de contactos e de grupos.
        /// Para isso tem que obter autorização de acesso aos contactos.
        /// </summary>
        public async static Task<GoogleContactsDoc> CreateNewAndFillAsync(CancellationToken ct)
        {
            GoogleContactsDoc doc = new GoogleContactsDoc();

            // pede autorização
            await doc.kontag.AuthorizeAsync(ct);
            if (doc.kontag.Connected)
            {
                // carrega as listas
                GroupsQuery gq = new GroupsQuery(GroupsQuery.CreateGroupsUri("default"))
                {
                    NumberToRetrieve = 99999,
                    ShowDeleted = false
                };
                doc.groupFeed = doc.contactsRequest.Get<Group>(gq);
                doc.groupList = new List<Group>(doc.groupFeed.Entries);
                //
                ContactsQuery cq = new ContactsQuery(ContactsQuery.CreateContactsUri("default"))
                {
                    NumberToRetrieve = 99999,
                    ShowDeleted = false
                };
                doc.contactFeed = doc.contactsRequest.Get<Contact>(cq);
                doc.contactList = new List<Contact>(doc.contactFeed.Entries);
            }
            // 
            return doc;
        }

        /// <summary>
        /// Cria um documento com os dados lidos do disco do caminho indicado.
        /// Na pasta deve existir um ficheiro de grupos e um ficheiro de contactos.
        /// Serve para consultas off-line e para testes.
        /// </summary>
        public static GoogleContactsDoc CreateFromDisk(string path)
        {
            GoogleContactsDoc doc = new GoogleContactsDoc();
            doc.directory = path;
            string pg = Path.Combine(path, "grupos.xml");
            string pc = Path.Combine(path, "contactos.xml");
            if (File.Exists(pg) && File.Exists(pc))
            {
                using (FileStream stream = File.Open(pg, FileMode.Open, FileAccess.Read))
                {
                    doc.groupFeed = doc.contactsRequest.Parse<Group>(stream, new Uri(ContactsQuery.CreateGroupsUri("default")));
                    doc.groupList = new List<Group>(doc.groupFeed.Entries);
                }
                using (FileStream stream = File.Open(pc, FileMode.Open, FileAccess.Read))
                {
                    doc.contactFeed = doc.contactsRequest.Parse<Contact>(stream, new Uri(ContactsQuery.CreateContactsUri("default")));
                    doc.contactList = new List<Contact>(doc.contactFeed.Entries);
                }
            }
            else
            {
                throw new GoogleContactsException(string.Format("Não foram encontrados os ficheiros necessários em {0}", path));
            }
            return doc;
        }
        #endregion
        #region Atualização de dados no Google
        /// <summary>
        /// Guardar alterações na conta Google.
        /// </summary>
        /// <returns>Devolve TRUE se não ocorrer nenhum erro.</returns>
        /// <remarks>
        /// Se der erro nos grupos não grava os contactos.
        /// A informação do erro no elemento BatchData de cada um dos registos.
        /// </remarks>
        public async Task<bool> SaveToGoogle()
        {
            if (Status != GoogleContactsDocStatusEnum.Online)
            {
                throw new GoogleContactsException("Método SaveGroups foi chamado para um documento Offline.");
            }
            return await Task<bool>.Run(() =>
            {
                if (hasChangesGroups)
                {
                    if (saveNovosGroups() == false)
                    {
                        return false;
                    }
                }
                if (hasChangesContacts)
                {
                    if (saveContacts() == false)
                    {
                        return false;
                    }
                }
                return true;
            });
        }
        /// <summary>
        /// Cria e envia um bacth request para gravar os novos grupos. 
        /// </summary>
        /// <returns>Retorna TRUE se todos os grupos forem criados com sucesso e a resposta for bem recebida.</returns>
        /// <remarks>Se houver algum erro ao receber a resposta ficamos sem saber o que aconteceu.
        /// Isso pode levar à duplicação de grupos ao tentar gravar uma segunda vez.</remarks>
        private bool saveNovosGroups()
        {
            // começa por obter a lista de iten com alterações
            bool correuTudoBem = false;
            List<Group> requestFeed = new List<Group>(groupList.Where(g => g.BatchData != null));
            if (requestFeed.Count == 0)
            {
                throw new GoogleContactsException("Não foram encontrados grupos com alterações.");
            }
            if (requestFeed.Count > GoogleContactsExtensions.MaxBatchEntryNumber)
            {
                throw new GoogleContactsException("Demasiadas alterações");
            }

            // envia as alterações para o Google
            Feed<Group> responseFeed = contactsRequest.Batch<Group>(
                 requestFeed
                 , new Uri(GoogleContactsExtensions.BatchGroupsUrl)
                 , GDataBatchOperationType.Default);
            if (responseFeed == null)
            {
                throw new GoogleContactsException("Ocorreu um erro ao tentar obter a resposta ao pedido de gravação dos grupos.");
            }

            // tratamento da resposta - atualizar o Id dos grupos novos e nos contactos respetivos
            correuTudoBem = true;
            foreach (Group retorno in responseFeed.Entries)
            {
                if (retorno.BatchData.Interrupt != null)
                {
                    // ver comentário no save dos contactos
                    retorno.BatchData.Interrupt.ThrowGoogleContactsException(retorno.Title);
                }
                Group original = groupList.Find(g => g.BatchData != null && g.BatchData.Id == retorno.BatchData.Id);
                if (original == null)
                {
                    throw new GoogleContactsException(string.Format("Ocorreu um erro após a gravação dos grupos. Não foi encontrado o grupo «{0}» na lista original.", retorno.Title));
                }
                if (retorno.BatchData.Status.Code == (int)HttpStatusCode.Created)
                {
                    // percorrer a lista de contactos com este grupo para lhe colocar o id definitivo
                    updateContactsGroupmembershipId(original.Id, retorno.Id);
                    original.Id = retorno.Id;
                    original.BatchData = null;
                }
                else if (retorno.BatchData.Status.Code != (int)HttpStatusCode.OK)
                {
                    correuTudoBem = false;
                    original.BatchData.Status.Code = retorno.BatchData.Status.Code;
                    original.BatchData.Status.Reason = retorno.BatchData.Status.Reason;
                }
            }
            if (correuTudoBem)
            {
                hasChangesGroups = false;
            }
            return correuTudoBem;
        }
        /// <summary>
        /// Percorre a lista de contactos e atualiza o Id dos grupos novos que já foram associados a contactos.
        /// Isto é chamado após a gravação de grupos com a qual obtemos o Id do grupo dado pelo Google.
        /// </summary>
        private void updateContactsGroupmembershipId(string idGroupOriginal, string idGroupNovo)
        {
            // Só é necessário fazer isto em Contactos com grupos novos, logo com alterações pendentes
            foreach (Contact contacto in contactList.Where(g => g.BatchData != null))
            {
                GroupMembership gmToUpdate = contacto.GroupMembership.FirstOrDefault(gm => gm.HRef == idGroupOriginal);
                if (gmToUpdate != default(GroupMembership))
                {
                    gmToUpdate.HRef = idGroupNovo;
                }
            }
        }
        /// <summary>
        /// Cria e envia um bacth request para gravar os contactos (insert/update/delete). 
        /// </summary>
        /// <returns>Retorna TRUE se todas as operações forem efetuadas com sucesso e a resposta for bem recebida.</returns>
        /// <remarks>Se houver algum erro ao receber a resposta ficamos sem saber o que aconteceu...</remarks>
        private bool saveContacts()
        {
            bool correuTudoBem = true;
            if (contactList.Find(c => c.BatchData != null) == null)
            {
                throw new GoogleContactsException("Não foram encontrados contactos novos ou com alterações.");
            }

            // gravar em ciclos porque o Google não aceita muitas alterações de uma só vez
            while (true)
            {
                bool nesteCicloTudoBem = false;
                List<Contact> requestFeed = new List<Contact>(contactList.Where(c => c.BatchData != null).Take(GoogleContactsExtensions.MaxBatchEntryNumber));
                if (requestFeed.Count == 0)
                {
                    break;
                }

                // pedido ao Google
                Feed<Contact> responseFeed = contactsRequest.Batch<Contact>(
                     requestFeed
                     , new Uri("https://www.google.com/m8/feeds/contacts/default/full/batch")
                     , GDataBatchOperationType.Default);
                if (responseFeed == null)
                {
                    throw new GoogleContactsException("Ocorreu um erro ao tentar obter a resposta ao pedido de gravação dos contactos.");
                }

                // tratamento da resposta - atualizar o Id dos grupos novos e os contactos
                nesteCicloTudoBem = true;
                foreach (Contact retorno in responseFeed.Entries)
                {
                    if (retorno.BatchData.Interrupt != null)
                    {
                        // Quando isto acontece, BatchData.Id é null, não dá para localizar o elemento.
                        // O google diz que é muito raro acontecer.
                        // Para dar erro basta comentar algumas linhas nos atributos (ToXmlFeedString)
                        retorno.BatchData.Interrupt.ThrowGoogleContactsException(retorno.Title);
                    }
                    Contact original = contactList.Find(c => c.BatchData != null && c.BatchData.Id == retorno.BatchData.Id);
                    if (original == null)
                    {
                        throw new GoogleContactsException(string.Format("Ocorreu um erro após a gravação dos contactos. Não foi encontrado o contacto «{0}» na lista original.", retorno.Id));
                    }
                    if (retorno.BatchData.Status.Code == (int)HttpStatusCode.Created)
                    {
                        original.BatchData = null;
                        original.Id = retorno.Id;
                    }
                    else if (retorno.BatchData.Status.Code != (int)HttpStatusCode.OK)
                    {
                        nesteCicloTudoBem = false;
                        original.BatchData.Status.Code = retorno.BatchData.Status.Code;
                        original.BatchData.Status.Reason = retorno.BatchData.Status.Reason;
                    }
                }
                if (nesteCicloTudoBem == false)
                {
                    correuTudoBem = false;
                    break;
                }
            }
            if (correuTudoBem)
            {
                hasChangesContacts = false;
            }
            return correuTudoBem;
        }
        #endregion

        /// <summary>
        /// Serializar todos os dados recebidos para um ficheiro de grupos e outro de contactos na pasta indicada em path.
        /// Se o e-mail da conta for conhecido, o programa cria uma pasta para o e-mail.
        /// </summary>
        public void SaveToDisk(string path)
        {
            if (this.Status == GoogleContactsDocStatusEnum.Online)
            {
                if (!string.IsNullOrEmpty(kontag.Email))
                {
                    path = Path.Combine(path, kontag.Email);
                }
            }
            else
            {
                if (path != directory)
                {
                    path = Path.Combine(path, Path.GetFileName(directory));
                }
            }
            Directory.CreateDirectory(path);
            using (XmlTextWriter xmlWriter = new XmlTextWriter(Path.Combine(path, "grupos.xml"), Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument(false);
                groupFeed.AtomFeed.SaveToXml(xmlWriter);
            }
            using (XmlTextWriter xmlWriter = new XmlTextWriter(Path.Combine(path, "contactos.xml"), Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument(false);
                contactFeed.AtomFeed.SaveToXml(xmlWriter);
            }
        }

        /// <summary>
        /// Acesso read-only à lista de grupos.
        /// </summary>
        /// <remarks>
        /// O Feed é um IEnumerable. 
        /// Copiamos as referencias dos objetos para uma lista (os dados não são duplicados) para se poder
        /// adicionar novos elementos e usar em grelhas, por exemplo.
        /// </remarks>
        public IList<Group> GroupEntriesList
        {
            get 
            { 
                return groupList.AsReadOnly(); 
            }
        }
        /// <summary>
        /// Adiciona um item à lista de grupos em memória. Fica com um id provisório para se poder fazer ligação aos contactos.
        /// </summary>
        /// <returns>
        /// O id provisório.
        /// </returns>
        public Group AddGroup(Group newGroup)
        {
            newGroup.Id = new Uri(string.Format("http://localhost/{0}", Guid.NewGuid().ToString())).AbsoluteUri;
            newGroup.BatchData = new GDataBatchEntryData(newGroup.Id, GDataBatchOperationType.insert);
            groupList.Add(newGroup);
            hasChangesGroups = true;
            return newGroup;
        }
        List<Group> groupList;
        Feed<Group> groupFeed;
        /// <summary>
        /// Acesso read-only à lista de contactos.
        /// </summary>
        /// <remarks>
        /// Ver remarks da propriedade GroupEntriesList
        /// </remarks>
        public IList<Contact> ContactEntriesList
        {
            get
            {
                return contactList.AsReadOnly();
            }
        }
        /// <summary>
        /// Adiciona um item à lista de contactos em memória. Fica com um id provisório.
        /// </summary>
        /// <returns>
        /// O id provisório.
        /// </returns>
        public Contact AddContact(Contact newContact)
        {
            newContact.Id = new Uri(string.Format("http://localhost/{0}", Guid.NewGuid().ToString())).AbsoluteUri;
            newContact.BatchData = new GDataBatchEntryData(newContact.Id, GDataBatchOperationType.insert);
            contactList.Add(newContact);
            hasChangesContacts = true;
            return newContact;
        }

        /// <summary>
        /// Importa um contacto de uma conta para esta. Acrescenta sempre um grupo "importado de ... em ..."
        /// </summary>
        /// <param name="contatoOriginal">O contacto a importar</param>
        /// <param name="documentoOrigem">O documento origem</param>
        /// <returns>O novo contacto criado.</returns>
        public Contact ImportContact(Contact contatoOriginal, GoogleContactsDoc documentoOrigem)
        {
            /// Duplica-se o contacto
            Contact contatoDestino = contatoOriginal.Duplicate();
            
            // remove-se o GroupMembership porque o id dos grupos varia sempre por conta
            contatoDestino.GroupMembership.Clear();

            // cria-se um grupo para identificar os contactos importados (se ainda não existir)
            string nome_grupo = string.Format("Importado de {0} em {1:d}", documentoOrigem.ContaId, DateTime.Today);
            Group importGroup = groupList.Find(g => g.Title == nome_grupo);
            if (importGroup == null)
            {
                importGroup = AddGroup(new Group()
                {
                    Title = nome_grupo
                });
            }
            contatoDestino.GroupMembership.Add(new GroupMembership()
            {
                HRef = importGroup.Id
            });
            AddContact(contatoDestino);
            return contatoDestino;
        }

        /// <summary>
        /// Marca um contacto para ser atualizado quando for feita a gravação.
        /// Permite a adição ou remoção de um grupo.
        /// </summary>
        /// <returns>O próprio contacto</returns>
        public Contact UpdateContact(Contact existingContact, Group grupo = null, bool adicionar = true)
        {
            if (grupo != null)
            {
                GroupMembership gmExiste = existingContact.GroupMembership.FirstOrDefault(gm => gm.HRef == grupo.Id);
                if (adicionar && gmExiste == null)
                {
                    existingContact.GroupMembership.Add(
                        new GroupMembership()
                        {
                            HRef = grupo.Id
                        });
                }
                else if (!adicionar && gmExiste != null)
                {
                    existingContact.GroupMembership.Remove(gmExiste);
                }
            }
            if (existingContact.BatchData == null)
            {
                // se não for null: ou já foi marcado com Update ou já foi marcado com Insert (contacto novo)
                // em qualquer dos casos não há nada mais a fazer.
                existingContact.BatchData = new Google.GData.Client.GDataBatchEntryData(existingContact.Id, GDataBatchOperationType.update);
            }
            hasChangesContacts = true;
            return existingContact;
        }
        List<Contact> contactList;
        Feed<Contact> contactFeed;
        /// <summary>
        /// Informa do estado do documento: online ou offline
        /// </summary>
        public GoogleContactsDocStatusEnum Status
        {
            get
            {
                return kontag.Connected 
                    ? GoogleContactsDocStatusEnum.Online 
                    : GoogleContactsDocStatusEnum.Offline;
            }
        }
        /// <summary>
        /// Id do documento - pode ser o id da conta ou a pasta
        /// </summary>
        public string ContaId
        {
            get
            {
                return kontag.Connected ? kontag.Email : directory; 
            }
        }
        /// <summary>
        /// Retorna true se o documento tem alterações
        /// </summary>
        public bool TemAlteracoes
        {
            get { return hasChangesGroups || hasChangesContacts; }
        }
        //
        private bool hasChangesGroups;
        private bool hasChangesContacts;
        private GoogleContactsAccount kontag;
        private string directory;
        private GoogleContactsDoc()
        {
            kontag = new GoogleContactsAccount();
            directory = "";
            hasChangesGroups = false;
            hasChangesContacts = false;
        }
        private ContactsRequest contReq = null;
        private ContactsRequest contactsRequest
        {
            get
            {
                if (contReq == null)
                {
                    if (kontag.Connected)
                    {
                        contReq = new ContactsRequest(new RequestSettings(GoogleContactsManagerClientId.ApplicationName, kontag.ParametersAuth));
                    }
                    else
                    {
                        contReq = new ContactsRequest(new RequestSettings(GoogleContactsManagerClientId.ApplicationName));
                    }
                }
                return contReq;
            }
        }
    }
}
