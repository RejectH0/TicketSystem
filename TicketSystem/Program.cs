using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TicketSystem
{
    // Version 1.0 //01FEB2020 2030
    class Ticket : IDisposable
    {
        private static int idNext = 1;
        private static int idDestroy = -1;
        public int TicketNumber { get; private set; }

        public String Summary { get; set; }
        public String Status { get; set; }
        public String Priority { get; set; }
        public Person Submitter { get; set; }
        public Person Assigned { get; set; }
        public WatcherList<Person> Watching { get; set; }

        public Ticket(String Summary, String Status, String Priority, String Submitter, String Assigned, String Watching)
        {

            if (idDestroy == -1)
            {
                this.TicketNumber = Ticket.idNext;
                Ticket.idNext++;
            }
            else
            {
                this.TicketNumber = Ticket.idDestroy;
            }
            this.Summary = Summary;
            this.Status = Status;
            this.Priority = Priority;

            if (Person.AllPeople.Any(item => item.Equals(Submitter)))
            {
                this.Submitter = Person.AllPeople.Find(item => item.FullName.Equals(Submitter));
            }
            else
            {
                this.Submitter = new Person(Submitter);
            }

            if (Person.AllPeople.Any(item => item.FullName.Equals(Assigned)))
            {
                this.Assigned = Person.AllPeople.Find(item => item.FullName.Equals(Assigned));
            } else
            {
                this.Assigned = new Person(Assigned); 
            }

            this.Watching = new WatcherList<Person>();

            var personList = Watching.Split('|');
            Dump(personList);

            for (int g = 0; g < personList.Length; g++)
            {
                if (Person.AllPeople.Any(item => item.FullName.Equals(personList[g])))
                {
                    this.Watching.Add(Person.AllPeople.Find(item => item.FullName.Equals(personList[g])));
                }
                else
                {
                    this.Watching.Add(new Person(personList[g]));
                }
            }

        }
        private static void Dump(object o)
        {
            string json = JsonConvert.SerializeObject(o, Formatting.Indented);
            Console.WriteLine(json);
        }
        public class WatcherList<Person> : List<Person>
        {
            public override string ToString()
            {
                var str = "";
                int thisCount = this.Count();

                if (thisCount > 0)
                {
                    for (int g = 0; g < thisCount; g++)
                    {
                        str += this.ElementAt(g) + "|";
                    }
                }
                return str.Substring(0, str.Length - 1);
            }
        }

        public override string ToString()
        {
            var str = TicketNumber + "," + Summary + "," + Status + "," + Priority + "," + Submitter + "," + Assigned;
            int numWatching = Watching.Count();

            if (numWatching > 0)
            {
                str += ",";

                str += Watching.ToString();

            }

            return str;
        }

        public void Dispose()
        {
            Ticket.idDestroy = this.TicketNumber;
        }

        private void LogError(string eMessage, string location)
        {
            Console.WriteLine(eMessage, location);
        }
    }

    class Person : IDisposable
    {
        private static int idNext = 1;
        private static int idDestroy = -1;
        public static List<Person> AllPeople = new List<Person>();
        public int idNumber { get; private set; }
        public String FullName { get; set; }
        public String Email { get; private set; }
        public String Phone { get; private set; }

        public Person(String FullName)
        {
            if (idDestroy == -1)
            {
                this.idNumber = Person.idNext;
                Person.idNext++;
            }
            else
            {
                this.idNumber = Person.idDestroy;
            }

            this.FullName = FullName;
            this.Email = "null";
            this.Phone = "null";

            Person.AllPeople.Add(this);
        }

        public override string ToString()
        {
            return FullName;
        }

        public void Dispose()
        {
            Person.idDestroy = this.idNumber;
        }

    }

    class Program
    {
        public static List<Ticket> tickets = new List<Ticket>();
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
                "List All Tickets",
                "List All People",
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

                Console.Write("[=+> # <+=] Choose ({0}-{1})", menuMin, menuMax);           // menu prompt
                Console.SetCursorPosition(5, Console.CursorTop); // Move the cursor to the '#' in the prompt
                StringBuilder sb = new StringBuilder(); // Stringbuilder object that will hold the user's menu choice
                ConsoleKeyInfo cki;                     // ConsoleKeyInfo object
                cki = Console.ReadKey();                // Get the input from the user
                sb.Append(cki.KeyChar);                 // Throw that input into the StringBuilder object so it can be parsed.

                int userChoice = 0;
                try
                {
                    userChoice = Int32.Parse(sb.ToString());
                }
                catch
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
                        ListAllTickets();
                        break;
                    case 4:
                        ListAllPeople();
                        break;
                    case 5:
                        ExitGracefully();
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

        public void ListAllTickets()
        {
            DisplayHeader();

            Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,25}\n", "TicketID", "Summary", "Status", "Priority", "Submitter", "Assigned", "Watching");

            foreach (var item in tickets)
            {
                Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,25}\n", item.TicketNumber, item.Summary, item.Status, item.Priority, item.Submitter, item.Assigned, item.Watching);
            }

            PressEnterToContinue();

        }
        public void ListAllPeople()
        {
            DisplayHeader();
            Console.Write("{0,-10}{1,-25}{2,-25}{3,-25}\n", "idNumber", "Full Name", "E-Mail", "Phone");
            foreach (var item in Person.AllPeople)
            {
                Console.Write("{0,-10}{1,-25}{2,-25}{3,-25}\n", item.idNumber, item.FullName, item.Email, item.Phone);
            }
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
            Console.WriteLine("{0,15}", invalidChoice);
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
            Console.WriteLine("Read File");
            var pathWithEnv = @"%USERPROFILE%\Documents\GS Ticket System-Tickets.txt";
            var fileData = Environment.ExpandEnvironmentVariables(pathWithEnv);

            Console.WriteLine("File Location: " + pathWithEnv);

            if (!File.Exists(fileData))
            {
                DisplayHeader();
                Console.WriteLine("The file does not exist, so I will create test data for you.");
                tickets.Add(new Ticket("This is a bug ticket", "Open", "High", "Drew Kjell", "Jane Doe", "Drew Kjell|John Smith|Bill Jones"));
                Console.WriteLine("Test Data has been committed to memory.");
                WriteFile();
                Console.WriteLine("Test Data has been written out to the file.");

                PressEnterToContinue();
            }

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
                        // The first field is NOT all digits, therefore this is our Header Row. Let's display it on the console for reasons.
                        Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,25}\n", new object[] { arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6] });
                    }
                    else
                    {
                        // we're actually discarding the ticket's number as saved in the file (arr[0]) because the Ticket() 
                        // constructor takes care of all ticket number creation.
                        // if user does not read file first and instead enters new tickets first, reading the file could result
                        // in overwriting or loss of data, so we'll just let the constructor deal with adding the ticket number.
                        // BUT, we'll display the ticket numbers from the file anyway, because why not?
                        Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,25}\n", new object[] { arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6] });
                        tickets.Add(new Ticket(arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]));
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
            var pathWithEnv = @"%USERPROFILE%\Documents\GS Ticket System-Tickets.txt";
            var fileData = Environment.ExpandEnvironmentVariables(pathWithEnv);
            try
            {
                using (StreamWriter sw = new StreamWriter(fileData, false)) // bool false = NO append.
                {
                    // CSV Header
                    sw.Write("TicketID,Summary,Status,Priority,Submitter,Assigned,Watching1|Watching2\n");

                    foreach (var ticket in tickets)
                    {
                        String str = ticket.ToString();
                        Console.WriteLine("Output to File:\n{0}\n", str);
                        sw.Write(str + "\n");
                    }
                    sw.Close();
                }
            }
            catch (IOException e)
            {
                Console.Write("The file could not be written: {0}", e.Message);
            }
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
