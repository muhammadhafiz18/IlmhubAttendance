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

    [Function("TelegramWebhook")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var attendanceService = new AttendanceService();
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var json = await new StreamReader(req.Body).ReadToEndAsync();

        Console.WriteLine(json);

        var update = System.Text.Json.JsonSerializer.Deserialize<Models.Update>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // fullname
        });
        
        // Debug logging to see the structure
        logger.LogInformation("Full update object: {update}", JsonSerializer.Serialize(update));
        logger.LogInformation("Message object: {message}", JsonSerializer.Serialize(update.Message));
        logger.LogInformation("ForwardOrigin object: {forwardOrigin}", JsonSerializer.Serialize(update.Message.ForwardOrigin));
        
        logger.LogInformation("{update.Message.ForwardDate}", update.Message.ForwardDate);
        logger.LogInformation("{update.Message.ForwardDate}", update.Message.ForwardOrigin?.SenderUser.Username);

        if (update.Message.Text?.ToLower() == "/start")
        {
            logger.LogInformation("/start is received from " + update.Message.Chat.FirstName, update.Message.Chat.LastName, update.Message.Chat.UserName, update.Message.Chat.Id);

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

           logger.LogDebug("Keyboard with 'I am here button' is created");

            try
            {
                await botClient.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "Welcome! Please share your location to mark your attendance.",
                replyMarkup: replyKeyboard
                );

                logger.LogDebug("message is sent to {update.Message.Chat.Id}", update.Message.Chat.Id);

                logger.LogInformation("keyboard is sent to " + update.Message.Chat.FirstName, update.Message.Chat.LastName, update.Message.Chat.UserName, update.Message.Chat.Id);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Keyboard was not sent {ex.Message}", ex.Message);
            }
        }

        else if(update.Message.ForwardDate != 0)
        {
            var forwarderUsername = update.Message.ForwardOrigin?.SenderUser?.Username ?? "unknown user";
            logger.LogInformation("Received location that was forwarded from {forwarderUsername}", forwarderUsername);
        
            await botClient.SendMessage(update.Message.Chat.Id, "please use button for sending location!");
        }        

        else if (update?.Message.Location != null)
        {
            logger.LogInformation("location is received from " + update.Message.Chat.FirstName, update.Message.Chat.LastName, update.Message.Chat.UserName, update.Message.Chat.Id);

            var student = new Student
            {
                ChatId = update.Message.Chat.Id,
                FullName = update.Message.Chat.FirstName + " " + update.Message.Chat.LastName,
                UserName = update.Message.Chat.UserName,
                LastLatitude = update.Message.Location.Latitude,
                LastLongitude = update.Message.Location.Longitude
            };

            string result = attendanceService.MarkAttendance(student);
            await botClient.SendMessage(update.Message.Chat.Id, result);

            logger.LogInformation($"Attendance result '{result}' is sent to {update.Message.Chat.FirstName}, {update.Message.Chat.UserName}, {update.Message.Chat.Id}");
        }

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}