using WTelegram;
using System.Threading.Tasks;
using TL;
using System.Collections.Generic;
using System.Timers;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using TClientWPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;

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
        private long groupToWatch;
        private string log;
        private bool reloginOnFaildeResume;

        public event PropertyChangedEventHandler PropertyChanged;

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
            Helpers.Log = (lvl, str) => Log = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} [{"TDIWE!"[lvl]}] {str}\n";
            Initialize();
            SetTimer();
        }

        private void Initialize()
        {
            users = new Dictionary<long, User>();
            chats = new Dictionary<long, ChatBase>();
            favoritesMsgs = new List<string>();
            favorites = InputPeer.Self;
            groupToWatch = settings.ObservedChannel;

            pattern = new PatternMatching(settings.RegexPattern);
            client = new Client(Config);
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
                default:
                    return null;
            }
        }

        public async Task ConnectAndCheckMsg()
        {
            try
            {
                await client.LoginUserIfNeeded(null, reloginOnFaildeResume);
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
                //Если есть дубликат в избранном
                if (favoritesMsgs.Contains(currentMsg.message))
                    return;

                bool IsMatch = pattern.IsMatch(currentMsg.message);
                if (IsMatch)
                {
                    favoritesMsgs.Add(currentMsg.message);
                    Messages_Dialogs allDialogs = await client.Messages_GetAllDialogs();
                    ChatBase fromChat = allDialogs.chats[groupToWatch];
                    await client.Messages_ForwardMessages(fromChat, new[] { currentMsg.ID }, new[] { Helpers.RandomLong() }, favorites);
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
            client.Dispose();
            users.Clear();
            chats.Clear();
            favoritesMsgs.Clear();
            client = null;
            chats = null;
            users = null;
            favoritesMsgs = null;
        }
    }
}
