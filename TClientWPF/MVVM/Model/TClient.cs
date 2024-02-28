using WTelegram;
using System.Threading.Tasks;
using TL;
using System.Collections.Generic;
using System.Timers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.IO;
using System.Text;
using TClientWPF.Services;

namespace TClientWPF.Model
{
    class TClient : INotifyPropertyChanged
    {
        private Logger logger;
        private StreamWriter wTelegramLogs;
        private Timer reconnectionTimer;
        private Dictionary<long, User> users;
        private Dictionary<long, ChatBase> chats;
        private Dictionary<long, ChatBase> chatsList;
        private List<string> favoritesMsgs;
        private InputPeer favorites;
        private Client client;
        private Settings settings;
        private FileStream sessionFileStream;
        private PatternMatching patternMatching;
        private User user;
        private string sessionFilePath;
        private long channelID;
        private string log;
        private bool online;
        private bool autoreconnect;
        private int countOfGeneralFWDMessages;
        private int checkHistoryFWDMessages;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ConnectionDropped, ConnectionRestored;

        public Settings Settings
        {
            set => settings = value;
        }

        public PatternMatching PatternMatching
        {
            set => patternMatching = value;
        }

        public long ChannelID
        {
            get => channelID;
            set => channelID = value;
        }

        public Dictionary<long, ChatBase> ChatsList
        {
            get => chatsList;
            set
            {
                chatsList = value;
                OnPropertyChanged();
            }
        }

        public bool Autoreconnect
        {
            get => autoreconnect;
            set => autoreconnect = value;
        }

        public User User
        {
            get => user;
            private set 
            {
                user = value;
                OnPropertyChanged();
            }
        }

        public int CountOfHistoryFWDMessages
        {
            get => checkHistoryFWDMessages;
            set => checkHistoryFWDMessages = value;
        }

        public int CountOfGeneralFWDMessages
        {
            get => countOfGeneralFWDMessages;
            private set 
            {
                countOfGeneralFWDMessages = value;
                OnPropertyChanged();
            }
        }

        public bool IsOnline
        {
            get => online;
            private set
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

        public TClient()
        {
            logger = Logger.GetInstance();
            sessionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"WTelegram.session");
            Helpers.Log = (lvl, str) => wTelegramLogs?.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{"TDIWE!"[lvl]}] {str}");
            SetReconnectionTimer();
        }

        public void Initialize()
        {
            users = new Dictionary<long, User>();
            chats = new Dictionary<long, ChatBase>();
            chatsList = new Dictionary<long, ChatBase>();
            favoritesMsgs = new List<string>();
            favorites = InputPeer.Self;
            countOfGeneralFWDMessages = 0;
            channelID = 0;

            wTelegramLogs = new ("TClient.log", true, Encoding.UTF8) { AutoFlush = true };
            sessionFileStream = new FileStream(sessionFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            client = new Client(Config, sessionFileStream);
            client.OnUpdate += Client_OnUpdate;
            client.OnOther += Client_OnOther;
            IsOnline = client.Disconnected;
        }

        public async Task Connect()
        {
            User = await client.LoginUserIfNeeded();
            IsOnline = !client.Disconnected;
        }

        public async Task GetUserChats()
        {
            Messages_Chats messagesChats = await client.Messages_GetAllChats();
            ChatsList = messagesChats.chats;
        }

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
                    return Interaction.InputBox("Введите проверочный код который был выслан вам в Telegram");
                default:
                    return null;
            }
        }

        public async Task CheckHistory()
        {
            await FillFavoritesList();
            await CheckOldMessages();
        }

        private async Task Client_OnUpdate(UpdatesBase updates)
        {
            updates.CollectUsersChats(users, chats);
            foreach (Update update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm when unm.message.Peer.ID == channelID:
                        await ForwardMessage(unm.message);
                        break;
                }
            }
        }

        private Task Client_OnOther(IObject arg)
        {
            if (arg is ReactorError)
            {
                IsOnline = !client.Disconnected;
                Dispose();
                ConnectionDropped?.Invoke(this, EventArgs.Empty);
                if (Autoreconnect) reconnectionTimer.Start();
            }
            return null;
        }

        private void OnTimer_Elapsed(object sender, ElapsedEventArgs e) => Recconect();

        private async void Recconect()
        {
            reconnectionTimer.Stop();
            Initialize();
            logger.AddText($"WARNING: Пытаемся переподключиться...");
            try
            {
                await Connect();
                await GetUserChats();
                ConnectionRestored?.Invoke(this, EventArgs.Empty);
                logger.AddText($"INFO: Переподключение выполнено успешно.");
            }
            catch (ArgumentException aEx)
            {
                logger.AddText($"ERROR: Во время переподключения возникла ошибка - {aEx.Message}");
                Dispose();
                if (Autoreconnect) 
                    reconnectionTimer.Start();
            }
        }

        private void SetReconnectionTimer()
        {
            reconnectionTimer = new Timer(3000);
            reconnectionTimer.Elapsed += OnTimer_Elapsed;
        }

        private async Task ForwardMessage(MessageBase messageBase, [CallerMemberName] string memberName = "")
        {
            if (messageBase is not Message currentMsg)
                return;

            if (favoritesMsgs.Contains(currentMsg.message) || string.IsNullOrEmpty(patternMatching.Expression))
                return;

            bool IsMatch = patternMatching.IsMatch(currentMsg.message);
            if (IsMatch)
            {
                Messages_Dialogs allDialogs = await client.Messages_GetAllDialogs();
                ChatBase fromChat = allDialogs.chats[channelID];
                await client.Messages_ForwardMessages(fromChat, new[] { currentMsg.ID }, new[] { Helpers.RandomLong() }, favorites);
                favoritesMsgs.Add(currentMsg.message);

                if (memberName.Equals("CheckOldMessages"))
                {
                    CountOfHistoryFWDMessages++;
                    CountOfGeneralFWDMessages++;
                }
                else CountOfGeneralFWDMessages++;
            }
        }

        private async Task FillFavoritesList()
        {
            favoritesMsgs.Clear();
            Messages_MessagesBase messageBaseList = await client.Messages_GetHistory(favorites);
            foreach (MessageBase favoriteMsg in messageBaseList.Messages)
                if (favoriteMsg is Message currentMsg)
                    favoritesMsgs.Add(currentMsg.message);
        }

        public async Task CheckOldMessages()
        {
            Messages_Dialogs allDialogs = await client.Messages_GetAllDialogs();
            InputPeer tdPeer = allDialogs.chats[channelID];
            Messages_MessagesBase messages = await client.Messages_GetHistory(tdPeer);
            foreach (MessageBase tmp in messages.Messages)
                await ForwardMessage(tmp);
        }

        public void Dispose()
        {
            wTelegramLogs?.Close();
            wTelegramLogs = null;
            sessionFileStream?.Close();
            client?.Dispose();
            users?.Clear();
            chats?.Clear();
            favoritesMsgs?.Clear();
            client = null;
            chats = null;
            sessionFileStream = null;
            users = null;
            favoritesMsgs = null;
            CountOfGeneralFWDMessages = 0;
            IsOnline = false;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
