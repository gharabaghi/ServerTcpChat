using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerTcpChat.Classes;
using CommonChatTypes;

namespace ServerTcpChat.Classes
{
    public delegate void DialogReceiveMessage(BaseServerDialogMessage p_message);
    public delegate void ChatReceiveMessage(AuthServerChatMessage p_messsage);
    public delegate void MakeMessagesOffline(string p_offline_user_name, List<FinalMessageForServer> p_messages);
    public delegate void UserLeaveAllChats(string p_user_name);
    public delegate void RemoveAThreadDialogs(int p_thread_id);
    public delegate void RemoveAUserDialogs(string p_user_name);
    public delegate bool ThreadisLoggedIn(int p_thread_id);
    public delegate void UserOfflineRequest(string p_user_name);
    public delegate void RegisterThread(int p_thread_id);
    public delegate void ThreadRemoveWorks(int p_thread_id);
    public delegate void RegisterAThreadRequest(int p_thread_id);

    
    public class OfflineUserWorks
    {

        ChangeStatusToOffline change_status_to_offline;
        OfflineUser offline_user;
        GetUserPresenceState get_user_presence_state;
        GetOnlineUserThreadID get_online_user_thread_id;
        RemoveThread remove_thread;

        UserLeaveAllChats user_leave_all_chats;
        RemoveAThreadDialogs remove_a_thread_dialog;
        RemoveAUserDialogs remove_a_user_dialog;

        public OfflineUserWorks(ChangeStatusToOffline p_change_status_to_offline, OfflineUser p_offline_user, GetUserPresenceState p_get_user_presence_state
            , GetOnlineUserThreadID p_get_online_user_thread_id, UserLeaveAllChats p_user_leave_all_chats, RemoveAThreadDialogs p_remove_a_thread_dialog
            , RemoveAUserDialogs p_remove_a_user_dialog, RemoveThread p_remove_thread)
        {
            change_status_to_offline = p_change_status_to_offline;
            offline_user = p_offline_user;
            get_user_presence_state = p_get_user_presence_state;
            get_online_user_thread_id = p_get_online_user_thread_id;
            user_leave_all_chats = p_user_leave_all_chats;
            remove_a_thread_dialog = p_remove_a_thread_dialog;
            remove_a_user_dialog = p_remove_a_user_dialog;
            remove_thread = p_remove_thread;
        }

        public void MakeMessagesOffline(string p_offline_user_name, List<FinalMessageForServer> p_messages) 
        {
            if (get_user_presence_state(p_offline_user_name) == UserPresenceState.OfflineStatus)
            {
                offline_user(p_offline_user_name);
            }
        }

        public void OfflineUser(string p_user_name)
        {
            if (get_user_presence_state(p_user_name) == UserPresenceState.OnlineStatus)
            {
                int thread_id = get_online_user_thread_id(p_user_name);
                change_status_to_offline(p_user_name);
                remove_a_user_dialog(p_user_name);
                user_leave_all_chats(p_user_name);
                remove_a_thread_dialog(thread_id);
                offline_user(p_user_name);
                remove_thread(thread_id);
            }
        }
    }

    public class ThreadWorks
    {

        RegisterThread register_thread;
        IsThereUnauthWorkerThread is_there_unauth_worker_thread;
        RemoveThread remove_thread;
        RemoveAThreadDialogs remove_a_thread_dialogs;

        public ThreadWorks( RegisterThread p_register_thread, IsThereUnauthWorkerThread p_is_there_unauth_worker_thread
        , RemoveThread p_remove_thread, RemoveAThreadDialogs p_remove_a_thread_dialogs)
        {
            register_thread = p_register_thread;
            is_there_unauth_worker_thread = p_is_there_unauth_worker_thread;
            remove_thread = p_remove_thread;
            remove_a_thread_dialogs = p_remove_a_thread_dialogs;
        }

        public void RemoveThread(int p_thread_id)
        {
            remove_thread(p_thread_id);
            remove_a_thread_dialogs(p_thread_id);
        }

        public void RegisterAThread(int p_thread_id)
        {
            if (!is_there_unauth_worker_thread(p_thread_id))
            {
                register_thread(p_thread_id);
            }
        }

    }

    public class AuthenticateAndDistribute
    {
        GetAThreadUserName get_a_thread_user_name;
        IsThereUnauthWorkerThread is_there_unauth_worker_thread;

        DialogReceiveMessage dialog_receive_message;
        ChatReceiveMessage chat_receive_message;
        MakeMessagesOffline make_messages_offline; 
        UserOfflineRequest offline_user;  
        ThreadisLoggedIn thread_is_logged_in;
        RegisterAThreadRequest register_a_thread_request;
        ThreadRemoveWorks thread_remove_works;

        public AuthenticateAndDistribute(GetAThreadUserName p_get_a_thread_user_name, IsThereUnauthWorkerThread p_is_there_unauth_worker_thread, DialogReceiveMessage p_dialog_receive_message
        , ChatReceiveMessage p_chat_receive_message, MakeMessagesOffline p_make_messages_offline, UserOfflineRequest p_offline_user, ThreadisLoggedIn p_thread_is_logged_in
        , RegisterAThreadRequest p_register_a_thread_request, ThreadRemoveWorks p_thread_remove_works)
        {
            get_a_thread_user_name = p_get_a_thread_user_name;
            is_there_unauth_worker_thread = p_is_there_unauth_worker_thread;
            dialog_receive_message = p_dialog_receive_message;
            chat_receive_message = p_chat_receive_message;
            make_messages_offline = p_make_messages_offline;
            offline_user = p_offline_user;
            thread_is_logged_in = p_thread_is_logged_in;
            register_a_thread_request = p_register_a_thread_request;
            thread_remove_works = p_thread_remove_works;
        }

        public void Receive(MessageFromServerWorkerQueueObject p_global_message_to_server_from_queue)
        {
            if (thread_is_logged_in(p_global_message_to_server_from_queue.Get_thread_id))
            {
                string user_name = get_a_thread_user_name(p_global_message_to_server_from_queue.Get_thread_id);
                if (user_name == null)
                {
                    return;
                }
                if (p_global_message_to_server_from_queue.Get_message_from_worker.Get_type_of_message_from_server_worker == TypeOfMessageFromServerWorker.FinalMessageForServer)
                {
                    FinalMessageForServer user_message = null;
                    try
                    {
                        user_message = (FinalMessageForServer)p_global_message_to_server_from_queue.Get_message_from_worker.Get_message_from_server_worker_object;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    if (user_message.Get_message_type == TypeOfMessage.Chat)
                    {
                        ChatMessageForServer user_chat_message = null;
                        try
                        {
                            user_chat_message = ((ChatMessageForServer)user_message.Get_message_object);
                            chat_receive_message(new AuthServerChatMessage(user_name, user_chat_message));
                            return;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                    else if (user_message.Get_message_type == TypeOfMessage.Dialog)
                    {
                        DialogMessageForServer user_dialog_message = null;
                        try
                        {
                            user_dialog_message = (DialogMessageForServer)user_message.Get_message_object;
                            dialog_receive_message(new AuthServerDialogMessage(user_dialog_message, user_name));
                            return;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
                else if (p_global_message_to_server_from_queue.Get_message_from_worker.Get_type_of_message_from_server_worker == TypeOfMessageFromServerWorker.MakeOfflineMessage)
                {
                    
                    List<FinalMessageForServer> make_offline_messages = new List<FinalMessageForServer>();
                    try
                    {
                        make_offline_messages = (List<FinalMessageForServer>)p_global_message_to_server_from_queue.Get_message_from_worker.Get_message_from_server_worker_object;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    make_messages_offline(user_name, make_offline_messages);
                    return;
                }
                else if (p_global_message_to_server_from_queue.Get_message_from_worker.Get_type_of_message_from_server_worker == TypeOfMessageFromServerWorker.ofllienRequestMessage)
                {
                    offline_user(user_name);
                    return;
                }
            }
            else if (!thread_is_logged_in(p_global_message_to_server_from_queue.Get_thread_id))
            {
                if (!is_there_unauth_worker_thread(p_global_message_to_server_from_queue.Get_thread_id))
                {
                    register_a_thread_request(p_global_message_to_server_from_queue.Get_thread_id);
                }
                if (p_global_message_to_server_from_queue.Get_message_from_worker.Get_type_of_message_from_server_worker == TypeOfMessageFromServerWorker.FinalMessageForServer)
                {
                    FinalMessageForServer user_message = null;
                    try
                    {
                        user_message = ((FinalMessageForServer)p_global_message_to_server_from_queue.Get_message_from_worker.Get_message_from_server_worker_object);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    if (user_message.Get_message_type == TypeOfMessage.Dialog)
                    {
                        DialogMessageForServer user_dialog_message = null;
                        try
                        {
                            user_dialog_message = (DialogMessageForServer)user_message.Get_message_object;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                        dialog_receive_message(new UnAuthServerDialogMessage(user_dialog_message, p_global_message_to_server_from_queue.Get_thread_id));
                        return;
                    }
                }
                if(p_global_message_to_server_from_queue.Get_message_from_worker.Get_type_of_message_from_server_worker == TypeOfMessageFromServerWorker.ofllienRequestMessage)
                {
                    thread_remove_works(p_global_message_to_server_from_queue.Get_thread_id);
                }
            }
        }

    }

    
}
