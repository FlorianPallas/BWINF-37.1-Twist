using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Twist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string TextTwist = String.Empty;
        private string TextEnttwist = String.Empty;
        private string[] Woerterliste;

        public MainWindow()
        {
            InitializeComponent();
        }

        // +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+

        private bool DateiEinlesen(out string Text, string Titel)
        {
            OpenFileDialog Dialog = new OpenFileDialog()
            {
                DefaultExt = ".txt",
                Filter = "Textdateien|*.txt",
                Multiselect = false,
                CheckFileExists = true,
                Title = Titel
            };

            if(Dialog.ShowDialog() == true)
            {
                Text = File.ReadAllText(Dialog.FileName);
                return true;
            }
            else
            {
                Text = String.Empty;
                return false;
            }
        }

        private bool Twist(string Text, out string TextTwisted)
        {
            StringBuilder SB = new StringBuilder(Text);

            List<int> BuchstabenIndizes = new List<int>();

            // Text druchlaufen
            for(int I = 0; I < SB.Length; I++)
            {
                if(Char.IsLetter(SB[I]))
                {
                    BuchstabenIndizes.Add(I);
                }
                else
                {
                    if (BuchstabenIndizes.Count - 2 < 2) { BuchstabenIndizes.Clear(); continue; }

                    // Letzten und ersten Buchstabe entfernen
                    BuchstabenIndizes.RemoveAt(BuchstabenIndizes.Count - 1);
                    BuchstabenIndizes.RemoveAt(0);

                    // Indizes twisten
                    char[] Buchstaben = new char[BuchstabenIndizes.Count];
                    for(int J = 0; J < BuchstabenIndizes.Count; J++)
                    {
                        Buchstaben[J] = SB[BuchstabenIndizes[J]];
                    }

                    Twist_Mix(ref Buchstaben);

                    for (int J = 0; J < BuchstabenIndizes.Count; J++)
                    {
                        SB[BuchstabenIndizes[J]] = Buchstaben[J];
                    }

                    BuchstabenIndizes.Clear();
                }
            }

            TextTwisted = SB.ToString();
            return true;
        }

        private void Twist_Mix(ref char[] Buchstaben)
        {
            // Knuth-Fisher-Yates-Shuffle

            // Random Objekt mit zufälligem Seed
            Random RDM = new Random(DateTime.Now.Millisecond);

            for(int I = Buchstaben.Length - 1; I > 0; I--)
            {
                // Index zwischen 0 und I
                int J = RDM.Next(0, I);

                // I und J tauschen
                char Temp = Buchstaben[I];
                Buchstaben[I] = Buchstaben[J];
                Buchstaben[J] = Temp;
            }
        }

        private bool Enttwist(string Text, out string TextEnttwisted)
        {
            StringBuilder SB = new StringBuilder(Text);

            List<int> BuchstabenIndizes = new List<int>();

            // Text druchlaufen
            for (int I = 0; I < SB.Length; I++)
            {
                if (Char.IsLetter(SB[I]))
                {
                    BuchstabenIndizes.Add(I);
                }
                else
                {
                    if (BuchstabenIndizes.Count < 4) { BuchstabenIndizes.Clear(); continue; }

                    // Buchstaben Array setzen
                    char[] Buchstaben = new char[BuchstabenIndizes.Count];
                    for (int J = 0; J < BuchstabenIndizes.Count; J++)
                    {
                        Buchstaben[J] = SB[BuchstabenIndizes[J]];
                    }

                    bool WortGefunden = false;

                    foreach(string Wort in Woerterliste)
                    {
                        if(Entttwist_Passend(new string(Buchstaben), Wort))
                        {
                            for (int J = 1; J < Buchstaben.Length-1; J++)
                            {
                                Buchstaben[J] = Wort[J];
                            }

                            WortGefunden = true;
                            break;
                        }
                    }

                    // Kein passendes Wort gefunden
                    if(!WortGefunden)
                    {
                        for (int J = 0; J < Buchstaben.Length; J++)
                        {
                            Buchstaben[J] = '?';
                        }
                    }

                    // String zusammenfügen
                    for (int J = 0; J < BuchstabenIndizes.Count; J++)
                    {
                        SB[BuchstabenIndizes[J]] = Buchstaben[J];
                    }

                    BuchstabenIndizes.Clear();
                }
            }

            TextEnttwisted = SB.ToString();
            return true;
        }

        private bool Entttwist_Passend(string WortZiel, string Wort)
        {
            // Das Wort muss gleich lang sein
            if (Wort.Length != WortZiel.Length) { return false; }

            // Der erste und der letzte Buchstabe muss übereinstimmen
            if (Char.ToLower(Wort[0]) != Char.ToLower(WortZiel[0])) { return false; }
            if (Char.ToLower(Wort[WortZiel.Length - 1]) != Char.ToLower(WortZiel[WortZiel.Length - 1])) { return false; }

            var WortZielListe = WortZiel.ToList();
            var WortListe = Wort.ToList();

            // Das Wort muss aus den gleichen Buchstaben bestehen
            foreach(char Buchstabe in WortZielListe)
            {
                int Index = WortListe.FindIndex(a => a == Char.ToLower(Buchstabe));
                if (Index == -1) { return false; }
                WortListe.RemoveAt(Index);
            }

            return true;
        }

        // +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+

        private async void ButtonTwist_Click(object sender, RoutedEventArgs e)
        {
            TextBoxEnttwist.Text = String.Empty;
            await Task.Delay(100);

            Twist(TextTwist, out string TextTwisted);

            TextBoxEnttwist.Text = TextTwisted;
            TextEnttwist = TextTwisted;
        }

        private async void ButtonEnttwist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bitte wählen Sie nun die Wörterbuch-Datei aus.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            DateiEinlesen(out string Woerterbuch, "Wörterbuch-Datei auswählen...");

            Woerterliste = Woerterbuch.Split('\n');

            for(int I = 0; I < Woerterliste.Length; I++)
            {
                Woerterliste[I] = Woerterliste[I].ToLower();
            }

            TextBoxTwist.Text = String.Empty;
            await Task.Delay(100);

            Enttwist(TextEnttwist, out string TextEnttwisted);

            TextBoxTwist.Text = TextEnttwisted;
            TextTwist = TextEnttwisted;
        }

        private void ButtonOeffnenTwist_Click(object sender, RoutedEventArgs e)
        {
            DateiEinlesen(out string DateiInhalt, "Twist-Datei auswählen...");

            TextBoxTwist.Text = DateiInhalt;
            TextTwist = DateiInhalt;
        }

        private void ButtonOeffnenEnttwist_Click(object sender, RoutedEventArgs e)
        {
            DateiEinlesen(out string DateiInhalt, "Enttwist-Datei auswählen...");

            TextBoxEnttwist.Text = DateiInhalt;
            TextEnttwist = DateiInhalt;
        }
    }
}
