using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerTcpChat.Classes;
using System.Threading;
using CommonChatTypes;

namespace ServerTcpChat.Classes
{

    public delegate bool DB_IsThereUserPass(string p_user_name, string p_password);
    public delegate List<string> DB_GetUserFriendsList(string p_user_name);
    public delegate bool RemoveThread(int p_thread_id);
    public delegate void StartClientFriendChangedStatusInformDialog(string p_user_to_infrom, string p_user_changed_status, UserStatus p_user_tatus);
    public delegate void DB_CreateNewUser(string p_user_name, string p_password);
    public delegate int GetOnlineUserThreadID(string p_user_name);
    public delegate string GetAThreadUserName(int p_thread_id);
    public delegate void DB_AddOfflineMessage(string p_sender_user_name, string p_receiver_user_name, string p_message_text);

    public delegate bool ChangeStatusToOffline(string p_user_name);

    public delegate bool OfflineUser(string p_user_name);

    public delegate UserPresenceState GetUserPresenceState(string p_user_name);


    public class ServerCore
    {
        DB_Controller controller;
        StartClientFriendChangedStatusInformDialog start_client_friend_changed_status_inform_dialog;
        StartSendToClinetFormalMessage start_send_to_client_fromal_message;

        UserManager user_manager;
        ThreadManager thread_manager;
        SendData send_data;

        public ServerCore(Dictionary<int, UserData> p_all_users_logged_in, StartClientFriendChangedStatusInformDialog p_start_client_friend_changed_status_inform_dialog, StartSendToClinetFormalMessage p_start_send_to_client_fromal_message
            , List<int> p_all_threads, SendToDistributerConstruct p_send_to_distributer_construct, object p_distributer_pulse_object)
        {
            int h = 0;
            while (h < 4)
            {
                try
                {
                    controller = new DB_Controller();
                    h = 0;
                    break;
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    h++;
                    continue;
                }
            }
            if (h > 3)
            {
                Console.WriteLine("Error in working with database");
                try
                {
                    Environment.Exit(5);
                }
                catch
                {
                }
                Thread.CurrentThread.Abort();
                return;
            }

            start_client_friend_changed_status_inform_dialog = p_start_client_friend_changed_status_inform_dialog;
            thread_manager = new ThreadManager(p_all_threads);
            start_send_to_client_fromal_message = p_start_send_to_client_fromal_message;

            user_manager = new UserManager(p_all_users_logged_in, new DB_IsThereUserPass(controller.IsThereUserPass), new DB_GetUserFriendsList(controller.GetUserFriendsList)
            , new DB_IsThereUser(controller.IsThereUser), new RemoveThread(SC_RemoveThread), new DB_CreateNewUser(controller.AddNewUser)
            , new StartClientFriendChangedStatusInformDialog(SC_StartClientFriendChangedStatusInformDialog), new StartSendToClinetFormalMessage(SC_StartSendToClinetFormalMessage)
            , new CreateOfflineMessage(SC_DB_AddOfflineMessage), new RegisterThread(SC_RegisterAThread));

            send_data = new SendData(p_send_to_distributer_construct, new IsLoggedIn(SC_IsLoggedIn), new GetOnlineUserThreadID(SC_GetOnlineUserThreadID),
                new GetAThreadUserName(SC_GetAThreadUserName), new IsThereUnauthWorkerThread(SC_IsThereUnauthWorkerThread), p_distributer_pulse_object);
        }

        public bool SC_RemoveThread(int p_thread_id)
        {
            return thread_manager.RemoveAThread(p_thread_id);
        }
        public bool SC_IsThereUnauthWorkerThread(int p_thread_id)
        {
            return thread_manager.IsThereUnAuthWorkerThread(p_thread_id);
        }
        public void SC_RegisterAThread(int p_thread_id)
        {
            thread_manager.RegisterAThread(p_thread_id);
        }

        public void SC_AuthSend(string p_user_name, DialogMessageForClient p_message)
        {
            send_data.AuthSend(p_user_name, p_message);
        }
        public void SC_UnAuthSend(int p_thread_id, DialogMessageForClient p_message)
        {
            send_data.UnAuthsend(p_thread_id, p_message);
        }
        public void SC_ChatSend(string p_receiver_user_name, ChatMessageForClient p_message)
        {
            send_data.AuthSend(p_receiver_user_name, p_message);
        }

        public Se_BaseBooleanFunctionResult SC_Login(int p_thread_id, string p_user_name, string p_password)
        {
            return user_manager.SignIn(p_thread_id, p_user_name, p_password);
        }
        public bool SC_IsLoggedIn(string p_user_name)
        {
            return user_manager.IsLoggedIn(p_user_name);
        }
        public Se_BaseBooleanFunctionResult SC_SignUp(string p_user_name, string p_password)
        {
            return user_manager.SignUp(p_user_name, p_password);
        }
        public bool SC_ChangeStatusToOffline(string p_user_name)
        {
            return user_manager.ChangeStatusToOffline(p_user_name);
        }
        public bool SC_OfflineUser(string p_user_name)
        {
            return user_manager.OfflineUser(p_user_name);
        }
        public int SC_GetOnlineUserThreadID(string p_user_name)
        {
            return user_manager.GetOnlineUserThreadID(p_user_name);
        }
        public string SC_GetAThreadUserName(int p_thread_id)
        {
            return user_manager.GetAThreadUserName(p_thread_id);
        }
        public Se_BaseBooleanFunctionResult SC_OnlineAreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            return user_manager.AreFriends(p_first_person_user_name, p_second_person_user_name);
        }
        public List<string> SC_GetOnlineUserFriendList(string p_user_name)
        {
            return user_manager.GetOnlineUserFriendList(p_user_name);
        }
        public void SC_ReloadFriendList(string p_user_name)
        {
            user_manager.ReloadFriendList(p_user_name);
        }
        public UserPresenceState SC_GetUserPresenceState(string p_user_name)
        {
            return user_manager.GetUserPresenceState(p_user_name);
        }
        public void SC_SendFormalMessageToUser(FormalMessage p_message, string p_receiver_user_name)
        {
            user_manager.SendFormalMessageToUser(p_message, p_receiver_user_name);
        }
        public bool SC_ThreadisLoggedIn(int p_thread_id)
        {
            return user_manager.ThreadisLoggedIn(p_thread_id);
        }
        public UserStatus SC_GetUserStatus(string p_user_name)
        {
            return user_manager.GetUserStatus(p_user_name);
        }

        public bool SC_DB_IsThereUserPass(string p_user_name, string p_passworrd)
        {
            return controller.IsThereUserPass(p_user_name, p_passworrd);
        }
        public List<string> SC_DB_GetUserFriendsList(string p_user_name)
        {
            return controller.GetUserFriendsList(p_user_name);
        }
        public bool SC_DB_IsThereUser(string p_user_name)
        {
            return controller.IsThereUser(p_user_name);
        }
        public void SC_DB_AddNewUser(string p_user_name, string p_password)
        {
            controller.AddNewUser(p_user_name, p_password);
        }
        public void SC_DB_InsertFriendshipRelation(string p_first_person_user_name, string p_second_person_user_name)
        {
            controller.InsertFriendshipRelation(p_first_person_user_name, p_second_person_user_name);
        }
        public bool SC_DB_AreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            return controller.AreFriends(p_first_person_user_name, p_second_person_user_name);
        }
        public void SC_DB_AddOfflineMessage(string p_reciever_user_name, OfflineMessage p_message)
        {
            controller.AddOfflineMessage(p_message.Get_message_id, p_message.Get_sender_user_id, p_reciever_user_name, p_message.Get_message_text);
        }
        public void SC_DB_CreateAddAgreement(int p_agreement_id, string p_starter_user_name, string p_invited_user_name)
        {
            controller.CreateAddAgreement(p_agreement_id, p_starter_user_name, p_invited_user_name);
        }
        public void SC_DB_RemoveAgreement(int p_agreement_id)
        {
            controller.RemoveAgreement(p_agreement_id);
        }
        public List<AgreementInvitationInfo> SC_DB_GetUserAgreementInvitation(string p_user_name)
        {
            return controller.GetUserAgreementInvitation(p_user_name);
        }
        public void SC_DB_OfflineAddToFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            controller.AddToFriends(p_first_person_user_name, p_second_person_user_name);
        }
        public List<OfflineMessage> SC_DB_LoadUserOfflineMessages(string p_user_name)
        {
            return controller.LoadUserOfflineMessages(p_user_name);
        }
        public void SC_DB_RemoveUserOfflineMessages(string p_user_name, List<int> p_message_ids)
        {
            controller.RemoveUserOfflineMessages(p_user_name, p_message_ids);
        }
        public bool SC_DB_IsThereAgreement(int p_agreement_id)
        {
            return controller.IsThereAgreement(p_agreement_id);
        }
        public void SC_DB_AddAgreementDone(int p_agreement_id)
        {
            controller.AddAgreementDone(p_agreement_id);
        }
        public List<Agreement> SC_DB_GetAllAdAgreements()
        {
            return controller.GetAllAddAgreements();
        }
        public AgreementInvitationInfo SC_DB_GetAUserAgreementInvitation(string p_user_name, int p_agreement_id)
        {
            return controller.GetAUserAgreementInvitation(p_user_name, p_agreement_id);
        }

        public void SC_StartClientFriendChangedStatusInformDialog(string p_user_to_infrom, string p_user_changed_status, UserStatus p_user_tatus)
        {
            start_client_friend_changed_status_inform_dialog(p_user_to_infrom, p_user_changed_status, p_user_tatus);
        }
        public void SC_StartSendToClinetFormalMessage(string p_receiver_user_name, string p_sender_user_name, string p_message_text, int p_message_id)
        {
            start_send_to_client_fromal_message(p_receiver_user_name, p_sender_user_name, p_message_text, p_message_id);
        }

    }


    public class UserManager
    {
        Dictionary<int, UserData> all_users_logged_in;

        DB_IsThereUserPass is_there_user_pass;
        DB_GetUserFriendsList get_user_friend_list;
        DB_IsThereUser is_there_user;
        RemoveThread remove_thread;
        DB_CreateNewUser create_new_user;
        StartClientFriendChangedStatusInformDialog start_client_friend_changed_status_inform_dialog;
        StartSendToClinetFormalMessage start_send_to_client_fromal_message;
        CreateOfflineMessage add_offline_meesge;
        RegisterThread register_a_thread;


        public UserManager(Dictionary<int, UserData> p_all_users_logged_in, DB_IsThereUserPass p_is_there_user_pass, DB_GetUserFriendsList p_get_user_friend_list
            , DB_IsThereUser p_is_there_user, RemoveThread p_remove_thread, DB_CreateNewUser p_create_new_user, StartClientFriendChangedStatusInformDialog p_start_client_friend_changed_status_inform_dialog
            , StartSendToClinetFormalMessage p_start_send_to_client_fromal_message, CreateOfflineMessage p_add_offline_meesge, RegisterThread p_register_a_thread)
        {
            all_users_logged_in = p_all_users_logged_in;

            is_there_user_pass = p_is_there_user_pass;
            get_user_friend_list = p_get_user_friend_list;
            is_there_user = p_is_there_user;
            remove_thread = p_remove_thread;
            create_new_user = p_create_new_user;
            start_client_friend_changed_status_inform_dialog = p_start_client_friend_changed_status_inform_dialog;
            start_send_to_client_fromal_message = p_start_send_to_client_fromal_message;
            add_offline_meesge = p_add_offline_meesge;
            register_a_thread = p_register_a_thread;
        }

        public Se_BaseBooleanFunctionResult SignIn(int p_thread_id, string p_user_name, string p_password)
        {
            if (!is_there_user_pass(p_user_name, p_password))
            {
                return new Se_BooleanFunctionRejResult("User name or password is incorrect");
            }
            if (all_users_logged_in.ContainsKey(p_thread_id))
            {
                return new Se_BooleanFunctionRejResult("An illegal Action");
            }
            if (IsLoggedIn(p_user_name))
            {
                return new Se_BooleanFunctionRejResult("you are online in the server");
            }
            if (all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Online))
                || all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Offline)))
            {
                return new Se_BooleanFunctionRejResult("you are in the server");
            }

            List<string> friends_list = get_user_friend_list(p_user_name);
            try
            {
                all_users_logged_in.Add(p_thread_id, new UserData(p_user_name, friends_list, UserStatus.Online));
                foreach (string friend_name in friends_list)
                {
                    if (IsLoggedIn(friend_name))
                    {
                        start_client_friend_changed_status_inform_dialog(friend_name, p_user_name, UserStatus.Online);
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                register_a_thread(p_thread_id);
                throw new System.Data.DataException("an error occrured");
            }
            return new Se_BooleanFunctionAccResult();

        }

        public void SendFormalMessageToUser(FormalMessage p_message, string p_receiver_user_name)
        {
            int message_id = HelperFunctions.GetGUID();
            add_offline_meesge(p_receiver_user_name, new OfflineMessage(message_id, p_message.Get_sender_user_id, p_message.Get_message_text));
            if (IsLoggedIn(p_receiver_user_name))
            {
                start_send_to_client_fromal_message(p_receiver_user_name, p_message.Get_sender_user_id, p_message.Get_message_text, message_id);
            }
        }

        public bool IsLoggedIn(string p_user_name)
        {
            if (all_users_logged_in.Values.Contains(new UserData(p_user_name, new List<string>(), UserStatus.Online)))
            {
                return true;
            }
            return false;
        }

        public bool ThreadisLoggedIn(int p_thread_id)
        {
            if (all_users_logged_in.ContainsKey(p_thread_id))
                return true;
            return false;
        }

        public Se_BaseBooleanFunctionResult SignUp(string p_user_name, string p_password) 
        {
            if (p_password.Length < 6)
            {
                return new Se_BooleanFunctionRejResult("password length is less than 6");
            }
            if (p_user_name.Trim().Length < 5)
            {
                return new Se_BooleanFunctionRejResult("User name length must be more than 4");
            }

            foreach (char p_char in p_password)
            {
                if (!char.IsLetterOrDigit(p_char) && p_char != '_')
                {
                    return new Se_BooleanFunctionRejResult("you can just use letter and digit and underline for password");
                }
            }
            foreach (char u_char in p_user_name)
            {
                if (!char.IsLetterOrDigit(u_char) && u_char != '_')
                {
                    return new Se_BooleanFunctionRejResult("you can just use letter and digit and underline for user name");
                }
            }

            bool user_exist = true;
            try
            {
                user_exist = is_there_user(p_user_name.Trim());
            }
            catch (Exception)
            {
                return new Se_BooleanFunctionRejResult("A problem occured");
            }
            if (user_exist)
            {
                return new Se_BooleanFunctionRejResult("this user name is using by another person");
            }

            if (p_user_name.Length >= 30 || p_password.Length >= 30)
            {
                return new Se_BooleanFunctionRejResult("length of username and/or password is more than 30 characters");
            }


            try
            {
                create_new_user(p_user_name, p_password);
            }
            catch (Exception)
            {
                return new Se_BooleanFunctionRejResult("A problem occured");
            }
            return new Se_BooleanFunctionAccResult();


        }

        public bool ChangeStatusToOffline(string p_user_name)
        {
            List<string> backup_user_friends_list = new List<string>();
            if (all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Online)))
            {
                for (int i = 0; i < all_users_logged_in.Count; i++)
                {
                    if (all_users_logged_in.ElementAt(i).Value.Get_user_name == p_user_name && all_users_logged_in.ElementAt(i).Value.Get_Set_status == UserStatus.Online)
                    {
                        int temp_key = all_users_logged_in.ElementAt(i).Key;
                        all_users_logged_in[temp_key].Get_Set_status = UserStatus.Offline;
                        backup_user_friends_list = all_users_logged_in[temp_key].Get_Set_friends_list;
                        foreach (string t_user_name in backup_user_friends_list)
                        {
                            if (IsLoggedIn(t_user_name))
                            {
                                start_client_friend_changed_status_inform_dialog(t_user_name, p_user_name, UserStatus.Offline);
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public bool OfflineUser(string p_user_name)
        {
            if (all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Offline)))
            {
                for (int i = 0; i < all_users_logged_in.Count; i++)
                {
                    if (all_users_logged_in.ElementAt(i).Value.Get_user_name == p_user_name)
                    {
                        int temp_key = all_users_logged_in.ElementAt(i).Key;
                        all_users_logged_in.Remove(all_users_logged_in.ElementAt(i).Key);
                        return true;
                    }
                }
            }
            return false;
        }

        public int GetOnlineUserThreadID(string p_user_name)
        {
            if (all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Online)))
            {
                for (int i = 0; i < all_users_logged_in.Count; i++)
                {
                    if (all_users_logged_in.ElementAt(i).Value.Get_user_name == p_user_name && all_users_logged_in.ElementAt(i).Value.Get_Set_status == UserStatus.Online)
                    {
                        return all_users_logged_in.ElementAt(i).Key;
                    }
                }
            }
            return 0;
        }

        public string GetAThreadUserName(int p_thread_id)
        {
            if (all_users_logged_in.ContainsKey(p_thread_id))
            {
                return all_users_logged_in[p_thread_id].Get_user_name;
            }
            return null;
        }

        public UserPresenceState GetUserPresenceState(string p_user_name)
        {
            if (all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Online)))
            {
                return UserPresenceState.OnlineStatus;
            }
            else if (all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Offline)))
            {
                return UserPresenceState.OfflineStatus;
            }
            return UserPresenceState.NotPresent;
        }

        public UserStatus GetUserStatus(string p_user_name)
        {
            if (all_users_logged_in.ContainsValue(new UserData(p_user_name, new List<string>(), UserStatus.Online)))
            {
                return UserStatus.Online;
            }
            return UserStatus.Offline;
        }

        public Se_BaseBooleanFunctionResult AreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            if (all_users_logged_in.ContainsValue(new UserData(p_first_person_user_name, new List<string>(), UserStatus.Online)))
            {
                for (int i = 0; i < all_users_logged_in.Count; i++)
                {
                    if (all_users_logged_in.ElementAt(i).Value.Get_user_name == p_first_person_user_name)
                    {
                        List<string> all_friedns = all_users_logged_in.ElementAt(i).Value.Get_Set_friends_list;
                        if (all_friedns.Contains(p_second_person_user_name))
                        {
                            return new Se_BooleanFunctionAccResult();
                        }
                        return new Se_BooleanFunctionRejResult("This two persons are not friends");
                    }
                }
            }
            else if (all_users_logged_in.ContainsValue(new UserData(p_second_person_user_name, new List<string>(), UserStatus.Online)))
            {
                for (int i = 0; i < all_users_logged_in.Count; i++)
                {
                    if (all_users_logged_in.ElementAt(i).Value.Get_user_name == p_second_person_user_name)
                    {
                        List<string> all_friedns = all_users_logged_in.ElementAt(i).Value.Get_Set_friends_list;
                        if (all_friedns.Contains(p_first_person_user_name))
                        {
                            return new Se_BooleanFunctionAccResult();
                        }
                        return new Se_BooleanFunctionRejResult("This two persons are not friends");
                    }
                }
            }
            return new Se_BooleanFunctionRejResult("User is not Online");
        }

        public List<string> GetOnlineUserFriendList(string p_user_name)
        {
            if (!IsLoggedIn(p_user_name))
                return new List<string>();
            for (int i = 0; i < all_users_logged_in.Count; i++)
            {
                if (all_users_logged_in.ElementAt(i).Value.Equals(new UserData(p_user_name, new List<string>(), UserStatus.Online)))
                {
                    return all_users_logged_in.ElementAt(i).Value.Get_Set_friends_list;
                }
            }
            return new List<string>();
        }

        public void ReloadFriendList(string p_user_name)
        {
            if (!IsLoggedIn(p_user_name))
                return;
            for (int i = 0; i < all_users_logged_in.Count; i++)
            {
                if (all_users_logged_in.ElementAt(i).Value.Equals(new UserData(p_user_name, new List<string>(), UserStatus.Online)))
                {
                    all_users_logged_in.ElementAt(i).Value.Get_Set_friends_list = get_user_friend_list(p_user_name);
                }
            }
        }
    }

    public class ThreadManager
    {
        List<int> all_threads;

        public ThreadManager(List<int> p_all_threads)
        {
            all_threads = p_all_threads;
        }

        public bool IsThereUnAuthWorkerThread(int p_thread_id)
        {
            if (all_threads.Contains(p_thread_id))
                return true;
            return false;
        }

        public bool RemoveAThread(int p_thread_id)
        {
            if (all_threads.Contains(p_thread_id))
            {
                all_threads.Remove(p_thread_id);
                return true;
            }
            return false;
        }

        public void RegisterAThread(int p_thread_id)
        {
            if (!all_threads.Contains(p_thread_id))
                all_threads.Add(p_thread_id);
        }
    }

    public class SendData
    {
        IsLoggedIn is_logged_in;
        GetOnlineUserThreadID get_online_user_thread_id;
        GetAThreadUserName get_thread_user_name;
        IsThereUnauthWorkerThread is_there_unauth_worker_thread;

        SendToDistributerConstruct send_to_distributer_construct;
        object distributer_pulse_object;

        public SendData(SendToDistributerConstruct p_send_to_distributer_construct, IsLoggedIn p_is_logged_in, GetOnlineUserThreadID p_get_online_user_thread_id
            , GetAThreadUserName p_get_thread_user_name, IsThereUnauthWorkerThread p_is_there_unauth_worker_thread, object p_distributer_pulse_object)
        {
            send_to_distributer_construct = p_send_to_distributer_construct;
            distributer_pulse_object = p_distributer_pulse_object;

            is_logged_in = p_is_logged_in;
            get_online_user_thread_id = p_get_online_user_thread_id;
            get_thread_user_name = p_get_thread_user_name;
            is_there_unauth_worker_thread = p_is_there_unauth_worker_thread;
        }

        public void AuthSend(string p_user_name, DialogMessageForClient p_message)
        {
            int thread_id = get_online_user_thread_id(p_user_name);
            if (thread_id == 0)
                return;
            FinalMessageForClient message_for_user = new FinalMessageForClient(TypeOfMessage.Dialog, p_message);
            MessageToDistributer distributer_object = new MessageToDistributer(thread_id, new MessageToServerWorker(TypeOfMessageToServerWorker.FinalMessageToClient
                , message_for_user), TypeOfMessageToDistributer.MessageToServerWorker);

            lock (distributer_pulse_object)
            {
                send_to_distributer_construct.send_to_distributer_queue.Enqueue(distributer_object);
                send_to_distributer_construct.send_to_distribuer_queue_flag = true;
                Monitor.Pulse(distributer_pulse_object);
            }
        }

        public void AuthSend(string p_user_name, ChatMessageForClient p_message)
        {
            int thread_id = get_online_user_thread_id(p_user_name);
            if (thread_id == 0)
                return;
            FinalMessageForClient message_for_user = new FinalMessageForClient(TypeOfMessage.Chat, p_message);
            MessageToDistributer distributer_object = new MessageToDistributer(thread_id, new MessageToServerWorker(TypeOfMessageToServerWorker.FinalMessageToClient
                , message_for_user), TypeOfMessageToDistributer.MessageToServerWorker);

            lock (distributer_pulse_object)
            {
                send_to_distributer_construct.send_to_distributer_queue.Enqueue(distributer_object);
                send_to_distributer_construct.send_to_distribuer_queue_flag = true;
                Monitor.Pulse(distributer_pulse_object);
            }
        }

        public void UnAuthsend(int p_thread_id, DialogMessageForClient p_message)
        {
            if (!is_there_unauth_worker_thread(p_thread_id))
                return;
            FinalMessageForClient message_for_user = new FinalMessageForClient(TypeOfMessage.Dialog, p_message);
            MessageToDistributer distributer_object = new MessageToDistributer(p_thread_id, new MessageToServerWorker(TypeOfMessageToServerWorker.FinalMessageToClient, message_for_user), TypeOfMessageToDistributer.MessageToServerWorker);
            lock (distributer_pulse_object)
            {
                send_to_distributer_construct.send_to_distributer_queue.Enqueue(distributer_object);
                send_to_distributer_construct.send_to_distribuer_queue_flag = true;
                Monitor.Pulse(distributer_pulse_object);
            }
        }

    }

}
