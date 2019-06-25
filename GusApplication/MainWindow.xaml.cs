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
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
public partial class MainWindow : Window
{
    Połącz połącz = new Połącz(new UslugaBIRzewnPublClient());
    string sid;
    public MainWindow()
    {
        InitializeComponent();
        sid = połącz.Zaloguj();
    }

    //Przekazanie numeru NIP do obiektu, którego metoda wyszukiwania Gusu odczytuje
    public ParametryWyszukiwania GetParametry(string nip)
    {
        return new ParametryWyszukiwania
        {
            Nip = nip
        };
    }

    //Pobranie wartości zawartej między znacznikiem podanym jako parametr
    public string TekstElementuXMl(XmlDocument doc, string element)
    {
        return doc.SelectSingleNode("root/dane/" + element).InnerText;
    }

    //Sprawdzanie czy Firma o podanym numerze NIP istnieje, przz sprawdzenie nazwy pierwszego wewnętrznego znacznika
    public bool CzyNipIstnieje(XmlDocument doc)
    {
        XmlNode pierwszyZnacznik = doc.SelectSingleNode("/root/dane[1]");
        if (pierwszyZnacznik.FirstChild.Name == "ErrorCode")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Przycisk 
    private void SzukajButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (nipTextBox.Text.Length != 10) //Sprawdzanie czy podany łańcuch ma 10 znaków
            {
                MessageBox.Show("Numer NIP musi mieć 10 znaków");
            }
            else
            {
                ParametryWyszukiwania parametr = GetParametry(nipTextBox.Text);
                var firma = połącz.Klient.DaneSzukajPodmioty(parametr);
                XmlDocument firmaXML = new XmlDocument();
                firmaXML.LoadXml(firma);

                if (CzyNipIstnieje(firmaXML)) //Wypełnianie okna informacjami o firmie
                {
                    regonLabel.Content = TekstElementuXMl(firmaXML, "Regon");
                    nazwaTexTBlock.Text = TekstElementuXMl(firmaXML, "Nazwa");
                    województwoLabel.Content = TekstElementuXMl(firmaXML, "Wojewodztwo");
                    adresLabel.Content = TekstElementuXMl(firmaXML, "Miejscowosc") + " " + TekstElementuXMl(firmaXML, "Ulica") + " " + TekstElementuXMl(firmaXML, "NrNieruchomosci");
                    gminaLabel.Content = TekstElementuXMl(firmaXML, "Gmina");
                    typLabel.Content = TekstElementuXMl(firmaXML, "Typ");
                    kodPocztowyLabel.Content = TekstElementuXMl(firmaXML, "KodPocztowy");
                    powiatLabel.Content = TekstElementuXMl(firmaXML, "Powiat");
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
