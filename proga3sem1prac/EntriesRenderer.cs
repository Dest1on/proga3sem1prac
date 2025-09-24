using System;
using System.Collections.Generic;

namespace NebulaConsole
{
    public class EntriesRenderer
    {
        private List<EntryRecord> _items;
        private int _pageHeight;

        public EntriesRenderer()
        {
            _items = new List<EntryRecord>();
            _pageHeight = Console.WindowHeight - 8;
        }

        public void Add(EntryRecord r) => _items.Add(r);

        // Левая половинка ------------------------------------------------------------------------------------
        public string RenderLeftColumns(UIFrame frame, int rowIndex, bool isLeftSide)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Cyan;

            var widths = frame.LeftColumnWidths();
            int maxCols = isLeftSide ? 3 : 4;
            string outStr = "";
            outStr += '\u2551';

            if (widths == null || widths.Length < maxCols)
            {
                outStr += new string(' ', Console.WindowWidth / 2 - 1) + '\u2551';
                return outStr;
            }

            for (int col = 0; col < maxCols; col++)
            {
                int gap = widths[col];

                if (rowIndex == 0 && col == 0)
                {
                    outStr += "..".PadRight(gap, ' ');
                    outStr += (col == maxCols - 1 ? '\u2551' : '\u2502');
                    continue;
                }

                int idx = col * _pageHeight + rowIndex;
                if (idx >= _items.Count)
                {
                    outStr += new string(' ', gap);
                    outStr += (col == maxCols - 1 ? '\u2551' : '\u2502');
                    continue;
                }

                var it = _items[idx];
                string name = it.Name();
                string ext = it.Ext();

                int need = name.Length + ext.Length + 1;
                if (gap < need)
                {
                    int cut = gap - ext.Length - 1;
                    if (cut <= 0) cut = 1;
                    string w = name.Length > cut ? name.Substring(0, cut) : name;
                    if (w.Length > 0)
                    {
                        if (w.Length == 1) w = "~";
                        else w = w.Substring(0, w.Length - 1) + "~";
                    }
                    outStr += w + " ";
                    outStr += ext.PadLeft(gap - w.Length - 1, ' ');
                    outStr += (col == maxCols - 1 ? '\u2551' : '\u2502');
                }
                else
                {
                    outStr += name.PadRight(gap - ext.Length, ' ') + " " + ext + (col == maxCols - 1 ? '\u2551' : '\u2502');
                }
            }

            return outStr;
        }

        // Правая половинка ----------------------------------------------------------------------------
        public string RenderRightColumns(UIFrame frame, int rowIndex)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Cyan;

            var widths = frame.RightColumnWidths();
            string outStr = "";
            outStr += '\u2551';

            if (widths == null || widths.Length < 4)
            {
                return outStr.PadRight(Console.WindowWidth / 2);
            }

            if (rowIndex == 0)
            {
                outStr += "..".PadRight(widths[0], ' ') + '\u2502';
                outStr += Cut(">КАТАЛОГ<", widths[1]).PadLeft(widths[1], ' ') + '\u2502';
                outStr += Cut("11.11.11", widths[2]).PadLeft(widths[2], ' ') + '\u2502';
                int pad = widths[3] - 2; if (pad < 0) pad = 0;
                outStr += Cut("24:60", widths[3]).PadLeft(pad, ' ') + '\u2551';
                return outStr;
            }

            if (rowIndex >= _items.Count)
            {
                outStr += new string(' ', Console.WindowWidth / 2 - 1) + '\u2551';
                return outStr;
            }

            var rec = _items[rowIndex];
            string leftName = Cut(rec.Name(), widths[0] - rec.Ext().Length - 1);
            outStr += leftName + " " + rec.Ext() + '\u2502';
            outStr += Cut(rec.Size(), widths[1]).PadLeft(widths[1], ' ') + '\u2502';
            outStr += Cut(rec.Date(), widths[2]).PadLeft(widths[2], ' ') + '\u2502';
            int pad2 = widths[3] - 2; if (pad2 < 0) pad2 = 0;
            outStr += Cut(rec.Time(), widths[3]).PadLeft(pad2, ' ') + '\u2551';

            return outStr;
        }

        // Обрезка
        private string Cut(string s, int len)
        {
            if (len <= 0) return "";
            if (s.Length > len)
            {
                if (len <= 1) return "~";
                return s.Substring(0, len - 1) + "~";
            }
            return s;
        }
    }
}
