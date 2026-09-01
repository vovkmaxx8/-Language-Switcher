# switcher.rb
# Language Switcher (Auto-detect) на Ruby

RUS_TO_ENG = {
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
}

ENG_TO_RUS = RUS_TO_ENG.invert
ENG_TO_RUS.merge!({
  'yo'=>'ё', 'zh'=>'ж', 'kh'=>'х', 'ts'=>'ц', 'ch'=>'ч',
  'sh'=>'ш', 'shch'=>'щ', 'yu'=>'ю', 'ya'=>'я',
  'Yo'=>'Ё', 'Zh'=>'Ж', 'Kh'=>'Х', 'Ts'=>'Ц', 'Ch'=>'Ч',
  'Sh'=>'Ш', 'Shch'=>'Щ', 'Yu'=>'Ю', 'Ya'=>'Я'
})

def detect_language(text)
  rus_count = text.count { |c| RUS_TO_ENG.key?(c) }
  eng_count = text.count { |c| c.match?(/[a-zA-Z]/) && !RUS_TO_ENG.key?(c) }
  if rus_count > eng_count
    'ru'
  elsif eng_count > rus_count
    'en'
  else
    'unknown'
  end
end

def transliterate(text, direction = 'auto')
  if direction == 'auto'
    lang = detect_language(text)
    if lang == 'ru'
      direction = 'ru_to_en'
    elsif lang == 'en'
      direction = 'en_to_ru'
    else
      return text
    end
  end

  dict = direction == 'ru_to_en' ? RUS_TO_ENG : ENG_TO_RUS
  result = ''
  i = 0
  while i < text.length
    found = false
    (5).downto(1) do |len|
      if i + len <= text.length
        sub = text[i, len]
        if dict.key?(sub)
          result << dict[sub]
          i += len
          found = true
          break
        end
      end
    end
    unless found
      result << text[i]
      i += 1
    end
  end
  result
end

if ARGV.empty?
  # читаем из stdin
  text = STDIN.read.strip
  if text.empty?
    $stderr.puts "Ошибка: не введён текст"
    exit 1
  end
else
  text = ARGV.join(' ')
end

reverse = false
detect = false
args = ARGV.dup
while arg = args.shift
  case arg
  when '-r', '--reverse'
    reverse = true
  when '-d', '--detect'
    detect = true
  when '-h', '--help'
    puts "Использование: ruby switcher.rb [опции] [текст]"
    puts "  -r, --reverse   Принудительно переключить в другую раскладку (инвертировать)"
    puts "  -d, --detect    Только определить язык (ru / en)"
    puts "  -h, --help      Показать справку"
    exit 0
  else
    # остальной текст
    text = (args.unshift(arg)).join(' ')
    break
  end
end

if detect
  puts detect_language(text)
  exit 0
end

direction = 'auto'
if reverse
  lang = detect_language(text)
  if lang == 'ru'
    direction = 'en_to_ru'
  elsif lang == 'en'
    direction = 'ru_to_en'
  else
    direction = 'ru_to_en'
  end
end
result = transliterate(text, direction)
puts result
