# switcher.py
# Language Switcher (Auto-detect) на Python

import sys
import argparse

# Словарь для переключения с русской на английскую раскладку
RUS_TO_ENG = {
    'а': 'a', 'б': 'b', 'в': 'v', 'г': 'g', 'д': 'd', 'е': 'e', 'ё': 'yo',
    'ж': 'zh', 'з': 'z', 'и': 'i', 'й': 'y', 'к': 'k', 'л': 'l', 'м': 'm',
    'н': 'n', 'о': 'o', 'п': 'p', 'р': 'r', 'с': 's', 'т': 't', 'у': 'u',
    'ф': 'f', 'х': 'kh', 'ц': 'ts', 'ч': 'ch', 'ш': 'sh', 'щ': 'shch',
    'ъ': '', 'ы': 'y', 'ь': '', 'э': 'e', 'ю': 'yu', 'я': 'ya',
    'А': 'A', 'Б': 'B', 'В': 'V', 'Г': 'G', 'Д': 'D', 'Е': 'E', 'Ё': 'Yo',
    'Ж': 'Zh', 'З': 'Z', 'И': 'I', 'Й': 'Y', 'К': 'K', 'Л': 'L', 'М': 'M',
    'Н': 'N', 'О': 'O', 'П': 'P', 'Р': 'R', 'С': 'S', 'Т': 'T', 'У': 'U',
    'Ф': 'F', 'Х': 'Kh', 'Ц': 'Ts', 'Ч': 'Ch', 'Ш': 'Sh', 'Щ': 'Shch',
    'Ъ': '', 'Ы': 'Y', 'Ь': '', 'Э': 'E', 'Ю': 'Yu', 'Я': 'Ya'
}

# Обратный словарь (английская -> русская)
ENG_TO_RUS = {v: k for k, v in RUS_TO_ENG.items()}
# Дополнительные составные соответствия
ENG_TO_RUS.update({
    'yo': 'ё', 'zh': 'ж', 'kh': 'х', 'ts': 'ц', 'ch': 'ч',
    'sh': 'ш', 'shch': 'щ', 'yu': 'ю', 'ya': 'я',
    'Yo': 'Ё', 'Zh': 'Ж', 'Kh': 'Х', 'Ts': 'Ц', 'Ch': 'Ч',
    'Sh': 'Ш', 'Shch': 'Щ', 'Yu': 'Ю', 'Ya': 'Я'
})

def detect_language(text):
    """
    Определяет язык текста (ru / en / unknown).
    """
    rus_count = sum(1 for c in text if c in RUS_TO_ENG)
    eng_count = sum(1 for c in text if c.isalpha() and c not in RUS_TO_ENG)
    if rus_count > eng_count:
        return 'ru'
    elif eng_count > rus_count:
        return 'en'
    else:
        return 'unknown'

def transliterate(text, direction='auto'):
    """
    Преобразует текст между раскладками.
    direction: 'auto' (автоопределение), 'ru_to_en', 'en_to_ru'
    """
    if direction == 'auto':
        lang = detect_language(text)
        if lang == 'ru':
            direction = 'ru_to_en'
        elif lang == 'en':
            direction = 'en_to_ru'
        else:
            return text

    result = []
    i = 0
    if direction == 'ru_to_en':
        # Ищем наиболее длинные совпадения
        while i < len(text):
            found = False
            for length in range(5, 0, -1):
                if i + length <= len(text):
                    substr = text[i:i+length]
                    if substr in RUS_TO_ENG:
                        result.append(RUS_TO_ENG[substr])
                        i += length
                        found = True
                        break
            if not found:
                result.append(text[i])
                i += 1
    else:  # en_to_ru
        while i < len(text):
            found = False
            for length in range(5, 0, -1):
                if i + length <= len(text):
                    substr = text[i:i+length]
                    if substr in ENG_TO_RUS:
                        result.append(ENG_TO_RUS[substr])
                        i += length
                        found = True
                        break
            if not found:
                result.append(text[i])
                i += 1
    return ''.join(result)

def main():
    parser = argparse.ArgumentParser(description='Переключатель языков (автоопределение)')
    parser.add_argument('text', nargs='*', help='Текст для переключения раскладки')
    parser.add_argument('-r', '--reverse', action='store_true', help='Принудительно переключить в другую раскладку (инвертировать)')
    parser.add_argument('-d', '--detect', action='store_true', help='Только определить язык, не переключать')
    args = parser.parse_args()

    if args.text:
        text = ' '.join(args.text)
    else:
        text = sys.stdin.read().strip()

    if not text:
        print("Ошибка: не введён текст", file=sys.stderr)
        sys.exit(1)

    if args.detect:
        lang = detect_language(text)
        print(lang)
        return

    direction = 'auto'
    if args.reverse:
        # инвертируем направление
        detected = detect_language(text)
        if detected == 'ru':
            direction = 'en_to_ru'
        elif detected == 'en':
            direction = 'ru_to_en'
        else:
            direction = 'ru_to_en'  # по умолчанию

    result = transliterate(text, direction)
    print(result)

if __name__ == '__main__':
    main()
