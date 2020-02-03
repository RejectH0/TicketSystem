using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TicketSystem
{
    // Version 1.2 //02FEB2020 1945
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

            if (Person.AllPeople.Any(item => item.FullName.Equals(Submitter)))
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
            }
            else
            {
                this.Assigned = new Person(Assigned);
            }

            this.Watching = new WatcherList<Person>();
            if (!(Watching == null || Watching.Equals("none")))
            {
                var personList = Watching.Split('|');

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

        }
        public Ticket(String Summary, String Status, String Priority, Person Submitter, Person Assigned, List<Person> Watching)
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

            this.Submitter = Person.AllPeople.Find(item => item.Equals(Submitter));
            this.Assigned = Person.AllPeople.Find(item => item.Equals(Assigned));
            this.Watching = new WatcherList<Person>();
            foreach (var item in Watching)
            {
                this.Watching.Add(item);
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
                if (thisCount > 1)
                {
                    for (int g = 0; g < thisCount; g++)
                    {
                        str += this.ElementAt(g) + "|";
                    }
                    return str.Substring(0, (str.Length) - 1);
                } else
                {
                    str += this.ElementAt(0);
                    return str;
                }
                
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
        private static int index = 0;
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
        private int PrintMainMenu()
        {
            string menuName = "Main";
            string[] menuChoices = new string[]
            {
                "Read Ticket File",
                "Create Ticket",
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
                        CreateTicket();
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

        public void CreateTicket()
        {
            List<string> summarySelection = new List<string>()
            {
                "Bug Ticket",
                "Problem Ticket",
                "Information Only Ticket",
                "Move/Add/Change Ticket",
                "Test Ticket"
            };

            List<string> prioritySelection = new List<string>()
            {
                "Low",
                "Routine",
                "Priority",
                "Emergency"
            };


            DisplayHeader();
            Console.SetCursorPosition(0, 6);
            string menuText = "-=+> Create Ticket <+=-";
            Console.SetCursorPosition(0, 6);
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (menuText.Length / 2)) + "}", menuText));
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Please choose Ticket Priority:");
            
            string ticketPriority = MenuItemSelection(prioritySelection);
            Console.SetCursorPosition(75, 7);
            string ticketStatus = "New";
            Console.WriteLine("Ticket Status   : {0}",ticketStatus);
            Console.SetCursorPosition(75, 8);
            Console.WriteLine("Ticket Priority : {0}", ticketPriority);
            Console.SetCursorPosition(0, 7);
            ConsoleSpaces(50,5);
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Please choose Summary Selection:");
            string ticketSummary = MenuItemSelection(summarySelection);
            Console.SetCursorPosition(75, 9);
            Console.WriteLine("Ticket Summary  : {0}", ticketSummary);
            Console.SetCursorPosition(0, 7);
            ConsoleSpaces(50, 5);
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Who is submitting this ticket?");
            Person ticketSubmitter = MenuItemPersonSelection();
            Console.SetCursorPosition(75, 10);
            Console.WriteLine("Ticket Submitter: {0}", ticketSubmitter);
            Console.SetCursorPosition(0, 7);
            ConsoleSpaces(50,5);
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Please assign this ticket: ");
            Person ticketAssigned = MenuItemPersonSelection();
            Console.SetCursorPosition(75, 11);
            Console.WriteLine("Ticket Assigned : {0}", ticketAssigned);

            ConsoleKey userResponse;
            //string Key;
            List<Person> watchers = new List<Person>();

            do
            {
                Console.SetCursorPosition(0, 7);
                ConsoleSpaces(60,10);
                Console.SetCursorPosition(0, 7);
                Console.WriteLine("Please choose who will watch this ticket:");
                Person watcher = MenuItemPersonSelection();
                watchers.Add(watcher);
                Console.SetCursorPosition(75, (11 + (watchers.Count())));
                Console.WriteLine("Watcher #{0}: {1}", watchers.Count(), watcher);
                Console.SetCursorPosition(0, 7);
                ConsoleSpaces(60,10);
                Console.SetCursorPosition(0, 7);
                Console.WriteLine("Would you like to add another Watcher? (Y/N): ");
                userResponse = Console.ReadKey(true).Key;
                Console.SetCursorPosition(0, 7);
                ConsoleSpaces(60,10);
            } while (userResponse == ConsoleKey.Y);

            tickets.Add(new Ticket(ticketSummary, ticketStatus, ticketPriority, ticketSubmitter, ticketAssigned, watchers));
            Console.SetCursorPosition(75, 11);
            PressEnterToContinue();
        }

        private void ConsoleSpaces(int spaces, int lines)
        {
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            
            
            for (int i = 0; i < lines; i++)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop + i);
                for (int g = 0; g < spaces; g++)
                {
                    Console.Write(" ");
                }
                
            }
        }
        public Person MenuItemPersonSelection()
        {
            index = 0;
            string str = "";
            Console.CursorVisible = false;
            List<string> people = new List<string>();
            people.Add("[New]");
            
            foreach (var item in Person.AllPeople)
            {
                people.Add(item.FullName);
            }

            while (str.Length == 0)
            {
                str = drawMenu(people, Console.CursorLeft, Console.CursorTop);
            }
            Console.CursorVisible = true;
            
            if (str.Equals("[New]")) {
                Console.SetCursorPosition(10, 5);
                string newName = getStringValue("Please enter the person's Full Name");
                Console.SetCursorPosition(10, 5);
                ConsoleSpaces(50,2);
                new Person(newName);
                return Person.AllPeople.Find(item => item.FullName.Equals(newName));
            } else
            {
                return Person.AllPeople.Find(item => item.FullName.Equals(str));
            }

        }

        public string MenuItemSelection(List<string> menuItems)
        {
            index = 0;
            string str = "";

            Console.CursorVisible = false;
            while (str.Length == 0)
            {
                str = drawMenu(menuItems,Console.CursorLeft,Console.CursorTop);
            }
            Console.CursorVisible = true;
            return str;
        }

        private string drawMenu(List<string> items,int cursorLeft, int cursorTop)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
            for (int g = 0; g < items.Count; g++)
            {
                if (g == index)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(items[g]);
                }
                else
                {
                    Console.WriteLine(items[g]);
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

            ConsoleKeyInfo ckey = Console.ReadKey();

            if (ckey.Key == ConsoleKey.DownArrow)
            {
                if (index == items.Count - 1)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop); 
                    index = 0;
                }
                else
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop); 
                    index++;
                }
            }
            else if (ckey.Key == ConsoleKey.UpArrow)
            {

                if (index <= 0)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop); 
                    index = items.Count - 1;
                }
                else
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop); 
                    index--;
                }
            }
            else if (ckey.Key == ConsoleKey.Enter)
            {
                return items[index];
            }
            else
            {
                return "";
            }
            return "";
        }
        public void ListAllTickets()
        {
            DisplayHeader();

            Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,-20}\n", "TicketID", "Summary", "Status", "Priority", "Submitter", "Assigned", "Watching");

            var ticketsCount = tickets.Count();
            for (int g = 0; g < ticketsCount; g++)
            {
                bool isEmpty = !tickets[g].Watching.Any();
                if (isEmpty)
                {
                    Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,-30}\n", tickets[g].TicketNumber, tickets[g].Summary, tickets[g].Status, tickets[g].Priority, tickets[g].Submitter, tickets[g].Assigned, "none");
                } else
                {
                    Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,-30}\n", tickets[g].TicketNumber, tickets[g].Summary, tickets[g].Status, tickets[g].Priority, tickets[g].Submitter, tickets[g].Assigned, tickets[g].Watching);
                }
            }

            PressEnterToContinue();

        }
        private static void Dump(object o)
        {
            string json = JsonConvert.SerializeObject(o, Formatting.Indented);
            Console.WriteLine(json);
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
            Console.ReadKey(false);
            Console.WriteLine();
        }

        private string getStringValue(String prompt)
        {
            var str = "";
            while (true)
            {
                Console.Write((prompt != null) ? prompt : "Please enter your response");
                Console.Write(": ");
                str = Console.ReadLine();

                if (str.Equals("-99"))
                {
                    ExitGracefully();
                }
                else if (str.Equals(""))
                {
                    Console.WriteLine("Invalid entry. Please try again.");
                }
                else
                {
                    return str;
                }
            }
        }

        public void ReadFile()
        {
            DisplayHeader();
            Console.WriteLine("Read File");
            var pathWithEnv = @"%USERPROFILE%\Documents\GS Ticket System-Tickets.txt";
            var fileData = Environment.ExpandEnvironmentVariables(pathWithEnv);

            Console.WriteLine("File Location: " + fileData);

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
                    List<string> list = new List<string>(line.Split(','));
                    
                    // handling for blank lines in the file
                    if (list.Count() == 1)
                    {
                        continue;
                    }

                    // handling for a blank Watchers field
                    if (list.Count() == 6)
                    {
                        list.Add("none");
                    }
                    // remember there's a header row!
                    // TicketID, Summary, Status, Priority, Submitter, Assigned, Watching
                    // 1,This is a bug ticket,Open,High,Drew Kjell,Jane Doe,Drew Kjell|John Smith|Bill Jones
                    if (!list[0].All(char.IsDigit))
                    {
                        // The first field is NOT all digits, therefore this is our Header Row. Let's display it on the console for reasons.
                        Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,25}\n", list.ToArray());
                    }
                    else
                    {
                        // we're actually discarding the ticket's number as saved in the file (list[0]) because the Ticket() 
                        // constructor takes care of all ticket number creation.
                        // if user does not read file first and instead enters new tickets first, reading the file could result
                        // in overwriting or loss of data, so we'll just let the constructor deal with adding the ticket number.
                        // BUT, we'll display the ticket numbers from the file anyway, because why not?
                        Console.Write("{0,-10}{1,-25}{2,-10}{3,-10}{4,-15}{5,-15}{6,25}\n", list.ToArray());
                        tickets.Add(new Ticket(list[1], list[2], list[3], list[4], list[5], list[6]));
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
