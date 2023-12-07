using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

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
    }

    public void RemoveTask(Task task) {
        Tasks?.Remove(task);
    }

    public void UpdateTask(Task task, string? description, string? status) {
        var updateObject = Tasks?.FirstOrDefault(task);

        if(description != null) {
            updateObject.Description = description;
        }

        if(status != null) {
            updateObject.Status = status;
        }
    }

    public void AddProgrammer(Programmer programmer) {
        Programmers?.Add(programmer);
    }

    public void RemoveProgrammer(Programmer programmer) {
        Programmers?.Remove(programmer);
    }

    public void UpdateProgrammer(Programmer programmer, string? fullName, List<string> skills) {
        var updateObject = Programmers?.FirstOrDefault(programmer);

        if(fullName != null) {
            updateObject.FullName = fullName;
        }

        if(skills != null) {
            updateObject.Skills = skills;
        }
    }
}

class Program {
    static void Main() {
        // List<Programmer> programmers = new List<Programmer>() {
        //     new Programmer {ProgrammerId = Guid.NewGuid(), FullName = "Dominik Machowski", Skills = new List<string>() {"HTML", "CSS", "JS"}},
        //     new Programmer {ProgrammerId = Guid.NewGuid(), FullName = "Karol Jurzyk", Skills = new List<string>() {"HTML", "CSS", "JS"}},
        //     new Programmer {ProgrammerId = Guid.NewGuid(), FullName = "Jan Piórkowski", Skills = new List<string>() {"HTML", "CSS", "JS"}},
        // };

        // List<Task> tasks = new List<Task>() {
        //     new Task {TaskId = Guid.NewGuid(), Description = "Lorem ipsum", Status = "To Do"},
        //     new Task {TaskId = Guid.NewGuid(), Description = "Lorem ipsum", Status = "Doing"},
        //     new Task {TaskId = Guid.NewGuid(), Description = "Lorem ipsum", Status = "To Do"}
        // };

        // SoftwareProject proj = new SoftwareProject {Tasks = tasks, Programmers = programmers, Name = "LoremIpsum", Deadline = "12-12-2024", Pricing = 100000};

        JsonSerializerOptions options = new JsonSerializerOptions { 
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // string jsonString = JsonSerializer.Serialize(proj, options);
        // File.WriteAllText("SoftwareProject.json", jsonString);

        string jsonDeserializeString = File.ReadAllText("SoftwareProject.json");
        SoftwareProject deserializedProj = JsonSerializer.Deserialize<SoftwareProject>(jsonDeserializeString, options);
        Console.WriteLine(deserializedProj.Name);
    }
}