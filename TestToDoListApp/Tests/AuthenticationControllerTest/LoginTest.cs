using AutoFixture;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.Contracts.Responses.Base;
using ToDoListApp.Enums;
using ToDoListAppTest.Fixtures;

namespace ToDoListAppTest.Tests.AuthenticationControllerTest
{
    public class LoginTest : IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly DockerWebApplicationFactoryFixture _factory;
        private readonly HttpClient _client;
        private readonly Fixture _fixture;

        public LoginTest(DockerWebApplicationFactoryFixture factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("Ps1234@", false)]
        public async Task Validate_Password_Login_With_Registration(string password, bool shouldFail)
        {
            var fakeEmail = new Faker<RegistrationRequestModel>()
               .RuleFor(x => x.Email, t => t.Internet.Email())
               .Generate(1)
               .First();

            var fakeRequest = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, "user")
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, fakeEmail.Email)
                .Create();


            var request = _fixture.Build<LoginRequestModel>()
                .With(x => x.Username, fakeRequest.Username)
                .With(x => x.Password, password)
                .Create();

            var register = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, fakeRequest);

            register.Should().NotBeNull();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Login, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<TokenResponseModel>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().Be(ErrorCode.ValidationError);

                body.Errors.Keys.Should().Contain(nameof(LoginRequestModel.Password));
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<TokenResponseModel>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();
                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
                body!.Data.AccessToken.Should().NotBeNull();
                body!.Data.RefreshToken.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("user", false)]
        public async Task Validate_UserName_Without_Registration(string username, bool shouldFail)
        {
            var fakeEmail = new Faker<RegistrationRequestModel>()
              .RuleFor(x => x.Email, t => t.Internet.Email())
              .Generate(1)
              .First();

            var fakeRequest = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, "user")
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, fakeEmail.Email)
                .Create();

            var request = _fixture.Build<LoginRequestModel>()
                .With(x => x.Username, username)
                .With(x => x.Password, fakeRequest.Password)
                .Create();

            var register = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, fakeRequest);

            register.Should().NotBeNull();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Login, request);

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

                body.Errors.Keys.Should().Contain(nameof(LoginRequestModel.Username));
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<TokenResponseModel>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();
                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
                body!.Data.AccessToken.Should().NotBeNull();
                body!.Data.RefreshToken.Should().NotBeNull();
            }
        }

        [Theory]
        //[InlineData(null, true)]
        //[InlineData("", true)]
        [InlineData("user", false)]
        [InlineData("user1", true)]
        public async Task Should_Throw_UserNotFoundException(string username, bool shouldFail)
        {
            var fakeEmail = new Faker<RegistrationRequestModel>()
              .RuleFor(x => x.Email, t => t.Internet.Email())
              .Generate(1)
              .First();

            var fakeRequest = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, "user")
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, fakeEmail.Email)
                .Create();

            var request = _fixture.Build<LoginRequestModel>()
                .With(x => x.Username, username)
                .With(x => x.Password, fakeRequest.Password)
                .Create();

            var register = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, fakeRequest);

            register.Should().NotBeNull();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Login, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<TokenResponseModel>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().Be(ErrorCode.UserNotFound);
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<TokenResponseModel>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();
                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
                body!.Data.AccessToken.Should().NotBeNull();
                body!.Data.RefreshToken.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("Ps12@", true)]
        [InlineData("Pass12345@", true)]
        [InlineData("Ps1234@", false)]
        public async Task Should_Throw_InvalidPaswordException(string password, bool shouldFail)
        {
            var fakeEmail = new Faker<RegistrationRequestModel>()
              .RuleFor(x => x.Email, t => t.Internet.Email())
              .Generate(1)
              .First();

            var fakeRequest = _fixture.Build<RegistrationRequestModel>()
                .With(x => x.Username, "user")
                .With(x => x.Password, "Ps1234@")
                .With(x => x.Email, fakeEmail.Email)
                .Create();

            var request = _fixture.Build<LoginRequestModel>()
                .With(x => x.Username, "user")
                .With(x => x.Password, password)
                .Create();

            var register = await _client.PostAsJsonAsync(HttpHelper.Urls.Register, fakeRequest);

            register.Should().NotBeNull();

            // act
            var httpResponse = await _client.PostAsJsonAsync(HttpHelper.Urls.Login, request);

            httpResponse.Should().NotBeNull();

            // assert
            if (shouldFail)
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<TokenResponseModel>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().Be(ErrorCode.InvalidPasword);
            }
            else
            {
                httpResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var content = await httpResponse.Content.ReadAsStringAsync();

                content.Should().NotBeNull();

                var body = JsonConvert.DeserializeObject<ApiResult<TokenResponseModel>>(content);

                body.Should().NotBeNull();

                body!.ErrorCode.Should().BeNull();
                body.Errors.Should().BeNullOrEmpty();

                body!.Data.Should().NotBeNull();
                body!.Data.AccessToken.Should().NotBeNull();
                body!.Data.RefreshToken.Should().NotBeNull();
            }
        }
    }
}
