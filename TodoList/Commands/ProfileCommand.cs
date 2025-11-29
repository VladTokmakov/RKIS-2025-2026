using System;
namespace Todolist
{
    public class ProfileCommand : ICommand
    {
        public Profile UserProfile { get; private set; }

        public ProfileCommand(Profile userProfile)
        {
            UserProfile = userProfile;
        }

        public void Execute()
        {
            if (UserProfile != null)
            {
                Console.WriteLine(UserProfile.GetInfo());
            }
            else
            {
                Console.WriteLine("Данные пользователя не найдены");
            }
        }
        public void Unexecute()
        {
        }
    }
}