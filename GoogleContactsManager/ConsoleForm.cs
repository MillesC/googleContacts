using Google.Contacts;
using Google.GData.Contacts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleContactsManager
{
    public partial class ConsoleForm : Form
    {
        public ConsoleForm()
        {
            InitializeComponent();
        }
        #region Write/WriteLine
        void Write(string text)
        {
            consoleTextBox.Text += text;
            consoleTextBox.SelectionStart = consoleTextBox.Text.Length;
            consoleTextBox.Refresh();
            Application.DoEvents();
        }
        void Write(string format, params object[] arg)
        {
            Write(string.Format(format, arg));
        }
        void WriteLine()
        {
            Write(Environment.NewLine);
        }
        void WriteLine(string text)
        {
            Write(text);
            WriteLine();
        }
        void WriteLine(string format, params object[] arg)
        {
            Write(string.Format(format, arg));
            WriteLine();
        }
        #endregion        
        private void limparToolStripMenuItem_Click(object sender, EventArgs e)
        {
            consoleTextBox.Text = "";
            consoleTextBox.Refresh();
            Application.DoEvents();
        }
        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private async void abrirContaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            try
            {
                WriteLine("A criar novo documento com autorização...");
                Task time = Task.Delay(60000, cancellationTokenSource.Token);
                Task main = GoogleContactsDocList.AddGoogle(cancellationTokenSource.Token);
                var r = await Task.WhenAny(main, time);
                if (r.Status == TaskStatus.Canceled)
                {
                    throw new OperationCanceledException();
                }
                else if (time.Status == TaskStatus.RanToCompletion)
                {
                    throw new GoogleContactsException("Foi ultrapassado o tempo destinado a esta tarefa (60 segundos)");
                }
                WriteLine("Documentos abertos: {0}", GoogleContactsDocList.DocumentList.Count);
            }
            catch (Exception ex)
            {
                cancellationTokenSource.Cancel(); // para cancelar as tasks em execução...
                WriteLine(ex.ToString());
            }
        }

        private void printGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GoogleContactsDocList.DocumentList.Count == 0)
                {
                    MessageBox.Show("Ainda não existe nenhum documento aberto.", "Contas", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                GoogleContactsDoc d = GoogleContactsDocList.DocumentList[0];
                foreach (Group g in d.GroupEntriesList)
                {
                    WriteLine(g.Title);
                    WriteLine(g.Id);
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }
        static void teste_PrintGroups(GoogleContactsDoc d)
        {
            foreach (Group g in d.GroupEntriesList)
            {
                Console.WriteLine(g.Title);
                Console.WriteLine(g.Id);
            }
        }
        static void teste_PrintContact(Contact c, GoogleContactsDoc d)
        {
            Console.WriteLine(c.Name.FullName);
            Console.WriteLine(c.PrimaryEmail.Address);
            Console.WriteLine(c.Id);
            Console.WriteLine("\t* GRUPOS *");
            foreach (GroupMembership gm in c.GroupMembership)
            {
                Console.WriteLine("\t{0}", gm.HRef);
                Console.WriteLine("\t{0}", d.GroupEntriesList.First(g => gm.HRef == g.Id).Title);
            }
            Console.WriteLine();
        }
        static void teste_CopiaContactoWeb()
        {
            GoogleContactsDoc origem = GoogleContactsDocList.AddGoogle(CancellationToken.None).Result;
            Console.WriteLine("origem {0}", origem.ContaId);
            GoogleContactsDoc destino = GoogleContactsDocList.AddGoogle(CancellationToken.None).Result;
            Console.WriteLine("destino {0}", destino.ContaId);
            Console.WriteLine();

            Contact contatoOrigem = origem.ContactEntriesList[5];
            Contact contatoDestino = destino.ImportContact(contatoOrigem, origem);

            teste_PrintContact(contatoOrigem, origem);

            teste_PrintContact(contatoDestino, destino);

            Console.WriteLine("destino.SaveToGoogle()={0}", destino.SaveToGoogle().Result);

            teste_PrintContact(contatoDestino, destino);

        }
        static void teste_CriarMuitosContactos()
        {
            GoogleContactsDoc d = GoogleContactsDocList.AddGoogle(CancellationToken.None).Result;
            for (int i = 302; i < 420; i++)
            {
                d.AddContact(teste_CriarContacto(i.ToString()));
            }
            Console.WriteLine("d.SaveToGoogle()={0}", d.SaveToGoogle().Result);
        }
        static Contact teste_CriarContacto(string nomeProprio)
        {
            Contact newEntry = new Contact();
            newEntry.Name = new Google.GData.Extensions.Name()
            {
                FullName = nomeProprio + " Teste",
                GivenName = nomeProprio,
                FamilyName = "Teste",
            };
            newEntry.Content = "Notes";
            newEntry.Emails.Add(new Google.GData.Extensions.EMail()
            {
                Primary = true,
                Rel = Google.GData.Extensions.ContactsRelationships.IsHome,
                Address = nomeProprio + ".teste@teste.zzz"
            });
            newEntry.Phonenumbers.Add(new Google.GData.Extensions.PhoneNumber()
            {
                Primary = true,
                Rel = Google.GData.Extensions.ContactsRelationships.IsWork,
                Value = "(206)555-1212",
            });
            newEntry.PostalAddresses.Add(new Google.GData.Extensions.StructuredPostalAddress()
            {
                Rel = Google.GData.Extensions.ContactsRelationships.IsHome,
                Primary = true,
                Street = "Rua de Costa Cabral, 2219",
                City = "Porto",
                Postcode = "4200-230",
                Country = "Portugal",
                FormattedAddress = "Rua de Costa Cabral, 2219",
            });
            return newEntry;
        }
        static void teste_CopiaContactoDisco()
        {
            GoogleContactsDoc origem = GoogleContactsDocList.AddPath(@"C:\Users\manuel\Documents\ContactosGoogle\c139adm@gmail.com");
            Console.WriteLine("origem {0}", origem.ContaId);
            GoogleContactsDoc destino = GoogleContactsDocList.AddPath(@"C:\Users\manuel\Documents\ContactosGoogle\gtmaltes865@gmail.com");
            Console.WriteLine("destino {0}", destino.ContaId);
            Console.WriteLine();

            //Contact contatoOrigem = origem.ContactEntriesList.First(c => c.Name.FullName == "Conselho 139 Admi");
            Contact contatoOrigem = origem.ContactEntriesList[5];
            Contact contatoDestino = destino.ImportContact(contatoOrigem, origem);


            teste_PrintContact(contatoOrigem, origem);

            teste_PrintContact(contatoDestino, destino);

        }
        static void teste_CriarGruposEContactos()
        {
            GoogleContactsDoc d = GoogleContactsDocList.AddGoogle(CancellationToken.None).Result;
            Group ng1 = d.AddGroup(new Group()
            {
                Title = "Salsa"
            });
            Group ng2 = d.AddGroup(new Group()
            {
                Title = "Merengue"
            });
            //
            Contact novo = new Contact()
            {
                Name = new Google.GData.Extensions.Name()
                {
                    FullName = "Teste 1",
                    GivenName = "Teste",
                    FamilyName = "1"
                }
            };
            Contact x = d.AddContact(novo);
            d.UpdateContact(x, ng1);
            //
            d.UpdateContact(d.ContactEntriesList[0], ng1);
            d.UpdateContact(d.ContactEntriesList[0], ng1);
            d.UpdateContact(d.ContactEntriesList[0], ng2);
            //
            teste_PrintGroups(d);
            teste_PrintContact(novo, d);
            teste_PrintContact(d.ContactEntriesList[0], d);
            Console.WriteLine();
            Console.WriteLine("d.SaveToGoogle()={0}", d.SaveToGoogle().Result);
            Console.WriteLine();
            teste_PrintGroups(d);
            teste_PrintContact(novo, d);
            teste_PrintContact(d.ContactEntriesList[0], d);
            Console.WriteLine();
        }

    
    }
}
