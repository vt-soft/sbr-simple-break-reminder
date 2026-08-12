using SBR.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBR;

public static class LangManager
{
   
    // Reference to the main form, so we can access its controls and change their text to the selected language
    private static MainForm mainForm;

    // We store all UserControl (Forms) in this dictionary.
    private static Dictionary<string, UserControl> screens = new Dictionary<string, UserControl>();

    // Here are stored translations for selected language. Key is the ID from .json file.
    private static Dictionary<string, string> translations = new Dictionary<string, string>();



    /// <summary>
    /// This method will initialize the LangChanger class with neccesarry refereces to mainForm and other Forms.
    /// </summary>
    /// <param name="aMainForm"></param>
    /// <param name="aScreens"></param>
    public static void Init(MainForm aMainForm, Dictionary<string,UserControl> aScreens)
    {
        mainForm = aMainForm;
        screens = aScreens;  
    }

    /// <summary>
    /// This method will update all text in the application to the selected language (from .json file).
    /// </summary>
    /// <param name="langCode"></param>
    public static void SetLanguage(string langCode)
    {
        // Debug.Print("(LangManager) Changing language to: " + langCode);

        // Load the JSON file for the selected language and populate the
        // "translations" dictionary with this new language
        LoadJsonLanguageFile(langCode);

        // Set proper language (based on the landCode) for MainForm.
        mainForm.ChangeLanguage();

        // Set proper language (based on the landCode) for all remaining Forms (User Controls).
        ((UcAlarm)screens["btnAlarm"]).ChangeLanguage();
        ((UcSettings)screens["btnSettings"]).ChangeLanguage();
        ((UcStatistics)screens["btnStatistics"]).ChangeLanguage();
        ((UcLanguage)screens["btnLanguage"]).ChangeLanguage();
        ((UcAbout)screens["btnAbout"]).ChangeLanguage();
    }


    /// <summary>
    /// Gets the translation for the given key from the "translations" dictionary.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string? GetString(string id)
    {
        return translations.TryGetValue(id, out var value) ? value : "NOT FOUND!";
    }



    /// <summary>
    /// Loads the JSON file for the selected language and populates the "translations" dictionary.
    /// </summary>
    /// <param name="langCode"></param>
    public static void LoadJsonLanguageFile(string langCode)
    {
        translations.Clear(); // Clear previous translations

        try
        {
            // Construct the path to the JSON file: Languages\{langCode}.json
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(appDirectory, "Languages", $"{langCode}.json");

            // Check if file exists
            if (!File.Exists(jsonFilePath))
            {
                MessageBox.Show($"Language file not found: {jsonFilePath}");
                return;
            }

            // Read the JSON file
            string jsonContent = File.ReadAllText(jsonFilePath);

            // Parse JSON and extract id and translated_string
            using (JsonDocument doc = JsonDocument.Parse(jsonContent))
            {
                JsonElement root = doc.RootElement;

                // Handle if root is an array
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in root.EnumerateArray())
                    {
                        if (item.TryGetProperty("id", out JsonElement idElement) &&
                            item.TryGetProperty("translated_string", out JsonElement translatedElement))
                        {
                            string id = idElement.GetString();
                            string translatedString = translatedElement.GetString();

                            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(translatedString))
                            {
                                translations[id] = translatedString;
                            }
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading JSON file: {ex.Message}");
        }
    }










}
