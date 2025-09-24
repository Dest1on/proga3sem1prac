using System;

namespace NebulaConsole
{
    public class EntryRecord
    {
        private string _name;
        private string _time;
        private string _date;
        private string _ext;
        private string _size;

        public EntryRecord(string name, string time, string date, string ext, string size)
        {
            _name = name;
            _time = time;
            _date = date;
            _ext = ext;
            _size = size;
        }

        public string Name() => _name;
        public string Time() => _time;
        public string Date() => _date;
        public string Ext() => _ext;
        public string Size() => _size;
    }
}

