using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Figgle;
using Gma.System.MouseKeyHook;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace KeyboardSounds
{
    internal class Program
    {
        static Random random = new Random();
        static IKeyboardMouseEvents globalHook;
        static WaveOutEvent waveOutEvent;
        static AudioFileReader audioFileReader;
        static readonly object lockObject = new object();

        [STAThread]
        static void Main()
        {
            string asciiArt = FiggleFonts.Standard.Render("Jan2k17");
            Console.SetWindowSize(Math.Min(120, Console.LargestWindowWidth), Math.Min(30, Console.LargestWindowHeight));
            Console.WriteLine(asciiArt);
            Console.WriteLine("FurryBook.de");

            globalHook = Hook.GlobalEvents();
            globalHook.KeyDown += GlobalHook_KeyDown;
            Application.Run();
            globalHook.KeyDown -= GlobalHook_KeyDown;
            globalHook.Dispose();
        }

        /*private static void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            string currentFolder = GetCurrentDirectory();
            string soundFilePath = Path.Combine(currentFolder, "Soundfile.wav");
            PlaySoundWithoutPitch(soundFilePath);
        }*/

        private static void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            string currentFolder = GetCurrentDirectory();
            string soundFilePath;

            if (e.Alt && e.KeyCode == Keys.F4) // ALT+F4
            {
                soundFilePath = Path.Combine(currentFolder, "AltF4Sound.wav");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Space)
            {
                soundFilePath = Path.Combine(currentFolder, "SpaceSound.wav");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                soundFilePath = Path.Combine(currentFolder, "EnterSound.wav");
            }
            else
            {
                soundFilePath = Path.Combine(currentFolder, "Soundfile.wav");
            }

            PlaySoundWithoutPitch(soundFilePath);
        }

        static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        static void PlaySoundWithoutPitch(string filePath)
        {
            lock (lockObject)
            {
                try
                {
                    // Falls bereits eine Wiedergabe läuft, diese stoppen und Ressourcen freigeben
                    if (waveOutEvent != null)
                    {
                        waveOutEvent.Stop();
                        waveOutEvent.Dispose();
                        audioFileReader.Dispose();
                    }

                    // Neue Instanzen für die aktuelle Wiedergabe erstellen
                    audioFileReader = new AudioFileReader(filePath);
                    waveOutEvent = new WaveOutEvent();
                    waveOutEvent.Init(audioFileReader);
                    waveOutEvent.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Abspielen des Sounds: " + ex.Message);
                }
            }
        }
    }
}
