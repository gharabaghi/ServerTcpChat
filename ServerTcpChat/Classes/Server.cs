using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerTcpChat.Classes;
using System.Threading;
using CommonChatTypes;


namespace ServerTcpChat
{
    class Server
    {

        AllDialogs all_dialogs;
        AllChats all_chats;
        AllAgreements all_agreements;
        ServerCore server_core;
        ThreadWorks thread_works;
        OfflineUserWorks offline_user_works;
        AuthenticateAndDistribute authenicate_and_distribute;

        public Server(Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> p_auth_dialogs
            , ref Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> p_unauth_dialogs, Dictionary<int, PrivateChat> p_all_private_chats
            , Dictionary<int, PublicChat> p_all_public_chats, ref Dictionary<int, AddAgreement> p_all_add_agreements, Dictionary<int, UserData> p_all_users_logged_in,
            List<int> p_all_threads, SendToDistributerConstruct p_send_to_distributer_construct, object p_distributer_pulse_object)
        {

            Se_ServerDelegateForDialogs server_delegates_for_dialogs = new Se_ServerDelegateForDialogs(new GetFriendsList(Se_GetUserfriendList), new GetUserStatus(Se_GetUserStatus)
            , new GetPublicChatIds(Se_GetPublicChatIds), new GetOfflineMessages(Se_GetAllUserOfflineMessages), new GetAllAgreementInvitation(Se_GetAllUserAgreementInvitation)
            , new AuthSend(Se_Authsend), new IsLoggedIn(Se_IsLoggedIn), new UnAuthSend(Se_UnAuthSend), new IsThereUnauthWorkerThread(Se_IsThereUnauthWorkerThread)
            , new Login(Se_Login), new ServerInformChatLeave(Se_ServerInformChatLeave), new OnlineAreFriends(Se_OnlineAreFriends), new CreatePrivateChat(Se_CreatePrivateChat)
            , new IsTherePrivateChat(Se_IsTherePrivateChat), new StartClientCreatedPrivateChatInform(Se_StartClientCreatedPrivateChatInform)
            , new JoinPublicChatRequest(Se_JoinPublicChatRequest), new IsUserInPublicChat(Se_IsUserInPublicChat), new GetPublicChatUsersList(Se_GetPublicChatUsersList)
            , new CreateFormalMessageRequest(Se_CreateFormalMessageRequest), new IstherUser(Se_IstherUser), new CreateOfflineMessage(Se_CreateOfflineMessage)
            , new SignUp(Se_Signup), new CreateAddAgreement(Se_CreateAddAgreement), new GetAgreementAnswer(Se_GetAgreementAnswer), new OfflineMessagesReadInform(Se_OfflineMessagesReadInform)
            , new GetPrivateChatInvitationAnswer(Se_GetPrivateChaInvitationAnswer), new GetAUserAgreementInvitation(Se_DB_GetAUserAgreementInvitation));

            server_core = new ServerCore(p_all_users_logged_in, new StartClientFriendChangedStatusInformDialog(Se_StartClientFriendChangedStatusInform)
               , new StartSendToClinetFormalMessage(Se_StartSendToClinetFormalMessage), p_all_threads, p_send_to_distributer_construct, p_distributer_pulse_object);

            all_dialogs = new AllDialogs(server_delegates_for_dialogs, ref p_auth_dialogs, ref p_unauth_dialogs);


            all_chats = new AllChats(p_all_private_chats, p_all_public_chats,
                new ChatSend(Se_ChatSend), new StartEjectedChatUserInform(Se_StartEjectedChatUserInform), new IsLoggedIn(Se_IsLoggedIn),
                new StartClientSomeoneJoinedChatInform(Se_StartClientSomeoneJoinedChatInform), new StartClientSomeoneLeftChatInform(Se_StartClientSomeoneLeftChatInform));

            all_agreements = new AllAgreements(ref p_all_add_agreements, new CreateFormalMessageRequest(Se_CreateFormalMessageRequest), new DB_IsThereUser(Se_IstherUser)
            , new DB_AddToFriends(Se_AddToFriends), new DB_AreFriends(Se_AreFriends), new DB_CreateAddAgreement(Se_DB_CreateAddAgreement), new DB_RemoveAgreement(Se_RemoveAgreement)
            , new IsLoggedIn(Se_IsLoggedIn), new StartClientFriendListChangedInformDialog(Se_StartClientFriendListChangedInformDialog), new ReloadFriendList(Se_ReloadFriendList)
            , new GetOnlineUserFriendList(Se_GetOnlineUserFriendList), new GetUserStatus(Se_GetUserStatus), new AddAgreementDone(Se_AddAgreementDone)
            , new GetAllAdAgreements(Se_GetAllAdAgreements), new StartClientInvitedAgreementInform(Se_StartClientInvitedAgreementInform));


            thread_works = new ThreadWorks(new RegisterThread(Se_RegisterThread), new IsThereUnauthWorkerThread(Se_IsThereUnauthWorkerThread)
            , new RemoveThread(Se_RemoveThread), new RemoveAThreadDialogs(Se_RemoveAThreadDialogs));


            offline_user_works = new OfflineUserWorks(new ChangeStatusToOffline(Se_ChangeStatusToOffline), new OfflineUser(Se_OfflineUser), new GetUserPresenceState(Se_GetUserPresenceState)
            , new GetOnlineUserThreadID(Se_GetOnlineUserThreadID), new UserLeaveAllChats(Se_UserLeavAllChats), new RemoveAThreadDialogs(Se_RemoveAThreadDialogs)
            , new RemoveAUserDialogs(Se_RemoveAUserDialogs),new RemoveThread(Se_RemoveThread));


            authenicate_and_distribute = new AuthenticateAndDistribute(new GetAThreadUserName(Se_GetAThreadUserName), new IsThereUnauthWorkerThread(Se_IsThereUnauthWorkerThread)
            , new DialogReceiveMessage(Se_DialogReceiveMessage), new ChatReceiveMessage(Se_ChatReceiveMessage), new MakeMessagesOffline(Se_MakeMessagesOffline)
            , new UserOfflineRequest(Se_UserOfflineRequest), new ThreadisLoggedIn(Se_ThreadIsLoggedIn), new RegisterAThreadRequest(Se_RegisterAThreadRequest)
            , new ThreadRemoveWorks(Se_ThreadRemoveWorks));
        }

        public void Receive(MessageFromServerWorkerQueueObject p_global_message_to_server_from_queue)
        {
            authenicate_and_distribute.Receive(p_global_message_to_server_from_queue);
        }



        protected bool Se_IsLoggedIn(string p_user_name)
        {
            return server_core.SC_IsLoggedIn(p_user_name);
        }
        protected void Se_Authsend(string p_user_name, DialogMessageForClient p_message)
        {
            server_core.SC_AuthSend(p_user_name, p_message);
        }
        protected void Se_UnAuthSend(int p_thread_id, DialogMessageForClient p_message)
        {
            server_core.SC_UnAuthSend(p_thread_id, p_message);
        }
        protected bool Se_IsThereUnauthWorkerThread(int p_thread_id)
        {
            return server_core.SC_IsThereUnauthWorkerThread(p_thread_id);
        }
        public List<AgreementInvitationInfo> Se_GetAllUserAgreementInvitation(string p_user_name)
        {
            return server_core.SC_DB_GetUserAgreementInvitation(p_user_name);
        }
        public List<OfflineMessage> Se_GetAllUserOfflineMessages(string p_user_name)
        {
            return server_core.SC_DB_LoadUserOfflineMessages(p_user_name);
        }
        public UserPresenceState Se_GetUserPresenceState(string p_user_name)
        {
            return server_core.SC_GetUserPresenceState(p_user_name);
        }
        public List<string> Se_GetUserfriendList(string p_user_name)
        {
            return server_core.SC_DB_GetUserFriendsList(p_user_name);
        }
        public Se_BaseBooleanFunctionResult Se_Login(int p_thread_id, string p_user_name, string p_password)
        {
            return server_core.SC_Login(p_thread_id, p_user_name, p_password);
        }
        public Se_BaseBooleanFunctionResult Se_OnlineAreFriends(string p_first_person, string p_second_person)
        {
            return server_core.SC_OnlineAreFriends(p_first_person, p_second_person);
        }
        public void Se_CreateFormalMessageRequest(FormalMessage p_message, string p_receiver_user_name)
        {
            server_core.SC_SendFormalMessageToUser(p_message, p_receiver_user_name);
        }
        public bool Se_IstherUser(string p_user_name)
        {
            return server_core.SC_DB_IsThereUser(p_user_name);
        }
        public void Se_CreateOfflineMessage(string p_receiver_user_name, OfflineMessage p_message)
        {
            server_core.SC_DB_AddOfflineMessage(p_receiver_user_name, p_message);
        }
        public Se_BaseBooleanFunctionResult Se_Signup(string p_user_name, string p_password)
        {
            return server_core.SC_SignUp(p_user_name, p_password);
        }
        public void Se_ReloadFriendList(string p_user_name)
        {
            server_core.SC_ReloadFriendList(p_user_name);
        }
        public List<string> Se_GetOnlineUserFriendList(string p_user_name)
        {
            return server_core.SC_GetOnlineUserFriendList(p_user_name);
        }
        public bool Se_AreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            return server_core.SC_DB_AreFriends(p_first_person_user_name, p_second_person_user_name);
        }
        public void Se_RegisterThread(int p_thread_id)
        {
            server_core.SC_RegisterAThread(p_thread_id);
        }
        public bool Se_ThreadIsLoggedIn(int p_thread_id)
        {
            return server_core.SC_ThreadisLoggedIn(p_thread_id);
        }
        public UserStatus Se_GetUserStatus(string p_user_name)
        {
            return server_core.SC_GetUserStatus(p_user_name);
        }
        public void Se_AddToFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            server_core.SC_DB_OfflineAddToFriends(p_first_person_user_name, p_second_person_user_name);
        }
        public void Se_RemoveAgreement(int p_agreement_id)
        {
            server_core.SC_DB_RemoveAgreement(p_agreement_id);
        }
        public void Se_DB_CreateAddAgreement(string p_starter_user_name, string p_user_to_add_name, int p_agreement_id)
        {
            server_core.SC_DB_CreateAddAgreement(p_agreement_id, p_starter_user_name, p_user_to_add_name);
        }
        public bool Se_RemoveThread(int p_thread_id)
        {
            return server_core.SC_RemoveThread(p_thread_id);
        }
        public bool Se_ChangeStatusToOffline(string p_user_name)
        {
            return server_core.SC_ChangeStatusToOffline(p_user_name);
        }
        public bool Se_OfflineUser(string p_user_name)
        {
            return server_core.SC_OfflineUser(p_user_name);
        }
        public int Se_GetOnlineUserThreadID(string p_user_name)
        {
            return server_core.SC_GetOnlineUserThreadID(p_user_name);
        }
        public string Se_GetAThreadUserName(int p_thread_id)
        {
            return server_core.SC_GetAThreadUserName(p_thread_id);
        }
        public void Se_AddAgreementDone(int p_agreement_id)
        {
            server_core.SC_DB_AddAgreementDone(p_agreement_id);
        }
        public void Se_OfflineMessagesReadInform(string p_user_name, List<int> p_message_ids)
        {
            server_core.SC_DB_RemoveUserOfflineMessages(p_user_name, p_message_ids);
        }
        public List<Agreement> Se_GetAllAdAgreements()
        {
            return server_core.SC_DB_GetAllAdAgreements();
        }
        public bool Se_DB_IsThereAgreement(int p_agreement_id)
        {
            return server_core.SC_DB_IsThereAgreement(p_agreement_id);
        }
        public AgreementInvitationInfo Se_DB_GetAUserAgreementInvitation(string p_user_name, int p_agreement_id)
        {
            return server_core.SC_DB_GetAUserAgreementInvitation(p_user_name, p_agreement_id);
        }

        public List<int> Se_GetPublicChatIds()
        {
            return all_chats.AllMa_GetPublicChatIDs();
        }
        public void Se_ServerInformChatLeave(string p_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            all_chats.AllMa_ChatLeave(p_user_name, p_chat_id, p_chat_type);
        }
        public Se_BaseIntFunctionResult Se_CreatePrivateChat(string p_first_person, string p_second_person)
        {
            return all_chats.AllMa_CreatePrivateChat(p_first_person, p_second_person);
        }
        public bool Se_IsTherePrivateChat(int p_chat_id)
        {
            return all_chats.AllMa_IsTherePrivateChat(p_chat_id);
        }
        public Se_BaseBooleanFunctionResult Se_JoinPublicChatRequest(string p_user_name, int p_chat_id)
        {
            return all_chats.AllMa_JoinPublicChatRequest(p_user_name, p_chat_id);
        }
        public bool Se_IsUserInPublicChat(string p_user_name, int p_chat_id)
        {
            return all_chats.AllMa_IsUserInPublicChat(p_user_name, p_chat_id);
        }
        public List<string> Se_GetPublicChatUsersList(int p_chat_id)
        {
            return all_chats.AllMa_GetPublicChatUsersList(p_chat_id);
        }
        public void Se_UserLeavAllChats(string p_user_name)
        {
            all_chats.AllMa_UserLeavAllChats(p_user_name);
        }
        public void Se_ChatReceiveMessage(AuthServerChatMessage p_messsage)
        {
            all_chats.AllMa_ReceiveMessage(p_messsage);
        }
        public void Se_ChatSend(string p_receiver_user_name, ChatMessageForClient p_chat_message)
        {
            server_core.SC_ChatSend(p_receiver_user_name, p_chat_message);
        }
        public void Se_CreatePubliChat(int p_chat_id, int p_max_users_count)
        {
            all_chats.AllMa_CreatePubliChat(p_chat_id, p_max_users_count);
        }
        public Se_BaseBooleanFunctionResult Se_GetPrivateChaInvitationAnswer(string p_user_name, int p_chat_id, bool p_answer)
        {
            return all_chats.GetAnswerToPrivateChatInvitation(p_user_name, p_chat_id, p_answer);
        }


        public Se_BaseBooleanFunctionResult Se_CreateAddAgreement(string p_starter_user_name, string p_invited_user_name)
        {
            return all_agreements.CreateAddAgreement(p_starter_user_name, p_invited_user_name);
        }
        public void Se_GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer, TypeOfAgreement p_agreement_type)
        {
            all_agreements.GetAgreementAnswer(p_user_name, p_agreement_id, p_answer, p_agreement_type);
        }

        public void Se_StartClientFriendListChangedInformDialog(string p_user_name, List<PersonStatus> p_new_friend_liast_and_status)
        {
            all_dialogs.CreatClientFriendListChangedInform(p_user_name, p_new_friend_liast_and_status);
        }
        public void Se_StartClientCreatedPrivateChatInform(string p_user_name, string p_starter_user_name, int p_chat_id)
        {
            all_dialogs.CreateClientCreatedPrivateChatInform(p_user_name, p_starter_user_name, p_chat_id);
        }

        public void Se_RemoveAThreadDialogs(int p_thread_id)
        {
            all_dialogs.RemoveAThreadDialogs(p_thread_id);
        }
        public void Se_RemoveAUserDialogs(string p_user_name)
        {
            all_dialogs.RemoveAUserDialogs(p_user_name);
        }
        public void Se_DialogReceiveMessage(BaseServerDialogMessage p_message)
        {
            all_dialogs.ReceiveMessage(p_message);
        }
        public void Se_StartEjectedChatUserInform(string p_user_name, int p_chat_id_user_ejected_from, string p_ejecting_comment, TypeOfChat p_chat_type)
        {
            all_dialogs.CreateInformEjectedChatUser(p_user_name, p_chat_id_user_ejected_from, p_ejecting_comment, p_chat_type);
        }
        public void Se_StartClientSomeoneLeftChatInform(string p_user_to_inform_name, string p_user_left_chat_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            all_dialogs.CreateClientSomeoneLeftChatInform(p_user_to_inform_name, p_user_left_chat_name, p_chat_id, p_chat_type);
        }
        public void Se_StartClientSomeoneJoinedChatInform(string p_user_to_inform_name, string p_joined_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            all_dialogs.CreateClientSomeoneJoinedChatInform(p_user_to_inform_name, p_joined_user_name, p_chat_id, p_chat_type);
        }
        public void Se_StartClientFriendChangedStatusInform(string p_user_to_inform_name, string p_user_changed_status_name, UserStatus p_new_status)
        {
            all_dialogs.CreateClientFriendChangedStatusInform(p_user_to_inform_name, p_user_changed_status_name, p_new_status);
        }
        public void Se_StartSendToClinetFormalMessage(string p_receiver_user_name, string p_sender_user_name, string p_message_text, int p_message_id)
        {
            all_dialogs.CreateSendToClinetFormalMessage(p_receiver_user_name, p_sender_user_name, p_message_text, p_message_id);
        }
        public void Se_StartClientInvitedAgreementInform(string p_user_name, int p_agreement_id)
        {
            all_dialogs.CreateClientInvitedAgreementInform(p_user_name, p_agreement_id);
        }

        public void Se_UserOfflineRequest(string p_user_name)
        {
            offline_user_works.OfflineUser(p_user_name);
        }
        public void Se_RegisterAThreadRequest(int p_thread_id)
        {
            thread_works.RegisterAThread(p_thread_id);
        }
        public void Se_ThreadRemoveWorks(int p_thread_id)
        {
            thread_works.RemoveThread(p_thread_id);
        }
        public void Se_MakeMessagesOffline(string p_offline_user_name, List<FinalMessageForServer> p_messages)
        {
            offline_user_works.MakeMessagesOffline(p_offline_user_name, p_messages);
        }
    }

    public static class ServerThread
    {
        public static void ThreadRunner(Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> p_auth_dialogs
            , ref Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> p_unauth_dialogs, Dictionary<int, PrivateChat> p_all_private_chats
            , Dictionary<int, PublicChat> p_all_public_chats, ref Dictionary<int, AddAgreement> p_all_add_agreements, Dictionary<int, UserData> p_all_users_logged_in,
            List<int> p_all_threads, SendToDistributerConstruct p_send_to_distributer_construct, ReceiveFromServerWorkerConstruct p_receive_from_worker_construct
            , object p_distributer_pulse_object, object p_server_thread_pulse_object)
        {
            Server server = new Server(p_auth_dialogs, ref p_unauth_dialogs, p_all_private_chats, p_all_public_chats, ref p_all_add_agreements, p_all_users_logged_in
                , p_all_threads, p_send_to_distributer_construct, p_distributer_pulse_object);

            while (true)
            {
                lock (p_server_thread_pulse_object)
                {
                    if (!p_receive_from_worker_construct.server_receive_queue_flag)
                        Monitor.Wait(p_server_thread_pulse_object);

                    if (p_receive_from_worker_construct.server_receive_quque.Count > 0)
                    {
                        MessageFromServerWorkerQueueObject message_for_server = p_receive_from_worker_construct.server_receive_quque.Dequeue();
                        if (p_receive_from_worker_construct.server_receive_quque.Count == 0)
                        {
                            p_receive_from_worker_construct.server_receive_queue_flag = false;
                        }
                        server.Receive(message_for_server);
                    }
                    else
                    {
                        p_receive_from_worker_construct.server_receive_queue_flag = false;
                    }
                }
            }
        }
    }
}
