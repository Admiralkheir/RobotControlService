using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using RobotControlService.Features.Robot;
using RobotControlService.Features.Robot.CreateRobot;
using RobotControlService.Features.Robot.GetRobotStatus;
using Xunit;
using Testcontainers.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RobotControlService.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using RobotControlService.Features.Auth;
using RobotControlService.Features.Auth.Login;
using Microsoft.AspNetCore.Mvc.Testing;
using RobotControlService.Domain.Entities;
using RobotControlService.Features.Robot.UpdateRobot;

namespace RobotControlService.Tests
{
    public class RobotControllerTests : IAsyncLifetime
    {
        private readonly MongoDbContainer _mongoDbContainer;
        private WebApplicationFactory<RobotControlService.Program> _factory;
        private HttpClient _client;

        public RobotControllerTests()
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
        public async Task CreateRobot_ReturnsOk_ForAdmin()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var robotDto = new CreateRobotDto("TestBot", "Temporary TestBot using for test cases. It will be deleted after test", new Position { X = 1, Y = 2, Orientation = 25 });

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/Robot/CreateRobot", robotDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRobotStatus_ReturnsOk_ForAdmin()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var robotDto = new CreateRobotDto("TestBot", "Temporary TestBot using for test cases. It will be deleted after test", new Position { X = 1, Y = 2, Orientation = 45 });
            await _client.PostAsJsonAsync("/api/v1/Robot/CreateRobot", robotDto);

            // Act
            var response = await _client.GetAsync($"/api/v1/Robot/GetRobotStatus?robotName=TestBot");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var status = await response.Content.ReadFromJsonAsync<GetRobotStatusResponse>();
            status.Should().NotBeNull();
            status!.RobotName.Should().Be("TestBot");
        }

        [Fact]
        public async Task DeleteRobot_ReturnsOk_ForAdmin()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var robotDto = new CreateRobotDto("TestBotToDelete", "Temporary TestBot using for test cases. It will be deleted after test", new Position { X = 1, Y = 1, Orientation = 45 });
            await _client.PostAsJsonAsync("/api/v1/Robot/CreateRobot", robotDto);

            // Act
            var response = await _client.DeleteAsync($"/api/v1/Robot/DeleteRobot?robotName=TestBotToDelete");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateRobot_ReturnsOk_ForAdmin()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var robotDto = new CreateRobotDto("TestBotToUpdate", "Temporary TestBot using for test cases. It will be deleted after test v1", new Position { X = 2, Y = 2, Orientation = 45 });
            await _client.PostAsJsonAsync("/api/v1/Robot/CreateRobot", robotDto);

            var updateRobotDto = new UpdateRobotDto("TestBotToUpdate", "Temporary TestBot using for test cases. It will be deleted after test v2");

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1/Robot/UpdateRobot", updateRobotDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
