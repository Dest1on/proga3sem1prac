using System;
using System.Collections.Generic;
using System.Linq;

namespace NebulaConsole
{
    public partial class UIFrame
    {
        // Массивы ширин колонок для левой и правой половин
        private int[] _leftColumnWidths;
        private int[] _rightColumnWidths;

        public UIFrame(int height, int width)
        {
            // Старательная установка 
            try
            {
                int bufW = Math.Max(Console.BufferWidth, width);
                int bufH = Math.Max(Console.BufferHeight, height);
                Console.SetBufferSize(bufW, bufH);
            }
            catch { /*Просто не получилось*/  }

            try
            {
                int winW = Math.Min(width, Console.LargestWindowWidth);
                int winH = Math.Min(height, Console.LargestWindowHeight);
                Console.SetWindowSize(Math.Max(1, winW), Math.Max(1, winH));
            }
            catch { /*Тут тоже терпим*/ }
        }

        private void UseActionBarColors()
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
        }

        private void UseMainTextColors()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void UsePanelColors()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        private void UseInversePanelColors()
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
        }

        // Строим строку заголовков внутри блока и одновременно вычисляем ширины колонок 
        // words — заголовки, parts — кол-во частей 
        private string BuildTitleRow(List<string> words, int parts, bool leftHalf)
        {
            int width = Console.WindowWidth / parts + 1;
            int columns = words.Count;
            int[] sep = new int[columns + 1];
            sep[0] = 0;
            sep[columns] = width - 1;
            for (int i = 1; i < columns; i++) sep[i] = i * width / columns;

            if (leftHalf)
            {
                _leftColumnWidths = new int[columns];
                for (int i = 0; i < columns; i++) _leftColumnWidths[i] = sep[i + 1] - sep[i] - 1;
            }
            else
            {
                _rightColumnWidths = new int[columns];
                for (int i = 0; i < columns; i++) _rightColumnWidths[i] = sep[i + 1] - sep[i] - 1;
            }

            char[] buffer = Enumerable.Repeat(' ', width).ToArray();
            for (int i = 0; i < sep.Length; i++)
            {
                int p = sep[i];
                if (p >= 0 && p < width) buffer[p] = '\u2502';
            }
            if (width > 0) { buffer[0] = '\u2551'; buffer[width - 1] = '\u2551'; }

            for (int i = 0; i < columns; i++)
            {
                int leftBound = sep[i] + 1;
                int rightBound = (i == columns - 1) ? sep[i + 1] - 1 : sep[i + 1];
                int segLen = rightBound - leftBound;
                string w = words[i];
                if (w.Length > segLen) w = w.Substring(0, Math.Max(0, segLen));
                int start = leftBound + (segLen - w.Length) / 2;
                for (int k = 0; k < w.Length; k++)
                {
                    int pos = start + k;
                    if (pos >= 0 && pos < width) buffer[pos] = w[k];
                }
            }

            return new string(buffer);
        }

        // Некостыльная отрисовка верхней панели ---------------------------------------------------------
        public void RenderTopBar()
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;

            // закрасим полностью строку
            string blank = new string(' ', Console.WindowWidth);
            Console.Write(blank);

            // вернём курсор в начало этой строки и рисуем поверх (гениально?)
            Console.CursorTop = Math.Max(0, Console.CursorTop - 1);
            Console.CursorLeft = 0;

            string[] labels = { "Левая", "Файл", "Диск", "Команды", "Правая" };

            foreach (var item in labels)
            {
                // отступ между метками
                Console.Write("".PadRight(Console.WindowWidth / 25, ' '));

                // первая буква — жёлтая
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(item[0]);

                // остаток слова — чёрный (как на картинке)
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(item.Substring(1));
            }

            // И печатаем время в правой части 
            int used = Console.CursorLeft;
            string time = "8:30";
            if (used < Console.WindowWidth)
            {
                int remain = Console.WindowWidth - used;
                if (remain >= time.Length)
                {
                    Console.Write("".PadRight(remain - time.Length, ' '));
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.Write(time);
                }
                else
                {
                    Console.Write("".PadRight(remain, ' '));
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        // Заголовки
        public void RenderColumnTitles()
        {
            UsePanelColors();
            string left = BuildTitleRow(new List<string> { "C: Имя", "Имя", "Имя" }, 2, true);

            // Печатаем левую часть с подсветкой вертикальных разделителей --------------------------------
            Console.Write(left[0]);
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 1; i < left.Length - 1; i++)
            {
                if (left[i] == '\u2502')
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(left[i]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else Console.Write(left[i]);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(left[left.Length - 1]);

            // Правая часть заголовка
            List<string> rightWords = new List<string> { "C: Имя", "Размер", "Дата", "Время" };
            string right = BuildTitleRow(rightWords, 2, false);
            Console.Write(right[0]);
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 1; i < right.Length - 1; i++)
            {
                if (right[i] == '\u2502')
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(right[i]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else Console.Write(right[i]);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(right[right.Length - 1]);

            Console.ResetColor();
            Console.WriteLine();
        }

        // Рисует верхнюю рамку/полосу --------------------------------------------------------------------
        public void DrawHeaderRibbon()
        {
            UsePanelColors();
            Console.Write('\u2554');
            for (int i = 1; i < (Console.WindowWidth / 4) - 3; i++) Console.Write('\u2550');
            Console.Write(" C:\\NC ");
            for (int i = 1; i < (Console.WindowWidth / 4) - 3; i++) Console.Write('\u2550');
            Console.Write('\u2557');

            Console.Write('\u2554');
            for (int i = 1; i < (Console.WindowWidth / 4) - 3; i++) Console.Write('\u2550');

            UseInversePanelColors();
            Console.Write(" C:\\NC ");
            UsePanelColors();
            for (int i = 1; i < (Console.WindowWidth / 4) - 5; i++) Console.Write('\u2550');
            Console.Write('\u2557');

            Console.WriteLine();
            Console.ResetColor();
        }

        // Нижняя рамка -----------------------------------------------------------------------------------
        public void DrawFooterRibbon()
        {
            UsePanelColors();
            Console.Write('\u255A');
            for (int i = 0; i < (Console.WindowWidth / 2) - 1; i++) Console.Write('\u2550');
            Console.Write('\u255D');
            Console.Write('\u255A');
            for (int i = 0; i < (Console.WindowWidth / 2) - 3; i++) Console.Write('\u2550');
            Console.Write('\u255D');
            Console.ResetColor();
        }

        public int[] LeftColumnWidths() => _leftColumnWidths;
        public int[] RightColumnWidths() => _rightColumnWidths;

        // Строка каталога -----------------------------------------------------------------------------------
        public void RenderCatalogueLine()
        {
            int halfW = Console.WindowWidth / 2 - 1;
            for (int side = 0; side < 2; side++)
            {
                Console.Write('\u255F');
                int len = (side == 0) ? halfW : halfW - 2;
                for (int i = 0; i < len; i++) Console.Write('\u2500');
                Console.Write('\u2562');
            }
            Console.WriteLine();

            string s = ">КАТАЛОГ< 11.11.11  24:60";
            Console.Write('\u2551');
            Console.Write("..");
            Console.Write(s.PadLeft(halfW - 2));
            Console.Write('\u2551');
            Console.Write('\u2551');
            Console.Write("..");
            Console.Write(s.PadLeft(halfW - 4));
            Console.Write('\u2551');
        }

        // Нижняя панель команд ----------------------------------------------------------------------------
        public void RenderActionBar()
        {
            string[] actions = { "Помощь", "Вызов ", "Чтение", "Правка", "Копия ", "НовИмя", "НовКат", "Удал-е", "Меню  ", "Выход " };
            UseMainTextColors();
            Console.WriteLine("C:\\NC>");
            for (int i = 0; i < actions.Length; i++)
            {
                Console.Write(i + 1);
                UseActionBarColors();
                Console.Write(actions[i]);
                UseMainTextColors();
                if (i != actions.Length - 1) Console.Write(" ");
            }
        }
    }
}

