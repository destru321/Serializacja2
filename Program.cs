using System.Data.Common;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

public static class Global {
    public static JsonSerializerOptions options = new JsonSerializerOptions { 
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
}
public class Task {
    [JsonPropertyName("Task_Id")]
    public Guid TaskId {get;set;}
    [JsonPropertyName("Task_description")]
    public string? Description {get;set;}
    [JsonPropertyName("Task_status")]
    public string? Status {get;set;}
}

public class Programmer {
    [JsonPropertyName("Programmer_Id")]
    public Guid ProgrammerId {get;set;}
    [JsonPropertyName("Programmer_fullname")]
    public string? FullName {get;set;}
    [JsonPropertyName("Programmer_skills")]
    public List<string>? Skills {get;set;}
}

public class SoftwareProject {
    public List<Task>? Tasks {get;set;}
    public List<Programmer>? Programmers {get;set;}
    public string? Name {get;set;}
    public string? Deadline {get;set;}
    public int Pricing {get;set;}

    public void AddTask(Task task) {
        Tasks?.Add(task);

        string jsonString = JsonSerializer.Serialize(this, Global.options);
        File.WriteAllText("SoftwareProject.json", jsonString);
    }

    public void RemoveTask(Guid id) {
        var updateObject = Tasks?.RemoveAll(task => task.TaskId == id);

        string jsonString = JsonSerializer.Serialize(this, Global.options);
        File.WriteAllText("SoftwareProject.json", jsonString);
    }

    public void UpdateTask(Guid id, string? status) {
        var updateObject = Tasks?.FirstOrDefault(task => task.TaskId == id);
        
        updateObject.Status = status;

        string jsonString = JsonSerializer.Serialize(this, Global.options);
        File.WriteAllText("SoftwareProject.json", jsonString);
    }

    public void AddProgrammer(Programmer programmer) {
        Programmers?.Add(programmer);

        string jsonString = JsonSerializer.Serialize(this, Global.options);
        File.WriteAllText("SoftwareProject.json", jsonString);
    }

    public void RemoveProgrammer(Guid id) {

        Programmers.RemoveAll(programmer => id == programmer.ProgrammerId);

        string jsonString = JsonSerializer.Serialize(this, Global.options);
        File.WriteAllText("SoftwareProject.json", jsonString);
    }

    public void UpdateProgrammer(Guid id, List<string> skills) {
        var updateObject = Programmers?.FirstOrDefault(programmer => id == programmer.ProgrammerId);

        updateObject.Skills = skills;

        string jsonString = JsonSerializer.Serialize(this, Global.options);
        File.WriteAllText("SoftwareProject.json", jsonString);
    }

    public void GenerateReport() {
        var report = new {
            TasksCompleted = Tasks.FindAll(t => t.Status == "Done").Count,
            TasksToDo = Tasks.FindAll(t => t.Status == "To Do").Count,
            TasksDoing = Tasks.FindAll(t => t.Status == "Doing").Count,
            Tasks = Tasks.Select(t=> new{t.TaskId,t.Description,t.Status}),
            Programmers = Programmers.Select(p=> new{p.ProgrammerId,p.FullName,Skills = string.Join(", ",p.Skills)})
        };

        string jsonReport = JsonSerializer.Serialize(report);
        File.WriteAllText("Report.json",jsonReport);
    }
}



class Program {
    static void Main() {
        if(File.Exists("SoftwareProject.json") == false) {
            List<Programmer> programmers = new List<Programmer>() {
                new Programmer {ProgrammerId = Guid.NewGuid(), FullName = "Dominik Machowski", Skills = new List<string>() {"HTML", "CSS", "JS"}},
                new Programmer {ProgrammerId = Guid.NewGuid(), FullName = "Karol Jurzyk", Skills = new List<string>() {"HTML", "CSS", "JS"}},
                new Programmer {ProgrammerId = Guid.NewGuid(), FullName = "Jan Piórkowski", Skills = new List<string>() {"HTML", "CSS", "JS"}},
            };

            List<Task> tasks = new List<Task>() {
                new Task {TaskId = Guid.NewGuid(), Description = "Lorem ipsum", Status = "To Do"},
                new Task {TaskId = Guid.NewGuid(), Description = "Lorem ipsum", Status = "Doing"},
                new Task {TaskId = Guid.NewGuid(), Description = "Lorem ipsum", Status = "To Do"}
            };

            SoftwareProject proj = new SoftwareProject {Tasks = tasks, Programmers = programmers, Name = "LoremIpsum", Deadline = "12-12-2024", Pricing = 100000};

            string jsonString = JsonSerializer.Serialize(proj, Global.options);
            File.WriteAllText("SoftwareProject.json", jsonString);
        }

        string jsonDeserializeString = File.ReadAllText("SoftwareProject.json");
        SoftwareProject deserializedProj = JsonSerializer.Deserialize<SoftwareProject>(jsonDeserializeString, Global.options);

        Console.WriteLine("1. Wygeneruj raport");
        Console.WriteLine("2. Dodaj programiste do projektu");
        Console.WriteLine("3. Usuń programiste z projektu");
        Console.WriteLine("4. Edytuj skille programisty");
        Console.WriteLine("5. Dodaj nowego taska");
        Console.WriteLine("6. Usuń taska");
        Console.WriteLine("7. Edytuj status taska");

        Console.WriteLine(" ");
        Console.Write("Co chcesz zrobić?(wpisz cyfre): ");
        var input = int.Parse(Console.ReadLine());

        switch(input) {
            case 1:
                deserializedProj.GenerateReport();
                Console.WriteLine("Raport został wygenerowany, mieści sie w pliku Report.json!");
                break;
            case 2:
                Console.Write("Wprowadz imie i nazwisko programisty: ");
                string fullName = Console.ReadLine();

                Console.Write("Wypisz skille programisty (po przecinku ): ");
                string skillString = Console.ReadLine();
                List<string> skills = skillString.Split(',').ToList();

                Programmer programmer = new Programmer {
                    ProgrammerId = Guid.NewGuid(),
                    FullName = fullName,
                    Skills = skills
                };
                deserializedProj.AddProgrammer(programmer);
                break;

            case 3:
                deserializedProj.Programmers.ForEach(programmer => {
                    Console.Write($"{deserializedProj.Programmers.IndexOf(programmer) + 1}. ");
                    Console.WriteLine(programmer.FullName);
                });

                Console.Write("Wybierz programiste ktorego chcesz usunac: ");
                int id = int.Parse(Console.ReadLine());

                deserializedProj.RemoveProgrammer(deserializedProj.Programmers[id - 1].ProgrammerId);
                break;

            case 4:
                deserializedProj.Programmers.ForEach(programmer => {
                    Console.Write($"{deserializedProj.Programmers.IndexOf(programmer) + 1}. ");
                    Console.WriteLine(programmer.FullName);
                });

                Console.Write("Wybierz programiste ktorego chcesz edytowac: ");
                int id2 = int.Parse(Console.ReadLine());

                Console.Write("Wypisz nowe skille programisty (po przecinku ): ");
                string skillString2 = Console.ReadLine();
                List<string> skills2 = skillString2.Split(',').ToList();

                deserializedProj.UpdateProgrammer(deserializedProj.Programmers[id2 - 1].ProgrammerId, skills2);
                break;

            case 5:
                Console.Write("Wprowadz opis taska: ");
                string description = Console.ReadLine();

                Task task = new Task {
                    TaskId = Guid.NewGuid(),
                    Description = description,
                    Status = "To Do"
                };
                deserializedProj.AddTask(task);
                break;

            case 6:
                deserializedProj.Tasks.ForEach(task => {
                    Console.Write($"{deserializedProj.Tasks.IndexOf(task) + 1}. ");
                    Console.WriteLine(task.Description);
                });

                Console.Write("Wybierz taska ktorego chcesz usunac: ");
                int id3 = int.Parse(Console.ReadLine());

                deserializedProj.RemoveTask(deserializedProj.Tasks[id3 - 1].TaskId);
                break;

            case 7:
                deserializedProj.Tasks.ForEach(task => {
                    Console.Write($"{deserializedProj.Tasks.IndexOf(task) + 1}. ");
                    Console.WriteLine(task.Description);
                });

                Console.Write("Wybierz taska ktorego chcesz edytowac: ");
                int id4 = int.Parse(Console.ReadLine());

                Console.Write("Podaj nowy status taska: ");
                string status2 = Console.ReadLine();

                deserializedProj.UpdateTask(deserializedProj.Tasks[id4 - 1].TaskId, status2);
                break;
        }
    
    }
}