// LanguageSwitcher.java
// Language Switcher (Auto-detect) на Java

import java.io.*;
import java.util.*;

public class LanguageSwitcher {
    private static final Map<String, String> RUS_TO_ENG = new LinkedHashMap<>();
    private static final Map<String, String> ENG_TO_RUS = new LinkedHashMap<>();

    static {
        String[][] rusToEngData = {
            {"а","a"}, {"б","b"}, {"в","v"}, {"г","g"}, {"д","d"}, {"е","e"}, {"ё","yo"},
            {"ж","zh"}, {"з","z"}, {"и","i"}, {"й","y"}, {"к","k"}, {"л","l"}, {"м","m"},
            {"н","n"}, {"о","o"}, {"п","p"}, {"р","r"}, {"с","s"}, {"т","t"}, {"у","u"},
            {"ф","f"}, {"х","kh"}, {"ц","ts"}, {"ч","ch"}, {"ш","sh"}, {"щ","shch"},
            {"ъ",""}, {"ы","y"}, {"ь",""}, {"э","e"}, {"ю","yu"}, {"я","ya"},
            {"А","A"}, {"Б","B"}, {"В","V"}, {"Г","G"}, {"Д","D"}, {"Е","E"}, {"Ё","Yo"},
            {"Ж","Zh"}, {"З","Z"}, {"И","I"}, {"Й","Y"}, {"К","K"}, {"Л","L"}, {"М","M"},
            {"Н","N"}, {"О","O"}, {"П","P"}, {"Р","R"}, {"С","S"}, {"Т","T"}, {"У","U"},
            {"Ф","F"}, {"Х","Kh"}, {"Ц","Ts"}, {"Ч","Ch"}, {"Ш","Sh"}, {"Щ","Shch"},
            {"Ъ",""}, {"Ы","Y"}, {"Ь",""}, {"Э","E"}, {"Ю","Yu"}, {"Я","Ya"}
        };
        for (String[] pair : rusToEngData) {
            RUS_TO_ENG.put(pair[0], pair[1]);
        }
        // Обратный словарь
        for (Map.Entry<String, String> e : RUS_TO_ENG.entrySet()) {
            ENG_TO_RUS.put(e.getValue(), e.getKey());
        }
        // Дополнительные составные
        String[][] extra = {
            {"yo","ё"}, {"zh","ж"}, {"kh","х"}, {"ts","ц"}, {"ch","ч"},
            {"sh","ш"}, {"shch","щ"}, {"yu","ю"}, {"ya","я"},
            {"Yo","Ё"}, {"Zh","Ж"}, {"Kh","Х"}, {"Ts","Ц"}, {"Ch","Ч"},
            {"Sh","Ш"}, {"Shch","Щ"}, {"Yu","Ю"}, {"Ya","Я"}
        };
        for (String[] pair : extra) {
            ENG_TO_RUS.put(pair[0], pair[1]);
        }
    }

    private static String detectLanguage(String text) {
        int rusCount = 0, engCount = 0;
        for (char c : text.toCharArray()) {
            if (RUS_TO_ENG.containsKey(String.valueOf(c))) rusCount++;
            else if (Character.isLetter(c) && !RUS_TO_ENG.containsKey(String.valueOf(c))) engCount++;
        }
        if (rusCount > engCount) return "ru";
        else if (engCount > rusCount) return "en";
        else return "unknown";
    }

    private static String transliterate(String text, String direction) {
        if (direction.equals("auto")) {
            String lang = detectLanguage(text);
            if (lang.equals("ru")) direction = "ru_to_en";
            else if (lang.equals("en")) direction = "en_to_ru";
            else return text;
        }
        Map<String, String> dict = direction.equals("ru_to_en") ? RUS_TO_ENG : ENG_TO_RUS;
        StringBuilder result = new StringBuilder();
        int i = 0;
        while (i < text.length()) {
            boolean found = false;
            for (int len = 5; len > 0; len--) {
                if (i + len <= text.length()) {
                    String sub = text.substring(i, i + len);
                    if (dict.containsKey(sub)) {
                        result.append(dict.get(sub));
                        i += len;
                        found = true;
                        break;
                    }
                }
            }
            if (!found) {
                result.append(text.charAt(i));
                i++;
            }
        }
        return result.toString();
    }

    public static void main(String[] args) throws IOException {
        boolean reverse = false, detect = false;
        String text = null;

        List<String> argList = new ArrayList<>(Arrays.asList(args));
        for (int i = 0; i < argList.size(); i++) {
            String arg = argList.get(i);
            if (arg.equals("-r") || arg.equals("--reverse")) {
                reverse = true;
            } else if (arg.equals("-d") || arg.equals("--detect")) {
                detect = true;
            } else if (arg.equals("-h") || arg.equals("--help")) {
                System.out.println("Использование: java LanguageSwitcher [опции] [текст]\n" +
                        "  -r, --reverse   Принудительно переключить в другую раскладку (инвертировать)\n" +
                        "  -d, --detect    Только определить язык (ru / en)\n" +
                        "  -h, --help      Показать справку");
                return;
            } else {
                text = String.join(" ", argList.subList(i, argList.size()));
                break;
            }
        }

        if (text == null) {
            // Читаем из stdin
            BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
            StringBuilder sb = new StringBuilder();
            String line;
            while ((line = reader.readLine()) != null) {
                sb.append(line).append("\n");
            }
            text = sb.toString().trim();
        }

        if (text == null || text.isEmpty()) {
            System.err.println("Ошибка: не введён текст");
            System.exit(1);
        }

        if (detect) {
            System.out.println(detectLanguage(text));
            return;
        }

        String direction = "auto";
        if (reverse) {
            String lang = detectLanguage(text);
            if (lang.equals("ru")) direction = "en_to_ru";
            else if (lang.equals("en")) direction = "ru_to_en";
            else direction = "ru_to_en";
        }
        String result = transliterate(text, direction);
        System.out.println(result);
    }
}
