using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonChatTypes;

namespace ServerTcpChat.Classes
{
    public delegate void UserSendChatMessage(string p_receiver_user_name, ChatMessageObjectToClient p_chat_message);
    public delegate void ChatSend(string p_receiver_user_name, ChatMessageForClient p_chat_message);
    public delegate void RemoveChat(int p_chat_id);

    public delegate void StartEjectedChatUserInform(string p_ejected_user, int p_chat_id, string p_ejecting_comment, TypeOfChat p_chat_type);
    public delegate void StartClientSomeoneJoinedChatInform(string p_user_to_inform_name, string p_user_joined_name, int p_chat_id, TypeOfChat p_chat_type);
    public delegate void StartClientSomeoneLeftChatInform(string p_user_to_inform_name, string p_user_left_name, int p_chat_id, TypeOfChat p_chat_type);

    public enum PrivateChatStatus
    {
        UnAccepted,
        Accepted,
    }


    public abstract class BaseChat
    {
        protected int chat_id;
        protected TypeOfChat chat_type;
        protected List<ChatUser> all_users;
        protected int max_users_count;
        protected int min_user_count;


        protected ChatSend chat_send;
        protected RemoveChat remove_chat_from_manager;
        protected StartEjectedChatUserInform start_ejected_user_chat_inform_dialog;
        protected IsLoggedIn is_logged_in;

        public int Get_chat_id
        {
            get { return chat_id; }
        }
        public TypeOfChat Get_chat_type
        {
            get { return chat_type; }
        }
        public List<ChatUser> Get_all_users
        {
            get { return all_users; }
        }

        protected void BaseConstruct(TypeOfChat p_chat_type, int p_chat_id, ChatSend p_chat_send, RemoveChat p_remove_chat_from_manager
            , StartEjectedChatUserInform p_start_ejected_user_chat_inform_dialog, IsLoggedIn p_is_logged_in)
        {
            chat_type = p_chat_type;
            chat_id = p_chat_id;
            chat_send = p_chat_send;
            remove_chat_from_manager = p_remove_chat_from_manager;
            start_ejected_user_chat_inform_dialog = p_start_ejected_user_chat_inform_dialog;
            is_logged_in = p_is_logged_in;
        }

        public virtual void ReceiveMessageFromUser(AuthServerChatMessage p_chat_message)
        {
            ChatMessageObjectToClient chat_message_for_clients = new ChatMessageObjectToClient(p_chat_message.Get_user_name, p_chat_message.Get_chat_message.Get_text_message_object);
            foreach (ChatUser t_user in all_users)
            {
                if (t_user.Get_user_name == p_chat_message.Get_user_name)
                {
                    foreach (ChatUser r_user in all_users)
                    {
                        if (is_logged_in(r_user.Get_user_name))
                        {
                            r_user.Receive(chat_message_for_clients);
                        }
                    }
                }
            }
        }

        public abstract void LeaveChat(string p_user_left_chat_name);

        public void CH_ChatSend(string p_receiver_user_name, ChatMessageObjectToClient p_chat_message)
        {
            ChatMessageForClient chat_message = new ChatMessageForClient(HelperFunctions.GetGUID(), chat_id, chat_type, p_chat_message);
            chat_send(p_receiver_user_name, chat_message);
        }

        protected void RemoveChatFromManager()
        {
            remove_chat_from_manager(chat_id);
        }

        public bool HasUser(string p_user_name)
        {
            foreach (ChatUser c_user in all_users)
            {
                if (c_user.Get_user_name == p_user_name)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class PrivateChat : BaseChat
    {
        protected PrivateChatStatus status;
        string invited_user_name;

        StartClientSomeoneJoinedChatInform start_client_someone_joined_chat_inform;

        public PrivateChat(string p_first_person_user_name, string p_second_person_user_name, int p_chat_id, ChatSend p_send_chat_message_to_user, StartEjectedChatUserInform p_start_ejected_user_chat_inform_dialog
            , RemoveChat p_remove_chat_from_manager, IsLoggedIn p_is_logged_in, StartClientSomeoneJoinedChatInform p_start_client_someone_joined_chat_inform)
        {
            base.BaseConstruct(TypeOfChat.Private, p_chat_id, p_send_chat_message_to_user, p_remove_chat_from_manager, p_start_ejected_user_chat_inform_dialog, p_is_logged_in);
            all_users = new List<ChatUser>();
            max_users_count = 2;
            min_user_count = 1;
            status = PrivateChatStatus.UnAccepted;
            start_client_someone_joined_chat_inform = p_start_client_someone_joined_chat_inform;
            invited_user_name = p_second_person_user_name;
            all_users.Add(new ChatUser(p_first_person_user_name, new UserSendChatMessage(CH_ChatSend)));
        }

        public override void ReceiveMessageFromUser(AuthServerChatMessage p_chat_message)
        {
            ChatMessageObjectToClient chat_message_for_clients = new ChatMessageObjectToClient(p_chat_message.Get_user_name, p_chat_message.Get_chat_message.Get_text_message_object);
            if (status == PrivateChatStatus.Accepted)
            {
                foreach (ChatUser t_user in all_users)
                {
                    if (t_user.Get_user_name == p_chat_message.Get_user_name)
                    {
                        foreach (ChatUser r_user in all_users)
                        {
                            if (is_logged_in(r_user.Get_user_name))
                            {
                                r_user.Receive(chat_message_for_clients);
                            }
                        }
                    }
                }
            }
        }

        public override void LeaveChat(string p_user_left_chat_name)
        {
            if (status == PrivateChatStatus.Accepted && all_users.Count <= 2 && all_users.Contains(new ChatUser(p_user_left_chat_name, new UserSendChatMessage(CH_ChatSend))))
            {
                bool user_removed = false;
                for (int i = 0; i < all_users.Count; i++)
                {
                    if (all_users[i].Get_user_name == p_user_left_chat_name)
                    {
                        all_users.RemoveAt(i);
                        user_removed = true;
                    }
                }
                if (all_users.Count > 0 && user_removed)
                {
                    start_ejected_user_chat_inform_dialog(all_users[0].Get_user_name, Get_chat_id, p_user_left_chat_name + " Left the chat.", chat_type);
                }
                if (user_removed)
                {
                    remove_chat_from_manager(chat_id);
                }
            }
            else if (status == PrivateChatStatus.UnAccepted && all_users.Count == 1)
            {
                if (p_user_left_chat_name == invited_user_name)
                {
                    start_ejected_user_chat_inform_dialog(all_users[0].Get_user_name, Get_chat_id, p_user_left_chat_name + " Left the chat.", chat_type);
                    remove_chat_from_manager(chat_id);
                }
                else if (p_user_left_chat_name == all_users[0].Get_user_name)
                {
                    remove_chat_from_manager(chat_id);
                }
            }
        }

        public Se_BaseBooleanFunctionResult GetAnswerToChatInvitation(string p_user_name, bool p_answer)
        {
            if (status != PrivateChatStatus.UnAccepted)
            {
                return new Se_BooleanFunctionRejResult("this chat already accepeted");
            }
            if (invited_user_name != p_user_name)
            {
                return new Se_BooleanFunctionRejResult("you are not invited to this chat.");
            }
            if (p_answer == true)
            {
                if (all_users.Count > 2)
                {
                    return new Se_BooleanFunctionRejResult("chat is full");
                }
                if (all_users.Count == 0)
                {
                    return new Se_BooleanFunctionRejResult("incorrect state;");
                }
                if (all_users.Contains(new ChatUser(p_user_name, new UserSendChatMessage(CH_ChatSend))))
                {
                    return new Se_BooleanFunctionRejResult("youre name is already in this chat");
                }

                all_users.Add(new ChatUser(p_user_name, CH_ChatSend));
                start_client_someone_joined_chat_inform(all_users[0].Get_user_name, p_user_name, chat_id, chat_type);
                status = PrivateChatStatus.Accepted;
                return new Se_BooleanFunctionAccResult();
            }
            else
            {
                if (all_users.Count > 2)
                {
                    return new Se_BooleanFunctionRejResult("chat is full");
                }
                if (all_users.Count > 0)
                {
                    start_ejected_user_chat_inform_dialog(all_users[0].Get_user_name, Get_chat_id, p_user_name + " didnt accept your invite to chat", chat_type);
                }
                all_users.Clear();
                remove_chat_from_manager(chat_id);
                return new Se_BooleanFunctionAccResult();
            }
        }

        public bool IsThereUser(string p_user_name)
        {
            if (status == PrivateChatStatus.Accepted)
            {
                if (all_users.Count == 2)
                {
                    if (all_users[0].Get_user_name == p_user_name || all_users[1].Get_user_name == p_user_name)
                        return true;
                }
                else if (all_users.Count == 1)
                {
                    if (all_users[0].Get_user_name == p_user_name)
                        return true;
                }
                return false;
            }
            else if (status == PrivateChatStatus.UnAccepted)
            {
                if (all_users.Count == 1)
                {
                    if (all_users[0].Get_user_name == p_user_name || invited_user_name == p_user_name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class PublicChat : BaseChat
    {
        StartClientSomeoneJoinedChatInform start_client_someone_joined_chat_inform;
        StartClientSomeoneLeftChatInform start_client_someone_left_chat_inform;

        public PublicChat(int p_max_users_count, RemoveChat p_remove_chat_from_manager, int p_chat_id, StartClientSomeoneLeftChatInform p_start_client_someone_left_chat_inform
           , StartClientSomeoneJoinedChatInform p_start_client_someone_joined_chat_inform, ChatSend p_chat_send, IsLoggedIn p_is_logged_in
            , StartEjectedChatUserInform p_start_ejected_user_chat_inform_dialog)
        {
            base.BaseConstruct(TypeOfChat.Public, p_chat_id, p_chat_send, p_remove_chat_from_manager, p_start_ejected_user_chat_inform_dialog, p_is_logged_in);
            all_users = new List<ChatUser>();
            max_users_count = p_max_users_count;
            min_user_count = 0;
            start_client_someone_joined_chat_inform = p_start_client_someone_joined_chat_inform;
            start_client_someone_left_chat_inform = p_start_client_someone_left_chat_inform;
        }

        public Se_BaseBooleanFunctionResult JoinToChatRequest(string p_user_name)
        {
            if (all_users.Count >= max_users_count)
            {
                return new Se_BooleanFunctionRejResult("This public chat is full");
            }

            if (HasUser(p_user_name))
            {
                return new Se_BooleanFunctionRejResult("Youre name is in this chat ");
            }

            ChatUser new_user = new ChatUser(p_user_name, new UserSendChatMessage(CH_ChatSend));
            foreach (ChatUser c_user in all_users)
            {
                start_client_someone_joined_chat_inform(c_user.Get_user_name, p_user_name, Get_chat_id, chat_type);
            }
            all_users.Add(new_user);
            return new Se_BooleanFunctionAccResult();
        }

        public override void LeaveChat(string p_user_left_chat_name)
        {
            for (int i = 0; i < all_users.Count; i++)
            {
                if (all_users[i].Get_user_name == p_user_left_chat_name)
                {

                    all_users.RemoveAt(i);
                    foreach (ChatUser c_user in all_users)
                    {
                        start_client_someone_left_chat_inform(c_user.Get_user_name, p_user_left_chat_name, Get_chat_id, chat_type);
                    }
                }
            }
        }

        public bool IsThereUser(string p_user_name)
        {
            if (all_users.Contains(new ChatUser(p_user_name, new UserSendChatMessage(CH_ChatSend))))
                return true;
            return false;
        }

        public List<string> GetUsersList()
        {
            List<string> users_list = new List<string>();
            foreach (ChatUser t_user_data in all_users)
            {
                users_list.Add(t_user_data.Get_user_name);
            }
            return users_list;
        }

    }


    public abstract class Ma_BaseChatManager
    {
        protected TypeOfChat chat_type;

        protected ChatSend chat_send;
        protected StartEjectedChatUserInform start_ejected_user_chat_inform_dialog;
        protected IsLoggedIn is_logged_in;
        protected StartClientSomeoneJoinedChatInform start_client_someone_joined_chat_inform;

        public void BaseConstruct(TypeOfChat p_chat_type, ChatSend p_send_chat_message, StartEjectedChatUserInform p_start_ejected_user_chat_inform_dialog
            , IsLoggedIn p_is_logged_in)
        {
            chat_type = p_chat_type;
            chat_send = p_send_chat_message;
            start_ejected_user_chat_inform_dialog = p_start_ejected_user_chat_inform_dialog;
            is_logged_in = p_is_logged_in;
        }

        public abstract void ReceiveMessage(AuthServerChatMessage p_message);
        public abstract void LeaveChat(string p_user_name, int p_chat_id);
        public abstract void ChatRemoveItselfRequest(int p_chat_id);

        public void Ma_ChatSendChatMessage(string p_user_name, ChatMessageForClient p_chat_message)
        {
            chat_send(p_user_name, p_chat_message);
        }
        public void Ma_StartEjectedChatUserInform(string p_user_to_inform, int p_chat_id, string p_ejecting_comment, TypeOfChat p_chat_type)
        {
            start_ejected_user_chat_inform_dialog(p_user_to_inform, p_chat_id, p_ejecting_comment, p_chat_type);
        }
        public bool Ma_IsloggedIn(string p_user_name)
        {
            return is_logged_in(p_user_name);
        }
        public void Ma_StartClientSomeoneJoinedChatInform(string p_user_to_inform_name, string p_user_joined_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            start_client_someone_joined_chat_inform(p_user_to_inform_name, p_user_joined_name, p_chat_id, p_chat_type);
        }
    }

    public class Ma_PrivateChatManager : Ma_BaseChatManager
    {
        Dictionary<int, PrivateChat> all_chats;


        public Ma_PrivateChatManager(Dictionary<int, PrivateChat> p_all_chats, ChatSend p_chat_send,
            StartEjectedChatUserInform p_start_ejected_user_chat_inform_dialog, IsLoggedIn p_is_logged_in
            , StartClientSomeoneJoinedChatInform p_start_client_someone_joined_chat_inform)
        {
            base.BaseConstruct(TypeOfChat.Private, p_chat_send, p_start_ejected_user_chat_inform_dialog, p_is_logged_in);
            start_client_someone_joined_chat_inform = p_start_client_someone_joined_chat_inform;
            all_chats = p_all_chats;
        }

        public override void ReceiveMessage(AuthServerChatMessage p_message)
        {
            if (is_logged_in(p_message.Get_user_name))
            {
                if (all_chats.ContainsKey(p_message.Get_chat_message.Get_chat_id))
                {
                    all_chats[p_message.Get_chat_message.Get_chat_id].ReceiveMessageFromUser(p_message);
                }
            }
        }

        public Se_BaseIntFunctionResult CreateChat(string p_first_person_user_name, string p_second_person_user_name)
        {
            foreach (PrivateChat pr_chat in all_chats.Values)
            {
                if (pr_chat.IsThereUser(p_first_person_user_name))
                {
                    if (pr_chat.IsThereUser(p_second_person_user_name))
                    {
                        return new Se_IntFunctionRejResult("this chat already exist");
                    }
                }
            }
            int random_chat_id = HelperFunctions.GetGUID();
            if (all_chats.ContainsKey(random_chat_id))
            {
                return new Se_IntFunctionRejResult("please try again");
            }
            all_chats.Add(random_chat_id, new PrivateChat(p_first_person_user_name, p_second_person_user_name, random_chat_id, new ChatSend(Ma_ChatSendChatMessage)
            , new StartEjectedChatUserInform(Ma_StartEjectedChatUserInform), new RemoveChat(ChatRemoveItselfRequest), new IsLoggedIn(Ma_IsloggedIn)
            , new StartClientSomeoneJoinedChatInform(Ma_StartClientSomeoneJoinedChatInform)));
            return new Se_IntFunctionAccResult(random_chat_id);

        }

        public override void LeaveChat(string p_user_name, int p_chat_id)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                all_chats[p_chat_id].LeaveChat(p_user_name);
            }
        }

        public override void ChatRemoveItselfRequest(int p_chat_id)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                all_chats.Remove(p_chat_id);
            }
        }

        public bool IsTherePrivateChat(int p_chat_id)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UserLeavAllChats(string p_user_name)
        {
            for (int i = 0; i < all_chats.Count; i++)
            {
                if (all_chats.ElementAt(i).Value.IsThereUser(p_user_name))
                    all_chats.ElementAt(i).Value.LeaveChat(p_user_name);
            }
        }

        public Se_BaseBooleanFunctionResult GetAnswerToPrivateChatInvitation(string p_user_name, int p_chat_id, bool p_answer)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                return all_chats[p_chat_id].GetAnswerToChatInvitation(p_user_name, p_answer);
            }
            return new Se_BooleanFunctionRejResult("this chat id is not valid.");
        }

    }

    public class Ma_PublicChatManager : Ma_BaseChatManager
    {
        Dictionary<int, PublicChat> all_chats;

        StartClientSomeoneLeftChatInform start_client_someone_left_chat_inform;

        public Ma_PublicChatManager(Dictionary<int, PublicChat> p_all_chats, ChatSend p_chat_send, StartEjectedChatUserInform p_start_ejected_user_chat_inform_dialog
            , IsLoggedIn p_is_logged_in, StartClientSomeoneJoinedChatInform p_start_client_someone_joined_chat_inform
            , StartClientSomeoneLeftChatInform p_start_client_someone_left_chat_inform)
        {
            base.BaseConstruct(TypeOfChat.Public, p_chat_send, p_start_ejected_user_chat_inform_dialog, p_is_logged_in);

            all_chats = p_all_chats;
            start_client_someone_joined_chat_inform = p_start_client_someone_joined_chat_inform;
            start_client_someone_left_chat_inform = p_start_client_someone_left_chat_inform;

            Create(HelperFunctions.GetGUID(), 20);
            Create(HelperFunctions.GetGUID(), 50);
            Create(HelperFunctions.GetGUID(), 100);
        }


        public List<int> GetPublicChatIDs()
        {
            List<int> public_chat_ids = new List<int>();
            for (int i = 0; i < all_chats.Count; i++)
            {
                public_chat_ids.Add(all_chats.ElementAt(i).Key);
            }
            return public_chat_ids;
        }

        public Se_BaseBooleanFunctionResult JoinToChatRequest(string p_user_name, int p_chat_id)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                return all_chats[p_chat_id].JoinToChatRequest(p_user_name);
            }
            else
            {
                return new Se_BooleanFunctionRejResult("Chat ID is not valid");
            }
        }


        public override void ChatRemoveItselfRequest(int p_chat_id)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                all_chats.Remove(p_chat_id);
            }
        }
        public override void ReceiveMessage(AuthServerChatMessage p_message)
        {
            if (is_logged_in(p_message.Get_user_name))
            {
                if (all_chats.ContainsKey(p_message.Get_chat_message.Get_chat_id))
                {
                    all_chats[p_message.Get_chat_message.Get_chat_id].ReceiveMessageFromUser(p_message);
                }
            }
        }
        public override void LeaveChat(string p_user_name, int p_chat_id)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                all_chats[p_chat_id].LeaveChat(p_user_name);
            }
        }

        public void Create(int p_chat_id, int p_max_users_count)
        {
            all_chats.Add(p_chat_id, new PublicChat(p_max_users_count, new RemoveChat(ChatRemoveItselfRequest), p_chat_id, new StartClientSomeoneLeftChatInform(Ma_StartClientSomeoneLeftChatInform)
            , new StartClientSomeoneJoinedChatInform(Ma_StartClientSomeoneJoinedChatInform), chat_send, new IsLoggedIn(Ma_IsloggedIn)
            , new StartEjectedChatUserInform(start_ejected_user_chat_inform_dialog)));
        }

        public bool IsUserInPublicChat(string p_user_name, int p_public_chat_id)
        {
            if (all_chats.ContainsKey(p_public_chat_id))
            {
                if (all_chats[p_public_chat_id].IsThereUser(p_user_name))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetPublicChatUsersList(int p_chat_id)
        {
            if (all_chats.ContainsKey(p_chat_id))
            {
                return all_chats[p_chat_id].GetUsersList();
            }
            return new List<string>();
        }

        public void UserLeavAllChats(string p_user_name)
        {
            for (int i = 0; i < all_chats.Count; i++)
            {
                if (all_chats.ElementAt(i).Value.IsThereUser(p_user_name))
                    all_chats.ElementAt(i).Value.LeaveChat(p_user_name);
            }
        }

        public void Ma_StartClientSomeoneLeftChatInform(string p_user_to_inform_name, string p_user_left_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            start_client_someone_left_chat_inform(p_user_to_inform_name, p_user_left_name, p_chat_id, p_chat_type);
        }
    }


    public class AllChats
    {
        Ma_PrivateChatManager private_chat_manager;
        Ma_PublicChatManager public_chat_manager;

        ChatSend send_chat_message;
        StartEjectedChatUserInform start_ejected_user_chat_inform_dialog;
        IsLoggedIn is_logged_in;
        StartClientSomeoneJoinedChatInform start_client_someone_joined_chat_inform;
        StartClientSomeoneLeftChatInform start_client_someone_left_chat_inform;

        public AllChats(Dictionary<int, PrivateChat> p_all_private_chats, Dictionary<int, PublicChat> p_all_public_chats,
             ChatSend p_send_chat_message, StartEjectedChatUserInform p_start_ejected_user_chat_inform_dialog
            , IsLoggedIn p_is_logged_in, StartClientSomeoneJoinedChatInform p_start_client_someone_joined_chat_inform, StartClientSomeoneLeftChatInform p_start_client_someone_left_chat_inform)
        {
            send_chat_message = p_send_chat_message;
            start_ejected_user_chat_inform_dialog = p_start_ejected_user_chat_inform_dialog;
            start_client_someone_joined_chat_inform = p_start_client_someone_joined_chat_inform;
            start_client_someone_left_chat_inform = p_start_client_someone_left_chat_inform;
            is_logged_in = p_is_logged_in;

            private_chat_manager = new Ma_PrivateChatManager(p_all_private_chats, new ChatSend(AllMa_ChatSendChatMessage)
            , new StartEjectedChatUserInform(AllMa_StartEjectedChatUserInform), new IsLoggedIn(AllMa_IsloggedIn)
            , new StartClientSomeoneJoinedChatInform(AllMa_StartClientSomeoneJoinedChatInform));

            public_chat_manager = new Ma_PublicChatManager(p_all_public_chats, new ChatSend(AllMa_ChatSendChatMessage)
            , new StartEjectedChatUserInform(AllMa_StartEjectedChatUserInform), new IsLoggedIn(AllMa_IsloggedIn)
            , new StartClientSomeoneJoinedChatInform(AllMa_StartClientSomeoneJoinedChatInform)
            , new StartClientSomeoneLeftChatInform(AllMa_StartClientSomeoneLeftChatInform));

        }

        public void AllMa_ReceiveMessage(AuthServerChatMessage p_messsage)
        {
            if (!string.IsNullOrWhiteSpace(p_messsage.Get_chat_message.Get_text_message_object.Get_text_of_message))
            {
                if (p_messsage.Get_chat_message.Get_chat_type == TypeOfChat.Private)
                {
                    private_chat_manager.ReceiveMessage(p_messsage);
                }
                else if (p_messsage.Get_chat_message.Get_chat_type == TypeOfChat.Public)
                {
                    public_chat_manager.ReceiveMessage(p_messsage);
                }
            }
        }

        public void AllMa_ChatLeave(string p_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            if (p_chat_type == TypeOfChat.Private)
            {
                private_chat_manager.LeaveChat(p_user_name, p_chat_id);
            }
            else if (p_chat_type == TypeOfChat.Public)
            {
                public_chat_manager.LeaveChat(p_user_name, p_chat_id);
            }
        }

        public List<int> AllMa_GetPublicChatIDs()
        {
            return public_chat_manager.GetPublicChatIDs();
        }

        public void AllMa_CreatePubliChat(int p_chat_id, int p_max_users_count)
        {
            public_chat_manager.Create(p_chat_id, p_max_users_count);
        }

        public Se_BaseIntFunctionResult AllMa_CreatePrivateChat(string p_first_person_user_name, string p_second_person_user_name)
        {
            return private_chat_manager.CreateChat(p_first_person_user_name, p_second_person_user_name);
        }

        public bool AllMa_IsTherePrivateChat(int p_chat_id)
        {
            return private_chat_manager.IsTherePrivateChat(p_chat_id);
        }

        public bool AllMa_IsUserInPublicChat(string p_user_name, int p_public_chat_id)
        {
            return public_chat_manager.IsUserInPublicChat(p_user_name, p_public_chat_id);
        }

        public List<string> AllMa_GetPublicChatUsersList(int p_chat_id)
        {
            return public_chat_manager.GetPublicChatUsersList(p_chat_id);
        }

        public void AllMa_UserLeavAllChats(string p_user_name)
        {
            public_chat_manager.UserLeavAllChats(p_user_name);
            private_chat_manager.UserLeavAllChats(p_user_name);
        }

        public Se_BaseBooleanFunctionResult AllMa_JoinPublicChatRequest(string p_user_name, int p_chat_id)
        {
            return public_chat_manager.JoinToChatRequest(p_user_name, p_chat_id);
        }

        public Se_BaseBooleanFunctionResult GetAnswerToPrivateChatInvitation(string p_user_name, int p_chat_id, bool p_answer)
        {
            return private_chat_manager.GetAnswerToPrivateChatInvitation(p_user_name, p_chat_id, p_answer);
        }

        public void AllMa_ChatSendChatMessage(string p_user_name, ChatMessageForClient p_chat_message)
        {
            send_chat_message(p_user_name, p_chat_message);
        }
        public void AllMa_StartEjectedChatUserInform(string p_user_to_inform, int p_chat_id, string p_ejecting_comment, TypeOfChat p_chat_type)
        {
            start_ejected_user_chat_inform_dialog(p_user_to_inform, p_chat_id, p_ejecting_comment, p_chat_type);
        }
        public bool AllMa_IsloggedIn(string p_user_name)
        {
            return is_logged_in(p_user_name);
        }
        public void AllMa_StartClientSomeoneJoinedChatInform(string p_user_to_inform_name, string p_user_joined_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            start_client_someone_joined_chat_inform(p_user_to_inform_name, p_user_joined_name, p_chat_id, p_chat_type);
        }
        public void AllMa_StartClientSomeoneLeftChatInform(string p_user_to_inform_name, string p_user_left_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            start_client_someone_left_chat_inform(p_user_to_inform_name, p_user_left_name, p_chat_id, p_chat_type);
        }

    }


    public class ChatUser
    {
        string user_name;
        public string Get_user_name
        {
            get { return user_name; }
        }

        UserSendChatMessage send_chat_message;

        public ChatUser(string p_user_name, UserSendChatMessage p_send_chat_message)
        {
            user_name = p_user_name;
            send_chat_message = p_send_chat_message;
        }

        public void Receive(ChatMessageObjectToClient p_chat_message)
        {
            send_chat_message(user_name, p_chat_message);
        }

        public override bool Equals(object obj)
        {
            try
            {
                ChatUser t_chat_user = (ChatUser)obj;
                if (t_chat_user.Get_user_name == user_name)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

