using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using GusApplication.GusAPI;
using GusApplication.Data;

namespace GusApplication
{
    public partial class MainWindow : Window
    {
        Połącz połącz = new Połącz(new UslugaBIRzewnPublClient());
        string sid;
        public MainWindow()
        {
            InitializeComponent();
            sid = połącz.Zaloguj();
        }

        //Przekazanie numeru NIP do obiektu 
        public ParametryWyszukiwania PobierzParametry(string nip)
        {
            return new ParametryWyszukiwania
            {
                Nip = nip
            };
        }

        //Pobranie wartości zawartej między znacznikiem podanym jako parametr
        public string TekstZnacznikaXmlFirmy(XmlDocument xmlFirmy, string nazwaZnacznika)
        {
            return xmlFirmy.SelectSingleNode("root/dane/" + nazwaZnacznika).InnerText;
        }

        //Sprawdzanie czy Firma o podanym numerze NIP istnieje, przez sprawdzenie nazwy pierwszego wewnętrznego znacznika
        public bool CzyNipIstnieje(XmlDocument xmlFirmy)
        {
            XmlNode pierwszyZnacznik = xmlFirmy.SelectSingleNode("/root/dane[1]");
            if (pierwszyZnacznik.FirstChild.Name == "ErrorCode")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Zdarzenie obsługujące przycisk szukania
        private void SzukajButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (nipTextBox.Text.Length != 10) //Sprawdzanie czy podany łańcuch w TextBoxie ma 10 znaków
                {
                    MessageBox.Show("Numer NIP musi mieć 10 znaków");
                }
                else
                {
                    ParametryWyszukiwania parametryFirmy = PobierzParametry(nipTextBox.Text);
                    var firma = połącz.Klient.DaneSzukajPodmioty(parametryFirmy);
                    XmlDocument xmlFirmy = new XmlDocument();
                    xmlFirmy.LoadXml(firma);

                    if (CzyNipIstnieje(xmlFirmy)) //Wypełnianie okna informacjami o firmie
                    {
                        regonLabel.Content = TekstZnacznikaXmlFirmy(xmlFirmy, "Regon");
                        nazwaTexTBlock.Text = TekstZnacznikaXmlFirmy(xmlFirmy, "Nazwa");
                        województwoLabel.Content = TekstZnacznikaXmlFirmy(xmlFirmy, "Wojewodztwo");
                        adresLabel.Content = TekstZnacznikaXmlFirmy(xmlFirmy, "Miejscowosc") + ", " + TekstZnacznikaXmlFirmy(xmlFirmy, "Ulica") + " " + TekstZnacznikaXmlFirmy(xmlFirmy, "NrNieruchomosci");
                        gminaLabel.Content = TekstZnacznikaXmlFirmy(xmlFirmy, "Gmina");
                        typLabel.Content = TekstZnacznikaXmlFirmy(xmlFirmy, "Typ");
                        kodPocztowyLabel.Content = TekstZnacznikaXmlFirmy(xmlFirmy, "KodPocztowy");
                        powiatLabel.Content = TekstZnacznikaXmlFirmy(xmlFirmy, "Powiat");
                    }
                    else
                    {
                        MessageBox.Show("Brak firmy o podanym numerze NIP");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            połącz.Wylogoj(sid);
        }
    }
}
