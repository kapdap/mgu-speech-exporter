using System;

namespace MGUSpeechExporter
{
    public struct SubtitleEntry
    {
        public TimeSpan Time;
        public string Text;
        public string Character;
        public short Unknown;

        public int RawTimestamp;
        public short RawCharacter;
        public short RawUnknown;

        public SubtitleEntry(int timestamp, short character, short unknown)
        {
            Time = TimeSpan.FromSeconds(timestamp / 21.5d);
            Text = String.Empty;
            Character = ATFEntry.Characters.ContainsKey(character) ? ATFEntry.Characters[character] : character.ToString();
            Unknown = unknown;

            RawTimestamp = timestamp;
            RawCharacter = character;
            RawUnknown = unknown;
        }

        new public string ToString()
        {
            string text = $"{Time:hh\\:mm\\:ss\\.fff} ";
            text += $"({RawTimestamp}) ";
            text += $"{Character} ";
            text += $"({Unknown}):\r\n";
            text += Text;
            return text;
        }

        public string ToSubRip()
        {
            string text = $"{Time:hh\\:mm\\:ss\\,fff} --> ";
            text += $"\r\n";
            text += $"{Character}: {Text}";
            return text;
        }

        public string ToWebVTT()
        {
            string text = $"{Time:mm\\:ss\\.fff} --> ";
            text += $"\r\n";
            text += $"<v {Character}>{Text}";
            return text;
        }
    }
}