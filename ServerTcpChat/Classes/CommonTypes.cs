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
    public enum TypeOfMessageToDistributer
    {
        MessageToServerWorker,
        CancelAServerWorkerRequest,

    }
    public class MessageToDistributer
    {
        int thread_id;
        MessageToServerWorker message_to_server_worker;
        TypeOfMessageToDistributer type_of_message_to_distributer;

        public int Get_thread_id
        {
            get { return thread_id; }
        }
        public MessageToServerWorker Get_message_to_server_worker
        {
            get { return message_to_server_worker; }
        }
        public TypeOfMessageToDistributer Get_type_of_message_to_distributer
        {
            get { return type_of_message_to_distributer; }
        }

        public MessageToDistributer(int p_thread_id, MessageToServerWorker p_message_to_server_worker, TypeOfMessageToDistributer p_type_of_message_to_distributer)
        {
            thread_id = p_thread_id;
            message_to_server_worker = p_message_to_server_worker;
            type_of_message_to_distributer = p_type_of_message_to_distributer;
        }
    }

    public class MessageToServerWorker
    {
        TypeOfMessageToServerWorker type_of_message_to_serverworker;
        object message_to_server_worker_object;

        public TypeOfMessageToServerWorker Get_type_of_message_to_serverworker
        {
            get { return type_of_message_to_serverworker; }
        }
        public object Get_message_to_server_worker_object
        {
            get { return message_to_server_worker_object; }
        }

        public MessageToServerWorker(TypeOfMessageToServerWorker p_type_of_message_to_serverworker, object p_message_to_server_worker_object)
        {
            type_of_message_to_serverworker = p_type_of_message_to_serverworker;
            message_to_server_worker_object = p_message_to_server_worker_object;
        }

    }
    public enum TypeOfMessageToServerWorker
    {
        FinalMessageToClient,
        QuickCheckRequest,
    }

    public enum UserPresenceState
    {
        NotPresent,
        OfflineStatus,
        OnlineStatus
    }

    public class UserData
    {
        string user_name;
        List<string> friends_list;
        UserStatus status;

        public UserStatus Get_Set_status
        {
            get { return status; }
            set { status = value; }
        }
        public string Get_user_name
        {
            get { return user_name; }
        }
        public List<string> Get_Set_friends_list
        {
            get { return friends_list; }
            set { friends_list = value; }
        }

        public UserData(string p_user_name, List<string> p_friends_list, UserStatus p_status)
        {
            user_name = p_user_name;
            friends_list = p_friends_list;
            status = p_status;
        }

        public override bool Equals(object obj)
        {
            UserData t_user_data = null;
            try
            {
                t_user_data = (UserData)obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            if (t_user_data.Get_user_name == user_name && t_user_data.status == status)
                return true;
            return false;
        }



    }

    public class ServerWorkerData
    {
        public RemoveWorker remove_worker;

        public SendToWorkerConstruct send_to_worker_construct;
        public ReceiveFromServerWorkerConstruct receive_from_worker_construct;

        public object worker_pulse_object;
        public object server_pulse_object;

        public bool cancel_construct;
        public object cancel_construct_lock;

        public ServerWorkerData(ReceiveFromServerWorkerConstruct p_receive_from_worker_construct, object p_server_pulse_object, RemoveWorker p_remove_worker)
        {
            receive_from_worker_construct = p_receive_from_worker_construct;
            remove_worker = p_remove_worker;
            server_pulse_object = p_server_pulse_object;
            worker_pulse_object = new object();
            cancel_construct = false;
            cancel_construct_lock = new object();
            send_to_worker_construct = new SendToWorkerConstruct();
        }
    }

    public class SendToWorkerConstruct
    {
        public Queue<MessageToServerWorker> send_to_worker_quque;
        public bool send_to_worker_queue_flag;

        public SendToWorkerConstruct()
        {
            send_to_worker_quque = new Queue<MessageToServerWorker>();
            send_to_worker_queue_flag = false;
        }
    }

    public class ReceiveFromServerWorkerConstruct
    {
        public Queue<MessageFromServerWorkerQueueObject> server_receive_quque;
        public bool server_receive_queue_flag;

        public ReceiveFromServerWorkerConstruct(Queue<MessageFromServerWorkerQueueObject> p_server_receive_quque)
        {
            server_receive_quque = p_server_receive_quque;
            server_receive_queue_flag = false;
        }

    }

    public class WorkersPortNumberConstruct
    {
        public Queue<int> workers_port_number_queue;
        public bool workers_port_number_queue_flag;

        public WorkersPortNumberConstruct(Queue<int> p_workers_port_number_queue)
        {
            workers_port_number_queue = p_workers_port_number_queue;
            workers_port_number_queue_flag = false;
        }
    }

    public class SendToDistributerConstruct
    {
        public Queue<MessageToDistributer> send_to_distributer_queue;
        public bool send_to_distribuer_queue_flag;

        public SendToDistributerConstruct(Queue<MessageToDistributer> p_send_to_distributer_queue)
        {
            send_to_distributer_queue = p_send_to_distributer_queue;
            send_to_distribuer_queue_flag = false;
        }
    }

    public abstract class BaseServerDialogMessage
    {
        protected DialogMessageForServer message;

        public DialogMessageForServer Get_message
        {
            get { return message; }
        }
    }
    public class AuthServerDialogMessage : BaseServerDialogMessage
    {
        string user_name;

        public string Get_user_name
        {
            get { return user_name; }
        }
        public AuthServerDialogMessage(DialogMessageForServer p_message, string p_user_name)
        {
            message = p_message;
            user_name = p_user_name;
        }
    }
    public class UnAuthServerDialogMessage : BaseServerDialogMessage
    {
        int thread_id;

        public int Get_thread_id
        {
            get { return thread_id; }
        }
        public UnAuthServerDialogMessage(DialogMessageForServer p_message, int p_thread_id)
        {
            message = p_message;
            thread_id = p_thread_id;
        }
    }

    [Serializable]
    public class AuthServerChatMessage
    {
        string user_name;
        public string Get_user_name
        {
            get { return user_name; }
        }
        ChatMessageForServer chat_message;
        public ChatMessageForServer Get_chat_message
        {
            get { return chat_message; }
        }

        public AuthServerChatMessage(string p_user_name, ChatMessageForServer p_chat_message)
        {
            user_name = p_user_name;
            chat_message = p_chat_message;
        }
    }

    public class MessageFromServerWorkerQueueObject
    {
        int thread_id;
        MessageFromServerWorker message_from_worker;

        public MessageFromServerWorker Get_message_from_worker
        {
            get { return message_from_worker; }
        }
        public int Get_thread_id
        {
            get { return thread_id; }
        }

        public MessageFromServerWorkerQueueObject(int p_thread_id, MessageFromServerWorker p_message_from_worker)
        {
            thread_id = p_thread_id;
            message_from_worker = p_message_from_worker;
        }
    }

    public enum TypeOfMessageFromServerWorker
    {
        FinalMessageForServer,
        ofllienRequestMessage,
        MakeOfflineMessage,
    }
    public class MessageFromServerWorker
    {
        TypeOfMessageFromServerWorker type_of_message_from_server_worker;
        object message_from_server_worker_object;

        public TypeOfMessageFromServerWorker Get_type_of_message_from_server_worker
        {
            get { return type_of_message_from_server_worker; }
        }
        public object Get_message_from_server_worker_object
        {
            get { return message_from_server_worker_object; }
        }

        public MessageFromServerWorker(TypeOfMessageFromServerWorker p_type_of_message_from_server_worker, object p_message_from_server_worker_object)
        {
            type_of_message_from_server_worker = p_type_of_message_from_server_worker;
            message_from_server_worker_object = p_message_from_server_worker_object;
        }
    }
}
