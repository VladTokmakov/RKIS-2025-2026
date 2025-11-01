using System;

namespace Todolist
{
    class Program
    {
        private static Profile user;
        private static Todolist todoList = new Todolist();

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Токмаков и Неофидис");
            SetDataUserCommand();

            while (true)
            {
                Console.Write("Введите команду: ");
                string input = Console.ReadLine().Trim();

                ICommand command = CommandParser.Parse(input, todoList, user);                    
                if (command != null) command.Execute();
            }
        }

        static void SetDataUserCommand()
        {
            var setDataUserCommand = new SetDataUserCommand();
            setDataUserCommand.Execute();
            user = setDataUserCommand.User;
        }
    }
}