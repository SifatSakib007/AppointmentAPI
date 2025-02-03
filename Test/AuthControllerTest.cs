using AppointmentAPI.Controllers;
using AppointmentAPI.Models;
using AppointmentAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace Test;
[TestFixture]
public class AuthControllerTest
{
    private Mock<IAuthService> _authServiceMock;
    private AuthController _authController;

    [SetUp]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Test]
    public async Task Register_ShouldReturnBadRequest_WhenUserAlreadyExists()
    {
        // Arrange
        var request = new UserDto { Username = "testuser", Password = "Password123" };
        _authServiceMock
            .Setup(service => service.RegisterUser(request.Username, request.Password))
            .Returns("User already exists");

        // Act
        var result = await Task.Run(() => _authController.Register(request) as BadRequestObjectResult);

        // Assert
        Assert.NotNull(result);
        var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
        Assert.AreEqual("User already exists", responseObj.message.ToString());
    }

    [Test]
    public async Task Register_ShouldReturnOk_WhenUserIsRegistered()
    {
        // Arrange
        var request = new UserDto { Username = "newuser", Password = "Password123" };
        _authServiceMock
            .Setup(service => service.RegisterUser(request.Username, request.Password))
            .Returns("User registered successfully");

        // Act
        var result = await Task.Run(() => _authController.Register(request) as OkObjectResult);

        // Assert
        Assert.NotNull(result);
        var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
        Assert.AreEqual("User registered successfully", responseObj.message.ToString());
    }

    [Test]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var request = new UserDto { Username = "testuser", Password = "WrongPass123" };
        _authServiceMock
            .Setup(service => service.AuthenticateUser(request.Username, request.Password))
            .Returns<string>(null);

        // Act
        var result = await Task.Run(() => _authController.Login(request) as UnauthorizedObjectResult);

        // Assert
        Assert.NotNull(result);
        var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
        Assert.AreEqual("Invalid credentials", responseObj.message.ToString());
    }
}
