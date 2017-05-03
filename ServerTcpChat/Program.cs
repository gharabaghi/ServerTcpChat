using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerTcpChat.Classes;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using CommonChatTypes;
using System.Net.Sockets;

namespace ServerTcpChat
{
    class Program
    {

        static void Main(string[] args)
        {

            IPEndPoint server_udp_ip_endpoint = new IPEndPoint(IPAddress.Any, 0);
            int server_check_data = 0;
            IPAddress server_tcp_ip = IPAddress.Any;

            Console.WriteLine("Retrieve configurations");
            try
            {
                LoadConfigs(out server_udp_ip_endpoint, out server_check_data, out server_tcp_ip);
            }
            catch
            {
                Console.WriteLine("AN Error occured in working with config");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("loading data...");

            Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> all_auth_dialogs = CreateAllAuthDialogs();
            Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> all_unauth_dialogs = CreateAllUnAuthDialogs();

            Dictionary<int, PrivateChat> all_private_chats = new Dictionary<int, PrivateChat>();
            Dictionary<int, PublicChat> all_public_chats = new Dictionary<int, PublicChat>();

            Dictionary<int, AddAgreement> all_add_agreements = new Dictionary<int, AddAgreement>();

            Dictionary<int, UserData> all_users_logged_in = new Dictionary<int, UserData>();

            List<int> all_threads = new List<int>();

            Queue<MessageToDistributer> send_to_distributer_queue = new Queue<MessageToDistributer>();
            SendToDistributerConstruct send_to_disributer_construct = new SendToDistributerConstruct(send_to_distributer_queue);

            Queue<MessageFromServerWorkerQueueObject> server_receive_quque = new Queue<MessageFromServerWorkerQueueObject>();
            ReceiveFromServerWorkerConstruct receive_from_worker_construct = new ReceiveFromServerWorkerConstruct(server_receive_quque);

            object distributer_thread_pulse_object = new object();

            object server_thread_pulse_object = new object();

            Dictionary<int, KeyValuePair<Thread, ServerWorkerData>> all_workers_data = new Dictionary<int, KeyValuePair<Thread, ServerWorkerData>>();
            object all_workers_data_lock = new object();

            object producer_thread_pulse_object = new object();

            Queue<int> workers_port_number_queue = new Queue<int>();
            WorkersPortNumberConstruct workers_port_number_construct = new WorkersPortNumberConstruct(workers_port_number_queue);

            Thread server_thread = new Thread(() => ServerThread.ThreadRunner(all_auth_dialogs, ref all_unauth_dialogs, all_private_chats, all_public_chats
                 , ref all_add_agreements, all_users_logged_in, all_threads, send_to_disributer_construct, receive_from_worker_construct, distributer_thread_pulse_object
                 , server_thread_pulse_object));
            Thread.Sleep(100);

            Thread worker_producer_thread = new Thread(() => OtherThreads.WorkerProducerThread(all_workers_data, all_workers_data_lock, producer_thread_pulse_object
                , receive_from_worker_construct, workers_port_number_construct, server_thread_pulse_object
                , server_tcp_ip));
            Thread.Sleep(100);

            Thread distributer_thread = new Thread(() => OtherThreads.DistributerThread(send_to_disributer_construct, distributer_thread_pulse_object,
                all_workers_data, all_workers_data_lock));
            Thread.Sleep(100);

            Thread udp_thread = new Thread(() => OtherThreads.UDPThread(workers_port_number_construct, producer_thread_pulse_object, server_check_data
                , server_udp_ip_endpoint, server_tcp_ip));


            Console.WriteLine("starting server.");
            distributer_thread.Start();
            server_thread.Start();
            worker_producer_thread.Start();
            udp_thread.Start();
            try
            {
                Console.ReadLine(); 
            }
            catch
            {
                try
                {
                    server_thread.Abort();
                }
                catch
                {
                }
                try
                {
                    worker_producer_thread.Abort();
                }
                catch
                {
                }
                try
                {
                    distributer_thread.Abort();
                }
                catch
                {
                }
                try
                {
                    udp_thread.Abort();
                }
                catch
                {
                }
                try
                {
                    Environment.Exit(2);
                }
                catch
                {
                }
            }
            return;

        }

        private static void LoadConfigs(out IPEndPoint p_server_udp_ip_endpoint, out int p_server_check_data, out IPAddress p_server_tcp_ip)
        {
            System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string server_udp_port_string = AppConfig.AppSettings.Settings["ServerUdpPort"].Value;
            string udp_check_data_string = AppConfig.AppSettings.Settings["UdpCheckData"].Value;
            string server_ip_string = AppConfig.AppSettings.Settings["ServerIP"].Value;

            IPAddress server_udp_ip_address = IPAddress.Parse(server_ip_string);
            int server_udp_port_number = Convert.ToInt32(server_udp_port_string);
            int server_udp_check_data = Convert.ToInt32(udp_check_data_string);
            p_server_udp_ip_endpoint = new IPEndPoint(server_udp_ip_address, server_udp_port_number);
            p_server_check_data = server_udp_check_data;
            p_server_tcp_ip = IPAddress.Parse(server_ip_string);
            return;
        }

        private static Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> CreateAllAuthDialogs()
        {
            Dictionary<int, Se_AuthDialog> login_data_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> create_add_agreement_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> get_agreement_answer_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> server_read_offline_message_dialog_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_leave_chat_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_someone_left_chat_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> inform_ejected_chat_user_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> create_private_chat_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_join_public_chat_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_someone_joined_chat_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_friend_changed_status_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> send_to_client_formal_message_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> formal_message_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_friend_list_changed_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_created_private_chat_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_invited_agreement_inform_dialogs = new Dictionary<int, Se_AuthDialog>();

            Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> all_auth_dialogs = new Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>>();
            all_auth_dialogs.Add(TypeOfDialog.LoginDataRequest, login_data_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.CreateAddAgreement, create_add_agreement_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.GetAgreementAnswer, get_agreement_answer_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ServerReadOfflineMessagesInform, server_read_offline_message_dialog_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientLeaveChatRequest, client_leave_chat_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientSomeoneLeftChatInform, client_someone_left_chat_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.InformEjectedChatUser, inform_ejected_chat_user_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.CreatePrivateChatRequest, create_private_chat_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientJoinPublicChatRequest, client_join_public_chat_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientSomeoneJoinedChatInform, client_someone_joined_chat_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientFriendChangedStatusInform, client_friend_changed_status_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.SendToClinetFormalMessage, send_to_client_formal_message_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.FormalMessageRequest, formal_message_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientFriendListChangedInform, client_friend_list_changed_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientCreatedPrivateChatInform, client_created_private_chat_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientInvitedAgreementInform, client_invited_agreement_inform_dialogs);

            return all_auth_dialogs;
        }
        private static Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> CreateAllUnAuthDialogs()
        {
            Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> all_unauth_dialogs = new Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>>();
            Dictionary<int, Se_UnAuthDialog> login_equest_dialogs = new Dictionary<int, Se_UnAuthDialog>();
            Dictionary<int, Se_UnAuthDialog> client_signup_request_dialogs = new Dictionary<int, Se_UnAuthDialog>();
            all_unauth_dialogs.Add(TypeOfDialog.LoginRequest, login_equest_dialogs);
            all_unauth_dialogs.Add(TypeOfDialog.ClientSignupRequest, client_signup_request_dialogs);
            return all_unauth_dialogs;
        }

    }
}