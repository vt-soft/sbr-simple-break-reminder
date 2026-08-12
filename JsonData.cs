using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SBR
{
    // Classes neccesary to store data in  JSON file
    // Data for config (alarm) file:


    public class Day
    {
        public string DayDate { get; set; }
        public int DayTotalTime { get; set; } // sec
        public int DayIdleTime { get; set; } // sec
        public int DayWorkingTime { get; set; }  // sec
        public int DayIgnoredBreaks { get; set; }
    }

    public class Month
    {
        public string MonthDate { get; set; }
        public int MonthTotalTime { get; set; } // sec
        public int MonthIdleTime { get; set; } // sec
        public int MonthWorkingTime { get; set; }  // sec
        public float MonthIgnoredBreaks { get; set; } // must be float here, because it is average value
    }


    public class DataConfig
    {
        public string LanguageCode { get; set; } // like en-GB, de-DE, pl-PL, etc.
        public bool DarkMode { get; set; }
        public int AlarmTime1 { get; set; }  // Alarm time in minutes:
        public int AlarmTime2 { get; set; }
        public int AlarmTime3 { get; set; }
        public int AlarmTime4 { get; set; }
        public int SelectedAlarm { get; set; } // 1 - Aalarm1, 2 - Alarm2, 3 - Alarm3, 4 - Alarm4    
        public bool StartUp { get; set; }
        public bool SystemTray { get; set; }
        public bool PlaySound { get; set; }
        public string PlaySoundRButton { get; set; }
        public bool Emoticons { get; set; }
        public bool Pomodoro { get; set; }
        public bool PomodoroLongBreak { get; set; }
        public string SelectedColor { get; set; }   // There were troubles to deserialize Color type, so I have used string instead. 
        public string SelectedRadioButton { get; set; }
        public string CustomColor { get; set; }     // Our own color. 
        public List<Day> Days { get; set; }
        public List<Month> Months { get; set; }
    }


    

}
