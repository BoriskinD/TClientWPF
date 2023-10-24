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

namespace TClientWPF.Model
{
    class TClient : INotifyPropertyChanged
    {
        private Timer timer;
        private Dictionary<long, User> users;
        private Dictionary<long, ChatBase> chats;
        private Dictionary<long, ChatBase> myChats;
        private List<string> favoritesMsgs;
        private InputPeer favorites;
        private Client client;
        private Settings currentSettings;
        private FileStream sessionFileStream;
        private PatternMatching patternMatching;
        private User user;
        private string sessionFilePath;
        private long channelID;
        private string log;
        private bool online;
        private int countOfGeneralFWDMessages;  
        private int checkHistoryFWDMessages;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ConnectionStatusChanged;

        public long ChannelID
        {
            get => channelID;
            set => channelID = value;
        }

        public Dictionary<long, ChatBase> ChatsList
        {
            get => myChats;
            set
            {
                myChats = value;
                OnPropertyChanged();
            }
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

        public TClient(Settings settings, PatternMatching patternMatching)
        {
            this.patternMatching = patternMatching;
            currentSettings = settings;

            sessionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"WTelegram.session");
            Helpers.Log = (lvl, str) => Log = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} {str}\n";

            //SetTimer();
        }

        public void Initialize()
        {
            users = new Dictionary<long, User>();
            chats = new Dictionary<long, ChatBase>();
            myChats = new Dictionary<long, ChatBase>();
            favoritesMsgs = new List<string>();
            favorites = InputPeer.Self;
            countOfGeneralFWDMessages = 0;
            channelID = 0;

            sessionFileStream = new FileStream(sessionFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            client = new Client(Config, sessionFileStream);
            client.OnUpdate += Client_OnUpdate;
            client.OnOther += Client_OnOther;
            IsOnline = !client.Disconnected;
        }

        private string Config(string what)
        {
            switch (what)
            {
                case "api_id":
                    return currentSettings.Api_id;
                case "api_hash":
                    return currentSettings.Api_hash;
                case "phone_number":
                    return currentSettings.Phone_Number;
                case "verification_code":
                    return Interaction.InputBox("Введите проверочный код который был выслан вам в Telegram");
                default:
                    return null;
            }
        }

        public async Task LoginAndStartWorking()
        {
            User = await client.LoginUserIfNeeded();
            IsOnline = !client.Disconnected;
            await GetUserChats();

            //try
            //{
            //}
            //catch (ArgumentException)
            //{
            //    Dispose();
            //    timer.Start();
            //    return;
            //}
        }

        public async Task CheckHistory()
        {
            await FillFavoritesList();
            await CheckOldMessages();
        }

        private async Task GetUserChats()
        {
            Messages_Chats messagesChats = await client.Messages_GetAllChats();
            ChatsList = messagesChats.chats;
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
                    case UpdateNewMessage unm when unm.message.Peer.ID == channelID:
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
                ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
            }
            return Task.CompletedTask;
        }

        //private void SetTimer()
        //{
        //    timer = new Timer(3000);
        //    timer.Elapsed += OnTimer_Elapsed;
        //}

        //private async void OnTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    timer.Stop();
        //    Initialize();
        //    await LoginAndStartWorking();
        //}

        private async Task ForwardMessage(MessageBase messageBase, [CallerMemberName] string memberName = "")
        {
            if (messageBase is Message currentMsg)
            {
                if (favoritesMsgs.Contains(currentMsg.message) || string.IsNullOrEmpty(patternMatching.Expression))
                    return;

                bool IsMatch = patternMatching.IsMatch(currentMsg.message);
                if (IsMatch)
                {
                    favoritesMsgs.Add(currentMsg.message);
                    Messages_Dialogs allDialogs = await client.Messages_GetAllDialogs();
                    ChatBase fromChat = allDialogs.chats[channelID];
                    await client.Messages_ForwardMessages(fromChat, new[] { currentMsg.ID }, new[] { Helpers.RandomLong() }, favorites);

                    if (memberName.Equals("CheckOldMessages"))
                    {
                        CountOfHistoryFWDMessages++;
                        CountOfGeneralFWDMessages++;
                    }
                    else CountOfGeneralFWDMessages++;
                }
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
            sessionFileStream?.Close();
            client?.Dispose();
            IsOnline = !client.Disconnected;
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
