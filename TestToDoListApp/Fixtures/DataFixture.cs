using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.DAL.Entity.Identity;

namespace ToDoListAppTest.Fixtures
{
    internal class DataFixture
    {
        public static List<User> GetUsers(int count, bool useNewSeed = false)
        {
            return GetUserFaker(useNewSeed).Generate(count);
        }

        private static Faker<User> GetUserFaker(bool useNewSeed) 
        {
            var seed = 0;
            if (useNewSeed)
            {
                seed = Random.Shared.Next(10, int.MaxValue);
            }

            return new Faker<User>()
                .RuleFor(t => t.Email, (faker, t) => faker.Internet.Email(t.UserName))
                .UseSeed(seed);
        }

    }
}
