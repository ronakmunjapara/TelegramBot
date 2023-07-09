using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("Secratekey-Demo:APIToken");

            //var me = await botClient.GetMeAsync();
            //Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                // Generate fitness advice based on user message
                var advice = GetFitnessAdvice(messageText);

                // Send fitness advice as a text message
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: advice);
                // Echo received message text
                //Message sentMessage = await botClient.SendTextMessageAsync(
                //    chatId: chatId,
                //    text: "You said:\n" + messageText,
                //    cancellationToken: cancellationToken);
                //            Message sentMessage = await botClient.SendStickerAsync(
                //chatId: chatId,
                //sticker: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/sticker-dali.webp"),
                //cancellationToken: cancellationToken);

            }

            static string GetFitnessAdvice(string userMessage)
            {
                // Process the user message and generate fitness advice based on specific keywords or patterns
                string advice = string.Empty;

                if (userMessage.Contains("exercise"))
                {
                    advice = "Regular exercise is important for maintaining a healthy lifestyle. Try to incorporate a combination of cardiovascular exercises, strength training, and flexibility exercises in your routine.";
                }
                else if (userMessage.Contains("diet") || userMessage.Contains("nutrition"))
                {
                    advice = "Eating a balanced diet is crucial for achieving fitness goals. Focus on consuming a variety of fruits, vegetables, lean proteins, whole grains, and healthy fats.";
                }
                else
                {
                    advice = "Remember to listen to your body, stay consistent with your fitness routine, and make healthy choices for overall well-being.";
                }

                return advice;
            }
            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
        }
    }
}
