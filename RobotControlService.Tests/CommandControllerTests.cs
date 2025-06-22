using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Testcontainers.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RobotControlService.Data;
using System.Collections.Generic;
using System;
using RobotControlService.Features.Auth;
using RobotControlService.Features.Auth.Login;
using RobotControlService.Features.Command.GetCommand;
using RobotControlService.Features.Command.GetCommandHistory;
using RobotControlService.Features.Command.SendCommand;
using RobotControlService.Features.Command.UpdateCommandStatus;
using RobotControlService.Domain.Entities;
using RobotControlService.Features.Auth.CreateUser;
using Microsoft.Extensions.Primitives;
using RobotControlService.Features.Robot.CreateRobot;

namespace RobotControlService.Tests
{
    public class CommandControllerTests : IAsyncLifetime
    {
        private readonly MongoDbContainer _mongoDbContainer;
        private WebApplicationFactory<RobotControlService.Program> _factory;
        private HttpClient _client;

        public CommandControllerTests()
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

        private async Task<string> CreateUserAndGetRobotToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var robotDto = new RobotControlService.Features.Robot.CreateRobot.CreateRobotDto("TestBot", "Temporary TestBot using for test cases. It will be deleted after test", new Position { X = 1, Y = 2, Orientation = 45 });
            var response = await _client.PostAsJsonAsync("/api/v1/Robot/CreateRobot", robotDto);
            var createRobotResponse = await response.Content.ReadFromJsonAsync<CreateRobotResponse>();

            var createUserDto = new CreateUserDto("TestRobot", "robot123456", "Robot", new List<string>() { createRobotResponse.RobotId });
            var createUserResponse = await _client.PostAsJsonAsync("/api/v1/Auth/CreateUser", createUserDto);
            createUserResponse.EnsureSuccessStatusCode();

            // Login as the robot user to get the token
            var loginDto = new LoginDto("TestRobot", "robot123456");
            var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/Login", loginDto);
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            return loginResult!.Token;
        }

        private async Task<string> SendCommandAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var sendCommandDto = new SendCommandDto("TestAdmin", "TestBot", "Move", new Dictionary<string, string> { { "direction", "forward" }, { "distance", "1" } });
            var sendResponse = await _client.PostAsJsonAsync("/api/v1/Command/SendCommand", sendCommandDto);
            sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var sendResult = await sendResponse.Content.ReadAsStringAsync();
            // Try to extract CommandId from response JSON
            var commandId = System.Text.Json.JsonDocument.Parse(sendResult).RootElement.GetProperty("commandId").GetString();
            return commandId!;
        }

        [Fact]
        public async Task GetCommandHistory_ReturnsOk_ForAdmin()
        {
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var robotDto = new RobotControlService.Features.Robot.CreateRobot.CreateRobotDto("TestBot", "Temporary TestBot using for test cases. It will be deleted after test", new Position { X = 1, Y = 2, Orientation = 45 });

            await _client.PostAsJsonAsync("/api/v1/Robot/CreateRobot", robotDto);

            var sendCommandDto = new SendCommandDto("TestAdmin", "TestBot", "Move", new Dictionary<string, string> { { "direction", "forward" }, { "distance", "1" } });
            var sendCommandDto2 = new SendCommandDto("TestAdmin", "TestBot", "Move", new Dictionary<string, string> { { "direction", "forward" }, { "distance", "1" } });
            var sendCommandDto3 = new SendCommandDto("TestAdmin", "TestBot", "Move", new Dictionary<string, string> { { "direction", "forward" }, { "distance", "1" } });

            await _client.PostAsJsonAsync("/api/v1/Command/SendCommand", sendCommandDto);
            await _client.PostAsJsonAsync("/api/v1/Command/SendCommand", sendCommandDto2);
            await _client.PostAsJsonAsync("/api/v1/Command/SendCommand", sendCommandDto3);

            var response = await _client.GetAsync($"/api/v1/Command/GetCommandHistory?robotName=TestBot&pageIndex=1&pageSize=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task SendCommand_ReturnsOk_ForAdmin()
        {
            var token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var robotDto = new RobotControlService.Features.Robot.CreateRobot.CreateRobotDto("TestBot", "Temporary TestBot using for test cases. It will be deleted after test", new Position { X = 1, Y = 2, Orientation = 45 });
            await _client.PostAsJsonAsync("/api/v1/Robot/CreateRobot", robotDto);

            var sendCommandDto = new SendCommandDto("TestAdmin", "TestBot", "Move", new Dictionary<string, string> { { "direction", "forward" }, { "distance", "1" } });
            var response = await _client.PostAsJsonAsync("/api/v1/Command/SendCommand", sendCommandDto);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateCommandStatus_ReturnsOk_ForRobotRole()
        {
            var token = await GetAdminTokenAsync();
            var robotToken = await CreateUserAndGetRobotToken(token);
            var commandId = await SendCommandAsync(token);

            // Simulate robot role by using the same token (in real scenario, use robot JWT)
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", robotToken);
            var updateDto = new UpdateCommandStatusDto(commandId, "InProgress", null);
            var response = await _client.PutAsJsonAsync("/api/v1/Command/UpdateCommandStatus", updateDto);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updateResponse = await response.Content.ReadFromJsonAsync<UpdateCommandStatusResponse>();
            updateResponse.Should().NotBeNull();
            updateResponse!.RobotStatus.Should().BeOneOf("Moving", "Rotating");
        }
    }
}
