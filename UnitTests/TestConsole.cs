namespace UnitTests
{
    public static class TestConsole
    {
        public static void Passed(string testName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Test {testName} passed");
            Console.ResetColor();
        }

        public static void Failed(string testName)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Test {testName} failed");
            Console.ResetColor();
        }
    }
}
