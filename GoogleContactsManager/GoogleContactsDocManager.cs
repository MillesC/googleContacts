using Google.Contacts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleContactsManager
{
    /// <summary>
    /// Gestão da lista de documentos
    /// </summary>
    class GoogleContactsDocList : SortableBindableList<GoogleContactsDoc>
    {
        static GoogleContactsDocList documentList = new GoogleContactsDocList();
        internal static GoogleContactsDocList DocumentList
        {
            get { return GoogleContactsDocList.documentList; }
        }

        /// <summary>
        /// Adiciona um documento à lista a partir de uma pasta
        /// </summary>
        /// <param name="path"></param>
        public static GoogleContactsDoc AddPath(string path)
        {
            if (GoogleContactsDocList.DocumentList.Any<GoogleContactsDoc>(d => d.ContaId == path && d.Status == GoogleContactsDocStatusEnum.Offline))
            {
                throw new GoogleContactsException("O item escolhido já se encontra na lista.");
            }
            GoogleContactsDoc newDoc = GoogleContactsDoc.CreateFromDisk(path);
            GoogleContactsDocList.DocumentList.Add(newDoc);
            return newDoc;
        }

        /// <summary>
        /// Adiciona um documento à lista depois do utilizador dar autorização
        /// </summary>
        public static async Task<GoogleContactsDoc> AddGoogle(CancellationToken cancelToken)
        {
            GoogleContactsDoc newDoc = await GoogleContactsDoc.CreateNewAndFillAsync(cancelToken).ConfigureAwait(false);
            if (GoogleContactsDocList.DocumentList.Any<GoogleContactsDoc>(d => d.ContaId == newDoc.ContaId && d.Status == GoogleContactsDocStatusEnum.Online))
            {
                throw new GoogleContactsException("A conta escolhida já se encontra na lista.");
            }
            GoogleContactsDocList.DocumentList.Add(newDoc);
            return newDoc;
        }

        public static string GetDefaultPathForFiles()
        {
            string p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ContactosGoogle");
            Directory.CreateDirectory(p);
            return p;
        }
    }

    /// <summary>
    /// Uma classe para usar nos formulários de consulta
    /// </summary>
    public class GroupList : SortableBindableList<Group>
    {
        public GroupList(IList<Group> list)
            : base(list)
        {
        }
    }

}
