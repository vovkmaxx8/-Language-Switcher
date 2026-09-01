// switcher.go
// Language Switcher (Auto-detect) на Go

package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"
)

// Словарь для переключения с русской на английскую раскладку
var rusToEng = map[string]string{
	"а": "a", "б": "b", "в": "v", "г": "g", "д": "d", "е": "e", "ё": "yo",
	"ж": "zh", "з": "z", "и": "i", "й": "y", "к": "k", "л": "l", "м": "m",
	"н": "n", "о": "o", "п": "p", "р": "r", "с": "s", "т": "t", "у": "u",
	"ф": "f", "х": "kh", "ц": "ts", "ч": "ch", "ш": "sh", "щ": "shch",
	"ъ": "", "ы": "y", "ь": "", "э": "e", "ю": "yu", "я": "ya",
	"А": "A", "Б": "B", "В": "V", "Г": "G", "Д": "D", "Е": "E", "Ё": "Yo",
	"Ж": "Zh", "З": "Z", "И": "I", "Й": "Y", "К": "K", "Л": "L", "М": "M",
	"Н": "N", "О": "O", "П": "P", "Р": "R", "С": "S", "Т": "T", "У": "U",
	"Ф": "F", "Х": "Kh", "Ц": "Ts", "Ч": "Ch", "Ш": "Sh", "Щ": "Shch",
	"Ъ": "", "Ы": "Y", "Ь": "", "Э": "E", "Ю": "Yu", "Я": "Ya",
}

// Обратный словарь (английская -> русская)
var engToRus = make(map[string]string)

func init() {
	for k, v := range rusToEng {
		engToRus[v] = k
	}
	extra := map[string]string{
		"yo": "ё", "zh": "ж", "kh": "х", "ts": "ц", "ch": "ч",
		"sh": "ш", "shch": "щ", "yu": "ю", "ya": "я",
		"Yo": "Ё", "Zh": "Ж", "Kh": "Х", "Ts": "Ц", "Ch": "Ч",
		"Sh": "Ш", "Shch": "Щ", "Yu": "Ю", "Ya": "Я",
	}
	for k, v := range extra {
		engToRus[k] = v
	}
}

func detectLanguage(text string) string {
	rusCount := 0
	engCount := 0
	for _, r := range text {
		if _, ok := rusToEng[string(r)]; ok {
			rusCount++
		} else if (r >= 'a' && r <= 'z') || (r >= 'A' && r <= 'Z') {
			engCount++
		}
	}
	if rusCount > engCount {
		return "ru"
	} else if engCount > rusCount {
		return "en"
	}
	return "unknown"
}

func transliterate(text string, direction string) string {
	if direction == "auto" {
		lang := detectLanguage(text)
		if lang == "ru" {
			direction = "ru_to_en"
		} else if lang == "en" {
			direction = "en_to_ru"
		} else {
			return text
		}
	}
	dict := rusToEng
	if direction == "en_to_ru" {
		dict = engToRus
	}
	result := ""
	i := 0
	for i < len(text) {
		found := false
		for length := 5; length > 0; length-- {
			if i+length <= len(text) {
				substr := text[i : i+length]
				if val, ok := dict[substr]; ok {
					result += val
					i += length
					found = true
					break
				}
			}
		}
		if !found {
			result += string(text[i])
			i++
		}
	}
	return result
}

func main() {
	args := os.Args[1:]
	if len(args) == 0 {
		// Читаем из stdin
		scanner := bufio.NewScanner(os.Stdin)
		var lines []string
		for scanner.Scan() {
			lines = append(lines, scanner.Text())
		}
		text := strings.Join(lines, "\n")
		if text == "" {
			fmt.Fprintln(os.Stderr, "Ошибка: не введён текст")
			os.Exit(1)
		}
		processText(text, false, false)
		return
	}

	reverse := false
	detect := false
	var text string
	for i := 0; i < len(args); i++ {
		if args[i] == "-r" || args[i] == "--reverse" {
			reverse = true
		} else if args[i] == "-d" || args[i] == "--detect" {
			detect = true
		} else if args[i] == "-h" || args[i] == "--help" {
			fmt.Println(`Использование: go run switcher.go [опции] [текст]
  -r, --reverse   Принудительно переключить в другую раскладку (инвертировать)
  -d, --detect    Только определить язык (ru / en)
  -h, --help      Показать справку`)
			return
		} else {
			text = strings.Join(args[i:], " ")
			break
		}
	}
	if text == "" {
		fmt.Fprintln(os.Stderr, "Ошибка: не введён текст")
		os.Exit(1)
	}
	processText(text, reverse, detect)
}

func processText(text string, reverse, detect bool) {
	if detect {
		fmt.Println(detectLanguage(text))
		return
	}
	direction := "auto"
	if reverse {
		lang := detectLanguage(text)
		if lang == "ru" {
			direction = "en_to_ru"
		} else if lang == "en" {
			direction = "ru_to_en"
		} else {
			direction = "ru_to_en"
		}
	}
	result := transliterate(text, direction)
	fmt.Println(result)
}
