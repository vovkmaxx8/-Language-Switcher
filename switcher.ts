// switcher.ts
// Language Switcher (Auto-detect) на TypeScript

import * as fs from 'fs';
import * as readline from 'readline';

// Словарь для переключения с русской на английскую раскладку
const RUS_TO_ENG: Record<string, string> = {
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
};

// Обратный словарь (английская -> русская)
const ENG_TO_RUS: Record<string, string> = Object.fromEntries(
    Object.entries(RUS_TO_ENG).map(([k, v]) => [v, k])
);
// Дополнительные составные соответствия
const extra: Record<string, string> = {
    'yo': 'ё', 'zh': 'ж', 'kh': 'х', 'ts': 'ц', 'ch': 'ч',
    'sh': 'ш', 'shch': 'щ', 'yu': 'ю', 'ya': 'я',
    'Yo': 'Ё', 'Zh': 'Ж', 'Kh': 'Х', 'Ts': 'Ц', 'Ch': 'Ч',
    'Sh': 'Ш', 'Shch': 'Щ', 'Yu': 'Ю', 'Ya': 'Я'
};
Object.assign(ENG_TO_RUS, extra);

function detectLanguage(text: string): string {
    let rusCount = 0, engCount = 0;
    for (const c of text) {
        if (RUS_TO_ENG.hasOwnProperty(c)) rusCount++;
        else if (/[a-zA-Z]/.test(c)) engCount++;
    }
    if (rusCount > engCount) return 'ru';
    else if (engCount > rusCount) return 'en';
    else return 'unknown';
}

function transliterate(text: string, direction: string = 'auto'): string {
    if (direction === 'auto') {
        const lang = detectLanguage(text);
        if (lang === 'ru') direction = 'ru_to_en';
        else if (lang === 'en') direction = 'en_to_ru';
        else return text;
    }

    const dict = direction === 'ru_to_en' ? RUS_TO_ENG : ENG_TO_RUS;
    let result = '';
    let i = 0;
    while (i < text.length) {
        let found = false;
        for (let len = 5; len > 0; len--) {
            if (i + len <= text.length) {
                const substr = text.substring(i, i + len);
                if (dict.hasOwnProperty(substr)) {
                    result += dict[substr];
                    i += len;
                    found = true;
                    break;
                }
            }
        }
        if (!found) {
            result += text[i];
            i++;
        }
    }
    return result;
}

function main() {
    const args = process.argv.slice(2);
    let text: string | null = null;
    let reverse = false;
    let detect = false;

    for (let i = 0; i < args.length; i++) {
        if (args[i] === '-r' || args[i] === '--reverse') {
            reverse = true;
        } else if (args[i] === '-d' || args[i] === '--detect') {
            detect = true;
        } else if (args[i] === '-h' || args[i] === '--help') {
            console.log(`Использование: ts-node switcher.ts [опции] [текст]
  -r, --reverse   Принудительно переключить в другую раскладку (инвертировать)
  -d, --detect    Только определить язык (ru / en)
  -h, --help      Показать справку`);
            process.exit(0);
        } else {
            text = args.slice(i).join(' ');
            break;
        }
    }

    if (!text) {
        const rl = readline.createInterface({ input: process.stdin });
        let input = '';
        rl.on('line', line => input += line + '\n');
        rl.on('close', () => {
            if (input) processText(input.trimEnd());
            else console.error('Ошибка: не введён текст');
        });
    } else {
        processText(text);
    }

    function processText(text: string) {
        if (!text) {
            console.error('Ошибка: не введён текст');
            process.exit(1);
        }
        if (detect) {
            console.log(detectLanguage(text));
            return;
        }
        let direction = 'auto';
        if (reverse) {
            const lang = detectLanguage(text);
            if (lang === 'ru') direction = 'en_to_ru';
            else if (lang === 'en') direction = 'ru_to_en';
            else direction = 'ru_to_en';
        }
        const result = transliterate(text, direction);
        console.log(result);
    }
}

main();
