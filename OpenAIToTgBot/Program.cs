using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Llm.Api;
using Llm.Api.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAIToTgBot.Settings;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

const string dataFileName = "messages.json";
const string initialPrompt = "Вы полезный, умный, добрый и эффективный помощник с искусственным интеллектом. Вы всегда выполняете запросы пользователя в меру своих возможностей. Вы всегда отвечаете только на русском языке, если не попросят прямо ответить на другом языке.";
const string initialPromptForCoding = "You are a highly skilled senior software engineer, specializing in .NET and C# language. Your code is always safe and perfect. You write the code according to the Clean Code rules and best practice. You may ask clarifying questions about the task if you need to. Never use placeholders, shortcuts, or skip code. Always output full, concise, and complete code.";

var cts = new CancellationTokenSource();

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var llmSettings = config.GetSection("llmSettings").Get<LlmSettings>();
if (null == llmSettings)
{
    throw new ApplicationException("Configure llmSettings section");
}

var telegramSettings = config.GetSection("telegramSettings").Get<TelegramSettings>();
if (null == telegramSettings)
{
    throw new ApplicationException("Configure telegramSettings section");
}

var llm = LlmFactory.Create(llmSettings.Provider, llmSettings.Config);

string usedModel;
if (!string.IsNullOrWhiteSpace(llmSettings.Model))
{
    usedModel = llmSettings.Model;
}
else
{
    var models = await llm.GetModelsAsync(cts.Token);
    usedModel =  models.First().Name;
}

Console.WriteLine($"Use model: {usedModel}");

ConcurrentDictionary<long, List<MessageApiDto>>? messages = null;
if (llmSettings.SaveHistory && File.Exists(dataFileName))
{
    var jsonLoad = File.ReadAllText(dataFileName);
    messages = JsonConvert.DeserializeObject<ConcurrentDictionary<long, List<MessageApiDto>>>(jsonLoad);
}

messages ??= new ConcurrentDictionary<long, List<MessageApiDto>>();

var botClient = new TelegramBotClient(telegramSettings.Token);

var receiverOptions = new ReceiverOptions()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

var me = await botClient.GetMeAsync();
var botNamePattern = $@"\b@{Regex.Escape(me.Username!)}\b";

var messagesQueue = new ConcurrentQueue<Message>();

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

RequestMessageToLlmAsync(cts.Token);

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

cts.Cancel();

if (llmSettings.SaveHistory)
{
    var jsonSave = JsonConvert.SerializeObject(messages, Formatting.Indented);
    File.WriteAllText(dataFileName, jsonSave);
}

Console.WriteLine("Finished.");

return;

async void RequestMessageToLlmAsync(CancellationToken cancellationToken)
{
    while (!cancellationToken.IsCancellationRequested)
    {
        if (!messagesQueue.TryDequeue(out var message))
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            continue;
        }

        var messageText = message.Text!;

        if (message.Entities != null && message.EntityValues != null && message.Entities.Any(x => x.Type == MessageEntityType.Mention) && message.EntityValues.Any(x => x == "@" + me.Username))
        {
            messageText = Regex.Replace(messageText, botNamePattern, string.Empty);
        }

        var chatId = message.Chat.Id;

        if (string.Equals(messageText, "/start", StringComparison.InvariantCultureIgnoreCase))
        {
            messages.TryRemove(chatId, out _);
            continue;
        }

        if (!messages.TryGetValue(chatId, out var ml))
        {
            ml = new List<MessageApiDto>
            {
                new("system", initialPrompt)
            };
            messages.TryAdd(chatId, ml);
        }

        ml.Add(new MessageApiDto("user", messageText));

        Console.WriteLine($"{DateTime.Now}, User {chatId} ({message.Chat.Username})> {TruncateLongString(messageText)}");

        var request = new RequestApiDto(Model: usedModel, Messages: ml);

        var response = await llm.PromptAsync(request, cancellationToken);
        foreach (var m in response.Messages)
        {
            ml.Add(m);
            Console.WriteLine($"{DateTime.Now}, AI for {chatId} ({message.Chat.Username})> {TruncateLongString(m.Content)}");

            await SendMessageAsync(chatId, m.Content, cancellationToken);
        }
    }
}

Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
    {
        return Task.CompletedTask;
    }

    // Only process text messages
    if (string.IsNullOrWhiteSpace(message.Text))
    {
        return Task.CompletedTask;
    }

    var needProcessAi = false;
    if (message.Chat.Type == ChatType.Private)
    {
        needProcessAi = true;
    }
    else if(message.Entities != null && message.EntityValues != null && message.Entities.Any(x => x.Type == MessageEntityType.Mention) && message.EntityValues.Any(x => x == "@" + me.Username))
    {
        needProcessAi = true;
    }
    else if (message.ReplyToMessage?.From != null && message.ReplyToMessage.From.Id == me.Id)
    {
        needProcessAi = true;
    }

    if (needProcessAi)
    {
        messagesQueue.Enqueue(message);
    }

    return Task.CompletedTask;
}

async Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken)
{
    while (message.Length > 0)
    {
        var text = message.Length >= 4000 ? message.Substring(0, 4000) : message;

        try
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                parseMode: ParseMode.Markdown,
                text: text,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{DateTime.Now}, Exception: {e.Message}");

            try
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: text,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e2)
            {
                Console.WriteLine($"{DateTime.Now}, Exception: {e2.Message}");
            }
        }

        message = message.Substring(text.Length);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}

string TruncateLongString(string text)
{
    if (text.Length > 50)
    {
        text = text.Substring(0, 25) + " ... " + text.Substring(text.Length - 25);
    }

    return text;
}