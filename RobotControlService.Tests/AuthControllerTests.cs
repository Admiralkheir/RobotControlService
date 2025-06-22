using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using RobotControlService.Features.Auth;
using RobotControlService.Features.Auth.Login;
using RobotControlService.Features.Auth.GetUser;
using RobotControlService.Features.Auth.CreateUser;
using RobotControlService.Features.Auth.UpdateUser;
using Xunit;
using Testcontainers.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RobotControlService.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace RobotControlService.Tests
{
    public class AuthControllerTests : IAsyncLifetime
    {
        private readonly MongoDbContainer _mongoDbContainer;
        private WebApplicationFactory<RobotControlService.Program> _factory;
        private HttpClient _client;

        public AuthControllerTests()
        {
            _mongoDbContainer = new MongoDbBuilder()
                .WithImage("mongo:8.0")
                .WithReplicaSet("rs0")
                .WithCleanUp(true)
                .Build();


        }

        public async Task InitializeAsync()
        {
            await _mongoDbContainer.StartAsync();
            
            _factory = new WebApplicationFactory<RobotControlService.Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        var dict = new Dictionary<string, string>
                        {
                                        {"MongoConnection:ConnectionURI", _mongoDbContainer.GetConnectionString() },
                                        {"MongoConnection:DatabaseName", "RobotDb" },
                                        {"Jwt:Secret", "9e4fdc5baa20bc524a03fac342af7d916ec38111d5271d01a08fbe39ad95e5e8a0d9a0067b14d6d67fc21241886ffc8cd059101e8a79b971a1376035711b77a8" },
                                        {"Jwt:Issuer", "robotcontrol" },
                                        {"Jwt:Audience", "account" },
                                        {"TestAdminPassword", "admin123456" }
                        };
                        config.AddInMemoryCollection(dict);
                    });
                });
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            await _mongoDbContainer.DisposeAsync();
            _client.Dispose();
            _factory.Dispose();
        }

        private async Task<string> GetAdminTokenAsync()
        {
            var loginDto = new LoginDto("TestAdmin", "admin123456");
            var response = await _client.PostAsJsonAsync("/api/v1/Auth/Login", loginDto);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return loginResponse!.Token;
        }

        [Fact]
        public async Task Login_ReturnsOk_ForValidCredentials()
        {

            var loginDto = new LoginDto("TestAdmin", "admin123456");

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/Auth/Login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            loginResponse.Should().NotBeNull();
            loginResponse!.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GetUser_ReturnsOk_ForAdmin()
        {
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"/api/v1/Auth/GetUser?username=TestAdmin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var userResponse = await response.Content.ReadFromJsonAsync<GetUserResponse>();
            userResponse.Should().NotBeNull();
            userResponse!.Username.Should().Be("TestAdmin");
        }

        [Fact]
        public async Task CreateUser_ReturnsOk_ForAdmin()
        {
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var createUserDto = new CreateUserDto("newuser", "newpass12345", "Monitor", null);

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/Auth/CreateUser", createUserDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk_ForAdmin()
        {
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var updateUserDto = new UpdateUserDto("TestAdmin", "newadminpass", "Admin", null);

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1/Auth/UpdateUser", updateUserDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
