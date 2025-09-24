using System;
using System.Text;

namespace NebulaConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Кирилицв
            Console.OutputEncoding = Encoding.UTF8;
            UIFrame frame = new UIFrame(30, 80);

            // Основной фон
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            frame.RenderTopBar();
            frame.DrawHeaderRibbon();
            frame.RenderColumnTitles();

            EntriesRenderer renderer = new EntriesRenderer();

            // демонстрационные записи
            for (int i = 0; i < 70; i++)
            {
                EntryRecord rec = new EntryRecord(
                    $"GolangBackend{i} ic",
                    "01:60",
                    "29.02.2026",
                    "exe",
                    (i * 1000 / (i % 2 == 0 ? 40 : 15)).ToString()
                );
                renderer.Add(rec);
            }

            // Отрисовка строк списка — левая и правая половины
            for (int row = 0; row < Console.WindowHeight - 8; row++)
            {
                string left = renderer.RenderLeftColumns(frame, row, true);
                Console.Write(left);
                string right = renderer.RenderRightColumns(frame, row);
                Console.WriteLine(right);
            }

            frame.RenderCatalogueLine();
            Console.WriteLine();
            frame.DrawFooterRibbon();
            frame.RenderActionBar();

            Console.ReadKey();
        }
    }
}


