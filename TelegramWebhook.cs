using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Attendance.Services;
using Attendance.Models;
using System.Text.Json;

namespace Attendance;

public class TelegramWebhook(ILogger<TelegramWebhook> logger, 
                            TelegramBotClient botClient)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly ILogger<TelegramWebhook> _logger = logger;

    [Function("TelegramWebhook")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var attendanceService = new AttendanceService();
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var json = await new StreamReader(req.Body).ReadToEndAsync();
        var update = System.Text.Json.JsonSerializer.Deserialize<Update>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (update?.Message?.Type == MessageType.Text && update.Message.Text?.ToLower() == "/start")
        {
            _logger.LogInformation("/start is received from " + update.Message.Chat.FirstName, update.Message.Chat.LastName, update.Message.Chat.Username, update.Message.Chat.Id);

            var replyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton("🚩 I am here")
                {
                    RequestLocation = true
                }
            })
            {
                ResizeKeyboard = true
            };

            await _botClient.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "Welcome! Please share your location to mark your attendance.",
                replyMarkup: replyKeyboard
            );

            _logger.LogInformation("keyboard is sent to " + update.Message.Chat.FirstName, update.Message.Chat.LastName, update.Message.Chat.Username, update.Message.Chat.Id);
        }
        else if (update?.Message?.Type == MessageType.Location)
        {
            _logger.LogInformation("location is received from " + update.Message.Chat.FirstName, update.Message.Chat.LastName, update.Message.Chat.Username, update.Message.Chat.Id);

            var student = new Student
            {
                ChatId = update.Message.Chat.Id,
                FullName = update.Message.Chat.FirstName + " " + update.Message.Chat.LastName,
                UserName = update.Message.Chat.Username,
                LastLatitude = update.Message.Location.Latitude,
                LastLongitude = update.Message.Location.Longitude
            };

            string result = attendanceService.MarkAttendance(student);
            await _botClient.SendMessage(update.Message.Chat.Id, result);

            _logger.LogInformation($"Attendance result '{result}' is sent to {update.Message.Chat.FirstName}, {update.Message.Chat.Username}, {update.Message.Chat.Id}");
        }

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}