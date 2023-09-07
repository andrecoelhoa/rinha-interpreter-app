



var pathFiles = System.IO.Path.Exists("/var/rinha") ?
    System.IO.Directory.GetFiles("/var/rinha", "*.rinha.json") :
    System.IO.Directory.GetFiles(AppContext.BaseDirectory, "*.rinha.json");

foreach (var pathFile in pathFiles)
{
    System.Console.WriteLine($"Running {pathFile.Split('/').Last()}");
    var file = System.IO.File.ReadAllText(pathFile);
    var ast = JsonConvert.DeserializeObject<File>(file);
    var result = new Interpreter().Run(ast);
}

