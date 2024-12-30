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

    // Check for keywords related to exercise (cardio, strength, flexibility, etc.)
    if (userMessage.Contains("exercise") || userMessage.Contains("workout") || userMessage.Contains("training") || userMessage.Contains("cardio") || userMessage.Contains("strength"))
    {
        advice = "Exercise is the cornerstone of fitness! To keep your body challenged, it’s essential to incorporate both cardiovascular exercises and strength training. Try activities like running, cycling, or swimming for cardio, and weightlifting, resistance bands, or bodyweight exercises for strength. A well-rounded routine is key for optimal health and performance.";
        advice += "\n\nTip: Aim for at least 30 minutes of moderate exercise most days of the week, and don't forget to warm up and cool down to prevent injury!";
    }
    // Check for keywords related to diet or nutrition (healthy eating, meal planning, etc.)
    else if (userMessage.Contains("diet") || userMessage.Contains("nutrition") || userMessage.Contains("meal") || userMessage.Contains("food"))
    {
        advice = "A healthy diet fuels your body and supports your fitness goals. Focus on eating a variety of nutrient-dense foods such as lean proteins (chicken, fish, tofu), whole grains (brown rice, quinoa), fruits and vegetables, and healthy fats (avocados, nuts, olive oil). Avoid processed foods and sugary drinks.";
        advice += "\n\nTip: Eating small, balanced meals every 3-4 hours can help maintain energy levels and curb cravings. And don’t forget to stay hydrated throughout the day!";
    }
    // Check for keywords related to rest, recovery, and sleep
    else if (userMessage.Contains("rest") || userMessage.Contains("recovery") || userMessage.Contains("sleep") || userMessage.Contains("muscle recovery"))
    {
        advice = "Rest is just as important as exercise when it comes to achieving your fitness goals. Your muscles need time to repair and grow stronger, so make sure you’re getting enough sleep—around 7-9 hours per night. If you’re feeling sore, try active recovery like walking, stretching, or yoga to promote circulation.";
        advice += "\n\nTip: Incorporate rest days into your routine and listen to your body. Overtraining can lead to injury and burnout, so give yourself time to recover.";
    }
    // Check for keywords related to motivation, consistency, and setting fitness goals
    else if (userMessage.Contains("motivation") || userMessage.Contains("consistent") || userMessage.Contains("goals") || userMessage.Contains("inspiration"))
    {
        advice = "Motivation can ebb and flow, but consistency is the key to long-term success. Set realistic, achievable goals and break them down into smaller, manageable steps. Celebrate each victory along the way, whether it's adding extra reps, hitting a new personal best, or simply staying on track with your routine.";
        advice += "\n\nTip: Find a workout buddy or join a fitness community for extra accountability. Having a support system can help keep you motivated on tough days!";
    }
    // Check for keywords related to specific fitness goals (weight loss, muscle gain, endurance, etc.)
    else if (userMessage.Contains("weight loss") || userMessage.Contains("muscle gain") || userMessage.Contains("endurance"))
    {
        if (userMessage.Contains("weight loss"))
        {
            advice = "For weight loss, creating a calorie deficit is crucial. Focus on a combination of regular exercise, including both cardio and strength training, and mindful eating. Track your food intake to ensure you're not eating more than your body needs, but avoid extreme calorie restriction.";
            advice += "\n\nTip: Aim for 1-2 pounds of weight loss per week for sustainable results. Incorporate more whole foods and try meal prepping to stay on track!";
        }
        else if (userMessage.Contains("muscle gain"))
        {
            advice = "To build muscle, focus on progressive strength training, lifting heavier weights over time. Your muscles grow when they are challenged, so aim for 3-5 strength training sessions per week, targeting different muscle groups. Don’t forget the importance of recovery, including proper nutrition with a focus on protein.";
            advice += "\n\nTip: Aim to consume 1.6–2.2 grams of protein per kilogram of body weight to support muscle growth!";
        }
        else if (userMessage.Contains("endurance"))
        {
            advice = "Building endurance requires a mix of cardiovascular exercise and strength training. Gradually increase the intensity and duration of your workouts. Running, cycling, and swimming are excellent ways to build cardiovascular endurance. Strength training also plays a key role in supporting your joints and improving overall stamina.";
            advice += "\n\nTip: Add interval training to your cardio workouts for a great endurance boost. Short bursts of intense effort followed by rest can improve your stamina over time!";
        }
    }
    // Check for keywords related to supplements or pre-workout
    else if (userMessage.Contains("supplements") || userMessage.Contains("pre-workout") || userMessage.Contains("protein powder"))
    {
        advice = "Supplements can help fill nutritional gaps, but they should not replace a balanced diet. If you're looking to build muscle, a protein supplement can be helpful to meet your daily protein needs. Pre-workout supplements can enhance your energy and focus during training, but they should be used in moderation.";
        advice += "\n\nTip: Always consult a healthcare professional before adding new supplements to your routine, especially if you have any underlying health conditions.";
    }
    // Check for keywords related to specific types of exercise (yoga, pilates, HIIT, etc.)
    else if (userMessage.Contains("yoga") || userMessage.Contains("pilates") || userMessage.Contains("HIIT") || userMessage.Contains("stretching"))
    {
        if (userMessage.Contains("yoga") || userMessage.Contains("pilates"))
        {
            advice = "Yoga and Pilates are fantastic for building core strength, flexibility, and mental focus. These practices can also reduce stress and improve balance. Incorporate them into your routine 2-3 times a week to improve your overall mobility and recovery.";
            advice += "\n\nTip: Start with beginner classes if you're new to these practices, and gradually increase the difficulty as you get more comfortable.";
        }
        else if (userMessage.Contains("HIIT"))
        {
            advice = "HIIT (High-Intensity Interval Training) is a time-efficient way to burn fat and improve cardiovascular health. It involves short bursts of intense exercise followed by brief rest periods. HIIT can be done with bodyweight exercises or weights, and it’s great for building strength and endurance.";
            advice += "\n\nTip: HIIT workouts can be intense, so make sure to warm up beforehand and cool down afterward to prevent injury!";
        }
        else if (userMessage.Contains("stretching"))
        {
            advice = "Stretching is crucial for improving flexibility and preventing injuries. Aim to stretch after each workout to help reduce muscle tightness and improve your range of motion. Static stretching (holding a stretch) is effective post-workout, while dynamic stretching (controlled leg swings, arm circles) is ideal before exercise.";
            advice += "\n\nTip: Don’t rush through your stretches—hold each one for at least 20-30 seconds for the best results!";
        }
    }
    // General advice when no specific category is mentioned
    else
    {
        advice = "Fitness is a holistic journey, combining exercise, nutrition, rest, and mental focus. Stay consistent, and don’t be afraid to challenge yourself while respecting your body’s limits. Fitness is about feeling your best both inside and out!";
        advice += "\n\nTip: Find activities you enjoy so that staying fit becomes a lifelong habit. Whether it's dancing, hiking, or weightlifting, the key is to stay active and have fun!";
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
