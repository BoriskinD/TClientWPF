using WTelegram;
using System.Threading.Tasks;
using TL;
using System.Collections.Generic;
using System.Timers;
using System.Net.Sockets;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.IO;

namespace TClientWPF.Model
{
    class TClient : INotifyPropertyChanged
    {
        private Timer timer;
        private Dictionary<long, User> users;
        private Dictionary<long, ChatBase> chats;
        private List<string> favoritesMsgs;
        private InputPeer favorites;
        private Client client;
        private PatternMatching pattern;
        private Settings settings;
        private FileStream sessionFileStream;
        private User user;
        private string sessionFilePath;
        private long groupToWatch;
        private string log;
        private bool reloginOnFaildeResume;
        private bool online;
        private int countOfForwardedMsg;
        public event PropertyChangedEventHandler PropertyChanged;

        public User User
        {
            get => user;
            set 
            {
                user = value;
                OnPropertyChanged();
            }
        }

        public int CountOfForwardedMsg
        {
            get => countOfForwardedMsg;
            set 
            {
                countOfForwardedMsg = value;
                OnPropertyChanged();
            }
        }

        public bool IsOnline
        {
            get => online;
            set
            {
                online = value;
                OnPropertyChanged();
            }
        }

        public string Log
        {
            get => log;
            set
            {
                log += value;
                OnPropertyChanged();
            }
        }

        public TClient(Settings settings)
        {
            this.settings = settings;
            reloginOnFaildeResume = true;
            IsOnline = false;
            sessionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"WTelegram.session");
            sessionFileStream = new FileStream(sessionFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Helpers.Log = (lvl, str) => Log = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} {str}\n";
            SetTimer();
        }

        public void Initialize()
        {
            users = new Dictionary<long, User>();
            chats = new Dictionary<long, ChatBase>();
            favoritesMsgs = new List<string>();
            favorites = InputPeer.Self;
            groupToWatch = settings.ObservedChannel;
            countOfForwardedMsg = 0;

            pattern = new PatternMatching(settings.RegexPattern);
            client = new Client(Config, sessionFileStream);
            client.OnUpdate += Client_OnUpdate;
            client.OnOther += Client_OnOther;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string Config(string what)
        {
            switch (what)
            {
                case "api_id":
                    return settings.Api_id;
                case "api_hash":
                    return settings.Api_hash;
                case "phone_number":
                    return settings.Phone_Number;
                case "verification_code":
                    return Interaction.InputBox("You need to enter Verification Code that has been sent via app");
                default:
                    return null;
            }
        }

        public async Task ConnectAndCheckMsg()
        {
            try
            {
                User = await client.LoginUserIfNeeded(null, reloginOnFaildeResume);
                IsOnline = true;
                await FillFavoritesList();
                await CheckOldMessages();
            }
            catch (SocketException)
            {
                Dispose();
                timer.Start();
            }
        }

        private async Task<object> Client_OnUpdate(IObject arg)
        {
            if (arg is not UpdatesBase updates)
                return null;

            updates.CollectUsersChats(users, chats);
            foreach (Update update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm when unm.message.Peer.ID == groupToWatch:
                        await ForwardMessage(unm.message);
                        break;
                }
            }
            return null;
        }

        private async Task<Task> Client_OnOther(IObject arg)
        {
            if (arg is ReactorError)
            {
                Dispose();
                timer.Start();
            }
            return Task.CompletedTask;
        }

        private void SetTimer()
        {
            timer = new Timer(3000);
            timer.Elapsed += OnTimer_Elapsed;
        }

        private async void OnTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Initialize();
            await ConnectAndCheckMsg();
        }

        private async Task ForwardMessage(MessageBase messageBase)
        {
            if (messageBase is Message currentMsg)
            {
                if (favoritesMsgs.Contains(currentMsg.message))
                    return;

                bool IsMatch = pattern.IsMatch(currentMsg.message);
                if (IsMatch)
                {
                    favoritesMsgs.Add(currentMsg.message);
                    Messages_Dialogs allDialogs = await client.Messages_GetAllDialogs();
                    ChatBase fromChat = allDialogs.chats[groupToWatch];
                    await client.Messages_ForwardMessages(fromChat, new[] { currentMsg.ID }, new[] { Helpers.RandomLong() }, favorites);
                    CountOfForwardedMsg++;
                }
            }
        }

        private async Task FillFavoritesList()
        {
            Messages_MessagesBase messageBaseList = await client.Messages_GetHistory(favorites);
            foreach (MessageBase favoriteMsg in messageBaseList.Messages)
                if (favoriteMsg is Message currentMsg)
                    favoritesMsgs.Add(currentMsg.message);
        }

        public async Task CheckOldMessages()
        {
            Messages_Dialogs allDialogs = await client.Messages_GetAllDialogs();
            InputPeer tdPeer = allDialogs.chats[groupToWatch];
            Messages_MessagesBase messages = await client.Messages_GetHistory(tdPeer);
            foreach (MessageBase tmp in messages.Messages)
                await ForwardMessage(tmp);
        }

        public void Dispose()
        {
            sessionFileStream?.Close();
            client?.Dispose();
            users?.Clear();
            chats?.Clear();
            favoritesMsgs?.Clear();
            client = null;
            chats = null;
            users = null;
            favoritesMsgs = null;
            IsOnline = false;
        }
    }
}
