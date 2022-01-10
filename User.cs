class User()
{
    string Username{get;set;}
    string Age{get;set;}
    string Lastname{get;set;}
}

class Program
{
    static void Main(string[] args)
    {
        var records = new List<string>();
        string propName = Console.ReadLine();
        string propValue = Console.ReadLine();
        var properties = typeof(User).GetProperties();

        var newRecords = records.Where(rec => rec.Id == propValue);

        System.Console.WriteLine();   
    }
}

enum Properties
{
    Username,
    Age,
    Lastname,
}