using AutoFixture;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Json;
using ToDoListApp.BLL.Models.Dto;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses.Base;
using ToDoListApp.Enums;
using ToDoListAppTest.Fixtures;

namespace ToDoListAppTest.Tests.AuthenticationControllerTest
{
    public class RegistationTest : IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly DockerWebApplicationFactoryFixture _factory;
        private readonly HttpClient _client;
        private readonly Fixture _fixture;

        public RegistationTest(DockerWebApplicationFactoryFixture factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Get_Returns_401_Unauthorized_If_Not_Authenticated()
        {
            // arrange

            // act
            var response = await _client.GetAsync(HttpHelper.Urls.GetAllTasks);

            // assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("Tes", true)]
        [InlineData("TesName12", true)]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("TesName", false)]
        public async Task Validate_UserName_Length(string userName, bool shouldFail)
        {
            // arrange

            var fakeEmail = new Faker<RegistrationRequestModel>()
                .RuleFor(x => x.Email, t => t.Internet.Email())
                .Generate(1)
                .First();

            var request = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, userName)
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, fakeEmail.Email)
                .Create();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {

                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().Be(ErrorCode.ValidationError);

                body.Errors.Keys.Should().Contain(nameof(RegistrationRequestModel.Username));
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<UserDto>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();

                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("Ps12@", true)]
        [InlineData("Pass123456@", false)]
        [InlineData("Pass1234567@", false)]
        [InlineData("Pass12345678@", true)]
        [InlineData(null, true)]
        [InlineData("", true)]
        public async Task Validate_Password_Length(string password, bool shouldFail)
        {
            var fakeEmail = new Faker<RegistrationRequestModel>()
                 .RuleFor(x => x.Email, t => t.Internet.Email())
                 .RuleFor(x => x.Username, t => t.Name.LastName())
                 .Generate(1)
                 .First();

            var request = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, fakeEmail.Username)
                .With(x => x.Password, password)
                .With(x => x.Email, fakeEmail.Email)
                .Create();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().Be(ErrorCode.ValidationError);

                body.Errors.Keys.Should().Contain(nameof(RegistrationRequestModel.Password));
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<UserDto>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();

                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("user@gmail.com", false)]
        public async Task Validate_Email(string email, bool shouldFail)
        {
            var request = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, "user")
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, email)
                .Create();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().Be(ErrorCode.ValidationError);

                body.Errors.Keys.Should().Contain(nameof(RegistrationRequestModel.Email));
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<UserDto>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();

                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
            }
        }

        [Theory]
        //[InlineData(null, true)]
        //[InlineData("", true)]
        [InlineData("raptor@gmail.com", false)]
        public async Task Should_Throw_EmailAlreadyException(string email, bool shouldFail)
        {
            var request = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, "user")
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, email)
                .Create();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult>(content);

                body.Should().NotBeNull();

                body.ErrorCode.Should().Be(ErrorCode.EmailAlreadyExist);

                body.Errors.Keys.Should().Contain(nameof(RegistrationRequestModel.Email));
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<UserDto>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();

                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("Ps12@",true)]
        [InlineData("Pass12345@",false)]
        public async Task Should_Throw_UserNotCreatedException(string password, bool shouldFail)
        {
            var request = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, "bobik")
                .With(x => x.Password, password)
                .With(x => x.Email, "bob@gmail.com")
                .Create();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().Be(ErrorCode.ValidationError);

                body.Errors.Keys.Should().Contain(nameof(RegistrationRequestModel.Password));
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<UserDto>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();

                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
            }
        }

        [Theory]
        //[InlineData("Tes", true)]
        //[InlineData("TesName12", true)]
        [InlineData("test12")]
        //[InlineData(null, true)]
        //[InlineData("", true)]
        public async Task Should_Throw_UserNameAlreadyExistsException(string userName)
        {
            // arrange

            var fakeEmail = new Faker<RegistrationRequestModel>()
                .RuleFor(x => x.Email, t => t.Internet.Email())
                .Generate(1)
                .First();

            var request = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, userName)
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, fakeEmail.Email)
                .Create();


            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, request);
            var httpResponse1 = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, request);

            httpResponse.Should().NotBeNull();
            httpResponse1.Should().NotBeNull();

            // assert

            httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            httpResponse1.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await httpResponse1.Content.ReadAsStringAsync();

            content.Should().NotBeNull();

            var body = JsonConvert.DeserializeObject<ApiResult>(content);

            body.Should().NotBeNull();

            body!.ErrorCode.Should().Be(ErrorCode.UserNameAlredyExist);
        }
    }
}
