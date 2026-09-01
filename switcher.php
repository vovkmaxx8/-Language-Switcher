<?php
// switcher.php
// Language Switcher (Auto-detect) на PHP

if (php_sapi_name() !== 'cli') {
    die("Это консольное приложение.\n");
}

$rusToEng = [
    'а'=>'a', 'б'=>'b', 'в'=>'v', 'г'=>'g', 'д'=>'d', 'е'=>'e', 'ё'=>'yo',
    'ж'=>'zh', 'з'=>'z', 'и'=>'i', 'й'=>'y', 'к'=>'k', 'л'=>'l', 'м'=>'m',
    'н'=>'n', 'о'=>'o', 'п'=>'p', 'р'=>'r', 'с'=>'s', 'т'=>'t', 'у'=>'u',
    'ф'=>'f', 'х'=>'kh', 'ц'=>'ts', 'ч'=>'ch', 'ш'=>'sh', 'щ'=>'shch',
    'ъ'=>'', 'ы'=>'y', 'ь'=>'', 'э'=>'e', 'ю'=>'yu', 'я'=>'ya',
    'А'=>'A', 'Б'=>'B', 'В'=>'V', 'Г'=>'G', 'Д'=>'D', 'Е'=>'E', 'Ё'=>'Yo',
    'Ж'=>'Zh', 'З'=>'Z', 'И'=>'I', 'Й'=>'Y', 'К'=>'K', 'Л'=>'L', 'М'=>'M',
    'Н'=>'N', 'О'=>'O', 'П'=>'P', 'Р'=>'R', 'С'=>'S', 'Т'=>'T', 'У'=>'U',
    'Ф'=>'F', 'Х'=>'Kh', 'Ц'=>'Ts', 'Ч'=>'Ch', 'Ш'=>'Sh', 'Щ'=>'Shch',
    'Ъ'=>'', 'Ы'=>'Y', 'Ь'=>'', 'Э'=>'E', 'Ю'=>'Yu', 'Я'=>'Ya'
];

$engToRus = array_flip($rusToEng);
$engToRus += [
    'yo'=>'ё', 'zh'=>'ж', 'kh'=>'х', 'ts'=>'ц', 'ch'=>'ч',
    'sh'=>'ш', 'shch'=>'щ', 'yu'=>'ю', 'ya'=>'я',
    'Yo'=>'Ё', 'Zh'=>'Ж', 'Kh'=>'Х', 'Ts'=>'Ц', 'Ch'=>'Ч',
    'Sh'=>'Ш', 'Shch'=>'Щ', 'Yu'=>'Ю', 'Ya'=>'Я'
];

function detectLanguage($text) {
    global $rusToEng;
    $rusCount = 0;
    $engCount = 0;
    for ($i = 0; $i < mb_strlen($text); $i++) {
        $char = mb_substr($text, $i, 1);
        if (array_key_exists($char, $rusToEng)) {
            $rusCount++;
        } elseif (preg_match('/[a-zA-Z]/', $char)) {
            $engCount++;
        }
    }
    if ($rusCount > $engCount) return 'ru';
    elseif ($engCount > $rusCount) return 'en';
    else return 'unknown';
}

function transliterate($text, $direction = 'auto') {
    global $rusToEng, $engToRus;
    if ($direction == 'auto') {
        $lang = detectLanguage($text);
        if ($lang == 'ru') $direction = 'ru_to_en';
        elseif ($lang == 'en') $direction = 'en_to_ru';
        else return $text;
    }
    $dict = $direction == 'ru_to_en' ? $rusToEng : $engToRus;
    $result = '';
    $i = 0;
    $len = mb_strlen($text);
    while ($i < $len) {
        $found = false;
        for ($l = 5; $l > 0; $l--) {
            if ($i + $l <= $len) {
                $sub = mb_substr($text, $i, $l);
                if (array_key_exists($sub, $dict)) {
                    $result .= $dict[$sub];
                    $i += $l;
                    $found = true;
                    break;
                }
            }
        }
        if (!$found) {
            $result .= mb_substr($text, $i, 1);
            $i++;
        }
    }
    return $result;
}

$options = getopt('rdh', ['reverse', 'detect', 'help']);
$reverse = isset($options['r']) || isset($options['reverse']);
$detect = isset($options['d']) || isset($options['detect']);
$help = isset($options['h']) || isset($options['help']);

if ($help) {
    echo "Использование: php switcher.php [опции] [текст]\n";
    echo "  -r, --reverse   Принудительно переключить в другую раскладку (инвертировать)\n";
    echo "  -d, --detect    Только определить язык (ru / en)\n";
    echo "  -h, --help      Показать справку\n";
    exit(0);
}

$args = array_values(array_filter($argv, function($arg) {
    return !str_starts_with($arg, '-');
}));
array_shift($args); // удаляем имя скрипта

if (empty($args)) {
    // Читаем из stdin
    $text = file_get_contents('php://stdin');
    if ($text === false || $text === '') {
        fwrite(STDERR, "Ошибка: не введён текст\n");
        exit(1);
    }
    $text = trim($text);
} else {
    $text = implode(' ', $args);
}

if ($detect) {
    echo detectLanguage($text) . "\n";
    exit(0);
}

$direction = 'auto';
if ($reverse) {
    $lang = detectLanguage($text);
    if ($lang == 'ru') $direction = 'en_to_ru';
    elseif ($lang == 'en') $direction = 'ru_to_en';
    else $direction = 'ru_to_en';
}
$result = transliterate($text, $direction);
echo $result . "\n";
