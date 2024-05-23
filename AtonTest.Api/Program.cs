using AtonTest;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(x => x.UseStartup<Startup>());

        builder.Build().Run();
    }
}