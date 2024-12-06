using Resources;
using UnitTests;

public static class Program // figure out how to run a headless version of the OpenTK gamewindow
{
    static Storage storage = new();

    static void Main()
    {
        new ResourceTest().Run();
    }
}