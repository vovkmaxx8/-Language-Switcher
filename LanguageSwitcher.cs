// LanguageSwitcher.cs
// Language Switcher (Auto-detect) на C#

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class LanguageSwitcher
{
    private static Dictionary<string, string> rusToEng = new Dictionary<string, string>();
    private static Dictionary<string, string> engToRus = new Dictionary<string, string>();

    static LanguageSwitcher()
    {
        string[][] rusToEngData = {
            new[]{"а","a"}, new[]{"б","b"}, new[]{"в","v"}, new[]{"г","g"}, new[]{"д","d"},
            new[]{"е","e"}, new[]{"ё","yo"}, new[]{"ж","zh"}, new[]{"з","z"}, new[]{"и","i"},
            new[]{"й","y"}, new[]{"к","k"}, new[]{"л","l"}, new[]{"м","m"}, new[]{"н","n"},
            new[]{"о","o"}, new[]{"п","p"}, new[]{"р","r"}, new[]{"с","s"}, new[]{"т","t"},
            new[]{"у","u"}, new[]{"ф","f"}, new[]{"х","kh"}, new[]{"ц","ts"}, new[]{"ч","ch"},
            new[]{"ш","sh"}, new[]{"щ","shch"}, new[]{"ъ",""}, new[]{"ы","y"}, new[]{"ь",""},
            new[]{"э","e"}, new[]{"ю","yu"}, new[]{"я","ya"},
            new[]{"А","A"}, new[]{"Б","B"}, new[]{"В","V"}, new[]{"Г","G"}, new[]{"Д","D"},
            new[]{"Е","E"}, new[]{"Ё","Yo"}, new[]{"Ж","Zh"}, new[]{"З","Z"}, new[]{"И","I"},
            new[]{"Й","Y"}, new[]{"К","K"}, new[]{"Л","L"}, new[]{"М","M"}, new[]{"Н","N"},
            new[]{"О","O"}, new[]{"П","P"}, new[]{"Р","R"}, new[]{"С","S"}, new[]{"Т","T"},
            new[]{"У","U"}, new[]{"Ф","F"}, new[]{"Х","Kh"}, new[]{"Ц","Ts"}, new[]{"Ч","Ch"},
            new[]{"Ш","Sh"}, new[]{"Щ","Shch"}, new[]{"Ъ",""}, new[]{"Ы","Y"}, new[]{"Ь",""},
            new[]{"Э","E"}, new[]{"Ю","Yu"}, new[]{"Я","Ya"}
        };
        foreach (var pair in rusToEngData) rusToEng[pair[0]] = pair[1];
        foreach (var kv in rusToEng) engToRus[kv.Value] = kv.Key;

        // Дополнительные составные
        string[][] extra = {
            new[]{"yo","ё"}, new[]{"zh","ж"}, new[]{"kh","х"}, new[]{"ts","ц"}, new[]{"ch","ч"},
            new[]{"sh","ш"}, new[]{"shch","щ"}, new[]{"yu","ю"}, new[]{"ya","я"},
            new[]{"Yo","Ё"}, new[]{"Zh","Ж"}, new[]{"Kh","Х"}, new[]{"Ts","Ц"}, new[]{"Ch","Ч"},
            new[]{"Sh","Ш"}, new[]{"Shch","Щ"}, new[]{"Yu","Ю"}, new[]{"Ya","Я"}
        };
        foreach (var pair in extra) engToRus[pair[0]] = pair[1];
    }

    static string DetectLanguage(string text)
    {
        int rusCount = 0, engCount = 0;
        foreach (char c in text)
        {
            if (rusToEng.ContainsKey(c.ToString())) rusCount++;
            else if (char.IsLetter(c) && !rusToEng.ContainsKey(c.ToString())) engCount++;
        }
        if (rusCount > engCount) return "ru";
        else if (engCount > rusCount) return "en";
        else return "unknown";
    }

    static string Transliterate(string text, string direction)
    {
        if (direction == "auto")
        {
            string lang = DetectLanguage(text);
            if (lang == "ru") direction = "ru_to_en";
            else if (lang == "en") direction = "en_to_ru";
            else return text;
        }
        var dict = direction == "ru_to_en" ? rusToEng : engToRus;
        StringBuilder result = new StringBuilder();
        int i = 0;
        while (i < text.Length)
        {
            bool found = false;
            for (int len = 5; len > 0; len--)
            {
                if (i + len <= text.Length)
                {
                    string sub = text.Substring(i, len);
                    if (dict.ContainsKey(sub))
                    {
                        result.Append(dict[sub]);
                        i += len;
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
            {
                result.Append(text[i]);
                i++;
            }
        }
        return result.ToString();
    }

    static void Main(string[] args)
    {
        bool reverse = false, detect = false;
        string text = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-r" || args[i] == "--reverse") reverse = true;
            else if (args[i] == "-d" || args[i] == "--detect") detect = true;
            else if (args[i] == "-h" || args[i] == "--help")
            {
                Console.WriteLine(@"Использование: LanguageSwitcher [опции] [текст]
  -r, --reverse   Принудительно переключить в другую раскладку (инвертировать)
  -d, --detect    Только определить язык (ru / en)
  -h, --help      Показать справку");
                return;
            }
            else
            {
                text = string.Join(" ", args.Skip(i));
                break;
            }
        }

        if (text == null)
        {
            // Читаем из stdin
            using (var reader = new StreamReader(Console.OpenStandardInput()))
            {
                text = reader.ReadToEnd().Trim();
            }
        }

        if (string.IsNullOrEmpty(text))
        {
            Console.Error.WriteLine("Ошибка: не введён текст");
            Environment.Exit(1);
        }

        if (detect)
        {
            Console.WriteLine(DetectLanguage(text));
            return;
        }

        string direction = "auto";
        if (reverse)
        {
            string lang = DetectLanguage(text);
            if (lang == "ru") direction = "en_to_ru";
            else if (lang == "en") direction = "ru_to_en";
            else direction = "ru_to_en";
        }
        string result = Transliterate(text, direction);
        Console.WriteLine(result);
    }
}
