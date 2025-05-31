using System;
class Program
{
    static string filePath = "notes.txt";
    class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public string ToFileString()
        {
            return $"{Id}|{Title}|{Description}|{CreatedAt}|{string.Join(",", Tags)}";
        }
        public static Note FromFileString(string line)
        {
            string[] parts = line.Split('|');
            return new Note
            {
                Id = int.Parse(parts[0]),
                Title = parts[1],
                Description = parts[2],
                CreatedAt = DateTime.Parse(parts[3]),
                Tags = parts.Length > 4 ? new List<string>(parts[4].Split(',')) : new List<string>()
            };
        }


    }
    static List<Note> Notes = new List<Note>();
    static int nextId = 1;
    static void Main()
    {
        LoadNotes();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("📒 Simple Note Manager");
            Console.WriteLine("1. Add Note");
            Console.WriteLine("2. View Notes");
            Console.WriteLine("3. Edit Note");
            Console.WriteLine("4. Delete Note");
            Console.WriteLine("5. Search Notes");
            Console.WriteLine("6. Sort Notes");
            Console.WriteLine("7. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddNote();
                    break;
                case "2":
                    ViewNote();
                    break;

                case "3":
                    EditNote();
                    break;
                case "4":
                    DeleteNote();
                    break;
                case "5":
                    SearchNotes();
                    break;
                case "6":
                    SortNotes();
                    break;
                case "7":
                    return;
                default: Console.WriteLine("Invalid Choice!"); break;
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }

    static void LoadNotes()
    {
        if (!File.Exists(filePath)) return;
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] parts = line.Split("|");
            if (parts.Length >= 4)
            {
                Notes.Add(new Note
                {
                    Id = int.Parse(parts[0]),
                    Title = parts[1],

                    CreatedAt = DateTime.Parse(parts[2]),
                    Description = parts[3],
                    Tags = parts.Length > 4 ? new List<string>(parts[4].Split(',')) : new List<string>()

                });
            }

            if (Notes.Count > 0)
            {
                nextId = Notes[^1].Id + 1;
            }
        }
    }

    static void SearchNotes()
    {
        Console.WriteLine("Enter keyword to search for:");
        string keyword = Console.ReadLine();

        var results = Notes.FindAll(n => n.Title.ToLower().Contains(keyword) || n.Description.ToLower().Contains(keyword));
        if (results.Count == 0)
        {
            Console.WriteLine("🔍 No matching notes found.");
            return;
        }
        Console.WriteLine("🔎 Matching Notes:");

        foreach (var note in results)
        {
            Console.WriteLine($"{note.Id}. {note.Title}: \n {note.Description}");

        }
    }
    static void SaveNotes()
    {
        List<string> lines = new List<string>();
        foreach (var note in Notes)
        {
            lines.Add($"{note.Id}|{note.Title}|{note.CreatedAt}|{note.Description}|{note.Tags}");
        }

        File.WriteAllLines(filePath, lines);
    }


    static void AddNote()

    {
        Console.Write("Enter Title:");
        string Title = Console.ReadLine();
        Console.Write("Enter Description:");
        string Description = Console.ReadLine();
        Console.Write("Add tags (comma-separated, like school,urgent,fun): ");
        string tagInput = Console.ReadLine();
        List<string> tags = tagInput.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
        Notes.Add(new Note
        {
            Id = nextId++,
            Title = Title ?? "Untitled",
            Description = Description ?? "No Description Provided",
            CreatedAt = DateTime.Now,
            Tags = tags
        });
        Console.WriteLine("New Note Added!");
        SaveNotes();
    }
    static void ViewNote()
    {
        if (Notes.Count == 0)
        {
            Console.WriteLine("No Notes Yet");
            return;
        }
        foreach (var note in Notes)
        {
            Console.WriteLine($"{note.Id}. {note.Title}");
            Console.WriteLine($"  {note.CreatedAt}");

            Console.WriteLine($"{note.Description}");
            Console.WriteLine(new string('-', 20));
            Console.WriteLine($" Tags: {string.Join(", ", note.Tags)}");
            Console.WriteLine(new string('-', 40) + ">");
        }

    }
    static void ShowNotes()
    {
        foreach (var note in Notes)
        {
            Console.WriteLine($"{note.Id}. {note.Title}");
        }
    }

    static void EditNote()
    {
        ShowNotes();
        Console.WriteLine("Enter note number to edit:");
        int id = int.Parse(Console.ReadLine());
        Note note = Notes.Find(n => n.Id == id);
        if (note == null)
        {
            Console.WriteLine("No such notes found!");
            return;
        }

        Console.Write("New title: ");
        note.Title = Console.ReadLine();
        Console.Write("New content: ");
        note.Description = Console.ReadLine();

        Console.WriteLine("✅ Note updated!");
        SaveNotes();
    }

    static void DeleteNote()
    {
        ShowNotes();
        Console.Write("Enter note ID to delete: ");
        int id = int.Parse(Console.ReadLine());

        Note note = Notes.Find(n => n.Id == id);
        if (note == null)
        {
            Console.WriteLine("Note not found!");
            return;
        }


        Notes.Remove(note);
        Console.WriteLine("🗑️ Note deleted!");
        SaveNotes();
    }
    static void SortNotes()
    {
        Console.WriteLine("Sort by:");
        Console.WriteLine("1. Title A-Z");
        Console.WriteLine("2. Newest First");
        Console.WriteLine("3. Oldest First");
        Console.Write("Choose option: ");
        string input = Console.ReadLine();

        switch (input)
        {
            case "1":
                Notes = Notes.OrderBy(n => n.Title).ToList();
                break;
            case "2":
                Notes = Notes.OrderByDescending(n => n.CreatedAt).ToList();
                break;
            case "3":
                Notes = Notes.OrderBy(n => n.CreatedAt).ToList();
                break;
            default:
                Console.WriteLine("Invalid choice!");
                return;
        }

        Console.WriteLine("✅ Notes sorted!");
        ListNotes();

    }
    static void ListNotes()
    {
        foreach (var note in Notes)
        {
            Console.WriteLine($"{note.Id}. {note.Title}");
            Console.WriteLine($"   🕒 {note.CreatedAt}");
            Console.WriteLine($"   📌 Tags: {string.Join(", ", note.Tags)}");
            Console.WriteLine($"   📝 {note.Description}");
            Console.WriteLine();
        }
    }

}