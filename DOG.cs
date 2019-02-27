﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using DOG.Gen;
using Microsoft.Extensions.DependencyInjection;

namespace DOG
{
    public class DOG
    {
        //https://discordapp.com/oauth2/authorize?client_id=550055710038425610&permissions=378944&scope=bot

        public static DOG Instance => _instance ?? (_instance = new DOG());
        private static DOG _instance;

        private DOG()
        {
            _token = File.ReadAllText("token.txt");




            var ng = new NameGen();


            for (int i = 0; i < 50; i++)
            {
                var rnd = new Random(Guid.NewGuid().GetHashCode());
                var d = new Dogs
                {
                    AtkPower = rnd.Next(0,20),
                    Defense = rnd.Next(0, 20),
                    Health = rnd.Next(0, 20),
                    Will = rnd.Next(0, 20),
                    Intelligence = rnd.Next(0, 20)
                };

                Console.WriteLine(ng.GenerateDogName(d));
            }
        }

        internal DiscordSocketClient Client;
        internal DOGContext Context = new DOGContext();
        internal CommandService CommandService;

        private IServiceProvider _services;
        private readonly string _token;

        public async Task StartAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Debug
            });

            CommandService = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(CommandService)
                .BuildServiceProvider();

            await InstallCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, _token);
            await Client.StartAsync();

            Client.JoinedGuild += JoinedGuild;

            Client.Ready += () =>
            {
                Utility.SetPlaying("Woof!", Client);
                Utility.Log(new LogMessage(LogSeverity.Info, "Squid", $"Logged in as {Client.CurrentUser.Username}#{Client.CurrentUser.Discriminator}." +
                                                              $"\nServing {Client.Guilds.Count} guilds with a total of {Client.Guilds.Sum(guild => guild.Users.Count)} online users."));
                return Task.CompletedTask;
            };

            await Task.Delay(-1);
        }

        private Task JoinedGuild(SocketGuild guild)
        {
            Utility.Log(new LogMessage(LogSeverity.Info, "DOG",
                $"Joined new guild {guild.Name} with {guild.Users.Count}"));

            return Task.CompletedTask;
        }

        public async Task InstallCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message)) return;

            var argPos = 0;
            if (!(message.HasCharPrefix('*', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))) return;
            var context = new SocketCommandContext(Client, message);

            var result = await CommandService.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}