using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListAppTest.Fixtures;

namespace ToDoListAppTest.Tests.UsersTasksControllerTest
{
    public class UsersTasksControllerTest: IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly DockerWebApplicationFactoryFixture _factory;
        private readonly HttpClient _client;

        public UsersTasksControllerTest(DockerWebApplicationFactoryFixture factory, HttpClient client)
        {
            _factory = factory;
            _client = client;
        }

    }
}
