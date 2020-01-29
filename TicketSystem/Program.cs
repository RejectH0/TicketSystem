using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TicketSystem
{
    // Version 0.2a //28JAN2020 //1850
    class Ticket
    {
        public int TicketNumber;

        public String Summary { get; set; }
        public String Status { get; set; }
        public String Priority { get; set; }
        public Person Submitter { get; set; }
        public Person Assigned { get; set; }
        public List<People> Watching { get; set; }

        public Ticket(int TicketNumber, String Summary, String Status, String Priority, String Submitter, String Assigned, String Watching)
        {
            this.TicketNumber = TicketNumber;
            this.Summary = Summary;
            this.Status = Status;
            this.Priority = Priority;
            this.Submitter = Submitter;
            this.Assigned = Assigned;
            this.Watching = new List<People>();
        }

    }

    class Person : IDisposable
    {
        static private int idNext = 0;
        static private int idDestroy = -1;
        public int idNumber { get; private set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }

        public Person(String FullName, String Email, String Phone)
        {
            if(idDestroy == -1)
            {
                this.idNumber = Person.idNext;
                Person.idNext++;
            } else
            {
                this.idNumber = Person.idDestroy;
            }

            this.FullName = FullName;
            this.Email = Email;
            this.Phone = Phone;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void Dispose()
        {
            Person.idDestroy = this.idNumber;
        }

        public static implicit operator Person(string v)
        {
            throw new NotImplementedException();
        }
    }

    class People
    {
        public People(string psv)
        {
            var vals = psv.Split('|');

            for (int i = 0; i < vals.Length; i++)
            {
                Person person = new Person(vals[i], "null", "null");
            }
        }
    }
    class Program
    {
        public static void Main(string[] args)
        {
            DisplayHeader();
            DisplayMenu();
        }

        public static void DisplayHeader()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("*");
            }
            
            int winWidth = Console.WindowWidth - 1;
            Console.SetCursorPosition(winWidth, 1);
            Console.Write("*");

            Console.SetCursorPosition(0, 2);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("*");
            }
            string menuText = "Welcome to the Gregg Sperling Ticket System!";
            Console.SetCursorPosition(0, 1);
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (menuText.Length / 2)) + "}", menuText));
            Console.SetCursorPosition(0, 1);
            Console.Write("*");
            Console.SetCursorPosition(0, 5);
        }
        public static void DisplayMenu()
        {
            string[] menuChoices = new string[]
            {
                "Read File",
                "Create File",
                "End Program"
            };

            for (int i = 0; i < menuChoices.Length; i++)
            {
                Console.WriteLine((i+1) + ": "+menuChoices[i]);
            }


        }

        public static void ReadFile()
        {
            string fileData = "fileData.txt";
            if (File.Exists(fileData))
            {
                StreamReader sr = new StreamReader(fileData);

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] arr = line.Split(',');
                    // remember there's a header row!
                    // TicketID, Summary, Status, Priority, Submitter, Assigned, Watching
                    // 1,This is a bug ticket,Open,High,Drew Kjell,Jane Doe,Drew Kjell|John Smith|Bill Jones
                    if (!arr[0].All(char.IsDigit))
                    {
                        Console.WriteLine("Found Header Row!");
                    }
                    else
                    {
                        Ticket ticket = new Ticket(Int32.Parse(arr[0]), arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);
                    }
                }

                sr.Close();

            }
            else
            {
                Console.WriteLine("File does not exist.");
            }

        }

        public static void WriteFile()
        {
            string fileData = "fileData.txt";
            StreamWriter sw = new StreamWriter(fileData, true);
            sw.WriteLine("Header Row"); // Header Row goes here
                                        // write the code to write out the file data.

            sw.Close();
        }


        public static void DoBox(String text, Int32? width, Int32? height, Int32? upperLeft, Int32? lowerRight)
        {
            if (upperLeft < 1 || upperLeft > 70 || upperLeft == null)
            {
                upperLeft = 60;
            }
            if (lowerRight < 1 || lowerRight > 70 || lowerRight == null)
            {
                lowerRight = 80;
            }

        }

    }
}
