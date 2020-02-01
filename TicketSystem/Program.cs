using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TicketSystem
{
    // Version 0.4a //01FEB2020 1510
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

        public static implicit operator Person(string v) => new Person(v, "null", "null");
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
        public Program()
        {
            RunMainMenu();

        }
        public static void Main(string[] args)
        {
            new Program();
        }

        public void DisplayHeader()
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
        public int PrintMainMenu()
        {
            string menuName = "Main";
            string[] menuChoices = new string[]
            {
                "Read Ticket File",
                "Enter New Ticket",
                "Exit Program"
            };

            Console.WriteLine(menuName + " Menu");
            for (int i = 0; i < menuChoices.Length; i++)
            {
                Console.WriteLine((i + 1) + ": " + menuChoices[i]);
            }

            return menuChoices.Count();
        }
        private void RunMainMenu()
        {
            bool userContinue = true;
           
            while (userContinue)
            {
                DisplayHeader();
                int menuMin = 1;
                int menuMax = PrintMainMenu();                        // Show the Main Menu

                // Since the MainMenu should always contain < 10 items, we're only going to ask the user for one character
                // of input, and we're going to trap the heck out of it since we should never trust user input.

                Console.Write("[=+> # <+=] Choose ({0}-{1})",menuMin,menuMax);           // menu prompt
                Console.SetCursorPosition(5, Console.CursorTop); // Move the cursor to the '#' in the prompt
                StringBuilder sb = new StringBuilder(); // Stringbuilder object that will hold the user's menu choice
                ConsoleKeyInfo cki;                     // ConsoleKeyInfo object
                cki = Console.ReadKey();                // Get the input from the user
                sb.Append(cki.KeyChar);                 // Throw that input into the StringBuilder object so it can be parsed.

                int userChoice = 0;
                try
                {
                    userChoice = Int32.Parse(sb.ToString());
                } catch
                {
                    userChoice = 0;
                }

                switch (userChoice)
                {
                    case 1:
                        ReadFile();
                        break;
                    case 2:
                        EnterTicket();
                        break;
                    case 3:
                        ExitGracefully();
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 0:
                        break;
                    default:
                        break;
                }

            }
        }

        public void EnterTicket()
        {
            DisplayHeader();
            Console.WriteLine("Ticket Entry");
            PressEnterToContinue();
        }

        public void ExitGracefully()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("Now exiting this application...");
            WriteFile();
            PressEnterToContinue();
            System.Environment.Exit(0);
        }
        public void InvalidMenuChoice()
        {
            Console.Clear();
            DisplayHeader();
            string invalidChoice = "You have made an invalid selection and therefore must try again.";
            Console.WriteLine("{0,15}",invalidChoice);
            PressEnterToContinue();
        }

        public void PressEnterToContinue()
        {
            Console.Write("Press Enter To Continue: ");
            Console.ReadLine();
        }
        

        public void ReadFile()
        {
            DisplayHeader();
            string fileData = "tickets.csv";
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
                        Console.WriteLine("{0,5}{1,15}{2,15}{3,15}{4,15}{5,15}{6,15}", new object[] { arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]});
                    }
                    else
                    {
                        int importTicketNumber = 65535;
                        try
                        {
                            importTicketNumber = Int32.Parse(arr[0]);
                        } catch
                        {
                            importTicketNumber = 65535;
                        }
                        Ticket ticket = new Ticket(importTicketNumber, arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);
                    }
                }

                sr.Close();

            }
            else
            {
                Console.WriteLine("File does not exist.");
            }

            PressEnterToContinue();
        }

        public void WriteFile()
        {
            string fileData = "tickets.csv";
            StreamWriter sw = new StreamWriter(fileData, true);
            sw.Write("TicketID,Summary,Status,Priority,Submitter,Assigned,Watching1|Watching2\n");
                                        // write the code to write out the file data.

            sw.Close();
        }


        public void DoBox(String text, Int32? width, Int32? height, Int32? upperLeft, Int32? lowerRight)
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
